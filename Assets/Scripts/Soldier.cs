using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviour
{
    protected List<string> enemyTag = new List<string>();
    protected float attackRangeSoldier;  // Melee attack range 
    protected float engageRangeSoldier;  // Engagement range (move towards enemy)
    public int health;
    protected NavMeshAgent navMeshAgent;

    virtual protected void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
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

    virtual public void Attack(GameObject enemy) {}
    virtual public void Attack(GameObject enemy, bool isCharge) { }
    virtual public void Attack(GameObject enemy, float distance) { }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return health <= 0;
    }
}
