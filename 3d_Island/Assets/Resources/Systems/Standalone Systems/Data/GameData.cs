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
    public List<Tree_Data> trees = new List<Tree_Data>();
    public List<FertilityBoost_Data> fertilityBoosts = new List<FertilityBoost_Data>();
    public List<ExtroversionBoost_Data> extroversionBoosts = new List<ExtroversionBoost_Data>();
    public List<AggressivenessBoost_Data> aggressivenessBoosts = new List<AggressivenessBoost_Data>();
    public List<PowerBoost_Data> powerboosts = new List<PowerBoost_Data>();
    public List<HealthBoost_Data> healthboosts = new List<HealthBoost_Data>();
    public List<WoodPack_Data> woodpacks = new List<WoodPack_Data>();
    public List<StonePack_Data> stonepacks = new List<StonePack_Data>();
}
