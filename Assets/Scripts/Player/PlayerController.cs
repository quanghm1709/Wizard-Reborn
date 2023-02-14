using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : Core
{
    [Header("Combat")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float damageRange;
    [SerializeField] private float timeBtwHitCD;

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
        }else if(dirX  < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
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
        RaycastHit2D[] hit = Physics2D.LinecastAll(new Vector2(attackPoint.position.x, attackPoint.position.y), new Vector2(attackPoint.position.x + damageRange, attackPoint.position.y));
        foreach(var i in hit)
        {
            Debug.Log(i.collider.name);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(attackPoint.position, new Vector3(attackPoint.position.x + damageRange, attackPoint.position.y, attackPoint.position.z));
        //Gizmos.DrawWireCube(attackPoint.position, new Vector3(attackPoint.position.x+damageRange, attackPoint.position.y+ damageRange, attackPoint.position.z));
    }
}
