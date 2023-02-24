using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyCore : Core
{
    public Transform tar;

    [Header("Radar")]
    public float detectRange;
    public LayerMask detectLayer;
    public Detect detect;

    public virtual bool Detect()
    {
        return detect.Detecting(detectRange, detectLayer);
    }
    public void LoadHit()
    {
        timeBtwHitCD = timeBtwHit;
    }

    public abstract void ChangeState(CharacterState enemyState);
    public abstract void Flip();
}
