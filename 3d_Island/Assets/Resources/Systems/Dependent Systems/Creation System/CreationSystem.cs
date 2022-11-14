using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[System.Serializable]
public class CreationSystem
{
    [SerializeField] bool closePopUpOnCreate;
    [SerializeField] GameObject creationItemPopUp;
    [SerializeField] Transform createPopUpPosition;
    [SerializeField] public List<CButton> creationItems;

    List<RequirementData> currentBuildingRequirements = new List<RequirementData>();
    CButton selectedButton = null;
    ICreator creator;

    //Interface
    public void Initialize(ICreator creator)
    {
        this.creator = creator;

        foreach (CButton button in creationItems)
            button.onClick.AddListener(OnItemPress);
    }
    public GameObject SpawnItem(Vector3 position)
    {
        GameObject obj = null;

        if (selectedButton.name == "Magic House")
            obj = GameManager.instance.SpawnNamingHouse(position);
        else if (selectedButton.name == "Crafting Bench")
            obj = GameManager.instance.SpawnCraftingBench(position);
        else if (selectedButton.name == "Cloth Creator")
            obj = GameManager.instance.SpawnClothCreator(position);
        else if (selectedButton.name == "Axe")
            obj = GameManager.instance.SpawnAxe();
        else if (selectedButton.name == "Hat")
            obj = GameManager.instance.SpawnHat();

        ConsumeResources();

        return obj;
    }
    public void CreateAndStore()
    {
        var Item =  SpawnItem(Vector3.zero);
        PlayerSystem.instance.inventorySystem.Store(Item, false);
    }


    //Events
    void OnItemPress()
    {
        selectedButton = creationItems.Find(button => button.gameObject == EventSystem.current.currentSelectedGameObject);
        currentBuildingRequirements = GetRequirementsData();
        CreatePopUp();
    }


    //Internal
    void ConsumeResources()
    {
        PlayerSystem.instance.inventorySystem.ConsumeResources(currentBuildingRequirements);
    }
    List<RequirementData> GetRequirementsData()
    {
        List<RequirementData> requirements = new List<RequirementData>();

        if (selectedButton.name == "Magic House")
        {
            requirements = new List<RequirementData>()
            {
                new RequirementData() { itemTag = "WoodPack",  itemAmount = 2},
                new RequirementData() { itemTag = "StonePack",  itemAmount = 2},
            };
        }
        else if (selectedButton.name == "Crafting Bench")
        {
            requirements = new List<RequirementData>()
            {
                new RequirementData() { itemTag = "WoodPack",  itemAmount = 10},
                new RequirementData() { itemTag = "StonePack",  itemAmount = 10},
            };
        }
        else if (selectedButton.name == "Cloth Creator")
        {
            requirements = new List<RequirementData>()
            {
                new RequirementData() { itemTag = "WoodPack",  itemAmount = 15},
                new RequirementData() { itemTag = "StonePack",  itemAmount = 15},
            };
        }
        else if (selectedButton.name == "Axe")
        {
            requirements = new List<RequirementData>()
            {
                new RequirementData() { itemTag = "WoodPack",  itemAmount = 1},
                new RequirementData() { itemTag = "StonePack",  itemAmount = 1},
            };
        }
        else if (selectedButton.name == "Hat")
        {
            requirements = new List<RequirementData>()
            {
                new RequirementData() { itemTag = "WoodPack",  itemAmount = 1},
                new RequirementData() { itemTag = "StonePack",  itemAmount = 1},
            };
        }

        return requirements;
    }
    void CreatePopUp()
    {
        var Obj = ServicesProvider.Instantiate(creationItemPopUp,createPopUpPosition.transform);
        Obj.GetComponent<CreationItemPopUp>().Initialize(creator.OnCreatePress, selectedButton.name, currentBuildingRequirements, closePopUpOnCreate);
    }
}
