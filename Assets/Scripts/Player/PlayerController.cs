using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : Core, IDamage
{
    [Header("Combat")]
    [SerializeField] private List<Transform> attackPoint;
    [SerializeField] private float damageRange;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private float detectRange;
    public List<GameObject> enemyInRange;

    public bool isFacingRight = true;

    private float dirX;
    private float dirY;
    private float attackDuration;

    private void Update()
    {
        SetMove();
        if (timeBtwHitCD > 0)
        {
            timeBtwHitCD -= Time.deltaTime;
        }
        else
        {
            StartAttack();
        }

        currentMp += Time.deltaTime;
        if (currentMp >= maxMp)
        {
            currentMp = maxMp;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void SetMove()
    {
        //dirX = Input.GetAxisRaw("Horizontal");
        //dirY = Input.GetAxisRaw("Vertical");
        dirX = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        dirY = CrossPlatformInputManager.GetAxisRaw("Vertical");

        if (dirX != 0|| dirY != 0)
        {
            anim.SetBool("isMove", true);
        }
        else
        {
            anim.SetBool("isMove", false);
        }
        Flip();
    }

    private void Move()
    {
        if (canMove)
        {
            rb.velocity = new Vector2(dirX, dirY)*currentSpd;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
    private void Flip()
    {
        if(dirX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            isFacingRight = true;
        }else if(dirX  < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isFacingRight = false;
        }
    }

    private void StartAttack()
    {

        if (Input.GetKeyDown(KeyCode.Space) || CrossPlatformInputManager.GetButtonDown("Contact Btn")) 
        {
            StartCoroutine(Attack());           
        }     
    }

    private IEnumerator Attack()
    {
        timeBtwHitCD = timeBtwHit;
        RaycastHit2D[] hit;
        anim.SetBool("isAttack", true);
        anim.SetBool("isMove", false);

        canMove = false;

        foreach(Transform t in attackPoint)
        {
            if (isFacingRight)
            {
                hit = Physics2D.LinecastAll(new Vector2(t.position.x, t.position.y), new Vector2((t.position.x + damageRange), t.position.y), hitLayer);

            }
            else
            {
                hit = Physics2D.LinecastAll(new Vector2(t.position.x, t.position.y), new Vector2((t.position.x - damageRange), t.position.y), hitLayer);

            }
            if (hit.Length > 0)
            {
                foreach (var i in hit)
                {
                    // Debug.Log(i.collider.name);
                    i.collider.GetComponent<IDamage>().TakeDamage(currentAtk, maxAtk, 0);
                }
            }
        }

        


        yield return new WaitForSeconds(.3f);
        
        anim.SetBool("isAttack", false);
        anim.SetBool("isMove", true);
        canMove = true;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform t in attackPoint)
        {
            if (isFacingRight)
            {
                Gizmos.DrawLine(t.position, new Vector3((t.position.x + damageRange), t.position.y, t.position.z));
            }
            else
            {
                Gizmos.DrawLine(t.position, new Vector3((t.position.x - damageRange), t.position.y, t.position.z));
            }
        }


        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }

    public void TakeDamage(int atk, int maxAtk, float bonusDmg)
    {
        float damage = atk + maxAtk * bonusDmg;
       // yield return new WaitForSeconds(.1f);
        currentHp -= (int)damage;
        Debug.Log("hit");
        if (currentHp <= 0)
        {
            this.PostEvent(EventID.OnPlayerDead);
            gameObject.SetActive(false);
        }
    }

    public void TakeSusDamage(int totalDmg, float time)
    {
        throw new System.NotImplementedException();
    }

    public void UsingItem(float hp, float mp, float spd, bool isForever)
    {
        currentHp +=(int) (maxHp * hp);
        currentMp +=(int) (maxMp * mp);
        currentSpd += (maxSpd * spd);

        if (isForever)
        {
            maxHp += (int)(maxHp * hp);
            maxMp += (int)(maxMp * mp);
            maxSpd += (maxSpd * spd);
        }

        if(currentHp> maxHp)
        {
            currentHp = maxHp;
        }

        if (currentMp > maxMp)
        {
            currentMp = maxMp;
        }

        if (currentSpd > maxSpd)
        {
            currentSpd = maxSpd;
        }
    }
}
