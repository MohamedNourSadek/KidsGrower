using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Audio;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Data")]
    [SerializeField] AudioMixerGroup ambientAudioMixer;
    [SerializeField] AudioMixerGroup uiAudioMixer;
    [SerializeField] AudioMixerGroup sfxAudioMixer;
    [SerializeField] AudioData pressAudio;

    [Header("Audio Data")]
    [SerializeField] List<AudioData> walkClips = new List<AudioData>();
    [SerializeField] List<AudioData> hitClips = new List<AudioData>();
    [SerializeField] AudioData spawn;
    [SerializeField] AudioData treeShake;
    [SerializeField] AudioData rockShake;

    [Header("Ambient Settings")]
    [SerializeField] List<AudioData> ambientMusic = new List<AudioData>();
    [SerializeField] float ambientInSpeed = 1f;
    [SerializeField] float startAmbientAfter = 5f;
    [SerializeField] float timeBetweenAmbient = 5f;

    AudioSource ambientSource;
    int currentPlaying = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        InitializeAmbientMusic();
    }

    
    //Interface for controlling Volume
    public void SetUiVolume(float newVolume)
    {
        uiAudioMixer.audioMixer.SetFloat("Volume", GetdB(newVolume));
    }
    public void SetAmbient(float newVolume)
    {
        ambientAudioMixer.audioMixer.SetFloat("Volume", GetdB(newVolume));
    }
    public void SetSFX(float newVolume)
    {
        sfxAudioMixer.audioMixer.SetFloat("Volume", GetdB(newVolume));
    }


    //Sound effects
    public void PlayHit(GameObject sourceObj, float volume)
    {
        int random = UnityEngine.Random.Range(0, hitClips.Count - 1);
        CreateAndPlayAudio(sourceObj, sfxAudioMixer, hitClips[random], volume);
    }
    public void PlayWalk(GameObject sourceObj, float volume)
    {
        int random = UnityEngine.Random.Range(0, walkClips.Count - 1);
        CreateAndPlayAudio(sourceObj, sfxAudioMixer, walkClips[random], volume);
    }
    public void PlaySpawn(GameObject sourceObj)
    {
        CreateAndPlayAudio(sourceObj, sfxAudioMixer, spawn, 1f);
    }
    public void PlayTreeShake(GameObject sourceObj)
    {
        CreateAndPlayAudio(sourceObj, sfxAudioMixer, treeShake, 1f);
    }
    public void PlayRockShake(GameObject sourceObj)
    {
        CreateAndPlayAudio(sourceObj, sfxAudioMixer, rockShake, 1f);
    }



    //Ambient music
    void InitializeAmbientMusic()
    {
        ambientSource = this.gameObject.AddComponent<AudioSource>();
        ambientSource.outputAudioMixerGroup = ambientAudioMixer;
        ambientSource.loop = false;
        ambientSource.playOnAwake = false;

        StartCoroutine(PlayAmbient());
    }
    IEnumerator PlayAmbient()
    {
        yield return new WaitForSecondsRealtime(startAmbientAfter);

        ReshuffleAmbientList();

        while (true)
        {
            if (currentPlaying >= ambientMusic.Count - 1)
            {
                currentPlaying = 0;
                ReshuffleAmbientList();
            }
            else
            {
                currentPlaying++;
            }

            ambientSource.volume = 0f;
            ambientSource.clip = ambientMusic[currentPlaying].clip;
            ambientSource.Play();

            //Smoothly raise volume
            while (ambientSource.volume <= ambientMusic[currentPlaying].volume)
            {
                ambientSource.volume += Time.fixedDeltaTime * ambientInSpeed;
                yield return new WaitForFixedUpdate();
            }
            ambientSource.volume = ambientMusic[currentPlaying].volume;

            yield return new WaitForSeconds(ambientSource.clip.length + timeBetweenAmbient);
        }
    }
    void ReshuffleAmbientList()
    {
        List<AudioData> newList = new List<AudioData>();

        while (ambientMusic.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, ambientMusic.Count);
            newList.Add(ambientMusic[index]);
            ambientMusic.RemoveAt(index);
        }

        ambientMusic = newList;
    }


    //Button Related audio
    public void InitializeButton(CButton button)
    {
        button.onClick.AddListener(OnButtonPress);
    }
    public void OnButtonPress()
    {
        CreateAndPlayAudio(this.gameObject, uiAudioMixer, pressAudio, 1f);
    }
    IEnumerator DestoryComponent(AudioSource source)
    {
        yield return new WaitForSeconds(2f);
        Destroy(source);
    }


    //Internal Algorithms
    float GetdB(float volumeLinear)
    {
        return Mathf.Clamp((float)(20f * Math.Log10(volumeLinear)),-80f,20f);
    }
    void CreateAndPlayAudio(GameObject sourceObj, AudioMixerGroup group, AudioData audio, float audioMultiplier) 
    {
        AudioSource audioSource = sourceObj.AddComponent<AudioSource>();

        audioSource.outputAudioMixerGroup = group;
        audioSource.clip = audio.clip;
        audioSource.volume = audio.volume * audioMultiplier;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 20;
        audioSource.minDistance = 0;


        audioSource.Play();

        StartCoroutine(DestoryComponent(audioSource));
    }
}
