using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float healthPoints = 1f;
    public float respawnHealthPoints = 1f;      //base health points

    public enum Armour { AddedHealth,ReducedDamage}
    public Armour armourMethod = Armour.AddedHealth;
    public float armourPoints = 0f;

    public bool isImmune = false;

    public enum DoOnDeath { Respawn,Die}
    public DoOnDeath OnDeath = DoOnDeath.Respawn;

    public string SceneToLoad = "";

    private Vector3 respawnPosition;
    private Quaternion respawnRotation;

    public bool FollowHealth = false;
    bool UICreated = false;
    public Slider HealthBar;
    public Image armour;

    // Start is called before the first frame update
    void Start()
    {
        // store initial position as respawn location
        respawnPosition = transform.position;
        respawnRotation = transform.rotation;

        if (SceneToLoad == "") // default to current scene 
        {
            SceneToLoad = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }
    }

    void Awake()
    {
       Collider2D playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
       if(gameObject.tag.Equals("Enemy"))
        {
            Physics2D.IgnoreCollision(playerCollider, gameObject.GetComponent<Collider2D>(), true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //Update the UI
        if (HealthBar != null && armour != null)
        {
            if (armourPoints == 0)
                armour.gameObject.SetActive(false);
            else
                armour.gameObject.SetActive(true);
            armour.GetComponentInChildren<Text>().text = "Armour" + "\n" + armourPoints;
            HealthBar.value = healthPoints * 100 / respawnHealthPoints;
            HealthBar.GetComponentInChildren<Text>().text = "Health - " + healthPoints + " / " + respawnHealthPoints;
        }

        if(FollowHealth && HealthBar!=null)
        {
            if (healthPoints < respawnHealthPoints && !UICreated)
            {
                HealthBar = Instantiate(HealthBar, GameObject.Find("DynamicHealthBars").transform);
                HealthBar.name = "HealthFor" + gameObject.name;
                UICreated = true;
            }
            if (UICreated)
            {
                Vector3 posAboveObject = transform.position + new Vector3(0, transform.localScale.y / 2, 0);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(posAboveObject);
                HealthBar.transform.position = screenPos;
                HealthBar.value = healthPoints * 100 / respawnHealthPoints;

                if (healthPoints == respawnHealthPoints && UICreated)
                {
                    HealthBar.gameObject.SetActive(false);
                }
                if (healthPoints < respawnHealthPoints && UICreated)
                {
                    HealthBar.gameObject.SetActive(true);
                }
            }
        }

        // if the object is 'dead'
        if (healthPoints <= 0)
        {
            switch (OnDeath)
            {
                case DoOnDeath.Respawn:
                    transform.position = respawnPosition;   // reset the player to respawn position
                    transform.rotation = respawnRotation;
                    healthPoints = respawnHealthPoints; // give the player full health again
                    break;
                case DoOnDeath.Die:
                    Destroy(this.gameObject);
                    if (GameObject.Find("HealthFor" + gameObject.name) != null)
                        Destroy(GameObject.Find("HealthFor" + gameObject.name));
                    break;
            }
        }
    }

    public void ApplyDamage(float amount)
    {
        if (!isImmune)
        {
            switch(armourMethod)
            {
                case Armour.AddedHealth:
                    if (armourPoints == 0)
                    {
                        healthPoints = healthPoints - amount;
                    }
                    else if (amount <= armourPoints)
                    {
                        armourPoints = armourPoints - amount;
                    }
                    else
                    {
                        healthPoints = healthPoints + armourPoints - amount;
                        armourPoints = 0;
                    }
                    break;
                case Armour.ReducedDamage:
                    if(amount- armourPoints > 0)
                        healthPoints = healthPoints - (amount - armourPoints);
                    break;
            }
        }
    }

    public void ApplyHeal(float amount)
    {
        if (healthPoints + amount < respawnHealthPoints)
            healthPoints = healthPoints + amount;
        else
            healthPoints = respawnHealthPoints;
    }

    public void AddArmour(float amount)
    {
        armourPoints = armourPoints + amount;
    }

    public void updateRespawn(Vector3 newRespawnPosition, Quaternion newRespawnRotation)
    {
        respawnPosition = newRespawnPosition;
        respawnRotation = newRespawnRotation;
    }
}
