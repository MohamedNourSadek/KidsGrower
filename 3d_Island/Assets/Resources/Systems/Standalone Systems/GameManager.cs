using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CustomizingState { Detecting, Moving }

public class GameManager : MonoBehaviour, IInputUser
{
    [SerializeField] PostProcessingFunctions posProcessingFunctions;
    [SerializeField] LayerMask customizeDetectable;

    [Header("Game Design")]
    [SerializeField] PlayerSystem myPlayer;
    [SerializeField] GameObject eggAsset;
    [SerializeField] GameObject ballAsset;
    [SerializeField] GameObject fruitAsset;
    [SerializeField] GameObject harvestAsset;
    [SerializeField] GameObject fertilityBoostAsset;
    [SerializeField] GameObject extroversionBoostAsset;
    [SerializeField] GameObject aggressivenessBoostAsset;
    [SerializeField] GameObject powerBoostAsset;
    [SerializeField] GameObject healthBoostAsset;
    [SerializeField] GameObject woodpackAsset;
    [SerializeField] GameObject stonepackAsset;
    [SerializeField] GameObject seedAsset;
    [SerializeField] GameObject treeAsset;
    [SerializeField] NPC npcAsset;
    [SerializeField] Light mainLight;
    [SerializeField] List<Terrain> terrains;
    [SerializeField] GameObject deadchild;

    public static GameManager instance;
    CustomizingState customizingState = CustomizingState.Detecting;
    bool customizing = false;
    CustomizableObject lastdetected;
    Vector3 camCustomizingViewPos;
    Quaternion camCustomizingViewRot;
    AbstractMode modeHandler;

    public bool activeInput { get; set; }


