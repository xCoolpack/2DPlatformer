using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioMixer audioMixer;

    public static MusicController Instance;

    private float musicVolume = 1f;

    private void Awake()
    {            
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);       
        audioSource = GetComponent<AudioSource>();
        PlayMusic();
    }

    void FixedUpdate()
    {
        audioSource.volume = musicVolume;
    }

    public void PlayMusic()
    {
        audioSource.Play();
        audioSource.loop = true;
    }   

    public void VolumeUpdate(float volume)
    {
        audioMixer.SetFloat("GameVolume", volume);
    }
}
