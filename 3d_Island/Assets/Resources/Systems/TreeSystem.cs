using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSystem : MonoBehaviour, IDetectable
{
    [SerializeField] int _breakForce = 50;
    [SerializeField] int _breakTorque = 50;
    [SerializeField] Vector2 _reSeedTime = new Vector2(0f, 1f);


    [Header("references")]
    [SerializeField] Animator _animator;
    [SerializeField] GameObject _branch1;
    [SerializeField] GameObject _branch2;
    [SerializeField] GameObject _fruitAsset;
   
    public void Shake()
    {
        _animator.SetTrigger("Shake");
    }


    void Awake()
    {
        StartCoroutine(Seeding());
    }
    IEnumerator Seeding()
    {
        while(true)
        {
            float _randomTime = Random.Range(_reSeedTime.x, _reSeedTime.y);

            if (_branch1.GetComponent<CharacterJoint>() == null)
            {
                StartCoroutine(SpawnFruit(_branch1));
            }
            if (_branch2.GetComponent<CharacterJoint>() == null)
            {
                StartCoroutine(SpawnFruit(_branch2));
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

        GameObject _fruit = Instantiate(_fruitAsset, _myBrach.gameObject.transform.position, Quaternion.identity);

        _myBrach.connectedBody = _fruit.GetComponent<Rigidbody>();

        yield return new WaitForSecondsRealtime(3f);

        _myBrach.breakForce = _breakForce;
        _myBrach.breakTorque = _breakTorque;


    }

}
