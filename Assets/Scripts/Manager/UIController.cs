using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    public void ActionMenu()
    {
        if (menu.activeInHierarchy)
        {
            menu.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            menu.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
