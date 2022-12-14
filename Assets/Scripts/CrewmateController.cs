using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CrewmateController : Phaseable
{
    public GameObject selectorSmall;
    public GameObject selectorBig;
    public GameObject playerControlIndicator;
    private bool mouseOver = false;
    private GamePhaseLoop _phaseLoop;

    private Rigidbody2D _rigidbody2D;
    public bool playerControlled;
    private static int n_frames = 50 * 10;
    private ushort[] _savedInputs = new ushort[n_frames];
    private Vector2[] _savedPositions = new Vector2[n_frames];
    private int _frame;
    private Vector2 _initialPosition;
    private ushort _lastInput;
    public float speed = 1.5f;
    private bool _inputDown;
    public bool startFlipped;
    private bool pushingBoxRightNow = false;

    private Animator _animator;
    public SpriteRenderer spriteRenderer;
    private static readonly int AnimLeft = Animator.StringToHash("left");
    private static readonly int AnimRunning = Animator.StringToHash("running");
    private static readonly int AnimPanic = Animator.StringToHash("panic");
    private static readonly int AnimPush = Animator.StringToHash("push");

    public CapsuleCollider2D myHitbox;
    private bool alive = true;
    public bool rescued = false;
    public GameObject graphicsObj;
    public AudioSource deathSound;
    private static readonly int AnimDoor = Animator.StringToHash("door");
    private static readonly int AnimDeath = Animator.StringToHash("death");

    public float deathShakeMagnitude = 0.15f;
    public float deathShakeDuration = 0.2f;
    public float moveBoxShakeMagnitude = 0.05f;
    public float moveBoxShakeDuration = 0.05f;

    public bool SelectedForEvac => Game.selectedCrewmate == this;
    private bool _isMoving = false;
    private PositionPath _positionPath;

    private bool _inputConsumed;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        alive = true;
        _phaseLoop = FindObjectOfType<GamePhaseLoop>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = spriteRenderer.GetComponent<Animator>();
        _positionPath = GetComponentInChildren<PositionPath>();
        _initialPosition = transform.position;
    }

    void Update()
    {
        if (Game.currentPhase == GameState.Phase.EvacuationPhase && playerControlled && alive && !rescued) {
            if (_inputConsumed) {
                _lastInput = 0;
                _inputConsumed = false;
            }
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                _lastInput = (ushort) (_lastInput | 0b0000_1000);
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                _lastInput = (ushort) (_lastInput | 0b0000_0100);
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                _lastInput = (ushort) (_lastInput | 0b0000_0010);
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                _lastInput = (ushort) (_lastInput | 0b0000_0001);
            }

            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E))
            {
                _lastInput = (ushort) (_lastInput | 0b0001_0000);
            }
        }

        if (Game.currentPhase == GameState.Phase.EvacuationPhase && pushingBoxRightNow)
        {
            Game.ShakeCamera(moveBoxShakeMagnitude, deathShakeDuration);
        }
    }

    public override void Reset()
    {
        _frame = 0;
        _lastInput = 0;
        rescued = false;
        alive = true;
        graphicsObj.SetActive(true);
        gameObject.SetActive(true);
        transform.position = _initialPosition;
        myHitbox.enabled = true;

        //GetComponent<Rigidbody2D>().simulated = true;

        _animator.ResetTrigger(AnimDeath);
        _animator.Play("crewmate_idle");

        pushingBoxRightNow = false;
    }

    private void FixedUpdate()
    {
        if (Game.currentPhase == GameState.Phase.EvacuationPhase && alive)
        {
            if (_frame >= n_frames)
            {
                return;
            }

            if (playerControlled)
            {
                _savedInputs[_frame] = _lastInput;
                _savedPositions[_frame] = transform.position;
                //Debug.Log(input);
            }
            else if (_savedInputs != null)
            {
                _lastInput = _savedInputs[_frame];
            }
            else
            {
                // Stand around and die
            }

            ExecuteInput(_lastInput);

            Animate();

            _inputConsumed = true;
            _frame++;
        }
    }

    // 0001 1111
    // xxxI WASD
    private void ExecuteInput(ushort input)
    {
        Vector2 movement = new Vector2();
        bool interaction = false;
        if ((input & 0b0000001) != 0)
        {
            movement.x += 1;
        }

        if ((input & 0b0000010) != 0)
        {
            movement.y -= 1;
        }

        if ((input & 0b0000100) != 0)
        {
            movement.x -= 1;
        }

        if ((input & 0b0001000) != 0)
        {
            movement.y += 1;
        }

        if ((input & 0b0010000) != 0)
        {
            interaction = true;
        }

        if (movement.sqrMagnitude > 0)
            movement.Normalize();
        Move(movement);
        if (interaction)
        {
            if (!_inputDown)
                Interact();
            _inputDown = true;
        }
        else
        {
            _inputDown = false;
        }
    }

    private void Interact()
    {
        //Debug.Log("Character has interacted", this);
        GetComponent<InteractorBehaviourScript>().TriggerInteractions();
    }

    private void Move(Vector2 movement)
    {
        //Debug.Log(movement);
        var position = transform.position;
        _rigidbody2D.MovePosition(new Vector2(position.x, position.y) + speed * Time.fixedDeltaTime * movement);
    }

    public override void PhaseEvacuate()
    {
        if (playerControlled)
        {
            //Debug.Log("Deleting inputs");
            _savedInputs = new ushort[n_frames];
            _savedPositions = new Vector2[n_frames];
            _animator.SetBool(AnimPanic, false);
        }
        else
        {
            _animator.SetBool(AnimPanic, true);
        }

        UpdateSelector();
        if (SelectedForEvac)
        {
            // Game.DisplayFloatingText(transform.position, "'Get me out of here!'", 5);
        }
    }

    public override void PhasePlanning()
    {
        if (playerControlled)
        {
            // Make path from positions
            _positionPath.MakePath(_savedPositions);
        }

        UpdateSelector();

        // Set visuals
        _animator.SetBool(AnimRunning, false);
        _animator.SetBool(AnimLeft, startFlipped);
        _animator.SetBool(AnimPanic, true);
        spriteRenderer.flipX = startFlipped;
    }

    public void UpdateSelector()
    {
        GameState.Phase currentPhase = Game.currentPhase;
        if (currentPhase == GameState.Phase.EvacuationPhase && playerControlled && alive && !rescued) {
            playerControlIndicator.SetActive(true);
        } else {
            playerControlIndicator.SetActive(false);
        }
        if (currentPhase != GameState.Phase.PlanningPhase)
        {
            selectorSmall.SetActive(false);
            selectorBig.SetActive(false);
            _positionPath.gameObject.SetActive(false);
            _positionPath.visible = false;
            _positionPath.Alpha = 0;
            return;
        }


        if (mouseOver || SelectedForEvac)
        {
            selectorSmall.SetActive(true);
            selectorBig.SetActive(false);
            _positionPath.gameObject.SetActive(true);
            _positionPath.visible = true;
        }
        else
        {
            selectorSmall.SetActive(false);
            selectorBig.SetActive(true);
            _positionPath.visible = false;
        }

        if (Game.showingConfirmPopup)
        {
            selectorSmall.SetActive(false);
            selectorBig.SetActive(false);
        }
    }

    public void Rescue()
    {
        if (!alive)
        {
            // Cannot rescue what is dead
            return;
        }
        
        if (!rescued)
        {
            // Game.DisplayFloatingText(transform.position, "'I am safe!'", 5);
            graphicsObj.SetActive(false);
            rescued = true;
            Game.CheckWinCondition();
            UpdateSelector();
        }

        // if (Game.currentPhase==GameState.Phase.EvacuationPhase && SelectedForEvac) {
        //     _phaseLoop.NextPhase();
        // }
    }

    public void Kill()
    {
        if (rescued)
        {
            // Cannot kill if rescued
            return;
        }
        
        if (!alive)
        {
            Debug.LogError("Do you want to kill something that is already dead?????");
            Game.currentPhase = GameState.Phase.Unknown;
            return;
        }

        // CameraShake
        Game.ShakeCamera(deathShakeMagnitude, deathShakeDuration);
        
        //Audio
        deathSound.Play();

        //Game.DisplayFloatingText(transform.position, "'I am dead. No big surprise.'");
        myHitbox.enabled = false;
        _animator.SetTrigger(AnimDeath);
        //GetComponent<Rigidbody2D>().simulated = false;
        alive = false;
        UpdateSelector();
    }

    private void OnMouseEnter()
    {
        mouseOver = true;
        UpdateSelector();
    }

    public void OnMouseExit()
    {
        mouseOver = false;
        UpdateSelector();
    }

    public override void PhaseExplosion()
    {
        base.PhaseExplosion();
        if (alive)
            Kill();
    }

    public override void PhaseTutorial() {
        
        _animator.SetBool(AnimRunning, false);
        playerControlIndicator.SetActive(false);
        spriteRenderer.flipX = startFlipped;

    }

    private void OnMouseDown()
    {
//        print("A crewmate has been clicked.");
        if (Game.currentPhase == GameState.Phase.PlanningPhase)
        {
            Game.selectedCrewmate = this;
            _phaseLoop.SetPhaseEvac();
        }
    }

    private void Animate()
    {
        if ((_lastInput & 0b0000_1111) > 0)
        {
            _animator.SetBool(AnimRunning, true);
            _animator.SetBool(AnimLeft, (_lastInput & 0b0000_0100) > 0);
            spriteRenderer.flipX = (_lastInput & 0b0000_0100) > 0;
            _isMoving = true;
        }
        else
        {
            _animator.SetBool(AnimRunning, false);
            _isMoving = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var box = other.collider.GetComponent<BoxEntity>();
        if (box != null)
        {
            _animator.SetBool(AnimPush, true);
            pushingBoxRightNow = true;
            other.rigidbody.mass -= 3;
        }

        if (other.collider.tag.Equals("Door"))
        {
            _animator.SetBool(AnimDoor, true);
        }
    }


    private void OnCollisionStay2D(Collision2D other)
    {
        var box = other.collider.GetComponent<BoxEntity>();
        if (box != null)
        {
            if (_isMoving)
                _animator.SetBool(AnimPush, true);
            else
            {
                _animator.SetBool(AnimPush, false);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        var box = other.collider.GetComponent<BoxEntity>();
        if (box != null)
        {
            _animator.SetBool(AnimPush, false);
            pushingBoxRightNow = false;
            other.rigidbody.mass += 3;
        }

        if (other.collider.tag.Equals("Door"))
        {
            _animator.SetBool(AnimDoor, false);
        }
    }
}
