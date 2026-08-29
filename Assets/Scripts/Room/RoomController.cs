using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public int roomId;

    [SerializeField] private bool isEnemyRoom;
    [SerializeField] private int totalWave;
    public bool isClear;

    [SerializeField] private Transform[] detectRoom;
    [SerializeField] private GameObject[] teleportPoint;

    private bool playerIn = false;
    private EnemyCore boss;

    private void OnEnable()
    {
        this.RegisterListener(EventID.OnRoomClear, HandleRoomClear);
    }

    private void OnDisable()
    {
        this.RemoveListener(EventID.OnRoomClear, HandleRoomClear);
    }

    private void Start()
    {
        boss = GetComponentInChildren<EnemyCore>(true);
        if (boss != null && boss.type == EnemyType.Boss)
        {
            isEnemyRoom = true;
            totalWave = 0;
            boss.SetOwningRoom(this);
            boss.gameObject.SetActive(false);
        }

        if (isEnemyRoom)
        {
            if (boss == null)
            {
                totalWave = Random.Range(1, 4);
            }
        }
        else
        {
            OnRoomClear(roomId);
        }
    }

    private void HandleRoomClear(object param)
    {
        if (param is int clearedRoomId)
        {
            OnRoomClear(clearedRoomId);
        }
    }

    public void OnRoomClear(int param)
    {
        if(param == roomId)
        {
            for (int i = 0; i < detectRoom.Length; i++)
            {
                Collider2D[] hit = Physics2D.OverlapCircleAll(detectRoom[i].position, 1);
                if (hit.Length > 0)
                {
                    //Debug.Log("direct " + hit[0].name + " " +i + " " +roomId);
                    teleportPoint[i].SetActive(true);
                }
            }
        }
    }

    internal void ResetRoom()
    {
        foreach(GameObject g in teleportPoint)
        {
            g.SetActive(false);
        }
        isClear = false;
        playerIn = false;
    }

    public void CompleteCombatRoom()
    {
        if (isClear)
        {
            return;
        }

        isClear = true;
        this.PostEvent(EventID.OnRoomClear, roomId);
    }

    public void OnBossDefeated()
    {
        CompleteCombatRoom();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.name);
        if(collision.CompareTag("Player") && !playerIn && isEnemyRoom)
        {
            playerIn = true;
            if (boss != null)
            {
                boss.gameObject.SetActive(true);
            }
            else if (totalWave > 0 && EnemyGenerator.instance != null)
            {
                Vector3 spawnPoint = new Vector3(transform.position.x + 4.75f, transform.position.y + 4.25f, transform.position.z);
                StartCoroutine(EnemyGenerator.instance.GenerateEnemy(spawnPoint, totalWave, this));
            }
        }
    }
}
