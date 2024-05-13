using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWave : MonoBehaviour
{
    public float wave_len;
    public float angle;
    float starting_y;
    public float power;
    // Update is called once per frame
    void Start()
    {
        starting_y = transform.position.y;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, starting_y + Mathf.Sin(transform.position.x * wave_len));
        float x = transform.position.x + power * Time.deltaTime;
        float y = starting_y + Mathf.Sin(x * wave_len);

        Vector3 next_pos = new Vector3(x, y);
        next_pos.x = next_pos.x - transform.position.x;
        next_pos.y = next_pos.y - transform.position.y;
        angle = Mathf.Atan2(next_pos.y, next_pos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
