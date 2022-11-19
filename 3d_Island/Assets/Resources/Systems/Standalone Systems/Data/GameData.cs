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
    public List<NamingHouse_Data> namingHouses = new List<NamingHouse_Data>();
    public List<CraftingBench_Data> craftingBenches = new List<CraftingBench_Data>();
    public List<ClothCreator_Data> clothCreators = new List<ClothCreator_Data>();
    public List<Axe_Data> axes = new List<Axe_Data>();
    public List<Sword_Data> swords = new List<Sword_Data>();
    public List<Rock_Data> rocks = new List<Rock_Data>();
    public List<Hat_Data> hats = new List<Hat_Data>();
}
