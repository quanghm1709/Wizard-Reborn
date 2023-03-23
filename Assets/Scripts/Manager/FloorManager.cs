using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public static int currentFloor = 1;
    public static bool readyGenerate = false;

    private void Start()
    {
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnPlayerEnterGate, (param) => OnPlayerEnterGate());
    }

    private void OnPlayerEnterGate()
    {
        currentFloor++;
        SaveData.SaveSingleData("floor", currentFloor);
    }

    internal void Load()
    {
        currentFloor = SaveData.LoadSingleData("floor");
        readyGenerate = true;
    }
}
