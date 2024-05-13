using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    public enum SightShape { Square, Circle, Triangle}
    public SightShape shape = SightShape.Circle;
    [Range(0,360)]
    public float sight_angle = 0;
    //sight distance
    public float sightDistance = 30;
    //sight height
    public float sightHeight = 5;
    //can enemy see player
    [HideInInspector] public bool isPlayerVisible = false;
    //did enemy see player within timer time
    [HideInInspector] public bool chasePlayer = false;
    //how much time does the enemy remember player sight
    public float chaseTime = 5;
    //when was player last visible
    private float timeSincePlayerVisible = 0;
    //fix to sightHeight
    private float positionFix = 0;
    //checks enemy direction
    private bool facingRight = true;

    LayerMask mask;
    // Start is called before the first frame update
    void Start()
    {
        mask = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        facingRight = FacingRight();
        //IsSeeingThroughFloor();
        isPlayerVisible = IsPlayerVisible(facingRight);
        if (isPlayerVisible)
        {
            chasePlayer = true;
            timeSincePlayerVisible = 0;
        }
        else
        {
            if (timeSincePlayerVisible < chaseTime)
                timeSincePlayerVisible += Time.fixedDeltaTime;
            else
                chasePlayer = false;
        }
    }
    //checks enemy orientation
    private bool FacingRight()
    {
        return gameObject.GetComponent<EnemyMover>().facingRight;
    }
    //fixes seeing through the floor bug where sightHeight is bigger then distance to ground
    private void IsSeeingThroughFloor()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, sightHeight / 2, LayerMask.GetMask("Ground"));
        Debug.DrawRay(transform.position, Vector2.down * sightHeight / 2, Color.yellow);
        if (hit)
        {
            positionFix = sightHeight / 2 - hit.distance;
        }
        else
            positionFix = 0;
    }
    //checks if player is visible
    private bool IsPlayerVisible(bool facingRight)
    {
        Vector3 position = transform.position;
        position.y += positionFix;
        Vector3 dir = new Vector2(-Mathf.Sin(sight_angle / Mathf.Rad2Deg), Mathf.Cos(sight_angle / Mathf.Rad2Deg));
        if (shape == SightShape.Circle)
        {
            return Physics2D.CircleCast(position, sightDistance, dir, 0, mask, 0);
        }
        else
        {
            if (facingRight)
                return Physics2D.BoxCast(position, new Vector2(1, sightHeight), 0f, Vector2.right, sightDistance, mask);
            else
                return Physics2D.BoxCast(position, new Vector2(1, sightHeight), 0f, Vector2.left, sightDistance, mask);
        }
    }
    //debugging - if enemy is selected in game shows line of sight
    private void OnDrawGizmosSelected()
    {
        Vector3 position = transform.position;
        Gizmos.color = Color.cyan;
        if (shape == SightShape.Circle)
            Gizmos.DrawWireSphere(position, sightDistance);
        else
        {
            if (facingRight)
                position.x += sightDistance / 2;
            else
                position.x -= sightDistance / 2;
            position.y += positionFix;
            Gizmos.DrawWireCube(position, new Vector3(sightDistance, sightHeight, position.z));
        }
    }
}
