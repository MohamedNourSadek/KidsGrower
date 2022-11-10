using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequirementData : MonoBehaviour
{
    public string itemTag;
    public int itemAmount;

    public static bool IsRequirementTrue(InventorySystem inventory, List<RequirementData> requirements)
    {
        bool isTrue = true;

        foreach(RequirementData requirement in requirements)
        {
            if(inventory.Exists(requirement.itemTag))
            {
                if(inventory.GetAmount(requirement.itemTag) < requirement.itemAmount)
                {
                    isTrue = false;
                    break;
                }
            }
            else
            {
                isTrue = false;
                break;
            }
        }

        return isTrue;
    }
    public static string GetRequirementStatementText(InventorySystem inventory, List<RequirementData> requirements)
    {
        string result = "";

        foreach (RequirementData requirement in requirements)
        {
            if (inventory.Exists(requirement.itemTag))
            {
                result += requirement.itemTag + " " + inventory.GetAmount(requirement.itemTag) + "/" + requirement.itemAmount + "\n";
            }
            else
            {
                result += requirement.itemTag + " 0/" + requirement.itemAmount + "\n";
            }
        }


        return result;
    }
}
