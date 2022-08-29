using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSystem : MonoBehaviour, IDetectable
{
    [SerializeField] int breakForce = 50;
    [SerializeField] int breakTorque = 50;
    [SerializeField] Vector2 reSeedTime = new Vector2(0f, 1f);


    [Header("references")]
    [SerializeField] Animator animator;
    [SerializeField] GameObject branch1;
    [SerializeField] GameObject fruitAsset;

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public bool GotFruit()
    {
        if(branch1.GetComponent<CharacterJoint>() || branch1.GetComponent<CharacterJoint>())
            return true;
        else
            return false;
    }
    public void Shake()
    {
        animator.SetTrigger("Shake");
    }

    void Awake()
    {
        StartCoroutine(Seeding());
    }
    IEnumerator Seeding()
    {
        while(true)
        {
            float _randomTime = Random.Range(reSeedTime.x, reSeedTime.y);

            if (branch1.GetComponent<CharacterJoint>() == null)
            {
                StartCoroutine(SpawnFruit(branch1));
            }

            yield return new WaitForSecondsRealtime(_randomTime);
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
