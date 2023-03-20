using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Core : MonoBehaviour
{
    [Header("Data")]
    public string objName;
    public int currentHp;
    public int currentAtk;
    public float currentSpd;
    public int maxHp;
    public int maxAtk;
    protected float maxSpd;
    public float currentMp;
    public float maxMp;

    [Header("Combat")]
    public float timeBtwHitCD;
    [HideInInspector] public float timeBtwHit;

    [Header("Component")]
    public Rigidbody2D rb;
    public Animator anim;

    public bool canMove;
    public bool canAttack;
    

    protected void Start()
    {
        SetupData();
    }

    private void SetupData()
    {
        maxHp = currentHp;
        maxAtk = currentAtk;
        maxSpd = currentSpd;
        timeBtwHit = timeBtwHitCD;
    }

    protected void ReloadHit()
    {
        if (timeBtwHitCD > 0)
        {
            timeBtwHitCD -= Time.deltaTime;
            canAttack = false;
        }
        else
        {
            canAttack = true;
        }
    }

}
