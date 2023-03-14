using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage;
    public float speed;
    public float lifeTime;
    public GameObject target;
    public Rigidbody2D rb;

    private void Start()
    {
        rb.AddForce(transform.right * speed);
    }
    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<IDamage>().TakeDamage((int)damage, (int)damage, 0);
            gameObject.SetActive(false);
        }

    }
}
