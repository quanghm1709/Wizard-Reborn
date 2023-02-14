using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdle : State
{
    public override CharacterState GetState()
    {
        return CharacterState.Idle;
    }
}
