using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int _lockFrameRate = 60;

    [Header("Game Design")]
    [SerializeField] bool _showFrameRate;
    [SerializeField] PlayerSystem _myPlayer;
    [SerializeField] GameObject _eggAsset;
    [SerializeField] GameObject _ballAsset;
    [SerializeField] NPC _npcAsset;

    [Header("Save Keys")]
    public static string growTime_saveString = "growTime";
    public static string boredTime_saveString = "boredTime";
    public static string sleepTime_saveString = "sleepTime";
    public static string seekPlayerProb_saveString = "seekPlayer";
    public static string seekNpcProb_saveString = "seekNpc";
    public static string seekTreeProb_saveString = "seekTree";
    public static string seekBallProb_saveString = "seekBall";
    public static string dropBallProb_saveString = "dropBall";
    public static string throwBallOnPlayerProb_saveString = "throwBallPlayer";
    public static string throwBallOnNpcProb_saveString = "throwBallNPC";
    public static string punchNpcProb_saveString = "punchNPC";

    void Start()
    {
        Application.targetFrameRate = _lockFrameRate;

        if (_myPlayer == null)
            FindObjectOfType<PlayerSystem>();

        if (!_showFrameRate)
            UIController.uIController.ShowFrameRate("");

        LoadSettings();
    }
    void Update()
    {
        if (_showFrameRate)
            UIController.uIController.ShowFrameRate((1f / Time.deltaTime).ToString());
    }

    
    //Settings
    public void ApplySettings()
    {
        var _npcs = FindObjectsOfType<NPC>();
        List<NPC> npcList = new List<NPC>();
        foreach (NPC npc in _npcs)
        {
            npcList.Add(npc);
        }

        //Change NPC asset first
        foreach (SliderElement slider in UIController.uIController.GetSliders())
        {
            if (slider._saveName == growTime_saveString)
                _npcAsset.growTime = slider._mySlider.value;
            else if (slider._saveName == boredTime_saveString)
                _npcAsset.boredTime = slider._mySlider.value;
            else if (slider._saveName == sleepTime_saveString)
                _npcAsset.sleepTime = slider._mySlider.value;
            else if (slider._saveName == seekNpcProb_saveString)
                _npcAsset.seekNpcProb = slider._mySlider.value;
            else if (slider._saveName == seekPlayerProb_saveString)
                _npcAsset.seekPlayerProb = slider._mySlider.value;
            else if (slider._saveName == seekBallProb_saveString)
                _npcAsset.seekBallProb = slider._mySlider.value;
            else if (slider._saveName == seekTreeProb_saveString)
                _npcAsset.seekTreeProb = slider._mySlider.value;
            else if (slider._saveName == dropBallProb_saveString)
                _npcAsset.dropBallProb = slider._mySlider.value;
            else if (slider._saveName == throwBallOnNpcProb_saveString)
                _npcAsset.throwBallOnNpcProb = slider._mySlider.value;
            else if (slider._saveName == throwBallOnPlayerProb_saveString)
                _npcAsset.throwBallOnPlayerProb = slider._mySlider.value;
            else if (slider._saveName == punchNpcProb_saveString)
                _npcAsset.punchNpcProb = slider._mySlider.value;
        }

        //then change the rest to its values (for optimization)
        foreach (NPC npc in npcList)
        {
            npc.growTime = _npcAsset.growTime;
            npc.boredTime = _npcAsset.boredTime;
            npc.sleepTime = _npcAsset.sleepTime;
            npc.seekPlayerProb = _npcAsset.seekPlayerProb;
            npc.seekNpcProb = _npcAsset.seekNpcProb;
            npc.seekBallProb = _npcAsset.seekBallProb;
            npc.seekTreeProb = _npcAsset.seekTreeProb;
            npc.dropBallProb = _npcAsset.dropBallProb;
            npc.throwBallOnNpcProb = _npcAsset.throwBallOnNpcProb;
            npc.throwBallOnPlayerProb = _npcAsset.throwBallOnPlayerProb;
            npc.punchNpcProb = _npcAsset.punchNpcProb;
        }

        SaveSettings();
        UIController.uIController.ShowSettings(false);
    }
    public void LoadSettings()
    {
        foreach (SliderElement slider in UIController.uIController.GetSliders())
            slider._mySlider.value = PlayerPrefs.GetFloat(slider._saveName);

        ApplySettings();
    }
    public void SaveSettings()
    {
        foreach (SliderElement slider in UIController.uIController.GetSliders())
            PlayerPrefs.SetFloat(slider._saveName, slider._mySlider.value);
    }


    //for design Buttons
    public void SpawnBall()
    {
        Instantiate(_ballAsset.gameObject, _myPlayer.transform.position + _myPlayer.transform.forward * 2f + Vector3.up * 5, Quaternion.identity);
    }
    public void SpawnEgg()
    {
        Instantiate(_eggAsset.gameObject, _myPlayer.transform.position + _myPlayer.transform.forward * 2f + Vector3.up * 5, Quaternion.identity);
    }



}
