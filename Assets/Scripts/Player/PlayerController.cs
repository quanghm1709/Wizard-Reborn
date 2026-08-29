using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : Core, IDamage
{
    [Header("Combat")]
    [SerializeField] private List<Transform> attackPoint;
    [SerializeField] private float damageRange;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private float detectRange;
    public List<GameObject> enemyInRange;

    public bool isFacingRight = true;

    private float dirX;
    private float dirY;
    private float attackDuration;

    private void Update()
    {
        enemyInRange.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);
        SetMove();
        if (timeBtwHitCD > 0)
        {
            timeBtwHitCD -= Time.deltaTime;
        }
        else
        {
            StartAttack();
        }

        currentMp += Time.deltaTime;
        if (currentMp >= maxMp)
        {
            currentMp = maxMp;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void SetMove()
    {
        //dirX = Input.GetAxisRaw("Horizontal");
        //dirY = Input.GetAxisRaw("Vertical");
        dirX = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        dirY = CrossPlatformInputManager.GetAxisRaw("Vertical");

        if (dirX != 0|| dirY != 0)
        {
            anim.SetBool("isMove", true);
        }
        else
        {
            anim.SetBool("isMove", false);
        }
        Flip();
    }

    private void Move()
    {
        if (canMove)
        {
            rb.linearVelocity = new Vector2(dirX, dirY)*currentSpd;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    private void Flip()
    {
        if(dirX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            isFacingRight = true;
        }else if(dirX  < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isFacingRight = false;
        }
    }

    private void StartAttack()
    {

        if (Input.GetKeyDown(KeyCode.Space) || CrossPlatformInputManager.GetButtonDown("Contact Btn")) 
        {
            StartCoroutine(Attack());           
        }     
    }

    private IEnumerator Attack()
    {
        timeBtwHitCD = timeBtwHit;
        RaycastHit2D[] hit;
        anim.SetBool("isAttack", true);
        anim.SetBool("isMove", false);

        canMove = false;

        foreach(Transform t in attackPoint)
        {
            if (isFacingRight)
            {
                hit = Physics2D.LinecastAll(new Vector2(t.position.x, t.position.y), new Vector2((t.position.x + damageRange), t.position.y), hitLayer);

            }
            else
            {
                hit = Physics2D.LinecastAll(new Vector2(t.position.x, t.position.y), new Vector2((t.position.x - damageRange), t.position.y), hitLayer);

            }
            if (hit.Length > 0)
            {
                foreach (var i in hit)
                {
                    IDamage damageable = i.collider.GetComponent<IDamage>();
                    damageable?.TakeDamage(currentAtk, maxAtk, 0);
                }
            }
        }

        


        yield return new WaitForSeconds(.3f);
        
        anim.SetBool("isAttack", false);
        anim.SetBool("isMove", true);
        canMove = true;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform t in attackPoint)
        {
            if (isFacingRight)
            {
                Gizmos.DrawLine(t.position, new Vector3((t.position.x + damageRange), t.position.y, t.position.z));
            }
            else
            {
                Gizmos.DrawLine(t.position, new Vector3((t.position.x - damageRange), t.position.y, t.position.z));
            }
        }


        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }

    public void TakeDamage(int atk, int maxAtk, float bonusDmg)
    {
        float damage = atk + maxAtk * bonusDmg;
       // yield return new WaitForSeconds(.1f);
        currentHp -= (int)damage;
        Debug.Log("hit");
        if (currentHp <= 0)
        {
            this.PostEvent(EventID.OnPlayerDead);
            gameObject.SetActive(false);
        }
    }

    public void TakeSusDamage(int totalDmg, float time)
    {
        StartCoroutine(DamageOverTime(totalDmg, time));
    }

    private IEnumerator DamageOverTime(int totalDmg, float duration)
    {
        int ticks = Mathf.Max(1, Mathf.CeilToInt(duration));
        int damagePerTick = Mathf.Max(1, Mathf.CeilToInt((float)totalDmg / ticks));
        for (int i = 0; i < ticks && currentHp > 0; i++)
        {
            TakeDamage(damagePerTick, damagePerTick, 0f);
            yield return new WaitForSeconds(1f);
        }
    }

    public void UsingItem(float hp, float mp, float spd, bool isForever)
    {
        int hpGain = Mathf.RoundToInt(maxHp * hp);
        float mpGain = maxMp * mp;
        float speedGain = maxSpd * spd;

        currentHp += hpGain;
        currentMp += mpGain;
        currentSpd += (maxSpd * spd);

        if (isForever)
        {
            maxHp += hpGain;
            maxMp += mpGain;
            maxSpd += speedGain;
        }

        if(currentHp> maxHp)
        {
            currentHp = maxHp;
        }

        if (currentMp > maxMp)
        {
            currentMp = maxMp;
        }

        if (currentSpd > maxSpd)
        {
            currentSpd = maxSpd;
        }
    }

    public void LevelUp()
    {
        const float levelUpStatRatio = .02f;
        UsingItem(levelUpStatRatio, levelUpStatRatio, 0f, true);
    }

    internal void Save()
    {
        List<int> data = new List<int>
        {
            currentHp,
            (int)currentMp,
            maxHp,
            (int)maxMp,
            currentAtk,
            maxAtk
        };
        SaveData.SavePlayerData("Player", data);
    }

    internal void Load()
    {
        List<int> data = SaveData.LoadPlayerData("Player");

        currentHp = data[0];
        currentMp = data[1];
        maxHp = data[2];
        maxMp = data[3];
        currentAtk = data[4];
        maxAtk = data[5];
    }

    public PlayerSaveData CaptureSaveData()
    {
        return new PlayerSaveData
        {
            currentHp = currentHp,
            maxHp = maxHp,
            currentMp = currentMp,
            maxMp = maxMp,
            currentAtk = currentAtk,
            maxAtk = maxAtk,
            currentSpd = currentSpd,
            maxSpd = maxSpd,
            attackCooldown = timeBtwHit
        };
    }

    public void ApplySaveData(PlayerSaveData data)
    {
        if (data == null)
        {
            return;
        }

        maxHp = Mathf.Max(1, data.maxHp);
        currentHp = Mathf.Clamp(data.currentHp, 1, maxHp);
        maxMp = Mathf.Max(0f, data.maxMp);
        currentMp = Mathf.Clamp(data.currentMp, 0f, maxMp);
        maxAtk = Mathf.Max(1, data.maxAtk);
        currentAtk = Mathf.Clamp(data.currentAtk, 1, maxAtk);
        maxSpd = Mathf.Max(.1f, data.maxSpd);
        currentSpd = Mathf.Clamp(data.currentSpd, .1f, maxSpd);
        if (data.attackCooldown > 0f)
        {
            timeBtwHit = data.attackCooldown;
            timeBtwHitCD = 0f;
        }
    }
}
