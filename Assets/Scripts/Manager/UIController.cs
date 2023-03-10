using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    [Header("Data Bar")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider mpBar;
    [SerializeField] private Image expShow;
    [SerializeField] private Text currentLevel;
    [SerializeField] private GameObject menu;
    private PlayerController player;

    private void Start()
    {
        instance = this;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        GetPlayerCurrentData();
    }

    private void GetPlayerCurrentData()
    {
        hpBar.value = player.currentHp;
        hpBar.maxValue = player.maxHp;
        mpBar.value = player.currentMp;
        mpBar.maxValue = player.maxMp;
    }

    public void GetPlayerCurrentLevel(int level, float currentExp, float maxExp)
    {
        expShow.fillAmount = currentExp / maxExp;
        currentLevel.text = level.ToString();
    }

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
