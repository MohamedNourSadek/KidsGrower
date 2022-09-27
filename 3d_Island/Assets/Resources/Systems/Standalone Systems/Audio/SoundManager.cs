using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Audio;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Data")]
    [SerializeField] AudioMixerGroup ambientAudioMixer;
    [SerializeField] AudioMixerGroup uiAudioMixer;
    [SerializeField] AudioMixerGroup sfxAudioMixer;

    [SerializeField] AudioData pressAudio;

    [Header("FootSteps")]
    [SerializeField] List<AudioData> walkClips = new List<AudioData>();

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
        AudioSource source = this.gameObject.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = uiAudioMixer;
        source.loop = false;
        source.playOnAwake = false;

        source.clip = pressAudio.clip;
        source.volume = pressAudio.volume;

        source.Play();

        StartCoroutine(DestroyAfter(source, 1f));
    }
    IEnumerator DestroyAfter(AudioSource source, float t)
    {
        yield return new WaitForSecondsRealtime(t);
        Destroy(source);
    }

    
    //Internal Algorithms
    float GetdB(float volumeLinear)
    {
        return Mathf.Clamp((float)(20f * Math.Log10(volumeLinear)),-80f,20f);
    }
    float GetLinear(float volumeDB)
    {
        return Mathf.Pow(10f, volumeDB / 20f);
    }
}
