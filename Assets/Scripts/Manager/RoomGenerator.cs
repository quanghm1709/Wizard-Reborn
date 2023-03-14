using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private int distanceToEnd;
    [SerializeField] private Transform generatorPoint;
    [SerializeField] private Transform gridParent;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;

    [SerializeField] private GameObject instatiateRoom;
    [SerializeField] private GameObject startRoom;
    [SerializeField] private GameObject shopRoom;
    [SerializeField] private GameObject endRoom;

    public ObjectPool roomPool;
    //private List<GameObject> listRoom = new List<GameObject>();
    private Direct direct;
    private int currentRoomId = 1;
    private Vector3 startRoomPos;

    private void Start()
    {
        RegisterEvent();
        startRoomPos = generatorPoint.position;
        CreateFloor();   
    }

    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnPlayerEnterGate, (param) => OnPlayerEnterGate());
    }

    private void OnPlayerEnterGate()
    {
        ResetFloor();
    }

    private void ResetFloor()
    {
        foreach(GameObject g in roomPool.pooledGobjects)
        {
            g.GetComponent<RoomController>().ResetRoom();
            g.SetActive(false);
        }
        generatorPoint.position = startRoomPos;
        CreateFloor();
    }

    public void CreateFloor()
    {
        CreateSingleRoom(0, startRoom);
        GenerateRoom();
        CreateSingleRoom(0, endRoom);      
    }

    private void CreateSingleRoom(int i, GameObject room)
    {
        GameObject newRoom = roomPool.GetObject(room.name);//Instantiate(room, generatorPoint.position, generatorPoint.rotation);
        newRoom.transform.position = generatorPoint.position;
        newRoom.transform.parent = gridParent;

        newRoom.GetComponent<RoomController>().roomId = i;
        this.PostEvent(EventID.OnRoomClear, i);
        //listRoom.Add(newRoom);

        direct = (Direct)Random.Range(0, 4);
        MoveGenerationPoint();
    }

    private void GenerateRoom()
    {
        for (int i = 0; i < distanceToEnd; i++)
        {
            GameObject newRoom = roomPool.GetObject(instatiateRoom.name);//Instantiate(instatiateRoom, generatorPoint.position, generatorPoint.rotation);
            newRoom.transform.position = generatorPoint.position;
            newRoom.transform.parent = gridParent;
            
            newRoom.GetComponent<RoomController>().roomId = currentRoomId;
            currentRoomId++;
            //listRoom.Add(newRoom);

            direct = (Direct)Random.Range(0, 4);
            MoveGenerationPoint();

            while (Physics2D.OverlapCircle(generatorPoint.position, .2f))
            {
                MoveGenerationPoint();
            }
        }

    }

    private void MoveGenerationPoint()
    {
        switch (direct)
        {
            case Direct.up:
                generatorPoint.position += new Vector3(0f, yOffset, 0f);
                break;

            case Direct.down:
                generatorPoint.position += new Vector3(0f, -yOffset, 0f);
                break;

            case Direct.right:
                generatorPoint.position += new Vector3(xOffset, 0f, 0f);
                break;

            case Direct.left:
                generatorPoint.position += new Vector3(-xOffset, 0f, 0f);
                break;
        }
    }
}
