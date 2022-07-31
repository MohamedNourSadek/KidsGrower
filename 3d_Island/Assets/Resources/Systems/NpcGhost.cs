using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcGhost : MonoBehaviour
{
    [SerializeField] float _speed = 1f;
    [SerializeField] float _arrivalDistance = 1f;

    bool _moving = false;
    Vector3 _destination = new Vector3();


    void Awake()
    {
        _destination = MapSystem.GetRandomExplorationPoint();
        _moving = true;

        StartCoroutine(_TakeDecision());
    }
    IEnumerator _TakeDecision()
    {
        while(true)
        {
            if (_moving)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, _destination, _speed * Time.fixedDeltaTime);

                this.transform.LookAt(_destination);

                if ((this.transform.position - _destination).magnitude <= _arrivalDistance)
                    _moving = false;
            }
            else
            {
                _destination = MapSystem.GetRandomExplorationPoint();
                _moving = true;
            }

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        

    }
}
