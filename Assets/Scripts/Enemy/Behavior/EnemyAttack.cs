using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behaviour/Attack", fileName = "Attack")]
public class EnemyAttack : State
{
    public override CharacterState GetState()
    {
        return CharacterState.Attack;
    }
    public override void Action()
    {
        Attack();
    }

    private void Attack()
    {
        if(_agent.Detect())
        {
            _agent.rb.velocity = Vector2.zero;
            if (_agent.canAttack)
            {
                _agent.anim.SetBool("isAttack", true);
                _agent.anim.SetBool("isMove", false);

                if(_agent.type == EnemyType.Melee)
                {
                    MeleeAttack();
                }
                else
                {
                    RangeAttack();
                }

                _agent.LoadHit();
                _agent.ChangeState(CharacterState.Idle);
            }
        }
        else
        {
            _agent.ChangeState(CharacterState.Moving);
        }
    }

    private void MeleeAttack()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(_agent.dmgPoint.position, _agent.detectRange, _agent.detectLayer);

        if (hit.Length > 0)
        {
            Debug.Log(hit[0].name);
            hit[0].GetComponent<IDamage>().TakeDamage(_agent.currentAtk, _agent.maxAtk, 0);
        }
    }

    private void RangeAttack()
    {
        Vector2 lookDir = _agent.tar.transform.position - _agent.transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        GameObject g = Instantiate(_agent.enemyProjectile, _agent.dmgPoint.position, Quaternion.identity);
        g.transform.rotation = Quaternion.Euler(0, 0, angle);
        g.GetComponent<EnemyProjectile>().damage = _agent.currentAtk;
    }
}
