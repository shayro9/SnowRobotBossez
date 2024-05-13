using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    //Dashing numbers
    public float dashSpeed = 0;
    public float dashDistance = 0;
    float tempDashDistance = 0;
    public float cooldown = 0;
    private float cooldownCounter = 0; 
    [HideInInspector]public int dashDir; //Diraction of the dash

    //saving body
    private Rigidbody2D rb;
    //saving collider
    private BoxCollider2D col;
    //for collision check
    LayerMask mask;
    private float colliderHeight;
    private float colliderWidth;
    private Vector2 collider_offset;

    [HideInInspector]public float dashTime; //the time you change the velocity (Distance / speed)
    [HideInInspector]public bool isDashing= false; 
    private bool onCooldown = false;
    private bool Fire2axisInUse; //flag so the fire1 Axis acts like button down 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        colliderHeight = col.bounds.extents.y;
        colliderWidth = col.bounds.extents.x;
        collider_offset = col.offset;
        mask = LayerMask.GetMask("Ground");
    }

    void FixedUpdate()
    {
        //you can dash only if you pressed the fire1 button, you are not already dashing 
        //and you are not on cooldown
        if (Input.GetAxis("Fire2") != 0 && !Fire2axisInUse && !isDashing && !onCooldown)
        {
            DoDash();
            Fire2axisInUse = true;
        }

        //************************************************************
        
        //reset the flag
        if (Input.GetAxis("Fire2") == 0)
        {
            Fire2axisInUse = false;
        }

        //Change dash Velocity for the duration of dash Time
        if (dashTime>0)
        {
            int push_dir = isWallAhead();
            float push_from_corner = calculate_push_distance(push_dir);
            if (Mathf.Abs(isWallAhead()) == 1)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + push_from_corner * Mathf.Sign(push_dir));
                Debug.Log(push_dir + " " + push_from_corner);
            }

            rb.velocity = new Vector2(dashDir * dashSpeed, 0);
            dashTime -= Time.deltaTime;
        }

        //When the timer ends, set cooldown and stop the dash
        if(dashTime <= 0 && isDashing)
        {
            //rb.velocity = Vector2.zero;
            isDashing = false;
            onCooldown = true;
            //gameObject.GetComponent<Health>().isImmune = false;
        }

        if(onCooldown)
        {
            cooldownCounter -= Time.deltaTime;
        }
        if (cooldownCounter <= 0)
        {
            onCooldown = false;
        }
    }
    //Calculate the dashing time, set the cooldown and set the diraction of the dash
    public void DoDash()
    {
        cooldownCounter = cooldown;
        dashTime = dashDistance / dashSpeed;
        isDashing = true;
        //GetComponent<Health>().isImmune = true;
        if (gameObject.GetComponent<RightLeft>().facingRight)
        {
            dashDir = 1;
            return;
        }
        dashDir = -1;
    }

    private int isWallAhead()
    {
        Vector3 position = new Vector3();
        int facing_factor = 0;

        if (gameObject.GetComponent<RightLeft>().facingRight)
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

        bool up_check = Physics2D.Raycast(positionUp, Vector3.right * facing_factor, colliderWidth + 0.25f, mask),
             down_check = Physics2D.Raycast(positionDown, Vector3.right * facing_factor, colliderWidth + 0.25f, mask),
             center_check = Physics2D.Raycast(position, Vector3.right * facing_factor, colliderWidth + 0.25f, mask);

        if (up_check && down_check)
            return 0;
        if (up_check)
            if (center_check)
                return -2;
            else
                return -1;
        if (down_check)
            if (center_check)
                return 2;
            else
                return 1;
        return 0;   
    }

    private float calculate_push_distance(int dir)
    {
        Vector3 position = new Vector3();
        int facing_factor = 0;

        if (gameObject.GetComponent<RightLeft>().facingRight)
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

        switch (dir)
        {
            case -1:
                for (float i = 0; i < positionUp.y - position.y; i += 0.1f)
                {
                    Debug.DrawRay(new Vector2(positionUp.x, positionUp.y - i), Vector3.right * facing_factor * (colliderHeight + 0.25f), Color.red, 0.5f);
                    if (!Physics2D.Raycast(new Vector2(positionUp.x, positionUp.y - i), Vector3.right * facing_factor, colliderHeight + 0.25f, mask))
                        return i;
                }
                break;
            case 1:
                for (float i = 0; i < position.y - positionDown.y; i += 0.1f)
                {
                    Debug.DrawRay(new Vector2(positionDown.x, positionDown.y + i), Vector3.right * facing_factor * (colliderHeight + 0.25f), Color.red, 0.5f);
                    if (!Physics2D.Raycast(new Vector2(positionDown.x, positionDown.y + i), Vector3.right * facing_factor, colliderHeight + 0.25f, mask))
                        return i;
                }
                break;
        }
        return 0;
    }
}
