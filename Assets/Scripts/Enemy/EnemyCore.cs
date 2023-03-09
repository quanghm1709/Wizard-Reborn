using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyCore : Core
{
    public GameObject tar;
    public int id;
    public float dropExp;

    [Header("Radar")]
    public float detectRange;
    public LayerMask detectLayer;
    public Detect detect;

    [Header("Attack")]
    public float dmgRange;
    public Transform dmgPoint;

    public Vector2 movement;

    private void OnEnable()
    {
        tar = GameObject.Find("Player");
        currentHp = maxHp;
    }

    private void OnDisable()
    {
        this.PostEvent(EventID.OnEnemyDead, (int) dropExp );
    }
    public virtual bool Detect()
    {
        return detect.Detecting(detectRange, detectLayer);
    }
    public void LoadHit()
    {
        timeBtwHitCD = timeBtwHit;
    }

    public void ResetData()
    {
       
    }

    public abstract void ChangeState(CharacterState enemyState);
    public abstract void Flip();
}
