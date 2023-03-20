using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossSkillState
{
    None,
    Skill1,
    Skill2,
    Skill3,
}
public class BossController : MonoBehaviour
{
    public BossSkillState currentSkill;

    [Header("Skill1")]
    [SerializeField] private GameObject summonMonster;
    [SerializeField] private int total;

    [Header("Skill2")]
    [SerializeField] private GameObject projectile;

    [Header("Skill3")]
    [SerializeField] private Transform attackpoint;

    private void Start()
    {
        currentSkill = BossSkillState.Skill1;
    }

    public void Skill1() {
        Debug.Log("Skill1");
        for (int i = 0; i < total; i++)
        {
            Vector3 randpos = new Vector3(Random.Range(-transform.position.x - 2, transform.position.x + 2),
                                          Random.Range(-transform.position.y - 2, transform.position.y + 2),
                                          Random.Range(-transform.position.z - 2, transform.position.z + 2));
            GameObject g = EnemyGenerator.instance.enemyPool.GetObject(summonMonster.name);
            g.transform.position = randpos;
        }
        currentSkill = BossSkillState.Skill2;
    }
    public void Skill2() {
        Debug.Log("Skill2");
        currentSkill = BossSkillState.Skill3;
    }
    public void Skill3() {

        Debug.Log("Skill3");
        currentSkill = BossSkillState.Skill1;
    }
}
