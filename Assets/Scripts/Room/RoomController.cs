using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public int roomId;

    [SerializeField] private int totalWave;
    [SerializeField] public bool isClear;
    private bool playerIn = false;
    private void Start()
    {
        totalWave = Random.Range(1, 4);    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if(collision.tag == "Player" && !playerIn)
        {
            Vector3 spawnPoint = new Vector3(transform.position.x + 4.75f, transform.position.y + 4.25f, transform.position.z);
            StartCoroutine(EnemyGenerator.instance.GenerateEnemy(spawnPoint, totalWave, gameObject.GetComponent<RoomController>()));
            playerIn = true;
        }
    }
}
