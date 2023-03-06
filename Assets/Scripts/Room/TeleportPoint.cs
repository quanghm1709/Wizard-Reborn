using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direct
{
    up,
    down,
    left,
    right,
}

public class TeleportPoint : MonoBehaviour
{
    [SerializeField] private Direct direct;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            float pointX = collision.transform.position.x;
            float pointY = collision.transform.position.y;

            switch (direct)
            {
                case Direct.up:
                    pointY += 5;
                    break;
                case Direct.down:
                    pointY -= 5;
                    break;
                case Direct.left:
                    pointX -= 6;
                    break;
                case Direct.right:
                    pointX += 6;
                    break;
            }

            collision.transform.position = new Vector3(pointX, pointY, collision.transform.position.z);
        }
    }
}
