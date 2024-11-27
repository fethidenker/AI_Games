using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cavalry : MonoBehaviour
{
    public List<string> enemyTag = new List<string>();
    public float attackRangeSoldier = 1f;  // Melee attack range 
    public float engageRangeSoldier = 3f;  // Engagement range (move towards enemy)

    public int health = 150;
    public int attackDamage = 10;  
    
    public float attackCooldown = 1.5f;
    public bool isPanicked = false; // New flag for panic mode
    public float normalSpeed = 5f; // Default NavMeshAgent speed
    public float panicSpeed = 7f;

    private float lastAttackTime;
    private float lastSpecialDamage;
    private NavMeshAgent navMeshAgent;
    public bool isEngaged = false;

    public int chargeDamage = 30;
    public int specialAttack = 2;
    public float specialRecharge = 15f;

    public bool isCharge = false;

    void Start()
    {

        navMeshAgent = GetComponent<NavMeshAgent>();

        if (gameObject.CompareTag("RedCavalry"))
        {
            enemyTag.Add("BlueSoldier");
            enemyTag.Add("BlueCavalry");
            enemyTag.Add("BlueArcher");
        }
        else if (gameObject.CompareTag("BlueCavalry"))
        {
            enemyTag.Add("RedSoldier");
            enemyTag.Add("RedCavalry");
            enemyTag.Add("RedArcher");
        }


    }

    void Update()
    {
        GameObject nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, nearestEnemy.transform.position);

            if (distance <= attackRangeSoldier)
            {
                // Stop moving and attack the enemy
                if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
                {
                    navMeshAgent.SetDestination(transform.position); // Stop the agent
                }
                Attack(nearestEnemy);
            }
            else if (distance <= engageRangeSoldier)
            {
                // Move toward the enemy if within detection range but outside attack range
                if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
                {
                    navMeshAgent.SetDestination(nearestEnemy.transform.position);
                }
            }
        }
    }

    public GameObject FindNearestEnemy()
    {
        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (string tag in enemyTag)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject enemy in enemies)
            {
                if (enemy == null) continue;

                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }

        return nearestEnemy;
    }

    public void Attack(GameObject enemy)
    {
        if(Time.time - lastSpecialDamage <= specialRecharge)
        {
            specialAttack = 2;
        }
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (enemy.tag == "BlueSoldier" || enemy.tag == "RedSoldier") 
            {
                SoldierHealth enemySoldier = enemy.GetComponent<SoldierHealth>();
                if (enemySoldier != null)
                {
                    if (specialAttack != 0)
                    {
                        enemySoldier.TakeDamage(chargeDamage);
                        specialAttack -= 1;
                        if (specialAttack == 0)
                        {
                            lastSpecialDamage = Time.time;
                        }
                    }
                    else
                    {
                        enemySoldier.TakeDamage(attackDamage);
                    }
                }
                lastAttackTime = Time.time;

            }
            if (enemy.tag == "BlueCavalry" || enemy.tag == "RedCavalry")
            {

                Cavalry enemySoldier = enemy.GetComponent<Cavalry>();
                if (enemySoldier != null)
                {
                    if (specialAttack != 0)
                    {
                        enemySoldier.TakeDamage(chargeDamage);
                        specialAttack -= 1;
                        if (specialAttack == 0)
                        {
                            lastSpecialDamage = Time.time;
                        }
                    }
                    else
                    {
                        enemySoldier.TakeDamage(attackDamage);
                    }
                    lastAttackTime = Time.time;
                }
            }
            if (enemy.tag == "BlueArcher" || enemy.tag == "RedArcher")
            {

                Archer enemySoldier = enemy.GetComponent<Archer>();
                if (enemySoldier != null)
                {
                    if (specialAttack != 0)
                    {
                        enemySoldier.TakeDamage(chargeDamage);
                        specialAttack -= 1;
                        if (specialAttack == 0)
                        {
                            lastSpecialDamage = Time.time;
                        }
                    }
                    else
                    {
                        enemySoldier.TakeDamage(attackDamage);
                    }
                    lastAttackTime = Time.time;
                }
            }

        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return health <= 0;
    }
}
