using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float enableTimeCD;
    [SerializeField] private Animator anim;
    private float enableTime;
    private float cycleDuration;
    private float disableTime = 0;

    private void Awake()
    {
        cycleDuration = enableTimeCD;
    }

    private void OnEnable()
    {
        enableTime = cycleDuration;
        enableTimeCD = cycleDuration;
        disableTime = 0f;
    }

    private void Update()
    {
        if(enableTimeCD > 0)
        {
            enableTimeCD -= Time.deltaTime;
            anim.SetBool("isEnable", true);
        }
        else
        {
            anim.SetBool("isEnable", false);
            disableTime += Time.deltaTime;

            if(disableTime >= enableTime)
            {
                enableTimeCD = enableTime;
                disableTime = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enableTimeCD > 0)
        {
            if (collision.tag == "Player" || collision.tag == "Enemy")
            {
                IDamage damageable = collision.GetComponent<IDamage>();
                damageable?.TakeDamage((int)damage, (int)damage, 0);
            }
        }

    }
}
