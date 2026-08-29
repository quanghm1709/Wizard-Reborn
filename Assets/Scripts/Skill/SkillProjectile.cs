using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillProjectile : MonoBehaviour
{
    public float damage;
    public float speed;
    public float lifeTime;
    public GameObject target;
    public Rigidbody2D rb;
    public ElementType element;

    private void Update()
    {
        if (target == null || !target.activeInHierarchy)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        Vector2 lookDir = target.transform.position - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            IDamage damageable = collision.GetComponent<IDamage>();
            damageable?.TakeDamage((int)damage, (int)damage, 0);
            EnemyCore enemy = collision.GetComponent<EnemyCore>();
            enemy?.ApplyElement(element, Mathf.RoundToInt(damage));
            Destroy(gameObject);
        }
        
    }
}
