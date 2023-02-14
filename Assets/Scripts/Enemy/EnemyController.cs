using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Core
{
    [Header("Behavior")]
    [SerializeField] public List<State> states;
    [HideInInspector] public CharacterState _characterState;

    private void Update()
    {
        var curState = GetState(_characterState);
        curState.Init(this);
        curState.Action();
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

}
