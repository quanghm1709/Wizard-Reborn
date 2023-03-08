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

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            collision.GetComponent<IDamage>().TakeDamage((int)damage, (int)damage, 0);
        }
        gameObject.SetActive(false);
    }
}
