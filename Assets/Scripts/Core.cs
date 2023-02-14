using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Core : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] protected string objName;
    [SerializeField] protected int currentHp;
    [SerializeField] protected int currentAtk;
    [SerializeField] protected float currentSpd;
    protected int maxHp;
    protected int maxAtk;
    protected float maxSpd;

    [Header("Component")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Animator anim;

    [SerializeField] protected bool canMove;

    protected void Start()
    {
        SetupData();
    }

    private void SetupData()
    {
        maxHp = currentHp;
        maxAtk = currentAtk;
        maxSpd = currentSpd;
    }

}
