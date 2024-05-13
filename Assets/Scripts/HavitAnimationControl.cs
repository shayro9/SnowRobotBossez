using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HavitAnimationControl : MonoBehaviour
{
    public float time_to_explode = 3;
    float timer;
    bool start_count = false;
    public AnimationClip die_animation;

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Unhide") || GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Explode"))
            GetComponent<EnemyMover>().preventing_animation = true;
        else
            GetComponent<EnemyMover>().preventing_animation = false;

        if (GetComponent<EnemyMover>().chase)
        {
            GetComponent<Animator>().SetTrigger("UnHide");
            start_count = true;
        }
        if (start_count)
            if (timer > time_to_explode || GetComponent<EnemyMover>().distance_from_target < 0.5)
            {
                GetComponent<Animator>().SetTrigger("Explode");
                float t = die_animation.length - Time.deltaTime * die_animation.length * 20;
                Invoke("Kill", t);
            }
            else
                timer += Time.deltaTime;
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
