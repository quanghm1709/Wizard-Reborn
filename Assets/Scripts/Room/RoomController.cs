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

    private void Update()
    {
        if (!isEnemyRoom)
        {
            OnRoomClear(roomId);
        }
    }
    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnRoomClear, (param) => OnRoomClear((int)param));
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.name);
        if(collision.tag == "Player" && !playerIn && totalWave > 0)
        {
            Vector3 spawnPoint = new Vector3(transform.position.x + 4.75f, transform.position.y + 4.25f, transform.position.z);
            StartCoroutine(EnemyGenerator.instance.GenerateEnemy(spawnPoint, totalWave, gameObject.GetComponent<RoomController>()));
            playerIn = true;
            //CameraController.instance.GetCurrentRoom(gameObject);
        }
    }
}
