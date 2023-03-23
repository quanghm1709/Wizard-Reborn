using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private FloorManager floorManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerLevelManager playerLevelManager;
    [SerializeField] private RoomGenerator roomGenerator;

    private void Start()
    {
        if (SaveData.HasKey("Player"))
        {
            try
            {
                floorManager.Load();
                playerController.Load();
                playerLevelManager.Load();
                roomGenerator.Load();
                Debug.Log("Load Success");
            }catch(Exception ex)
            {
                Debug.Log(ex.Message);
            }

        }
        else
        {
            FloorManager.readyGenerate = true;
        }
    }
}
