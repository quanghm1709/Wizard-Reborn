using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detect : MonoBehaviour
{
    public bool Detecting(float detectRange, LayerMask detectLayer)
    {
        Collider2D[] enemyIn = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), detectRange, detectLayer);
        if (enemyIn.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public List<GameObject> GetClosetEnemy(float detectRange, LayerMask detectLayer)
    {
        Collider[] multiEnemy = Physics.OverlapSphere(transform.position, detectRange, detectLayer);
        float closetDistance = Mathf.Infinity;
        Transform trans = null;

        return null;
    }
}
