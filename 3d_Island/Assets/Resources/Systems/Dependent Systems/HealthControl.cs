using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ActionDelgate();

[System.Serializable]
public class HealthControl
{
    [SerializeField] bool DestroyOnDeath = true;
    [SerializeField] public int currentHealth = 30;
    [SerializeField] public int maxHealth = 30;

    public event ActionDelgate OnDeath;

    GameObject myEntity;

    
    public void Initialize(GameObject entity)
    {
        myEntity = entity;

        UIGame.instance.CreateSlider(entity, entity.transform, maxHealth);
        UIGame.instance.UpdateSlider(entity, currentHealth);

        if(DestroyOnDeath)
        {
            OnDeath += DestroyMe;
        }
    }
    public void Update() 
    {
        UIGame.instance.UpdateSlider(myEntity, currentHealth);
    }

    public GameObject GetMyEntity()
    {
        if (myEntity != null)
            return myEntity;
        else
            return null;
    }
    public void GetAttacked(int amount, Transform attacker)
    {
        Vector3 direction = (this.myEntity.transform.position - attacker.position).normalized;
        
        myEntity.GetComponent<Rigidbody>().AddForce(amount * 25f * direction, ForceMode.Acceleration);

        GetDamaged(amount);
    }
    void GetDamaged(int amount)
    {
        SoundManager.instance.PlayRockShake(myEntity);

        currentHealth -= amount;

        if (currentHealth < 0)
        {
            UIGame.instance.ShowRepeatingMessage("Dead !", myEntity.transform, 1f, 1f, new ConditionChecker(true));

            OnDeath.Invoke();

            currentHealth = maxHealth;
        }

        UIGame.instance.ShowRepeatingMessage("Damage", myEntity.transform, 1f, 1f, new ConditionChecker(true));
    }

    void DestroyMe()
    {
        ServicesProvider.instance.DestroyObject(myEntity);
        GameManager.instance.SpawnBoost("PowerBoost", myEntity.transform.position);
    }
}
