using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDestroyer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Projectile" || col.transform.parent.tag == "Projectile")
            Destroy(col.gameObject);
    }
}
