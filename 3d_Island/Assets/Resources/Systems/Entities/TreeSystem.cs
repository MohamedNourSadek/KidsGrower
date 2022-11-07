using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSystem : Tearable, IDetectable, ISavable
{
    [SerializeField] int breakForce = 50;
    [SerializeField] int breakTorque = 50;
    [SerializeField] Vector2 reSeedTime = new Vector2(0f, 1f);

    [Header("references")]
    [SerializeField] Animator animator;
    [SerializeField] GameObject branch1;
    [SerializeField] GameObject fruitAsset;

    void Awake()
    {
        StartCoroutine(Seeding());
    }
    public GameObject GetGameObject()
    {
        if (this.gameObject == null)
            return null;
        else 
            return this.gameObject;
    }
    public void LoadData(SaveStructure savaData)
    {
        Tree_Data tree = (Tree_Data)savaData;
        transform.position = tree.position.GetVector();
        transform.rotation = tree.rotation.GetQuaternion();
        tearingDownCount = tree.tearDownCount;
    }
    public Tree_Data GetData()
    {
        Tree_Data tree_Data = new Tree_Data();
        tree_Data.position = new nVector3(transform.position);
        tree_Data.rotation = new nQuaternion(transform.rotation);
        tree_Data.tearDownCount = tearingDownCount;
        return tree_Data;
    }

    public bool GotFruit()
    {
        if(branch1.GetComponent<CharacterJoint>() || branch1.GetComponent<CharacterJoint>())
            return true;
        else
            return false;
    }
    public override void Shake()
    {
        animator.SetTrigger("Shake");
        if (SoundManager.instance != null)
            SoundManager.instance.PlayTreeShake(this.gameObject);
    }


    IEnumerator Seeding()
    {
        while(true)
        {
            float _randomTime = Random.Range(reSeedTime.x, reSeedTime.y);

            yield return new WaitForSecondsRealtime(_randomTime);

            if (branch1.GetComponent<CharacterJoint>() == null)
            {
                StartCoroutine(SpawnFruit(branch1));
            }

        }
    }
    IEnumerator SpawnFruit(GameObject _branch)
    {
        CharacterJoint _myBrach;
        //
        if (_branch.GetComponent<CharacterJoint>() == null)
        {
            _myBrach = _branch.AddComponent<CharacterJoint>();
        }
        else
        {
            _myBrach = _branch.GetComponent<CharacterJoint>();
        }

        GameObject _fruit = Instantiate(fruitAsset, _myBrach.gameObject.transform.position, Quaternion.identity);

        _myBrach.connectedBody = _fruit.GetComponent<Rigidbody>();

        yield return new WaitForSecondsRealtime(3f);

        _myBrach.breakForce = breakForce;
        _myBrach.breakTorque = breakTorque;


    }
}
