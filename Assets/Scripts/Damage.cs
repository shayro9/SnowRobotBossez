using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Damage : MonoBehaviour
{
    public int damageAmount = 0;
    Rigidbody2D rb;
    LayerMask mask;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponentInParent<Health>() != null)
        {
            //dir.Normalize();
            collision.gameObject.GetComponentInParent<Health>().ApplyDamage(damageAmount);
            Destroy(gameObject);
        }
    }
}
