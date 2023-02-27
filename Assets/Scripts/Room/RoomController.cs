using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private int totalWave;
    [SerializeField] private bool isClear;

    private void Start()
    {
        totalWave = Random.Range(1, 4);    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        EnemyGenerator.instance.GenerateEnemy(transform, totalWave);
    }
}
