using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyType
{
    Melee,
    Range,
    Boss,
}

public abstract class EnemyCore : Core
{
    public EnemyType type;
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
    public GameObject enemyProjectile;
    public Vector2 movement;

    [Header("UI")]
    public Slider hpBar;

    private void OnEnable()
    {
        tar = GameObject.Find("Player");

        maxHp += (int)(maxHp * .2f * FloorManager.currentFloor);
        maxAtk += (int)(maxAtk * .2f * FloorManager.currentFloor);

        currentHp = maxHp;
        currentAtk = maxAtk;

        hpBar.maxValue = maxHp;
        hpBar.value = currentHp;
        ChangeState(CharacterState.Idle);
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
