using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] List<Transform> zombieSpawnLocations;
    [SerializeField] float updateEvery = 1f;
    [SerializeField] float maxNumber = 10;

    void Awake()
    {
        StartCoroutine(SpawnZombiesAtNight());
    }
    IEnumerator SpawnZombiesAtNight()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(updateEvery);

            if(DayNightControl.instance.IsDay() == false && Zombie.count < maxNumber)
            {
                SpawnRegularZombie();
            }
        }
    }
    void SpawnRegularZombie()
    {
        var _randomLocation = Random.Range(0, zombieSpawnLocations.Count);
        Vector3 randomLocation = zombieSpawnLocations[_randomLocation].transform.position;

        GameManager.instance.SpawnRegularZombie(randomLocation).GetComponent<Zombie>();
    }
}
