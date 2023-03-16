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
    }
}
