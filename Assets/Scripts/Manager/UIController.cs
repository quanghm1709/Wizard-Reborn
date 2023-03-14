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

    [Header("Menu")]
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private float loadTime;
    [SerializeField] private bool isPlayerEnterGate = false;

    private float loadTimeCD;
    private PlayerController player;

    private void Start()
    {
        instance = this;
        RegisterEvent();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        loadTimeCD = loadTime;
        loadTime = 0;
    }

    private void Update()
    {
        GetPlayerCurrentData();
        ActionLoading();
    }

    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnPlayerEnterGate, (param) => isPlayerEnterGate = true);
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
            loadingScreen.SetActive(true);
            loadingSlider.maxValue = loadTimeCD;
            loadingSlider.value = loadTime;
            loadTime += Time.deltaTime;

            if (loadTime >= loadTimeCD)
            {
                loadingScreen.SetActive(false);
                loadTime = 0;
                isPlayerEnterGate = false;
            }
        }
    }
}
