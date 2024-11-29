using Unity.VisualScripting;
using UnityEngine;

public class Archer : Soldier
{
    [SerializeField]
    GameObject prefabArrow;

    float arrowVelocity = 600;
    float elapsedSeconds = 0f;
    float landingRadius = 0.5f;

    protected override void Start()
    {
        health = 120;
        
        base.Start();
        
        if (gameObject.CompareTag("RedArcher"))
        {
            enemyTag.Add("BlueMilitia");
            enemyTag.Add("BlueCavalry");
            enemyTag.Add("BlueArcher");
        }
        else
        {
            enemyTag.Add("RedMilitia");
            enemyTag.Add("RedCavalry");
            enemyTag.Add("RedArcher");
        }
    }

    override public void Attack(GameObject enemy)
    {
        elapsedSeconds += Time.deltaTime;

        if (elapsedSeconds > 1f)
        {
            if (enemy == null) return;

            GameObject arrow = Instantiate(prefabArrow, transform.position, transform.rotation);

            Vector3 horz = (enemy.transform.position - transform.position).normalized;
            Vector3 vert = Vector3.up;
            Vector3 offset = new Vector3(Random.Range(-landingRadius, landingRadius), Random.Range(-landingRadius, landingRadius), Random.Range(-landingRadius, landingRadius));
            Vector3 arrowDirection = (horz + vert + offset).normalized;

            arrow.GetComponent<Rigidbody>().AddForce(arrowVelocity * arrowDirection);
            arrow.GetComponent<Arrow>().archerTag = gameObject.tag;
            Physics.IgnoreCollision(arrow.GetComponent<Collider>(), GetComponent<Collider>());

            elapsedSeconds = 0;
        }
    }
}
