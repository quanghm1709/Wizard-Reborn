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

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;

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
    }

    public void StartGame()
    {
        isLoadScene = true;
        loadSlider.maxValue = 2f;
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
