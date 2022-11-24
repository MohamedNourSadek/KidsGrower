using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour, ISavable, IDetectable
{
    [SerializeField] public HealthControl healthControl;
    [SerializeField] protected DetectorSystem detector;
    [SerializeField] protected NavMeshAgent myAgent;

    [SerializeField] int dayDamagePerSec = 5;
    [SerializeField] float attackedTime = 1f;
    public static int count = 0;

    public void LoadData(SaveStructure saveData)
    {
        Zombie_Data zombieData = (Zombie_Data)saveData;

        transform.position = zombieData.position.GetVector();
        transform.rotation = zombieData.rotation.GetQuaternion();
        healthControl.currentHealth = zombieData.health;
    }
    public Zombie_Data GetData()
    {
        Zombie_Data zombieData = new Zombie_Data();

        zombieData.position = new nVector3(transform.position);
        zombieData.rotation = new nQuaternion(transform.rotation);
        zombieData.health = healthControl.currentHealth;

        return zombieData;
    }

    void Start()
    {
        count++;
        healthControl.Initialize(this.gameObject);
        StartCoroutine(DayDamageLoop());
    }
    private void Update()
    {
        healthControl.Update();
    }
    void OnDestroy()
    {
        UIGame.instance.DestroySlider(gameObject);  
        count--;
    }

    IEnumerator DayDamageLoop()
    {
        while(true)
        {
            if(DayNightControl.instance.IsDay() == true)
            {
                healthControl.GetAttacked(dayDamagePerSec, this.transform);

                yield return new WaitForSecondsRealtime(1f);
            }

            yield return new WaitForFixedUpdate();
        }
    }
    public GameObject GetGameObject()
    {
        return this.gameObject;    
    }
}
