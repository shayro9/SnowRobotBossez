using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationControl : MonoBehaviour
{
    Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        bool run = GetComponent<RightLeft>().move.x != 0;
        bool dash = GetComponent<Dash>().isDashing;
        bool jump = !GetComponent<Jump>().grounded;

        if (run && jump || run && dash)
            animator.SetBool("run", !run);
        else
            animator.SetBool("run", run);

        if(jump && dash)
            animator.SetBool("Jump", !jump);
        else
            animator.SetBool("Jump", jump);

        animator.SetBool("Dash",dash);
    }
}
