using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMechineAnimationControl : MonoBehaviour
{
    Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool jump = GetComponent<BossJump>().active;
        if (jump)
            animator.SetTrigger("Jump");

        bool attack = false;
        for (int i = 0; i <= transform.childCount; i++)
        { 
            if (GetComponentsInChildren<BossShooter>()[i].timer <= 0 && GetComponentsInChildren<BossShooter>()[i].active)
                attack = true;
        }
        
        animator.SetBool("Attack",attack);
    }
}
