using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


[System.Serializable]
public class AppearanceControl 
{
    [Header("Cloth Positions")]
    [SerializeField] public GameObject hatPosition;

    [Header("Cloth")]
    [SerializeField] public GameObject hat;

    [Header("Appearance")]
    [SerializeField] SkinnedMeshRenderer upperBody;
    [SerializeField] SkinnedMeshRenderer face;
    [SerializeField] List<SkinnedMeshRenderer> downBody;
    [SerializeField] List<SkinnedMeshRenderer> handsLegs;
    [SerializeField] List<GameObject> horns;

    [Header("Scale")]
    [SerializeField] GameObject wholeBody;
    [SerializeField] Vector3 initialScale = new Vector3(1f, 1f, 1f);
    [SerializeField] Vector3 finalScale = new Vector3(5f, 5f, 5f);

    [Header("Colors")]
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color extrovertColor;
    [SerializeField] Color fertilityColor;
    [SerializeField] Color powerColor;
    [SerializeField] Color healthColor;

    CharacterParameters character;

    public void Initialize(CharacterParameters character)
    {
        this.character = character;
    }
    public void UpdateAppearance()
    {
        float ageFactor = character.age / character.deathTime;
        wholeBody.transform.localScale = initialScale + (ageFactor * (finalScale - initialScale));

        //face
        face.material.color = Color.Lerp(normalColor, healthColor, character.GetFitness());

        //upper
        upperBody.material.color = Color.Lerp(normalColor, extrovertColor, character.GetExtroversion());

        //down
        foreach (SkinnedMeshRenderer renderer in downBody)
            renderer.material.color = Color.Lerp(normalColor, fertilityColor, character.GetFertility());

        //Horns
        foreach (GameObject obj in horns)
            obj.transform.localScale = new Vector3(obj.transform.localScale.x, 0, obj.transform.localScale.z) + (Vector3.up * character.GetAggressiveness());

        //hands and legs
        foreach (SkinnedMeshRenderer renderer in handsLegs)
            renderer.material.color = Color.Lerp(normalColor, powerColor, character.GetPower());
    }

    public void DropHat()
    {
        if(hat != null)
        {
            ServicesProvider.instance.DestroyObject(hat);
            hat = null;
            GameManager.instance.SpawnHat();
        }
    }
}
