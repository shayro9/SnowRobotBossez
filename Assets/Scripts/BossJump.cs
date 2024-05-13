using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossJump : MonoBehaviour
{
    [Range(0,90)]
    public float jump_deg = 45;
    public float jump_distance;
    public float max_h_time;
    public bool active = false;
    public bool facing_right = false;
    public float delay = 0;
    float counter = 0;
    bool count = false;

    float x_scale; 
    Rigidbody2D rb;
    // Update is called once per frame
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        x_scale = transform.localScale.x;
    }

    void Update()
    {
        if (transform.position.x > 0)
        {
            transform.localScale = new Vector3(x_scale, transform.localScale.y);
            facing_right = false;
        }
        else
        {
            transform.localScale = new Vector3(-x_scale, transform.localScale.y);
            facing_right = true;
        }

        if (active || count)
        {
            count = true;
            active = false;
            if (counter >= delay)
            {
                Vector3 NavVector;
                float power;
                if (facing_right)
                {
                    NavVector = new Vector2(-Mathf.Sin((360 - jump_deg) / Mathf.Rad2Deg), Mathf.Cos(jump_deg / Mathf.Rad2Deg));
                    power = Mathf.Sqrt(jump_distance * Physics2D.gravity.y * rb.gravityScale / (-2 * NavVector.x * NavVector.y));
                }
                else
                {
                    NavVector = new Vector2(-Mathf.Sin(jump_deg / Mathf.Rad2Deg), Mathf.Cos(jump_deg / Mathf.Rad2Deg));
                    power = Mathf.Sqrt(-jump_distance * Physics2D.gravity.y * rb.gravityScale / (-2 * NavVector.x * NavVector.y));
                }
                max_h_time = -(power * NavVector.y) / (Physics2D.gravity.y * rb.gravityScale);
                Invoke("ShootDown", max_h_time);
                Invoke("StopShootDown", max_h_time * 2);
                rb.AddForce(NavVector * power, ForceMode2D.Impulse);
                counter = 0;
                count = false;
            }
            else
                counter += Time.deltaTime;
        }

    }

    void ShootDown()
    {
        GetComponent<BossShooter>().active = true;
    }
    void StopShootDown()
    {
        GetComponent<BossShooter>().active = false;
        Debug.Log("Y");
        GetComponent<BossController>().canShoot = true;
    }
}
