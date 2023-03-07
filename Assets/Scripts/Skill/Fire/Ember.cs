using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Active/Fire/Ember", fileName = "Ember")]
public class Ember : SkillCore
{
    public override void Action()
    {
        Vector3 pos;

        if (player.isFacingRight)
        {
            pos = new Vector3(player.transform.position.x + 1, player.transform.position.y, player.transform.position.z);
        }
        else
        {
            pos = new Vector3(player.transform.position.x - 1, player.transform.position.y, player.transform.position.z);
        }

        Instantiate(skillAnim, pos, Quaternion.identity);
        Collider2D[] hit = Physics2D.OverlapCircleAll(pos, dmgRange, layerToDamage);

        foreach (Collider2D c in hit)
        {
            c.GetComponent<IDamage>().TakeDamage((int)atk[0], (int)atk[0], 0);
        }
    }
}
