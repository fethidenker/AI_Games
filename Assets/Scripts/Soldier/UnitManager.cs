using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
    [SerializeField]
    GameObject prefabCapsule;
    
    public List<GameObject> soldiers;
    public float spacing = 2f;       
    public List<string> enemyTag = new List<string>();
    public List<string> allyTag = new List<string>();
    public float engageRange = 3f;   
    public float attackRange = 2f;   

    public Vector3 unitCenter;
    private bool anySoldierEngaged = false; 
    public bool isPanicked = false;
    private bool hasArrangedAfterEngagement = false;

    private void Awake()
    {
        for (int i = 0; i < 9; i++)
        {
            GameObject soldier = Instantiate<GameObject>(prefabCapsule, transform);
            Renderer rend = soldier.GetComponent<Renderer>();

            if (gameObject.CompareTag("RedSoldierUnit"))
            {
                soldier.tag = "RedSoldier";
                rend.material.color = Color.red;
            }
            else if (gameObject.CompareTag("BlueSoldierUnit"))
            {
                soldier.tag = "BlueSoldier";
                rend.material.color = Color.blue;
            }

            soldiers.Add(soldier);
        }
    }
    private void Start()
    {
        

        if (gameObject.CompareTag("RedSoldierUnit"))
        {
            enemyTag.Add("BlueSoldier");
            enemyTag.Add("BlueCavalry");
            enemyTag.Add("BlueArcher");

            allyTag.Add("RedSoldier");
            allyTag.Add("RedCavalry");
            allyTag.Add("RedArcher");

        }
        else if (gameObject.CompareTag("BlueSoldierUnit"))
        {
            enemyTag.Add("RedSoldier");
            enemyTag.Add("RedCavalry");
            enemyTag.Add("RedArcher");

            allyTag.Add("BlueSoldier");
            allyTag.Add("BlueCavalry");
            allyTag.Add("BlueArcher");
        }

        ArrangeGridInPlace();
    }

    
    private void Update()
    {

        for (int i = soldiers.Count - 1; i >= 0; i--) // Iterate backward
        {
            if (soldiers[i] == null || !soldiers[i].activeInHierarchy)
            {
                soldiers.RemoveAt(i); // Safely remove the soldier
            }
        }
        if (soldiers.Count == 0) 
        { 
            Destroy(gameObject);
        }

        unitCenter = CalculateGroupCenter();
        Panic(unitCenter);

        if (!isPanicked)
        {
            HandleEngagement();
        }
    }
    private void HandleEngagement()
    {
        anySoldierEngaged = false;

        foreach (GameObject soldier in soldiers)
        {
            if (soldier == null) continue;

            SoldierHealth soldierHealth = soldier.GetComponent<SoldierHealth>();
            if (soldierHealth != null)
            {
                GameObject nearestEnemy = soldierHealth.FindNearestEnemy();

                if (nearestEnemy != null)
                {
                    float distance = Vector3.Distance(soldier.transform.position, nearestEnemy.transform.position);

                    if (distance <= attackRange)
                    {
                        soldierHealth.Attack(nearestEnemy);  // Attack the nearest enemy
                    }
                    else if (distance <= engageRange)
                    {
                        anySoldierEngaged = true;

                        NavMeshAgent agent = soldier.GetComponent<NavMeshAgent>();
                        if (agent != null && agent.isActiveAndEnabled)
                        {
                            agent.SetDestination(nearestEnemy.transform.position);
                        }
                    }
                }
            }
        }

        if (anySoldierEngaged)
        {
            foreach (GameObject soldier in soldiers)
            {
                if (soldier == null) continue;

                SoldierHealth soldierHealth = soldier.GetComponent<SoldierHealth>();
                if (soldierHealth != null)
                {
                    GameObject nearestEnemy = soldierHealth.FindNearestEnemy();
                    if (nearestEnemy != null)
                    {
                        // Move all soldiers towards the nearest enemy
                        NavMeshAgent agent = soldier.GetComponent<NavMeshAgent>();
                        if (agent != null && agent.isActiveAndEnabled)
                        {
                            agent.SetDestination(nearestEnemy.transform.position);
                        }
                    }
                }
            }


            hasArrangedAfterEngagement = false;
        }
        else
        {
            if (!hasArrangedAfterEngagement)
            {
                ArrangeGrid(CalculateGroupCenter(), 3);
                hasArrangedAfterEngagement = true;
            }
        }
    }



    private void SetDestination(GameObject soldier, Vector3 position)
    {
        if (soldier == null) return; // Check if soldier is null
        NavMeshAgent agent = soldier.GetComponent<NavMeshAgent>();
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(position);
        }
    }

    public void ArrangeGrid(Vector3 startPosition, int rows)
    {
        int cols = Mathf.CeilToInt((float)soldiers.Count / rows);

        List<GameObject> activeSoldiers = soldiers.FindAll(soldier => soldier != null); // Filter out dead soldiers

        for (int i = 0; i < activeSoldiers.Count; i++)
        {
            // Skip destroyed soldiers
            if (activeSoldiers[i] == null) continue;

            int row = i / cols;
            int col = i % cols;

            Vector3 position = startPosition + new Vector3(col * spacing, 0, row * spacing);
            SetDestination(activeSoldiers[i], position);
        }
    }

    public void MoveFormation(Vector3 targetPosition)
    {
        Vector3 groupCenter = CalculateGroupCenter();

        foreach (GameObject soldier in soldiers)
        {
            if (soldier == null) continue;

            Vector3 offset = soldier.transform.position - groupCenter;
            Vector3 destination = targetPosition + offset;
            SetDestination(soldier, destination);
        }
    }

    public void ArrangeGridInPlace()
    {
        if (soldiers.Count == 0) return;

        Vector3 groupCenter = CalculateGroupCenter();
        int rows = Mathf.CeilToInt(Mathf.Sqrt(soldiers.Count));
        int cols = Mathf.CeilToInt((float)soldiers.Count / rows);

        for (int i = 0; i < soldiers.Count; i++)
        {
            if (soldiers[i] == null) continue;

            int row = i / cols;
            int col = i % cols;

            Vector3 position = groupCenter + new Vector3(col * spacing, 0, row * spacing);

            soldiers[i].transform.position = position;
        }
    }
    private void Panic(Vector3 startPosition)
    {
        float panicRadius = 15f; 
        float fleeRadius = 30f;
        int enemyCount = 0;
        int friendCount = 0;

        Collider[] colliders = Physics.OverlapSphere(startPosition, panicRadius);

        foreach (Collider collider in colliders)
        {
            GameObject obj = collider.gameObject;

            if (allyTag.Contains(obj.tag))
            {
                friendCount++;
            }
            else if (enemyTag.Contains(obj.tag))
            {
                enemyCount++;
            }
        }


        // Check panic condition: enemies outnumber friends 3:1
        if (enemyCount >= 3 * friendCount)
        {
            // Move soldiers away from the center of enemies
            Vector3 fleeDirection = (startPosition - CalculateEnemiesCenter(colliders)).normalized;
            Vector3 fleePosition = startPosition + fleeDirection * fleeRadius;

            isPanicked = true;

            MoveFormation(fleePosition);

            foreach (var soldier in soldiers)
            {
                if(soldier != null && soldier.activeInHierarchy)
                {
                    soldier.GetComponent<NavMeshAgent>().speed = 5f;
                }
            }
                

        }
        else
        {
            foreach (var soldier in soldiers)
            {
                if (soldier != null && soldier.activeInHierarchy)
                {
                    soldier.GetComponent<NavMeshAgent>().speed = 3.5f;
                }
            }
            isPanicked =false;
        }
    }

    private Vector3 CalculateEnemiesCenter(Collider[] colliders)
    {
        Vector3 enemyCenter = Vector3.zero;
        int enemyCount = 0;

        foreach (Collider collider in colliders)
        {
            GameObject obj = collider.gameObject;

            if (enemyTag.Contains(obj.tag))
            {
                enemyCenter += obj.transform.position;
                enemyCount++;
            }
        }

        return enemyCount > 0 ? enemyCenter / enemyCount : Vector3.zero;
    }


    private Vector3 CalculateGroupCenter()
    {
        if (soldiers.Count == 0) return transform.position;

        Vector3 center = Vector3.zero;
        int activeSoldiersCount = 0;

        foreach (GameObject soldier in soldiers)
        {
            if (soldier != null && soldier.activeInHierarchy)  
            {
                center += soldier.transform.position;
                activeSoldiersCount++;
            }
        }

        if (activeSoldiersCount == 0) return transform.position;
        return center / activeSoldiersCount;
    }

}