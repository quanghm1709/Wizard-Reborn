using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private void Update()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach(Collider2D c in hit)
        {
            if (c.tag == "Player")
            {
                c.transform.position = new Vector3(14f, 7f, 0);
                this.PostEvent(EventID.OnPlayerEnterGate);
            }
        }
        
    }
}