    //Private functions
    void Awake()
    {
        instance = this;
        
        InputSystem.SubscribeUser(this);

        if (myPlayer == null)
            FindObjectOfType<PlayerSystem>();

        camCustomizingViewPos = Camera.main.transform.position;
        camCustomizingViewRot = Camera.main.transform.rotation;

        posProcessingFunctions.Initialize();

        LoadAndApply();

        SetBlur(false);

        modeHandler = ModeFactory.CreateModeHandler(DataManager.instance.GetCurrentSession());

        StartCoroutine(autoSave());
    }
    void Update()
    {
        if (modeHandler != null)
            modeHandler.Update();
    }
    IEnumerator autoSave()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(60f);
            Save();
        }
    }
    void LoadAndApply()
    {
        SessionData sessionData = DataManager.instance.GetCurrentSession();
        SettingsData settingsData = DataManager.instance.GetSavedData().settings;

        foreach(NPC_Data npc_data in  sessionData.data.npcs)
            npc_data.SpawnWithData(npcAsset.gameObject, true);

        foreach (Ball_Data ball_data in sessionData.data.balls)
            ball_data.SpawnWithData(ballAsset, true);

        foreach (Egg_Data egg_data in sessionData.data.eggs)
            egg_data.SpawnWithData(eggAsset, true);

        foreach (Fruit_Data fruit_data in sessionData.data.fruits)
            fruit_data.SpawnWithData(fruitAsset, true);

        foreach (Harvest_Data harvest_Data in sessionData.data.harvests)
            harvest_Data.SpawnWithData(harvestAsset, true);

        foreach (Seed_Data seed_data in sessionData.data.seeds)
            seed_data.SpawnWithData(seedAsset, true);

        foreach (FertilityBoost_Data boost in sessionData.data.fertilityBoosts)
            boost.SpawnWithData(fertilityBoostAsset, true);

        foreach(ExtroversionBoost_Data boost in sessionData.data.extroversionBoosts)
            boost.SpawnWithData(extroversionBoostAsset, true);

        foreach (AggressivenessBoost_Data boost in sessionData.data.aggressivenessBoosts)
            boost.SpawnWithData(aggressivenessBoostAsset, true);

        foreach (PowerBoost_Data boost in sessionData.data.powerboosts)
            boost.SpawnWithData(powerBoostAsset, true);

        foreach (HealthBoost_Data boost in sessionData.data.healthboosts)
            boost.SpawnWithData(healthBoostAsset, true);
       
        foreach (WoodPack_Data woodPack in sessionData.data.woodpacks)
            woodPack.SpawnWithData(woodpackAsset, true);

        foreach (StonePack_Data stonePack in sessionData.data.stonepacks)
            stonePack.SpawnWithData(stonepackAsset, true);

        //tree is Done differently because it exists by default
        if (sessionData.data.trees.Count > 0)
        {
            var trees = FindObjectsOfType<TreeSystem>();

            foreach(TreeSystem tree in trees)
                Destroy(tree.gameObject);
            
            foreach (Tree_Data tree_data in sessionData.data.trees)
                tree_data.SpawnWithData(treeAsset, true);
        }
            
        sessionData.data.player.SpawnWithData(myPlayer.gameObject, false);

        SetAmbientVolume(settingsData.ambinetVolume);
        SetUIVolume(settingsData.uiVolume);
        SetSFXVolume(settingsData.sfxVolume);
        SetGrass(settingsData.grass);
        SetShadows(settingsData.shadows);

        UIGame.instance.LoadSavedUISettings(settingsData);
    }
    void Save()
    {
        SessionData sessionData = DataManager.instance.GetCurrentSession();

        sessionData.data.npcs = NPC_Data.GameToDate(FindObjectsOfType<NPC>());
        sessionData.data.balls = Ball_Data.GameToDate(FindObjectsOfType<Ball>());
        sessionData.data.eggs = Egg_Data.GameToDate(FindObjectsOfType<Egg>());
        sessionData.data.fruits = Fruit_Data.GameToDate(FindObjectsOfType<Fruit>());
        sessionData.data.harvests = Harvest_Data.GameToDate(FindObjectsOfType<Harvest>());
        sessionData.data.seeds = Seed_Data.GameToDate(FindObjectsOfType<Seed>());
        sessionData.data.trees = Tree_Data.GameToDate(FindObjectsOfType<TreeSystem>());
        sessionData.data.fertilityBoosts = FertilityBoost_Data.GameToDate(FindObjectsOfType<FertilityBoost>());
        sessionData.data.extroversionBoosts = ExtroversionBoost_Data.GameToDate(FindObjectsOfType<ExtroversionBoost>());
        sessionData.data.aggressivenessBoosts = AggressivenessBoost_Data.GameToDate(FindObjectsOfType<AggressivenessBoost>());
        sessionData.data.powerboosts = PowerBoost_Data.GameToDate(FindObjectsOfType<PowerBoost>());
        sessionData.data.healthboosts = HealthBoost_Data.GameToDate(FindObjectsOfType<HealthBoost>());
        sessionData.data.stonepacks = StonePack_Data.GameToDate(FindObjectsOfType<StonePack>());
        sessionData.data.woodpacks = WoodPack_Data.GameToDate(FindObjectsOfType<WoodPack>());
        sessionData.data.player = Player_Data.GameToData(myPlayer);
        sessionData.modeData = modeHandler.GetModeData();

        DataManager.instance.GetSavedData().settings = UIGame.instance.GetSavedUI();

        DataManager.instance.Modify(sessionData);

        StartCoroutine(UIGame.instance.SavingUI());
    }
    RaycastHit CastFromMouse()
    {
        RaycastHit hit;

        Vector2 mouse2d = InputSystem.GetMousePosition();
        Vector3 mousePosition = new(mouse2d.x, mouse2d.y, 2f);

        Ray ray = Camera.main.ScreenPointToRay(mousePosition, Camera.MonoOrStereoscopicEye.Mono);

        Physics.Raycast(ray, out hit, customizeDetectable);

        return hit;
    }


    //Settings
    public void SetCustomizing(bool state)
    {
        customizing = state;

        SetBlur(!state);
    }
    public void LockPlayer(bool state)
    {
        myPlayer.LockPlayer(!state);
    }
    public void SetPlaying(bool state)
    {
        SetBlur(!state);

        myPlayer.LockPlayer(state);
    }
    public void SetCamera(bool state)
    {
        if(state == false)
        {
            Camera.main.transform.position = camCustomizingViewPos;
            Camera.main.transform.rotation = camCustomizingViewRot;
        }
    }
    public void SetBlur(bool state)
    {
        posProcessingFunctions.SetBlur(state);
    }
    public void OpenMainMenu()
    {
        Save();
        SceneControl.instance.LoadScene(0);
    }
    public void ExitWithoutSaving()
    {
        SceneControl.instance.LoadScene(0);
    }
    public void OpenInventory(bool state)
    {
        UIGame.instance.DisplayInventory(state, myPlayer.inventorySystem);
    }
    public void SetSFXVolume(float volume)
    {
        SoundManager.instance.SetSFX(volume);
    }
    public void SetAmbientVolume(float volume)
    {
        SoundManager.instance.SetAmbient(volume);
    }
    public void SetUIVolume(float volume)
    {
        SoundManager.instance.SetUiVolume(volume);
    }
    public void SetGrass(bool state)
    {
        foreach(Terrain terrain in terrains)
        {
            terrain.drawTreesAndFoliage = state;
        }
    }
    public void SetShadows(bool state)
    {
        if (state)
            mainLight.shadows = LightShadows.Hard;
        else
            mainLight.shadows = LightShadows.None;
    }


    //For design Buttons
    public void SpawnBall()
    {
        Instantiate(ballAsset.gameObject, myPlayer.transform.position + myPlayer.transform.forward * 2f + Vector3.up * 5, Quaternion.identity);
    }
    public void SpawnEgg()
    {
        Instantiate(eggAsset.gameObject, myPlayer.transform.position + myPlayer.transform.forward * 2f + Vector3.up * 5, Quaternion.identity);
    }
    public void SpawnFruit()
    {
        Instantiate(fruitAsset.gameObject, myPlayer.transform.position + myPlayer.transform.forward * 2f + Vector3.up * 5, Quaternion.identity);
    }
    public void SpawnRandomItem()
    {
        var points = MapSystem.instance.GetExplorationPoints();

        int randomPoint = UnityEngine.Random.Range(0,points.Count);
        int randomItem = UnityEngine.Random.Range(0, 7);

        GameObject item = null;

        if (randomItem == 0)
            item = fertilityBoostAsset;
        else if (randomItem == 1)
            item = aggressivenessBoostAsset;
        else if (randomItem == 2)
            item = healthBoostAsset;
        else if (randomItem == 3)
            item = powerBoostAsset;
        else if (randomItem == 4)
            item = extroversionBoostAsset;
        else if (randomItem == 5)
            item = stonepackAsset;
        else if (randomItem == 6)
            item = woodpackAsset;

        Instantiate(item, points[randomPoint].position,Quaternion.identity);
    }
    public void SpawnBoost(string boostType)
    {
        GameObject boost = powerBoostAsset;

        if(boostType == "AggressivenessBoost")
        {
            boost = aggressivenessBoostAsset;
        }
        else if(boostType == "ExtroversionBoost")
        {
            boost = extroversionBoostAsset;
        }
        else if(boostType == "FertilityBoost")
        {
            boost = fertilityBoostAsset;
        }
        else if (boostType == "HealthBoost")
        {
            boost = healthBoostAsset;
        }
        else if (boostType == "PowerBoost")
        {
            boost = powerBoostAsset;
        }

        Instantiate(boost.gameObject, myPlayer.transform.position + myPlayer.transform.forward * 2f + Vector3.up * 5, Quaternion.identity);
    }
    public GameObject SpawnDeadBody_ReturnDeadBody(Vector3 position)
    {
        return Instantiate(deadchild, position, Quaternion.identity);
    }
    public Egg SpawnEgg_ReturnEgg(Vector3 position)
    {
        return Instantiate(eggAsset.gameObject, position, Quaternion.identity).GetComponent<Egg>();
    }
    public void SpawnSeed()
    {
        Instantiate(seedAsset.gameObject, myPlayer.transform.position + myPlayer.transform.forward * 2f + Vector3.up * 5, Quaternion.identity);
    }
    public void SpawnStonePack()
    {
        Instantiate(stonepackAsset.gameObject, myPlayer.transform.position + myPlayer.transform.forward * 2f + Vector3.up * 5, Quaternion.identity);
    }
    public void SpawnWoodPack()
    {
        Instantiate(woodpackAsset.gameObject, myPlayer.transform.position + myPlayer.transform.forward * 2f + Vector3.up * 5, Quaternion.identity);
    }
    public GameObject SpawnHarvest()
    {
        GameObject x =  Instantiate(harvestAsset.gameObject, myPlayer.transform.position + myPlayer.transform.forward * 2f + Vector3.up * 5, Quaternion.identity);

        return x;
    }

    
    //Input Interface
    public void PressInput()
    {
        if (customizing)
        {
            if ((customizingState == CustomizingState.Detecting))
            {
                RaycastHit hit = CastFromMouse();

                if (hit.collider.GetComponentInParent<CustomizableObject>())
                {
                    lastdetected = hit.collider.GetComponentInParent<CustomizableObject>();

                    customizingState = CustomizingState.Moving;

                    UIGame.instance.ChangeCustomizingIndicator("Selected object: " + lastdetected.name, Color.yellow);
                }
                else
                {
                    UIGame.instance.ChangeCustomizingIndicator("No Object Detected", Color.white);
                }
            }
            else if (customizingState == CustomizingState.Moving)
            {
                RaycastHit hit = CastFromMouse();

                if (hit.collider.gameObject.CompareTag("Ground"))
                {
                    lastdetected.transform.position = hit.point;
                    customizingState = CustomizingState.Detecting;

                    UIGame.instance.ChangeCustomizingIndicator("", Color.white);
                }
                else
                {
                    customizingState = CustomizingState.Detecting;
                    PressInput();
                }
            }
        }
    }
    public void PetInput() { }
    public void DashInput() { }
    public void PlantInput() { }
    public void ThrowInput() { }
    public void JumpInput() { }
    public void PickInput() { }
    public void MoveInput(Vector2 movementInput) { }
    public void RotateInput(Vector2 deltaRotate) { }
}
