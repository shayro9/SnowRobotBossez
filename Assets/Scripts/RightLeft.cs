using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightLeft : MonoBehaviour
{
    //player speed
    public float speed = 10;
    float smooth_speed = 0;
    //Sliding 
    [Range(0.0f,1.0f)]
    public float cutMoveSpeed = 0;

    public float accel = 10;

    //saving collider
    private BoxCollider2D col;
    //for collision check
    LayerMask mask;
    private float colliderHeight;
    private float colliderWidth;
    private Vector2 collider_offset;

    //saving body
    private Rigidbody2D rb;
    //Saves the sprite renderer component
    private SpriteRenderer spriteRenderer;

    public bool facingRight = true;
    public Vector2 move;
    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        colliderHeight = col.bounds.extents.y;
        colliderWidth = col.bounds.extents.x;
        collider_offset = col.offset;
        mask = LayerMask.GetMask("Ground");
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        move = Vector2.zero;
        move.x = Input.GetAxisRaw("Horizontal");
        if (isWallAhead() && facingRight && move.x > 0)
            move = Vector2.zero;
        if (isWallAhead() && !facingRight && move.x < 0)
            move = Vector2.zero;
        if (move.x != 0)
        {
            if (smooth_speed < speed)
            {
                smooth_speed += Time.deltaTime * accel;
                rb.velocity = new Vector2(move.x * smooth_speed, rb.velocity.y);
            }
            else
                rb.velocity = new Vector2(move.x * speed, rb.velocity.y);
        }
        //Debug.DrawLine(rb.position,rb.position + rb.velocity,Color.green);

        //Adjust facing diraction
        if (move.x > 0 && !facingRight)
            Flip();
        else if (move.x < 0 && facingRight)
            Flip();

        //Cut the velocity by a number between 0 and 1 when no button is pressed
        if(move.x == 0)
        {
            smooth_speed = 0;
            rb.velocity = new Vector2(rb.velocity.x*cutMoveSpeed, rb.velocity.y);
        }
    }

    //Flips the Player by changing the scale to (-)scale
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    private bool isWallAhead()
    {
        Vector3 position = new Vector3();
        int facing_factor = 0;

        if (facingRight)
        {
            facing_factor = 1;
            position = new Vector3(transform.position.x + collider_offset.x * 2, transform.position.y + collider_offset.y * 2);
        }
        else
        {
            facing_factor = -1;
            position = new Vector3(transform.position.x - collider_offset.x * 2, transform.position.y + collider_offset.y * 2);
        }

        Vector3 positionUp = position, positionDown = position;
        positionUp.y += colliderHeight;
        positionDown.y -= colliderHeight;
        Debug.DrawRay(position, Vector3.right * (colliderWidth + 0.25f) * facing_factor, Color.magenta);
        Debug.DrawRay(positionUp, Vector3.right * (colliderWidth + 0.25f) * facing_factor, Color.magenta);
        Debug.DrawRay(positionDown, Vector3.right * (colliderWidth + 0.25f) * facing_factor, Color.magenta);

        return Physics2D.Raycast(position, Vector3.right * facing_factor, colliderWidth + 0.25f, mask) ||
            Physics2D.Raycast(positionDown, Vector3.right * facing_factor, colliderWidth + 0.25f, mask) ||
            Physics2D.Raycast(positionUp, Vector3.right * facing_factor, colliderWidth + 0.25f, mask);
    }
}
