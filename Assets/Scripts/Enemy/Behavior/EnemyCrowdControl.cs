using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCrowdControl : State
{
    public override CharacterState GetState()
    {
        return CharacterState.CrowControl;
    }
    public override void Action()
    {
        
    }
}
