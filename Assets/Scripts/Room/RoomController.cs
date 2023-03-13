using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public int roomId;

    [SerializeField] private bool isEnemyRoom;
    [SerializeField] private int totalWave;
    [SerializeField] public bool isClear;

    [SerializeField] private Transform[] detectRoom;
    [SerializeField] private GameObject[] teleportPoint;

    private bool playerIn = false;
    private void Start()
    {
        RegisterEvent();
        if (isEnemyRoom)
        {
            totalWave = Random.Range(1, 4);
        }
        else
        {
            OnRoomClear(roomId);
        }
            
    }

    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnRoomClear, (param) => OnRoomClear((int)param));
    }

    private void OnRoomClear(int param)
    {
        if(param == roomId)
        {
            for (int i = 0; i < detectRoom.Length; i++)
            {
                Collider2D[] hit = Physics2D.OverlapCircleAll(detectRoom[i].position, 1);
                Debug.Log(hit.Length);
                if (hit.Length > 0)
                {
                    teleportPoint[i].SetActive(true);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if(collision.tag == "Player" && !playerIn && totalWave > 0)
        {
            Vector3 spawnPoint = new Vector3(transform.position.x + 4.75f, transform.position.y + 4.25f, transform.position.z);
            StartCoroutine(EnemyGenerator.instance.GenerateEnemy(spawnPoint, totalWave, gameObject.GetComponent<RoomController>()));
            playerIn = true;
        }
    }
}
