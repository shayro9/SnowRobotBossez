using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDiraction : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector2 dir = gameObject.GetComponent<Rigidbody2D>().velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
