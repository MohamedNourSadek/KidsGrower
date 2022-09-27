using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    [SerializeField] public float sfxVolume = 1f;
    [SerializeField] public float ambinetVolume = 1f;
    [SerializeField] public float uiVolume = 1f;
    [SerializeField] public bool grass = true;
    [SerializeField] public bool shadows = true;
}
