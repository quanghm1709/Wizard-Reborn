using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : Core, IDamage
{
    public List<GameObject> enemyInRange;
    public bool isFacingRight = true;

    private void Update()
    {
        currentMp += Time.deltaTime;
        if (currentMp >= maxMp)
        {
            currentMp = maxMp;
        }
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
        throw new System.NotImplementedException();
    }

    public void UsingItem(float hp, float mp, float spd, bool isForever)
    {
        currentHp +=(int) (maxHp * hp);
        currentMp +=(int) (maxMp * mp);
        currentSpd += (maxSpd * spd);

        if (isForever)
        {
            maxHp += (int)(maxHp * hp);
            maxMp += (int)(maxMp * mp);
            maxSpd += (maxSpd * spd);
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
        UsingItem((float)((maxHp / 10) * .2f), (float)((maxMp / 10) * .2f), 0, true);
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
}
