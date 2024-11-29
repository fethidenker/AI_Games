using UnityEngine;

public class Cavalry : Soldier
{
    int attackDamage = 10;  
    float attackCooldown = 1.5f;
    float lastAttackTime;
    int chargeDamage = 30;

    protected override void Start()
    {
        health = 150;
        
        base.Start();
        
        if (gameObject.CompareTag("RedCavalry"))
        {
            enemyTag.Add("BlueMilitia");
            enemyTag.Add("BlueCavalry");
            enemyTag.Add("BlueArcher");
        }
        else if (gameObject.CompareTag("BlueCavalry"))
        {
            enemyTag.Add("RedMilitia");
            enemyTag.Add("RedCavalry");
            enemyTag.Add("RedArcher");
        }
    }

    override public void Attack(GameObject enemy, bool isCharge)
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Soldier enemySoldier = enemy.GetComponent<Soldier>();
            if (enemySoldier != null)
            {
                if (isCharge)
                {
                    enemySoldier.TakeDamage(chargeDamage);
                }
                else
                {
                    enemySoldier.TakeDamage(attackDamage);
                }
            }
            lastAttackTime = Time.time;
        }
    }

}
