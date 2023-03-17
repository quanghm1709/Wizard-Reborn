using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public List<AudioSource> audioSource;

    private void Start()
    {
        instance = this;
        audioSource[0].Play();      
    }
}
