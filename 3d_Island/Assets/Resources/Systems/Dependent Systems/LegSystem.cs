using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LegSystem 
{
    [SerializeField] float walkAudioVolume;
    [SerializeField] List<Leg> legs = new List<Leg>();
    IController controller;

    public void Initialize(IController controller)
    {
        this.controller = controller;

        foreach (Leg leg in legs)
            leg.Initialize(this);
    }
    public void PlayWalk()
    {
        SoundManager.instance.PlayWalk(controller.GetBody().gameObject,walkAudioVolume);
    }


}
