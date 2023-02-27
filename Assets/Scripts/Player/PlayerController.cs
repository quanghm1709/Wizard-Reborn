using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : Core, IDamage
{
    [Header("Combat")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float damageRange;

    [HideInInspector] public bool isFacingRight = true;
    private float dirX;
    private float dirY;
    private float attackDuration;

    private void Update()
    {
        SetMove();
        Attack();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void SetMove()
    {
        dirX = Input.GetAxisRaw("Horizontal");//CrossPlatformInputManager.GetAxis("Horizontal") * currentSpd;
        dirY = Input.GetAxisRaw("Vertical");//CrossPlatformInputManager.GetAxis("Vertical") * currentSpd;

        if (dirX != 0 || dirY != 0)
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
            rb.velocity = new Vector2(dirX, dirY);
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

    private void Attack()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestAttack();
            canMove = false;
            attackDuration = .5f;
            anim.SetBool("isAttack", true);
            
        }
        attackDuration -= Time.deltaTime;
        if (attackDuration <= 0)
        {
            anim.SetBool("isAttack", false);
            canMove = true;
        }      
    }

    private void TestAttack()
    {
        float direct;
        RaycastHit2D[] hit;

        if (isFacingRight)
        {
            hit = Physics2D.LinecastAll(new Vector2(attackPoint.position.x, attackPoint.position.y), new Vector2((attackPoint.position.x + damageRange), attackPoint.position.y));

        }
        else
        {
            hit = Physics2D.LinecastAll(new Vector2(attackPoint.position.x, attackPoint.position.y), new Vector2((attackPoint.position.x - damageRange), attackPoint.position.y));

        }
        
        foreach (var i in hit)
        {
            Debug.Log(i.collider.name);
            StartCoroutine(i.collider.GetComponent<IDamage>().TakeDamage(currentAtk, maxAtk, 0));        
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (isFacingRight)
        {
            Gizmos.DrawLine(attackPoint.position, new Vector3((attackPoint.position.x + damageRange), attackPoint.position.y, attackPoint.position.z));
        }
        else
        {
            Gizmos.DrawLine(attackPoint.position, new Vector3((attackPoint.position.x - damageRange), attackPoint.position.y, attackPoint.position.z));
        }
    }

    public IEnumerator TakeDamage(int atk, int maxAtk, float bonusDmg)
    {
        float damage = atk + maxAtk * bonusDmg;
        yield return new WaitForSeconds(.1f);
        currentHp -= (int)damage;
        if (currentHp <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void TakeSusDamage(int totalDmg, float time)
    {
        throw new System.NotImplementedException();
    }
}
