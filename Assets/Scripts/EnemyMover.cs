using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    //Is the current animation that is playing prevents the enemy from moving
    public bool preventing_animation = false;
    //enemy speed
    public enum MoveType { Force, ChangePosition}
    public MoveType move_type = MoveType.Force;
    [HideInInspector]
    public float enemySpeed = 0.2f;
    [Range(0.0f, 30.0f)]
    public float maxEnemySpeed = 0;
    //determines direction
    [HideInInspector] public bool facingRight = true;
    //saving body
    private Rigidbody2D rb;
    //Saves the boxcollider
    private BoxCollider2D col;

    public GameObject Platform;
    public float platformX;
    public float platformY;
    public Vector2 platformCenter;

    public bool is_flying = false;
    public float fly_hight;
    private bool high_set;
    [HideInInspector]
    public bool chase;
    [Range(0,20)]
    public float fly_distance;
    public Vector2[] Targets;
    [HideInInspector]
    public float distance_from_target;
    private Vector2[] temp_targets = new Vector2[] { };
    int target_index = 0;

    public float knockback;
    public float knockbackTime;
    [HideInInspector]
    public float knockbackTimeCount;

    bool canPatrol = true;
    public bool wait_between_moves = false;
    public float dazedTime;
    [HideInInspector]
    public bool knockbackRight;

    //mask for when to flip enemy
    LayerMask mask;
    //mask for when to jump
    LayerMask maskWall;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        //if enemy hits player or himself makes him not flip
        mask = ~LayerMask.GetMask("Player", "Enemy");
        //if enemy hits himself he shouldn't jump
        maskWall = ~LayerMask.GetMask("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        chase = GetComponent<EnemySight>().chasePlayer;
        if(!preventing_animation)
            if (Targets.Length == 0)
                if (chase)
                    canPatrol = true;
                else
                    canPatrol = false;
            else
                canPatrol = true;
        else
            canPatrol = false;

        if (knockbackTimeCount <= 0)
        {
            //patrol the platform if player is not in sight
            if (canPatrol)
            {
                Vector3 movement;
                //----------------Get if to chase the player from sight script------------

                if (chase)
                {
                    //------------there are always 2 or less chase targets----------------
                    target_index = target_index % 2;
                    //------------saves original movement---------------------------------
                    if (temp_targets.Length == 0)
                        temp_targets = Targets;

                    Vector2 player_pos = GameObject.FindGameObjectWithTag("Player").transform.position;

                    if (is_flying)
                    {
                        if (high_set)
                        {
                            fly_hight = player_pos.y + fly_hight;
                            high_set = false;
                        }
                        Targets = new Vector2[]
                        {new Vector2(player_pos.x - fly_distance,fly_hight),new Vector2(player_pos.x + fly_distance,fly_hight) };
                    }
                    else
                        Targets = new Vector2[]
                        {new Vector2(player_pos.x,player_pos.y)};
                }
                //-------------If he is not chasing reset targets----------------------
                else
                {
                    if (temp_targets.Length != 0)
                    {
                        Targets = temp_targets;
                        temp_targets = new Vector2[] { };
                    }
                    high_set = true;
                }

                //----------------Change Sprite to match diraction------------------------
                if (transform.position.x > Targets[target_index].x && facingRight)
                    Flip();
                else if (transform.position.x < Targets[target_index].x && !facingRight)
                    Flip();
                //----------------Calculate distance from target--------------------------
                distance_from_target = Vector2.Distance(transform.position, Targets[target_index]);

                //-------------If close enough -> switch target-----------------
                if (distance_from_target < 1)
                {
                    rb.velocity = rb.velocity/2;
                    //-------Enemy can rest in place between targets
                    if (wait_between_moves)
                    {
                        canPatrol = false;
                        float standingTime = Random.Range(1, 4);
                        Invoke("AllowPatrol", standingTime);
                    }
                    if (target_index == Targets.Length - 1)
                        target_index = 0;
                    else
                        target_index++;
                }

                //flips the enemy if needed
                if (Platform)
                    CheckDirectionFlip();

                //---------------Moves enemy----------------

                switch(move_type)
                {
                    case MoveType.Force:
                        movement = (new Vector3(Targets[target_index].x, Targets[target_index].y) - transform.position);
                        movement = new Vector3(movement.x, movement.y * Mathf.Abs(transform.position.y - Targets[target_index].y));
                        if (Mathf.Abs(movement.y) < 0.5)
                            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
                        Debug.DrawLine(transform.position, transform.position + movement);
                        rb.AddForce(movement.normalized * maxEnemySpeed);
                        break;
                    case MoveType.ChangePosition:
                        movement = Vector2.MoveTowards(transform.position, Targets[target_index], Time.deltaTime * maxEnemySpeed);
                        transform.position = movement;
                        break;

                }
                //Debug.DrawLine(rb.position, rb.position + rb.velocity, Color.green);
            }
            else
                rb.velocity = Vector2.zero;
        }
        else
        {
            if (knockbackRight)
            {
                rb.velocity = new Vector2(knockback, 0);
            }
            else
                rb.velocity = new Vector2(-knockback, 0);
            knockbackTimeCount -= Time.deltaTime;
            canPatrol = false;
        }
    }

    //checks if enemy is about to fall off platform and then flips accordingly
    private void CheckDirectionFlip()
    {
        Vector3 position = transform.position;
        if (facingRight)
            position.x += col.bounds.extents.x + 0.5f;
        else
            position.x -= col.bounds.extents.x + 0.5f;
        //checks if about to fall off platform
        RaycastHit2D fallingFromPlatform = Physics2D.Raycast(position, Vector2.down, col.bounds.extents.y + 0.5f);
        //debug
        Debug.DrawRay(position, Vector2.down * (col.bounds.extents.y + 0.5f), Color.yellow);
        //if enemy is falling from platform flip him
        if (!fallingFromPlatform)
        {
            Targets[0].x = Random.Range(platformCenter.x - platformX, platformCenter.x + platformX);
            Targets[0].y = platformCenter.y + platformY;
            Flip();
        }
    }

    //Flips the enemy by changing the scale to (-)scale
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    public void AllowPatrol()
    {
        canPatrol = true;
    }

    void OnDrawGizmosSelected()
    {
        if (Targets.Length > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < Targets.Length; i++)
                Gizmos.DrawWireSphere(Targets[i], 0.2f);
        }
    }
}
