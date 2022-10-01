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
    
    // Start is called before the first frame update
    void Start() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _initialPosition = transform.position;
    }

    void Reset() {
        _frame = 0;
        transform.position = _initialPosition;
    }
    private void FixedUpdate() {
        if (_frame > n_frames) {
            Reset();
        }
        ushort input = 0;
        if (userControlled) {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                input = (ushort)(input | 0b0000_1000);
            }
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                input = (ushort)(input | 0b0000_0100);
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                input = (ushort)(input | 0b0000_0010);
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                input = (ushort)(input | 0b0000_0001);
            }
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)) {
                input = (ushort)(input | 0b0001_0000);
            }
            _savedInputs [_frame] = input;
        } else if (_savedInputs != null) {
            input = _savedInputs [_frame];
        } else {
            // Stand around and die
        }
        ExecuteInput(input);
        
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

        Move(movement);
        if (interaction) {
            Interact();
        }
    }
    private void Interact() {
        Debug.Log("Character has interacted", this);
    }
    private void Move(Vector2 movement) {
        _rigidbody2D.AddForce(movement * Time.fixedDeltaTime);
    }
}
