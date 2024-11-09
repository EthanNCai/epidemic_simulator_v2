using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralMenuManager : MonoBehaviour
{
    public GameObject buildingMenu;
    public TimeManager timeManager;
    private void Start()
    {
        BuidableBehavior.OnBuildingPreviewStart += (sender, e) =>
        {
            CloseBuidingMenu();
        };
    }
    public void OpenBuildingMenu()
    {
        buildingMenu.SetActive(true);
        timeManager.PauseOrResume();
    }
    public void CloseBuidingMenu()
    {
        buildingMenu.SetActive(false);
        timeManager.PauseOrResume();
    }

}
