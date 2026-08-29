using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    public ObjectPool itemPool;

    private void Start()
    {
        instance = this;
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnPlayerEnterGate, HandlePlayerEnterGate);
    }

    private void OnDestroy()
    {
        this.RemoveListener(EventID.OnPlayerEnterGate, HandlePlayerEnterGate);
    }

    private void HandlePlayerEnterGate(object param)
    {
        OnPlayerEnterGate();
    }

    private void OnPlayerEnterGate()
    {
        foreach(GameObject g in itemPool.pooledGobjects)
        {
            g.SetActive(false);
        }
    }
}
