using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EnemyCore
{
    [Header("Behavior")]
    [SerializeField] public List<State> states;
     public CharacterState _characterState;

    private void Update()
    {
        var curState = GetState(_characterState);
        curState.Init(this);
        curState.Action();

        Flip();
    }

    private State GetState(CharacterState characterState)
    {
        foreach (var state in states)
        {
            if (state.GetState() == characterState)
                return state;
        }
        return null;
    }

    public override void ChangeState(CharacterState enemyState)
    {
        _characterState = enemyState;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }

    public override void Flip()
    {
        if(tar.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
