using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behaviour/Move", fileName = "Move")]
public class EnemyMove : State
{
    public override CharacterState GetState()
    {
        return CharacterState.Moving;
    }

    public override void Action() {
        Move();
    }

    private void Move()
    {
        if (!_agent.Detect() && _agent.canMove)
        {
            if (_agent.tar.transform.position.x > _agent.transform.position.x)
            {
                _agent.movement.x = 1;
            }
            else if (_agent.tar.transform.position.x < _agent.transform.position.x)
            {
                _agent.movement.x = -1;
            }

            if (_agent.tar.transform.position.y > _agent.transform.position.y)
            {
                _agent.movement.y = 1;
            }
            else if (_agent.tar.transform.position.y < _agent.transform.position.y)
            {
                _agent.movement.y = -1;
            }

            //_agent.transform.position = Vector2.MoveTowards(_agent.transform.position, _agent.tar.transform.position, _agent.currentSpd * Time.deltaTime);
            _agent.rb.velocity = _agent.movement * _agent.currentSpd;// * Time.deltaTime;

        }
        else
        {
            _agent.ChangeState(CharacterState.Attack);
        }
        
    }
}
