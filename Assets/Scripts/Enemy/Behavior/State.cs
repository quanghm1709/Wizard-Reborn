using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState
{
    Undefined = -1,
    Idle,
    Moving,
    Attack,
    CrowControl,
    Death,
}

public abstract class State : ScriptableObject
{
    protected EnemyCore _agent = null;
    public void Init(EnemyCore character)
    {
        _agent = character;
    }
    public abstract CharacterState GetState();
    public virtual void Action() { }

}

