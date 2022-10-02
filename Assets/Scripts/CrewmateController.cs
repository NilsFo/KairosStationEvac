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
    private bool mouseOver = false;
    private GamePhaseLoop _phaseLoop;

    private Rigidbody2D _rigidbody2D;
    public bool playerControlled;
    private static int n_frames = 50 * 10;
    private ushort[] _savedInputs = new ushort[n_frames];
    private int _frame;
    private Vector2 _initialPosition;
    private ushort _lastInput;
    public float speed = 1.5f;
    private bool _inputDown;
    public bool startFlipped;

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
    private static readonly int AnimDoor = Animator.StringToHash("door");
    private static readonly int AnimDeath = Animator.StringToHash("death");

    public bool SelectedForEvac => Game.selectedCrewmate == this;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        alive = true;
        _phaseLoop = FindObjectOfType<GamePhaseLoop>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = spriteRenderer.GetComponent<Animator>();
        _initialPosition = transform.position;
    }

    void Update()
    {
        if (Game.currentPhase == GameState.Phase.EvacuationPhase && playerControlled && alive)
        {
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
        
        GetComponent<Rigidbody2D>().simulated = true;

        _animator.ResetTrigger(AnimDeath);
        _animator.Play("crewmate_idle");

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

            _lastInput = 0;
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
        _rigidbody2D.MovePosition(new Vector2(position.x, position.y) + movement * speed * Time.fixedDeltaTime);
    }

    public override void PhaseEvacuate()
    {
        if (playerControlled)
        {
            //Debug.Log("Deleting inputs");
            _savedInputs = new ushort[n_frames];
            _animator.SetBool(AnimPanic, false);
        } else {
            _animator.SetBool(AnimPanic, true);
        }

        UpdateSelector();
        if (SelectedForEvac)
        {
            Game.DisplayFloatingText(transform.position, "'Get me out of here!'", 5);
        }
    }

    public override void PhasePlanning()
    {
        UpdateSelector();

        // Set visuals
        _animator.SetBool(AnimRunning, false);
        _animator.SetBool(AnimLeft, startFlipped);
        _animator.SetBool(AnimPanic, false);
        spriteRenderer.flipX = startFlipped;
    }

    public void UpdateSelector()
    {
        GameState.Phase currentPhase = Game.currentPhase;

        if (currentPhase != GameState.Phase.PlanningPhase)
        {
            selectorSmall.SetActive(false);
            selectorBig.SetActive(false);
            return;
        }

        if (mouseOver || SelectedForEvac)
        {
            selectorSmall.SetActive(true);
            selectorBig.SetActive(false);
        }
        else
        {
            selectorSmall.SetActive(false);
            selectorBig.SetActive(true);
        }
    }

    public void Rescue()
    {
        if (!rescued)
        {
            Game.DisplayFloatingText(transform.position, "'I am safe!'", 5);
            graphicsObj.SetActive(false);
            rescued = true;
            Game.CheckWinCondition();
        }

        // if (Game.currentPhase==GameState.Phase.EvacuationPhase && SelectedForEvac) {
        //     _phaseLoop.NextPhase();
        // }
    }

    public void Kill()
    {
        if (!alive)
        {
            Debug.LogError("Do you want to kill something that is already dead?????");
            Game.currentPhase = GameState.Phase.Unknown;
            return;
        }

        Game.DisplayFloatingText(transform.position, "'I am dead. No big surprise.'");
        myHitbox.enabled = false;
        graphicsObj.SetActive(false);
        //graphicsObj.SetActive(false);
        _animator.SetTrigger(AnimDeath);
        GetComponent<Rigidbody2D>().simulated = false;
        alive = false;
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

    private void OnMouseDown()
    {
        print("A crewmate has been clicked.");
        if (Game.currentPhase == GameState.Phase.PlanningPhase)
        {
            Game.selectedCrewmate = this;
            _phaseLoop.NextPhase();
        }
    }

    private void Animate()
    {
        if ((_lastInput & 0b0000_1111) > 0)
        {
            _animator.SetBool(AnimRunning, true);
            _animator.SetBool(AnimLeft, (_lastInput & 0b0000_0100) > 0);
            spriteRenderer.flipX = (_lastInput & 0b0000_0100) > 0;
        }
        else
        {
            _animator.SetBool(AnimRunning, false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        var box = other.collider.GetComponent<BoxEntity>();
        if (box != null) {
            _animator.SetBool(AnimPush, true);
            other.rigidbody.mass -= 3;
        }

        if (other.collider.tag.Equals("Door")) {
            _animator.SetBool(AnimDoor, true);
        }
    }
    
    
    private void OnCollisionStay2D(Collision2D other) {
        var box = other.collider.GetComponent<BoxEntity>();
        if (box != null) {
            _animator.SetBool(AnimPush, true);
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        var box = other.collider.GetComponent<BoxEntity>();
        if (box != null) {
            _animator.SetBool(AnimPush, false);
            other.rigidbody.mass += 3;
        }
        if (other.collider.tag.Equals("Door")) {
            _animator.SetBool(AnimDoor, false);
        }
        
    }
}
