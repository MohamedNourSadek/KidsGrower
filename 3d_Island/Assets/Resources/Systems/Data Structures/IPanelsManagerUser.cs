using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPanelsManagerUser
{
    public void OpenMenuPanel(string _menuInfo);
    public void OpenMenuPanelNonExclusive(string _menuInfo);
    public void CloseMenuPanelNonExclusive(string _menuInfo);
    public void ToggleMenuPanel(string _menuInfo);

}
