using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private int distanceToEnd;
    [SerializeField] private Transform generatorPoint;
    [SerializeField] private Transform gridParent;
    [SerializeField] private LayerMask roomLayer;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;

    [SerializeField] private GameObject instatiateRoom;
    [SerializeField] private GameObject startRoom;
    [SerializeField] private GameObject shopRoom;
    [SerializeField] private GameObject endRoom;

    [SerializeField] private List<GameObject> listRoom;
    [SerializeField] private ObjectPool trapPool;

    private Direct direct;
    private int currentRoomId = 1;
    private Vector3 startRoomPos;
    private int specialRoom = 0;

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
        foreach (GameObject g in trapPool.pooledGobjects)
        {
            g.SetActive(false);
        }

        foreach (GameObject g in listRoom)
        {
            Destroy(g);
        }
        listRoom = new List<GameObject>();



        generatorPoint.position = startRoomPos;
        CreateFloor();
    }

    public void CreateFloor()
    {
        GenerateRoom();     
    }

    private void GenerateRoom()
    {
        GameObject stRoom = Instantiate(startRoom, generatorPoint.position, generatorPoint.rotation);
        stRoom.transform.position = generatorPoint.position;
        stRoom.transform.parent = gridParent;
        direct = (Direct)Random.Range(0, 4);
        MoveGenerationPoint();
        listRoom.Add(stRoom);

        int randomPos = -1;
        if (specialRoom == 3)
        {
            randomPos = Random.Range(2, distanceToEnd-1);
        }

        for (int i = 0; i < distanceToEnd; i++)
        {
            GameObject newRoom = null;

            if (i == randomPos && specialRoom == 3)
            {
                newRoom = Instantiate(shopRoom, generatorPoint.position, generatorPoint.rotation);// roomPool.GetObject(instatiateRoom.name);//Instantiate(instatiateRoom, generatorPoint.position, generatorPoint.rotation);
            }
            else
            {
                newRoom = Instantiate(instatiateRoom, generatorPoint.position, generatorPoint.rotation);// roomPool.GetObject(instatiateRoom.name);//Instantiate(instatiateRoom, generatorPoint.position, generatorPoint.rotation);

                float rand = Random.Range(0f, 1f);
                if(rand > .4f)
                {
                    GameObject trap = trapPool.GetObject(TrapManager.trapGridName[Random.Range(0, TrapManager.trapGridName.Count - 1)]);
                   // trap.transform.parent = newRoom.transform;
                    trap.transform.position = newRoom.transform.position;
                }
            }


            newRoom.transform.position = generatorPoint.position;
            newRoom.transform.parent = gridParent;
            
            newRoom.GetComponent<RoomController>().roomId = currentRoomId;
            currentRoomId++;
            listRoom.Add(newRoom);
            direct = (Direct)Random.Range(0, 4);
            MoveGenerationPoint();

            while (Physics2D.OverlapCircle(generatorPoint.position, .2f, roomLayer))
            {
                MoveGenerationPoint();
            }
        }

        GameObject enRoom = Instantiate(endRoom, generatorPoint.position, generatorPoint.rotation);
        enRoom.transform.position = generatorPoint.position;
        enRoom.transform.parent = gridParent;
        direct = (Direct)Random.Range(0, 4);
        listRoom.Add(enRoom);

        specialRoom++;
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
