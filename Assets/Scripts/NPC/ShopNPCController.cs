using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopNPCController : MonoBehaviour
{
    [Header("Trade Item")]

    [Header("UI")]
    [SerializeField] private GameObject tradeNotify;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            tradeNotify.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        tradeNotify.SetActive(false);
    }
}
