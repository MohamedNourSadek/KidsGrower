using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour, ISavable, IDetectable
{
    [SerializeField] protected DetectorSystem detector;
    [SerializeField] protected NavMeshAgent myAgent;
    [SerializeField] int initialHealth = 30;
    [SerializeField] int currentHealth = 30;
    [SerializeField] int dayDamagePerSec = 5;
    [SerializeField] float attackedTime = 1f;

    public static int count = 0;
    bool beingAttacked = false;

    public void LoadData(SaveStructure saveData)
    {
        Zombie_Data zombieData = (Zombie_Data)saveData;

        transform.position = zombieData.position.GetVector();
        transform.rotation = zombieData.rotation.GetQuaternion();
        currentHealth = zombieData.health;
    }
    public Zombie_Data GetData()
    {
        Zombie_Data zombieData = new Zombie_Data();

        zombieData.position = new nVector3(transform.position);
        zombieData.rotation = new nQuaternion(transform.rotation);
        zombieData.health = currentHealth;

        return zombieData;
    }

    void Start()
    {
        count++;

        StartCoroutine(HealthControl());
    }
    void OnDestroy()
    {
        count--;
    }

    public void GetAttacked(int amount)
    {
        if (beingAttacked == false)
            StartCoroutine(GetAttackedEnum(amount));
    }
    IEnumerator HealthControl()
    {
        while(true)
        {
            if(DayNightControl.instance.IsDay() == true)
            {
                GetDamaged(dayDamagePerSec);

                yield return new WaitForSecondsRealtime(1f);
            }

            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator GetAttackedEnum(int amount)
    {
        beingAttacked = true;

        SoundManager.instance.PlayRockShake(this.gameObject);

        GetDamaged(amount);

        yield return new WaitForSecondsRealtime(attackedTime);

        beingAttacked = false;
    }
    void GetDamaged(int amount)
    {
        currentHealth -= amount;

        if (currentHealth < 0)
        {
            UIGame.instance.ShowRepeatingMessage("Dead !", this.transform, 1f, 1f, new ConditionChecker(true));

            Destroy(this.gameObject);

            GameManager.instance.SpawnBoost("PowerBoost", this.transform.position);
        }

        UIGame.instance.ShowRepeatingMessage("Damage", this.transform, 1f, 1f, new ConditionChecker(true));
    }
    public GameObject GetGameObject()
    {
        return this.gameObject;    
    }
}
