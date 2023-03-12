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
    private GameObject startRoom;
    private GameObject endRoom;

    private List<GameObject> listRoom = new List<GameObject>();
    private Direct direct;
    private int currentRoomId = 1;

    private void Start()
    {
        CreateStartRoom();
        GenerateRoom();
    }

    private void CreateStartRoom()
    {
        GameObject newRoom = Instantiate(instatiateRoom, generatorPoint.position, generatorPoint.rotation);
        newRoom.transform.parent = gridParent;
        newRoom.GetComponent<RoomController>().roomId = 0;
        this.PostEvent(EventID.OnRoomClear, 0);
        listRoom.Add(newRoom);

        direct = (Direct)Random.Range(0, 4);
        MoveGenerationPoint();
    }

    private void GenerateRoom()
    {
        for (int i = 0; i < distanceToEnd; i++)
        {
            GameObject newRoom = Instantiate(instatiateRoom, generatorPoint.position, generatorPoint.rotation);
            newRoom.transform.parent = gridParent;
            newRoom.GetComponent<RoomController>().roomId = currentRoomId;
            currentRoomId++;
            listRoom.Add(newRoom);

            //if (i + 1 == distanceToEnd)
            //{
            //    // newRoom.GetComponent<SpriteRenderer>().color = endColor;
            //    listRoom.RemoveAt(listRoom.Count - 1);

            //    endRoom = newRoom;
            //}

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
