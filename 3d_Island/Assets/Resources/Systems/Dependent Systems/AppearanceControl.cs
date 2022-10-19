using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


[System.Serializable]
public class AppearanceControl 
{
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

    NPC myController;

    public void Initialize(NPC npc)
    {
        myController = npc;
    }
    public void UpdateAppearance()
    {
        float ageFactor = myController.character.age / myController.character.deathTime;
        wholeBody.transform.localScale = initialScale + (ageFactor * (finalScale - initialScale));

        //face
        face.material.color = Color.Lerp(normalColor, healthColor, myController.character.GetHealth());

        //upper
        upperBody.material.color = Color.Lerp(normalColor, extrovertColor, myController.character.GetExtroversion());

        //down
        foreach (SkinnedMeshRenderer renderer in downBody)
            renderer.material.color = Color.Lerp(normalColor, fertilityColor, myController.character.GetFertility());

        //Horns
        foreach (GameObject obj in horns)
            obj.transform.localScale = new Vector3(obj.transform.localScale.x, 0, obj.transform.localScale.z) + (Vector3.up * myController.character.GetAggressiveness());

        //hands and legs
        foreach (SkinnedMeshRenderer renderer in handsLegs)
            renderer.material.color = Color.Lerp(normalColor, powerColor, myController.character.GetPower());
    }
}
