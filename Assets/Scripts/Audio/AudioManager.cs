using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        foreach (var sound in sounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.clip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.loop = sound.loop;
            sound.audioSource.playOnAwake = false;
        }
    }
    private void Start()
    {
       Play("Music");
    }
    
    public void PlayOneShot(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Ses : " + name + " bulunamadı");
        }
        s.audioSource.PlayOneShot(s.audioSource.clip);
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Ses : " + name + " bulunamadı");
        }

        s.audioSource.Play();
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Ses : " + name + " bulunamadı");
        }
        s.audioSource.Stop();
    }
    public void PlayWithRandomPitch(string name,float randomVal1,float randomVal2)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Ses : " + name + " bulunamadı");
            return;
        }

        s.audioSource.pitch = Random.Range(randomVal1, randomVal2);  
        s.audioSource.Play();
    }

}