using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


[System.Serializable]
public class PostProcessingFunctions
{
    [Header("References")]
    [SerializeField] Volume volume;

    DepthOfField depthOfField;


    public void Initialize()
    {
        VolumeProfile profile = volume.sharedProfile;
        depthOfField = (DepthOfField)(profile.components[2]);
    }
    public void SetBlur(bool _state)
    {
        if (!_state)
            depthOfField.active = false;
        else
            depthOfField.active = true;
    }
}
