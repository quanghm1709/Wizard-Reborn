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

public abstract class State : MonoBehaviour
{
    protected Core _agent = null;
    public void Init(Core character)
    {
        _agent = character;
    }
    public abstract CharacterState GetState();
    public virtual void Action() { }

}

