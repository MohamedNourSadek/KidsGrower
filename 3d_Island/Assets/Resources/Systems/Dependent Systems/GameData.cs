using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Player_Data player = new Player_Data();
    public List<NPC_Data> npcs = new List<NPC_Data>();
    public List<Ball_Data> balls = new List<Ball_Data>();
    public List<Egg_Data> eggs = new List<Egg_Data>();
    public List<Fruit_Data> fruits = new List<Fruit_Data>();
    public List<Harvest_Data> harvests = new List<Harvest_Data>();
    public List<Seed_Data> seeds = new List<Seed_Data>();

}
