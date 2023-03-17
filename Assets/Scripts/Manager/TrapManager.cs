using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapManager : MonoBehaviour
{
    public static List<string> trapGridName = new List<string>();
    [SerializeField] private List<string> trapName;

    private void Awake()
    {
        foreach(string s in trapName)
        {
            trapGridName.Add(s);
        }
    }
}
