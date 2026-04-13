using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(PlayerController))]
public class PlayerCombat : MonoBehaviour
{
    private PlayerController player;

    [Header("Combat Config")]
    public List<Transform> attackPoint;
    public float damageRange;
    public LayerMask hitLayer;
    public float detectRange;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (player.timeBtwHitCD > 0)
        {
            player.timeBtwHitCD -= Time.deltaTime;
        }
        else
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space) || CrossPlatformInputManager.GetButtonDown("Contact Btn")) 
        {
            StartCoroutine(AttackRoutine());           
        }     
    }

    private IEnumerator AttackRoutine()
    {
        player.timeBtwHitCD = player.timeBtwHit;
        RaycastHit2D[] hit;
        player.anim.SetBool("isAttack", true);
        player.anim.SetBool("isMove", false);

        player.canMove = false;

        foreach(Transform t in attackPoint)
        {
            Vector2 origin = new Vector2(t.position.x, t.position.y);
            Vector2 end = player.isFacingRight 
                ? new Vector2(t.position.x + damageRange, t.position.y) 
                : new Vector2(t.position.x - damageRange, t.position.y);

            hit = Physics2D.LinecastAll(origin, end, hitLayer);
            
            if (hit.Length > 0)
            {
                foreach (var i in hit)
                {
                    IDamage damageable = i.collider.GetComponent<IDamage>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(player.currentAtk, player.maxAtk, 0);
                    }
                }
            }
        }

        yield return new WaitForSeconds(.3f);
        
        player.anim.SetBool("isAttack", false);
        player.anim.SetBool("isMove", true);
        player.canMove = true;
    }

    private void OnDrawGizmos()
    {
        if (player == null) player = GetComponent<PlayerController>();
        if (player == null) return;

        Gizmos.color = Color.red;
        if (attackPoint != null)
        {
            foreach (Transform t in attackPoint)
            {
                if (player.isFacingRight)
                {
                    Gizmos.DrawLine(t.position, new Vector3(t.position.x + damageRange, t.position.y, t.position.z));
                }
                else
                {
                    Gizmos.DrawLine(t.position, new Vector3(t.position.x - damageRange, t.position.y, t.position.z));
                }
            }
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
