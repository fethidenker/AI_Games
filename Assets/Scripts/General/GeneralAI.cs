using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralAI : MonoBehaviour
{
    public List<string> enemyTag = new List<string>();
    public List<string> allyTag = new List<string>();

    void Start()
    {
        if (gameObject.CompareTag("RedArmy"))
        {
            enemyTag.Add("BlueSoldierUnit");
            enemyTag.Add("BlueCavalryUnit");
            enemyTag.Add("BlueArcherUnit");

            allyTag.Add("RedSoldierUnit");
            allyTag.Add("RedCavalryUnit");
            allyTag.Add("RedArcherUnit");
        }
        else if (gameObject.CompareTag("BlueArmy"))
        {
            enemyTag.Add("RedSoldierUnit");
            enemyTag.Add("RedCavalryUnit");
            enemyTag.Add("RedArcherUnit");

            allyTag.Add("BlueSoldierUnit");
            allyTag.Add("BlueCavalryUnit");
            allyTag.Add("BlueArcherUnit");
        }
        
    }

    void Update()
    {
        OrderUnitsToMove();
    }
    void OrderUnitsToMove()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "RedSoldierUnit" || child.tag == "BlueSoldierUnit")
            {
                UnitManager soldierManager = child.GetComponent<UnitManager>();
                Vector3 nearestEnemyPosition = FindNearestEnemy(child.position);
                float distanceToEnemy = Vector3.Distance(child.position, nearestEnemyPosition);
                if (distanceToEnemy > 5f)
                {
                    if (nearestEnemyPosition != Vector3.zero)
                    {
                        soldierManager.HandleFormationMovement(nearestEnemyPosition);
                    }
                }
            }
            if (child.tag == "RedCavalryUnit" || child.tag == "BlueCavalryUnit")
            {
                CavalryUnitManager cavalryManager = child.GetComponent<CavalryUnitManager>();
                Vector3 nearestEnemyPosition = FindNearestEnemy(child.position);
                float distanceToEnemy = Vector3.Distance(child.position, nearestEnemyPosition);
                if (distanceToEnemy > 5f)
                {
                    if (nearestEnemyPosition != Vector3.zero)
                    {
                        cavalryManager.HandleFormationMovement(nearestEnemyPosition);
                        Debug.Log(nearestEnemyPosition);
                    }
                }
            }
            /*else if (child.tag == "RedArcherUnit" || child.tag == "BlueArcherUnit")
            {
                
                ArcherUnitManager archerManager = child.GetComponent<ArcherUnitManager>();
                Vector3 nearestEnemyPosition = FindNearestEnemy(child.position);
                float distanceToEnemy = Vector3.Distance(child.position, nearestEnemyPosition);
                if (distanceToEnemy > 20f)
                {
                    if (nearestEnemyPosition != Vector3.zero)
                    {
                        Vector3 direction = (nearestEnemyPosition - child.position).normalized;
                        Vector3 stopPosition = nearestEnemyPosition - direction * 20f;
                        Debug.Log(stopPosition);
                        archerManager.HandleFormationMovement(stopPosition);
                    }
                }
            }*/

        }
    }

    private Vector3 FindNearestEnemy(Vector3 unitPosition)
    {
        float closestDistance = float.MaxValue;
        Vector3 closestEnemyPosition = Vector3.zero;

        foreach (string tag in enemyTag)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(unitPosition, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemyPosition = enemy.transform.position;
                }
            }
        }

        return closestEnemyPosition;
    }

}
