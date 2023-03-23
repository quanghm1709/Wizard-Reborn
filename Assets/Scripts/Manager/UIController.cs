using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [Header("Data Bar")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider mpBar;
    [SerializeField] private Image expShow;
    [SerializeField] private Text currentLevel;
    [SerializeField] private Text currentGold;

    [Header("Menu")]
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject quitScreen;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private float loadTime;
    [SerializeField] private bool isPlayerEnterGate = false;
    private bool endLoad = true;

    [Header("Floor")]
    [SerializeField] private Text floor;

    [Header("End Screen")]
    [SerializeField] private GameObject overPanel;

    private float loadTimeCD;
    private PlayerController player;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        //instance = this;

        RegisterEvent();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        loadTimeCD = loadTime;
        loadTime = 0;
    }

    private void Update()
    {
        GetPlayerCurrentData();
        ActionLoading();

        floor.text = "Floor " + FloorManager.currentFloor.ToString();
        currentGold.text = GoldManager.playerGold.ToString() + "g";
    }

    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnPlayerEnterGate, (param) => isPlayerEnterGate = true);
        this.RegisterListener(EventID.OnPlayerDead, (param) => OnPlayerDead());
    }

    private void OnPlayerDead()
    {
        Time.timeScale = 0f;
        overPanel.SetActive(true);
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
            this.PostEvent(EventID.OnOpenMenu);
            Time.timeScale = 0;
        }
    }

    public void ActionLoading()
    {
        if (isPlayerEnterGate)
        {
            endLoad = false;
            loadingScreen.SetActive(true);
            loadingSlider.maxValue = loadTimeCD;
            loadingSlider.value = loadTime;
            loadTime += Time.deltaTime;

            if (loadTime >= loadTimeCD)
            {
                loadingScreen.SetActive(false);
                loadTime = 0;
                endLoad = true;
                isPlayerEnterGate = false;
            }
        }
    }

    public void Replay()
    {
        Time.timeScale = 1f;
        isPlayerEnterGate = true;
        if (endLoad)
        {
            SceneManager.LoadScene("GameScene");
        }   
    }

    public void BackHome()
    {
        isPlayerEnterGate = true;
        if (endLoad)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("HomeScene");
        }
    }
    public void ActionQuit()
    {
        if (quitScreen.activeInHierarchy)
        {
            quitScreen.SetActive(false);
        }
        else
        {
            quitScreen.SetActive(true);
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
