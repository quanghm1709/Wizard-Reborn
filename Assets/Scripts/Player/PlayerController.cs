using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : Core, IDamage
{
    [Header("Combat")]
    [SerializeField] private Transform attackPoint;
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
        //GetEnemyInRange();
        //RemoveEnemyOutRange();
        SetMove();
        Attack();
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

        if (Input.GetKeyDown(KeyCode.Space) || CrossPlatformInputManager.GetButtonDown("Contact Btn")) 
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
            hit = Physics2D.LinecastAll(new Vector2(attackPoint.position.x, attackPoint.position.y), new Vector2((attackPoint.position.x + damageRange), attackPoint.position.y), hitLayer);

        }
        else
        {
            hit = Physics2D.LinecastAll(new Vector2(attackPoint.position.x, attackPoint.position.y), new Vector2((attackPoint.position.x - damageRange), attackPoint.position.y), hitLayer);

        }
        
        if(hit.Length > 0)
        {
            foreach (var i in hit)
            {
               // Debug.Log(i.collider.name);
               i.collider.GetComponent<IDamage>().TakeDamage(currentAtk, maxAtk, 0);
            }
        }

    }

    private void GetEnemyInRange()
    {
        Collider2D[] multiEnemy = Physics2D.OverlapCircleAll(transform.position, detectRange, hitLayer);
        foreach(Collider2D c in multiEnemy)
        {
            enemyInRange.Add(c.gameObject);
        }
    }

    private void RemoveEnemyOutRange()
    {
        foreach(GameObject t in enemyInRange)
        {
            if(Vector3.Distance(transform.position, t.transform.position) > detectRange)
            {
                enemyInRange.Remove(t);
            }
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
            gameObject.SetActive(false);
        }
    }

    public void TakeSusDamage(int totalDmg, float time)
    {
        throw new System.NotImplementedException();
    }
}
