using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public float health;

    [System.Serializable]
    public enum PatternType { Random, Constant}
    [System.Serializable]
    public struct Group { public PatternType type; public int health; }
    public Group[] groups;

    [System.Serializable]
    public enum SpecialAttack { Jump}
    public SpecialAttack special = SpecialAttack.Jump;

    [System.Serializable]
    public struct AttackManager { public int secs; public int health; public GameObject attack; public int group; }
    public AttackManager[] attack_manager;

    public int manager_index = 0;
    public bool canShoot = true;
    bool s_attckd = false;
    int group_index = 0;
    float timer = 0, health_counter = 0, last_health;

    void SwitchAttack(bool isRandom)
    {
        activateShooters(false);
        if(!s_attckd)
            DoSpecialAttack();
        if (canShoot)
        {
            Debug.Log("SS");
            if (isRandom)
                manager_index = Random.Range(0, attack_manager.Length);
            else
            {
                manager_index++;
                if (manager_index >= attack_manager.Length)
                    manager_index = 0;
            }

            while (attack_manager[manager_index].group != group_index)
            {
                if (isRandom)
                    manager_index = Random.Range(0, attack_manager.Length);
                else
                {
                    manager_index++;
                    if (manager_index >= attack_manager.Length)
                        manager_index = 0;
                }
            }
            activateShooters(true);
            health_counter = attack_manager[manager_index].health;
            timer = attack_manager[manager_index].secs;
            s_attckd = false;
        }
    }

    void DoSpecialAttack()
    {
        switch(special)
        {
            case SpecialAttack.Jump:
                GetComponent<BossJump>().active = true;
                canShoot = false;
                s_attckd = true;
                break;
        }
    }
    void activateShooters(bool active)
    {
        BossShooter[] shooters = attack_manager[manager_index].attack.GetComponents<BossShooter>();
        foreach(BossShooter s in shooters)
            s.active = active;
    }

    public float GetMaxHealth()
    {
        return GetComponent<Health>().healthPoints;
    }

    // Start is called before the first frame update
    void Start()
    {
        health = GetMaxHealth();
        last_health = health;
        timer = attack_manager[manager_index].secs;
        health_counter = attack_manager[manager_index].health;

        if (groups[0].type == PatternType.Random)
            while(attack_manager[manager_index].group != 0)
                manager_index = Random.Range(0, attack_manager.Length);
        activateShooters(true);
    }

    // Update is called once per frame
    void Update()
    {
        health = GetComponent<Health>().healthPoints;
        bool ran = false;
        if (health <= groups[group_index].health)
        {
            group_index++;
            SwitchAttack(false);
        }
        if (groups[group_index].type == PatternType.Random)
            ran = true;
        if (attack_manager[manager_index].secs != 0)
        {
            if (timer <= 0)
            {
                SwitchAttack(ran);
            }
            else
                timer -= Time.deltaTime;
        }

        if (attack_manager[manager_index].health != 0)
        {
            if (health_counter <= 0)
            {
                SwitchAttack(ran);
            }
            else if(health < last_health)
            {
                health_counter -= last_health - health;
                last_health = health;
            }
        }

    }
}
