using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum CustomizingState { Detecting, Moving }

public class GameManager : MonoBehaviour, IInputUser
{
    [SerializeField] bool lockFrameRate = false;
    [SerializeField] int frameRatelock = 60;
    [SerializeField] PostProcessingFunctions posProcessingFunctions;

    [Header("Game Design")]
    [SerializeField] bool showFrameRate;
    [SerializeField] PlayerSystem myPlayer;
    [SerializeField] GameObject eggAsset;
    [SerializeField] GameObject ballAsset;
    [SerializeField] GameObject fruitAsset;
    [SerializeField] GameObject harvestAsset;
    [SerializeField] GameObject seedAsset;
    [SerializeField] NPC npcAsset;

    public static GameManager instance;
    CustomizingState customizingState = CustomizingState.Detecting;
    bool customizing = false;
    CustomizableObject lastdetected;
    Vector3 camCustomizingViewPos;
    Quaternion camCustomizingViewRot;
    AbstractMode modeHandler;
    public List<AIParameter> aIParameters = new List<AIParameter>();


    void Start()
    {
        instance = this;
        
        InputSystem.SubscribeUser(this);

        if (lockFrameRate)
            Application.targetFrameRate = frameRatelock;
        else
            Application.targetFrameRate = 0;

        if (myPlayer == null)
            FindObjectOfType<PlayerSystem>();

        if (!showFrameRate)
            UIController.instance.UpdateFrameRate("");

        camCustomizingViewPos = Camera.main.transform.position;
        camCustomizingViewRot = Camera.main.transform.rotation;

        StartCoroutine(UpdateFrameRate());

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

        foreach(NPC_Data npc_data in  sessionData.data.npcs)
            npc_data.SpawnWithData(npcAsset.gameObject);

        foreach (Ball_Data ball_data in sessionData.data.balls)
            ball_data.SpawnWithData(ballAsset);

        foreach (Egg_Data egg_data in sessionData.data.eggs)
            egg_data.SpawnWithData(eggAsset);

        foreach (Fruit_Data fruit_data in sessionData.data.fruits)
            fruit_data.SpawnWithData(fruitAsset);

        foreach (Harvest_Data harvest_Data in sessionData.data.harvests)
            harvest_Data.SpawnWithData(harvestAsset);

        foreach (Seed_Data seed_data in sessionData.data.seeds)
            seed_data.SpawnWithData(seedAsset);

        sessionData.data.player.SpawnWithData(myPlayer.gameObject);

        aIParameters = DataManager.instance.GetCurrentSession().aIParameters;

        ApplyAiParametersToGame();
    }
    void ApplyAiParametersToGame()
    {
        UIController.instance.UpdateAISliders(aIParameters);

        //Get all NPCs (In game or asset)
        List<NPC> npcList = new List<NPC>();
        foreach (NPC npc in FindObjectsOfType<NPC>())
        {
            npcList.Add(npc);
        }
        npcList.Add(npcAsset.GetComponent<NPC>());

        foreach (NPC npc in npcList)
            npc.aIParameters = aIParameters;
    }
    public void Save()
    {
        SessionData sessionData = DataManager.instance.GetCurrentSession();
        
        sessionData.data.npcs = NPC_Data.GameToDate(FindObjectsOfType<NPC>());
        sessionData.data.balls = Ball_Data.GameToDate(FindObjectsOfType<Ball>());
        sessionData.data.eggs = Egg_Data.GameToDate(FindObjectsOfType<Egg>());
        sessionData.data.fruits = Fruit_Data.GameToDate(FindObjectsOfType<Fruit>());
        sessionData.data.harvests = Harvest_Data.GameToDate(FindObjectsOfType<Harvest>());
        sessionData.data.seeds = Seed_Data.GameToDate(FindObjectsOfType<Seed>());
        sessionData.data.player = Player_Data.GameToData(myPlayer);
        sessionData.modeData = modeHandler.GetModeData();
        sessionData.aIParameters = aIParameters;

        DataManager.instance.Modify(sessionData);

        StartCoroutine(UIController.instance.SavingUI());
    }


    IEnumerator UpdateFrameRate()
    {
        while(true)
        {
            if (showFrameRate)
                UIController.instance.UpdateFrameRate(((int)(1f / Time.deltaTime)).ToString());
            else
                UIController.instance.UpdateFrameRate("");

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime * 5f);
        }
    }
    RaycastHit CastFromMouse()
    {
        RaycastHit _hit;

        Vector2 _mouse2D = InputSystem.GetMousePosition();
        Vector3 _mousePosition = new(_mouse2D.x, _mouse2D.y, 2f);

        Ray _ray = Camera.main.ScreenPointToRay(_mousePosition, Camera.MonoOrStereoscopicEye.Mono);

        Physics.Raycast(_ray, out _hit);

        return _hit;
    }



    //Settings
    public void ApplyAiSlidersData()
    {
        aIParameters = UIController.instance.GetSlidersData();

        ApplyAiParametersToGame();
    }
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

        if (state)
        {
            myPlayer.gameObject.SetActive(true);
        }
        else
        {
            myPlayer.gameObject.SetActive(false);

            Camera.main.transform.position = camCustomizingViewPos;
            Camera.main.transform.rotation = camCustomizingViewRot;
        }
    }
    public void SetBlur(bool _state)
    {
        posProcessingFunctions.SetBlur(_state);
    }
    public void OpenMainMenu()
    {
        Save();
        SceneManager.LoadSceneAsync(0);
    }
    public void ExitWithoutSaving()
    {
        SceneManager.LoadSceneAsync(0);
    }



    //for design Buttons
    public void SpawnBall()
    {
        Instantiate(ballAsset.gameObject, myPlayer.transform.position + myPlayer.transform.forward * 2f + Vector3.up * 5, Quaternion.identity);
    }
    public void SpawnEgg()
    {
        Instantiate(eggAsset.gameObject, myPlayer.transform.position + myPlayer.transform.forward * 2f + Vector3.up * 5, Quaternion.identity);
    }
    public void SpawnSeed()
    {
        Instantiate(seedAsset.gameObject, myPlayer.transform.position + myPlayer.transform.forward * 2f + Vector3.up * 5, Quaternion.identity);
    }



    //Input Interface
    public void PressInput()
    {
        if (customizing)
        {
            if ((customizingState == CustomizingState.Detecting))
            {
                RaycastHit _hit = CastFromMouse();

                if (_hit.collider.GetComponentInParent<CustomizableObject>())
                {
                    lastdetected = _hit.collider.GetComponentInParent<CustomizableObject>();

                    customizingState = CustomizingState.Moving;

                    UIController.instance.CustomizeLog("Selected object: " + lastdetected.name, Color.yellow);
                }
                else
                {
                    UIController.instance.CustomizeLog("No Object Detected", Color.white);
                }
            }
            else if (customizingState == CustomizingState.Moving)
            {
                RaycastHit _hit = CastFromMouse();

                if (_hit.collider.gameObject.CompareTag("Ground"))
                {
                    lastdetected.transform.position = _hit.point;
                    customizingState = CustomizingState.Detecting;

                    UIController.instance.CustomizeLog("", Color.white);
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
    public void MoveInput(Vector2 _movementInput) { }
    public void RotateInput(Vector2 _deltaRotate) { }
}
