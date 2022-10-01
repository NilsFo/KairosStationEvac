using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewmateController : MonoBehaviour {
    private Rigidbody2D _rigidbody2D;
    public bool userControlled;
    private static int n_frames = 60 * 10; 
    private ushort[] _savedInputs = new ushort[n_frames];
    private int _frame;
    private Vector2 _initialPosition;
    private ushort _lastInput;
    public float speed = 1.5f;
    
    // Start is called before the first frame update
    void Start() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _initialPosition = transform.position;
    }

    void Update() {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            _lastInput = (ushort)(_lastInput | 0b0000_1000);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            _lastInput = (ushort)(_lastInput | 0b0000_0100);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            _lastInput = (ushort)(_lastInput | 0b0000_0010);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            _lastInput = (ushort)(_lastInput | 0b0000_0001);
        }
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E)) {
            _lastInput = (ushort)(_lastInput | 0b0001_0000);
        }
    }

    void Reset() {
        _frame = 0;
        transform.position = _initialPosition;
    }
    private void FixedUpdate() {
        if (_frame >= n_frames) {
            Reset();
        }
        ushort input = 0;
        if (userControlled) {
            _savedInputs [_frame] = _lastInput;
            input = _lastInput;
            //Debug.Log(input);
        } else if (_savedInputs != null) {
            input = _savedInputs [_frame];
        } else {
            // Stand around and die
        }
        ExecuteInput(input);
        
        _lastInput = 0;
        _frame++;
    }

    // 0001 1111
    // xxxI WASD
    private void ExecuteInput(ushort input) {
        Vector2 movement = new Vector2();
        bool interaction = false;
        if ((input & 0b0000001) != 0) {
            movement.x += 1;
        }
        if ((input & 0b0000010) != 0) {
            movement.y -= 1;
        }
        if ((input & 0b0000100) != 0) {
            movement.x -= 1;
        }
        if ((input & 0b0001000) != 0) {
            movement.y += 1;
        }
        if ((input & 0b0010000) != 0) {
            interaction = true;
        }
        if(movement.sqrMagnitude > 0)
            movement.Normalize();
        Move(movement);
        if (interaction) {
            Interact();
        }
    }
    private void Interact() {
        Debug.Log("Character has interacted", this);
    }
    private void Move(Vector2 movement) {
        //Debug.Log(movement);
        var position = transform.position;
        _rigidbody2D.MovePosition(new Vector2(position.x, position.y) + movement * speed * Time.fixedDeltaTime);
    }

    /*private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Default")) {
            _rigidbody2D.
        }
    }*/
}
