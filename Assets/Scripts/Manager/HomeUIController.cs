using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeUIController : MonoBehaviour
{
    [Header("Loading")]
    [SerializeField] private GameObject loadScene;
    [SerializeField] private Slider loadSlider;
    private bool isLoadScene = false;

    [Header("Function")]
    [SerializeField] private GameObject functionScreen;
    [SerializeField] private GameObject creScreen;
    [SerializeField] private GameObject settingScreen;
    [SerializeField] private GameObject startChoiceBtn;
    [SerializeField] private GameObject startBtn;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider sound;
    
    private void Start()
    {
        audioSource.Play();
    }

    private void Update()
    {
        if (isLoadScene)
        {
            loadScene.SetActive(true);
            loadSlider.value += Time.deltaTime;
            if(loadSlider.value>= loadSlider.maxValue)
            {
                SceneManager.LoadScene("GameScene");
            }
        }

        audioSource.volume = sound.value;
    }

    public void StartGame(int i)
    {
        if (i == 1)
        {
            SaveData.ResetData();
        }

        isLoadScene = true;
        loadSlider.maxValue = 2f;
    }

    public void StartChoice()
    {
        if (SaveData.HasKey("Player"))
        {
            startChoiceBtn.SetActive(true);
            startBtn.SetActive(false);
        }
        else
        {
            StartGame(1);
        }
        
    }
    public void OpenFunctionScreen(int i)
    {
        if (functionScreen.activeInHierarchy)
        {
            functionScreen.SetActive(false);
            creScreen.SetActive(false);
            settingScreen.SetActive(false);
        }
        else
        {
            functionScreen.SetActive(true);
            if (i == 0)
            {
                creScreen.SetActive(true);
            }
            else
            {
                settingScreen.SetActive(true);
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
