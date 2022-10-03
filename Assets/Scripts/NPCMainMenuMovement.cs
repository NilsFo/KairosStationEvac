using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NPCMainMenuMovement : MonoBehaviour
{
    private Vector3 initialPosition;
    public Vector2 velocity = new Vector2(5, 0);
    public float turnImmunity = 2;
    public bool panicd;

    public Rigidbody2D myBody;
    public Animator myAnimatior;
    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;

        if (panicd)
        {
            myAnimatior.SetBool("panic", true);
        }
        myAnimatior.SetBool("running", true);
    }

    // Update is called once per frame
    void Update()
    {
        turnImmunity = turnImmunity - Time.deltaTime;
        spriteRenderer.flipX = velocity.x < 0;
    }

    private void FixedUpdate()
    {
        Vector3 v = velocity;
        myBody.MovePosition(new Vector2(transform.position.x + velocity.x, transform.position.y + velocity.y));
    }

    public void TurnAround()
    {
        print("turned around");
        if (turnImmunity < 0)
        {
            velocity.x = velocity.x * -1;
            turnImmunity = 2;
        }
    }
}