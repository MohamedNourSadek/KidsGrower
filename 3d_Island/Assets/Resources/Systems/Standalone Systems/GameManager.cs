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

        sessionData.data.player.SpawnWithData(myPlayer.gameObject, false);

        aIParameters = DataManager.instance.GetCurrentSession().aIParameters;

        ApplyAiParametersToGame();
    }
    void ApplyAiParametersToGame()
    {
        UIGame.instance.UpdateAISliders(aIParameters);

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
    void Save()
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
    public void ApplyAiSlidersData()
    {
        aIParameters = UIGame.instance.GetSlidersData();

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



    //For design Buttons
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
