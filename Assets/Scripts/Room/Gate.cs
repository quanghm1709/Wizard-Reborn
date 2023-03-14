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
                c.transform.position = new Vector3(10.4499998f, 3.86999989f, 0);
                this.PostEvent(EventID.OnPlayerEnterGate);
            }
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
    }
}
