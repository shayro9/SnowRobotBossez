using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShooter : MonoBehaviour
{
    public float shots_a_sec;
    public float timer = 0;
    [System.Serializable]
    public enum AttackType { ShotGun, Straight, Arc, Wave};
    public AttackType attack_type = AttackType.ShotGun;
    public int shotgun_projectiles = 5;
    public int shotgun_spread_angle = 90;
    public GameObject projectile;
    public float power;
    public bool aim_on_player = false;
    public Vector2 init_aim = Vector2.zero;
    public float arc_distance;
    GameObject player;
    [Range(0, 2)]
    public float wave_len;
    public bool active;

    public Vector3[] getOpeningVectors(float opening_angle, Vector3 dir)
    {
        Vector3 NavVector = dir;
        if (opening_angle == 0)
        {
            Debug.DrawLine(transform.position, transform.position + NavVector * 10, Color.red);
            return new Vector3[] { NavVector };
        }
        float x = opening_angle / 2;
        float Nx, Ny, Ox1, Oy1, Ox2, Oy2, Ox3, Ox4;
        Nx = Mathf.Round(NavVector.x * 100000) / 100000;
        Ny = Mathf.Round(NavVector.y * 100000) / 100000;
        float delta = Mathf.Pow(2 * Mathf.Cos(x / Mathf.Rad2Deg) * Ny, 2) - 4 * (Nx * Nx + Ny * Ny) * (Mathf.Pow(Mathf.Cos(x / Mathf.Rad2Deg), 2) - Nx * Nx);
        Oy1 = (2 * Mathf.Cos(x / Mathf.Rad2Deg) * Ny + Mathf.Sqrt(delta)) / (2 * (Nx * Nx + Ny * Ny));
        Ox1 = Mathf.Sqrt(1 - Oy1 * Oy1);
        Oy2 = (2 * Mathf.Cos(x / Mathf.Rad2Deg) * Ny - Mathf.Sqrt(delta)) / (2 * (Nx * Nx + Ny * Ny));
        Ox2 = Mathf.Sqrt(1 - Oy2 * Oy2);

        Ox4 = -Mathf.Sqrt(1 - Oy2 * Oy2);
        Ox3 = -Mathf.Sqrt(1 - Oy1 * Oy1);
        Vector3[] results = new Vector3[4];
        List<Vector3> opening = new List<Vector3>();
        results[0] = new Vector3(Ox1, Oy1, 0);
        results[1] = new Vector3(Ox2, Oy2, 0);
        results[2] = new Vector3(Ox3, Oy1, 0);
        results[3] = new Vector3(Ox4, Oy2, 0);
        foreach (Vector3 r in results)
        {
            Debug.DrawLine(transform.position, transform.position + r);
            if (Mathf.Abs(Vector3.Angle(NavVector, r) - x) < 0.01)
            {
                Debug.DrawLine(transform.position, transform.position + r * 10, Color.red);
                opening.Add(r);
            }
        }
        Debug.DrawLine(transform.position, transform.position + NavVector);
        return opening.ToArray();
    }

    //Aiming on player
    public Vector3[] getShotDiractions(int rays_num, float opening_angle)
    {
        Vector3 aim_to = (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized;

        Vector3[] dirs = new Vector3[rays_num];
        switch (rays_num)
        {
            case 1:
                dirs[0] = getOpeningVectors(0, aim_to)[0];
                break;
            case 2:
                dirs = getOpeningVectors(opening_angle, aim_to);
                break;
            default:
                if (rays_num % 2 == 0)
                    for (int i = 0; i < rays_num; i++)
                    {
                        float relative_angle = opening_angle / (rays_num - 1);
                        dirs[i] = getOpeningVectors(relative_angle + relative_angle * 2 * (i / 2), aim_to)[i % 2];
                    }
                else
                {
                    dirs[0] = getOpeningVectors(0, aim_to)[0];
                    for (int i = 2; i <= rays_num; i++)
                    {
                        float relative_angle = opening_angle / (rays_num - 1);
                        dirs[i - 1] = getOpeningVectors(relative_angle * 2 * (i / 2), aim_to)[i % 2];
                    }
                }
                break;
        }
        return dirs;
    }

    //aims to a selected point
    public Vector3[] getShotDiractions(int rays_num, float opening_angle, Vector3 aimto)
    {
        Vector3[] dirs = new Vector3[rays_num];
        Vector3 aim_to = (aimto - transform.position).normalized;
        Debug.DrawLine(transform.position,aim_to);
        switch (rays_num)
        {
            case 1:
                dirs[0] = getOpeningVectors(0, aim_to)[0];
                break;
            case 2:
                dirs = getOpeningVectors(opening_angle, aim_to);
                break;
            default:
                if (rays_num % 2 == 0)
                    for (int i = 0; i < rays_num; i++)
                    {
                        float relative_angle = opening_angle / (rays_num - 1);
                        dirs[i] = getOpeningVectors(relative_angle + relative_angle * 2 * (i / 2), aim_to)[i % 2];
                    }
                else
                {
                    dirs[0] = getOpeningVectors(0, aim_to)[0];
                    for (int i = 2; i <= rays_num; i++)
                    {
                        float relative_angle = opening_angle / (rays_num - 1);
                        dirs[i - 1] = getOpeningVectors(relative_angle * 2 * (i / 2), aim_to)[i % 2];
                    }
                }
                break;
        }
        return dirs;
    }
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
            if (timer <= 0)
            {
                Debug.Log(timer + " " + gameObject);
                if (projectile)
                {
                    Vector3[] dirs;
                    switch (attack_type)
                    {
                        case AttackType.ShotGun:
                            if (!aim_on_player)
                                dirs = getShotDiractions(shotgun_projectiles, shotgun_spread_angle, init_aim);
                            else
                                dirs = getShotDiractions(shotgun_projectiles, shotgun_spread_angle);
                            break;
                        case AttackType.Straight:
                            if (!aim_on_player)
                                dirs = getShotDiractions(1, 0, init_aim);
                            else
                                dirs = getShotDiractions(1, 0);
                            break;
                        case AttackType.Arc:
                            dirs = getShotDiractions(1, 0, init_aim);

                            arc_distance = Vector3.Distance(transform.position, player.transform.position);

                            if (transform.position.x > player.transform.position.x)
                                power = Mathf.Sqrt(-arc_distance * Physics2D.gravity.y * projectile.GetComponent<Rigidbody2D>().gravityScale / (-2 * dirs[0].x * dirs[0].y));
                            else
                                power = Mathf.Sqrt(arc_distance * Physics2D.gravity.y * projectile.GetComponent<Rigidbody2D>().gravityScale / (-2 * dirs[0].x * dirs[0].y));
                            break;
                        case AttackType.Wave:
                            if (!aim_on_player)
                                dirs = getShotDiractions(1, 0, init_aim);
                            else
                                dirs = getShotDiractions(1, 0);
                            break;
                        default:
                            dirs = new Vector3[0];
                            break;
                    }

                    Vector3 spawn_pos = new Vector3();
                    spawn_pos = transform.position;
                    foreach (Vector3 dir in dirs)
                    {
                        GameObject newProjectile = Instantiate(projectile, spawn_pos, transform.rotation) as GameObject;
                        if (attack_type == AttackType.Wave)
                        {
                            if (!GetComponentInParent<BossJump>().facing_right)
                                newProjectile.transform.localScale = -newProjectile.transform.localScale;
                            newProjectile.AddComponent<ProjectileWave>();
                            newProjectile.GetComponent<ProjectileWave>().wave_len = wave_len;
                            newProjectile.GetComponent<ProjectileWave>().power = power;
                        }
                        if (!newProjectile.GetComponent<Rigidbody2D>())
                        {
                            newProjectile.AddComponent<Rigidbody2D>();
                        }
                        // Apply force to the newProjectile's Rigidbody component if it has one
                        Vector2 force = power * dir;
                        newProjectile.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
                    }
                }
                timer = 1 / shots_a_sec;
            }
            else
                timer -= Time.deltaTime;
        else
            timer = 0;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 position = transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, init_aim);
    }
}
