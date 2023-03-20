using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopNPCController : MonoBehaviour
{
    [Header("Trade Item")]

    [Header("UI")]
    [SerializeField] private GameObject tradeNotify;
    private bool isExchange = false;

    private void OnEnable()
    {
        isExchange = false;
    }

    public void Exchange()
    {
        PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();

        if(player.currentHp > (player.maxHp / 10) * 3)
        {
            player.currentHp -= (player.maxHp / 10) * 3;
            int rand = Random.Range(0, 1);
            if(rand == 0)
            {
                player.currentAtk += (player.maxAtk / 10);
                player.maxAtk += (player.maxAtk / 10);
            }
            else
            {
                player.timeBtwHit -= player.timeBtwHit / 10;
                if (player.timeBtwHit < .6f)
                {
                    player.timeBtwHit = .6f;
                }
            }
            tradeNotify.SetActive(false);
            isExchange = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !isExchange)
        {
            tradeNotify.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        tradeNotify.SetActive(false);
    }
}
