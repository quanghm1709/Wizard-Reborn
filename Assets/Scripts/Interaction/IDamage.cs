using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage 
{
    public IEnumerator TakeDamage(int atk, int maxAtk, float bonusDmg);
    public void TakeSusDamage(int totalDmg, float time);
}
