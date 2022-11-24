using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class ListAttributeUI 
{
    [SerializeField] TextMeshProUGUI levelNumber;
    [SerializeField] TextMeshProUGUI saveName;
    [SerializeField] AttributeUI age;
    [SerializeField] AttributeUI health;
    [SerializeField] AttributeUI fertility;
    [SerializeField] AttributeUI extroversion;
    [SerializeField] AttributeUI aggressiveness;
    [SerializeField] AttributeUI power;
     
    public void UpdateUI(CharacterParameters data)
    {
        levelNumber.text = "Level " + data.levelControl.GetLevel().ToString();
        saveName.text = data.saveName;
        age.value.text = data.age.ToString();

        health.value.text = data.GetFitness().ToString();
        fertility.value.text = data.GetFertility().ToString();
        extroversion.value.text = data.GetExtroversion().ToString();
        aggressiveness.value.text = data.GetAggressiveness().ToString();
        power.value.text = data.GetPower().ToString();
    }
}

