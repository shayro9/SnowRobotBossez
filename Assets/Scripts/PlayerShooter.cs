using UnityEngine;
using System.Collections;

public class PlayerShooter : MonoBehaviour
{
    // Reference to projectile prefab to shoot
    public GameObject projectile;
    public float power = 10.0f;
    Vector2 dir;


    // Reference to AudioClip to play
    public AudioClip shootSFX;

    // Update is called once per frame
    void Update()
    {
        // Detect if fire button is pressed
        if (Input.GetButtonDown("Fire1"))
        {
            // if projectile is specified
            if (projectile)
            {
                Vector3 spawn_pos = new Vector3();
                if (Input.GetAxisRaw("Vertical") > 0.1)
                {
                    spawn_pos = new Vector3(transform.position.x, transform.position.y + transform.localScale.y / 2, transform.position.z);
                    dir = Vector2.up;
                }
                else if (Input.GetAxisRaw("Vertical") < -0.1 && !gameObject.GetComponent<Jump>().grounded)
                {
                    spawn_pos = new Vector3(transform.position.x, transform.position.y - transform.localScale.y / 2, transform.position.z);
                    dir = Vector2.down;
                }
                else
                {
                    spawn_pos = new Vector3(transform.position.x + transform.localScale.x / 2, transform.position.y, transform.position.z);
                    if (gameObject.GetComponent<RightLeft>().facingRight)
                        dir = Vector2.right;
                    else
                        dir = Vector2.left;
                }

                GameObject newProjectile = Instantiate(projectile,spawn_pos, transform.rotation) as GameObject;

                // if the projectile does not have a rigidbody component, add one
                if (!newProjectile.GetComponent<Rigidbody2D>())
                {
                    newProjectile.AddComponent<Rigidbody2D>();
                }
                // Apply force to the newProjectile's Rigidbody component if it has one
                Vector2 force = power * dir;
                newProjectile.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

                // play sound effect if set
                /*if (shootSFX)
                {
                    if (newProjectile.GetComponent<AudioSource>())
                    { // the projectile has an AudioSource component
                      // play the sound clip through the AudioSource component on the gameobject.
                      // note: The audio will travel with the gameobject.
                        newProjectile.GetComponent<AudioSource>().PlayOneShot(shootSFX);
                    }
                    else
                    {
                        // dynamically create a new gameObject with an AudioSource
                        // this automatically destroys itself once the audio is done
                        AudioSource.PlayClipAtPoint(shootSFX, newProjectile.transform.position);
                    }
                }*/
            }
        }
    }
}