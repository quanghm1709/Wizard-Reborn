using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private float hp;
    [SerializeField] private float mp;
    [SerializeField] private float spd;
    [SerializeField] private bool isForever;
    [SerializeField] private bool isItemorGold;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (isItemorGold)
            {
                collision.GetComponent<PlayerController>().UsingItem(hp, mp, spd, isForever);
            }
            else
            {
                GoldManager.playerGold += Random.Range(1, 3);
            }
            
            gameObject.SetActive(false);
        }
    }
}
