using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Archer : MonoBehaviour
{
    [SerializeField]
    GameObject prefabArrow;

    public float arrowVelocity = 500f;
    private float elapsedSeconds = 0f;

    public List<string> enemyTag = new List<string>();
    public float runRange = 5f;
    public float attackRangeSoldier = 10f;  
    public float engageRangeSoldier = 20f;  // Engagement range (move towards enemy)

    public int health = 150;
    public int attackDamage = 5;  // Set attack damage to 10
    public float attackCooldown = 2f;
    public bool isPanicked = false; // New flag for panic mode
    public float normalSpeed = 3.5f; // Default NavMeshAgent speed
    public float panicSpeed = 5f;

    private float lastAttackTime;
    private NavMeshAgent navMeshAgent;
    public bool isEngaged = false;

    void Start()
    {

        navMeshAgent = GetComponent<NavMeshAgent>();

        if (gameObject.CompareTag("RedArcher"))
        {
            enemyTag.Add("BlueSoldier");
            enemyTag.Add("BlueCavalry");
            enemyTag.Add("BlueArcher");
        }
        else if (gameObject.CompareTag("BlueArcher"))
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
            if (distance <= runRange)
            {
                //Attack(nearestEnemy);
            }
            else if (distance <= attackRangeSoldier)
            {
                if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
                {
                    navMeshAgent.SetDestination(transform.position); 
                    ShootBow();
                }
            }
            else if (distance <= engageRangeSoldier)
            {
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
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (enemy.tag == "BlueSoldier" || enemy.tag == "RedSoldier")
            {
                SoldierHealth enemySoldier = enemy.GetComponent<SoldierHealth>();
                if (enemySoldier != null)
                {
                    enemySoldier.TakeDamage(attackDamage);
                }
                lastAttackTime = Time.time;
            }
            if (enemy.tag == "BlueCavalry" || enemy.tag == "RedCavalry")
            {
                Cavalry enemySoldier = enemy.GetComponent<Cavalry>();
                if (enemySoldier != null)
                {
                    enemySoldier.TakeDamage(attackDamage);
                }
                lastAttackTime = Time.time;
            }
            if (enemy.tag == "BlueArcher" || enemy.tag == "RedArcher")
            {
                Archer enemySoldier = enemy.GetComponent<Archer>();
                if (enemySoldier != null)
                {
                    enemySoldier.TakeDamage(attackDamage);
                }
                lastAttackTime = Time.time;
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

    public void ShootBow()
    {
        elapsedSeconds += Time.deltaTime;

        if (elapsedSeconds > 1f)
        {
            GameObject nearestEnemy = FindNearestEnemy();
            if (nearestEnemy == null) return;

            Vector3 arrowDirection = nearestEnemy.transform.position - transform.position;
            arrowDirection.Normalize();

            GameObject arrow = Instantiate(prefabArrow, transform.position, transform.rotation);
            Vector3 change = new Vector3(0, 1f, 0);
            Vector3 final = change + arrowDirection;
            final = final.normalized;
            arrow.GetComponent<Rigidbody>().AddForce(arrowVelocity * final);

            Physics.IgnoreCollision(arrow.GetComponent<Collider>(), GetComponent<Collider>());

            elapsedSeconds = 0f;
        }
    }
}
