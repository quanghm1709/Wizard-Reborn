using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public List<AudioSource> audioSource;
    private float runTime;
    private void Start()
    {
        instance = this;
        audioSource[0].Play();      
    }

    private void Update()
    {
        runTime += Time.deltaTime;
        if (runTime > 5)
        {
            audioSource[0].volume -= Time.deltaTime*.1f;
        }
        if (runTime > 10)
        {
            audioSource[0].Stop();
        }
    }
}
