using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    //***** jump is dependant on using ground layers, if you do not touch ground you will not be able to jump*****\\
    //saving collider
    private BoxCollider2D col;
    //checks if you can jump again
    public bool grounded;
    public int ceilinged;
    public float push_from_corners = 0;
    public float ground_ray_len = 0.25f;
    public float ceiling_ray_len = 0.25f;
    //timer to remember jump pressed
    float jump_pressed = 0;
    public float jump_pressed_remember_time = 0.2f;
    //timer to remember grounded
    float grounded_remmber = 0;
    public float grounded_remember_time = 0.2f;
    //check if object is player
    public bool isPlayer = false;
    //for collision check
    LayerMask mask;
    private float colliderHeight;
    private float colliderWidth;
    private Vector2 collider_offset = new Vector2();
    //for small vs big jumps
    private bool lastFrameReleaseButton = false;
    private Rigidbody2D rb;
    //set jump speed and how fast you fall when stopped pressing
    public float jumpMultiplier = 10;
    public float cutJumpSpeed = 0.5f;
    public float jumpHight = 1;
    public float fall_speed_threshold = 10;
    float jumpStartHight;
    public bool canDoubleJump = false;
    public bool unlockDoubleJump = false;

    private bool jumpAxisInUse = false;
    // Start is called before the first frame update
    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        colliderHeight = col.bounds.extents.y;
        colliderWidth = col.bounds.extents.x;
        collider_offset.x = col.offset.x;
        collider_offset.y = col.offset.y;
        mask = LayerMask.GetMask("Ground","Secret");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //checks if you touch a ground layer object
        grounded = isGrounded();
        ceilinged = checkCeiling();
        //if object is player use axis to jump
        if (isPlayer)
        {
            if (rb.velocity.y < 0 && Mathf.Abs(rb.velocity.y) >= fall_speed_threshold && !grounded)
                rb.velocity = new Vector2(rb.velocity.x,-fall_speed_threshold);

            //used to remember if the player pressed jump X time before the player touched the ground and jump at the frame he landed
            jump_pressed -= Time.deltaTime;
            if (Input.GetAxis("Jump") != 0 && !jumpAxisInUse)
            {
                jumpAxisInUse = true;
                jump_pressed = jump_pressed_remember_time;
            }
            //used to remember jump short time after player not grounded
            grounded_remmber -= Time.deltaTime;
            if(grounded)
            {
                grounded_remmber = grounded_remember_time;
            }
            //do the jump
            if (jump_pressed > 0 && grounded_remmber > 0)
            {
                jumpStartHight = gameObject.transform.position.y;
                jump_pressed = 0;
                grounded_remmber = 0;
                rb.velocity = new Vector2(rb.velocity.x, jumpMultiplier);
                grounded = false;
            }
            if (!grounded && Mathf.Abs(ceilinged) == 1)
            {
                push_from_corners = calculate_push_distance(ceilinged) + 0.1f;
                transform.position = new Vector3(transform.position.x + push_from_corners * Mathf.Sign(ceilinged), transform.position.y);
            }
            //stop the jump if the player stopped pressing
            if (Input.GetAxis("Jump") == 0)
            {
                jumpAxisInUse = false;
                if(!grounded)
                    if (rb.velocity.y > 0)
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * cutJumpSpeed);
            }
            //stops the jump if the player reached max jump hight
            if (transform.position.y > jumpStartHight + jumpHight)
                if (rb.velocity.y > 0)
                    rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }
    //checks wether player is above a ground lvl object
    private bool isGrounded ()
    {
        Vector3 position = new Vector3();
        if (gameObject.GetComponent<RightLeft>().facingRight)
            position = new Vector3(transform.position.x + collider_offset.x * 2, transform.position.y + collider_offset.y * 2);
        else
            position = new Vector3(transform.position.x - collider_offset.x * 2, transform.position.y + collider_offset.y * 2);

        Vector3 positionLeft = position ,positionRight = position;
        positionLeft.x += colliderWidth;
        positionRight.x -= colliderWidth;

        ////////////////////////////Debug///////////////////////////////////////
        Debug.DrawRay(position, Vector3.down * (colliderHeight + ground_ray_len));
        Debug.DrawRay(positionLeft, Vector3.down * (colliderHeight + ground_ray_len));
        Debug.DrawRay(positionRight, Vector3.down * (colliderHeight + ground_ray_len));

        return Physics2D.Raycast(position, Vector3.down, colliderHeight + ground_ray_len, mask) ||
            Physics2D.Raycast(positionRight, Vector3.down, colliderHeight + ground_ray_len, mask) ||
            Physics2D.Raycast(positionLeft, Vector3.down, colliderHeight + ground_ray_len, mask);
    }
    //function that returns -1 if there's a ceiling on the right, 1 if on the left and 0 if both or none
    private int checkCeiling()
    {
        Vector3 position = new Vector3();
        if (gameObject.GetComponent<RightLeft>().facingRight)
            position = new Vector3(transform.position.x + collider_offset.x * 2, transform.position.y + collider_offset.y * 2);
        else
            position = new Vector3(transform.position.x - collider_offset.x * 2, transform.position.y + collider_offset.y * 2);

        Vector3 positionLeft = position, positionRight = position;
        positionLeft.x += colliderWidth;
        positionRight.x -= colliderWidth;

        Debug.DrawRay(positionLeft, Vector3.up * (colliderHeight + ceiling_ray_len), Color.yellow);
        Debug.DrawRay(positionRight, Vector3.up * (colliderHeight + ceiling_ray_len), Color.yellow);
        Debug.DrawRay(position, Vector3.up * (colliderHeight + ceiling_ray_len), Color.yellow);

        if (Physics2D.Raycast(positionRight, Vector3.up, colliderHeight + ceiling_ray_len, mask) &&
            Physics2D.Raycast(positionLeft, Vector3.up, colliderHeight + ceiling_ray_len, mask))
            return 0;
        if (Physics2D.Raycast(positionRight, Vector3.up, colliderHeight + ceiling_ray_len, mask))
            if (Physics2D.Raycast(position, Vector3.up, colliderHeight + ceiling_ray_len, mask))
                return 2;
            else
                return 1;
        if (Physics2D.Raycast(positionLeft, Vector3.up, colliderHeight + ceiling_ray_len, mask))
            if (Physics2D.Raycast(position, Vector3.up, colliderHeight + ceiling_ray_len, mask))
                return -2;
            else
                return -1;
        return 0;
    }

    private float calculate_push_distance(int dir)
    {
        Vector3 position = new Vector3();
        if (gameObject.GetComponent<RightLeft>().facingRight)
            position = new Vector3(transform.position.x + collider_offset.x * 2, transform.position.y + collider_offset.y * 2);
        else
            position = new Vector3(transform.position.x - collider_offset.x * 2, transform.position.y + collider_offset.y * 2);

        Vector3 positionLeft = position, positionRight = position;
        positionLeft.x += colliderWidth;
        positionRight.x -= colliderWidth;

        switch (dir)
        {
            case 1:
                for (float i = 0; i < position.x - positionRight.x; i += 0.1f)
                {
                    Debug.DrawRay(new Vector2(positionRight.x + i, positionRight.y), Vector3.up * (colliderHeight + ceiling_ray_len), Color.red, 0.5f);
                    if (!Physics2D.Raycast(new Vector2(positionRight.x + i,positionRight.y), Vector3.up, colliderHeight + ceiling_ray_len, mask))
                        return i;
                }
                break;
            case -1:
                for (float i = 0; i < positionLeft.x - position.x; i += 0.1f)
                {
                    Debug.DrawRay(new Vector2(positionLeft.x - i, positionLeft.y), Vector3.up * (colliderHeight + ceiling_ray_len), Color.red, 0.5f);
                    if (!Physics2D.Raycast(new Vector2(positionLeft.x - i, positionLeft.y), Vector3.up, colliderHeight + ceiling_ray_len, mask))
                        return i;
                }
                break;
        }
        return 0;
    }
}
