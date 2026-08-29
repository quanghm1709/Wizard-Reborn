using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private bool triggered;

    private void OnEnable()
    {
        triggered = false;
    }

    private void Update()
    {
        if (triggered)
        {
            return;
        }

        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, .5f);
        foreach(Collider2D c in hit)
        {
            if (c.CompareTag("Player"))
            {
                triggered = true;
                c.transform.position = new Vector3(14f, 7f, 0);
                this.PostEvent(EventID.OnPlayerEnterGate);
            }
        }
        
    }
}
