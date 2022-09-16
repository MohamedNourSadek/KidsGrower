using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPanelsManagerUser
{
    public void OpenMenuPanel(string menuInfo);
    public void OpenMenuPanelNonExclusive(string menuInfo);
    public void CloseMenuPanelNonExclusive(string menuInfo);
    public void ToggleMenuPanel(string menuInfo);

}
