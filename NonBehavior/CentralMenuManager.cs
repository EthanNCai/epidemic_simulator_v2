using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralMenuManager : MonoBehaviour
{
    public GameObject buildingMenu;
    public GameObject revMenu;
    public GameObject developerMenu;
    public TimeManager timeManager;
    private void Start()
    {
        //init everything as inactive
        //buildingMenu.SetActive(false);

        // preview event Triggered -> close building menu
        BuidableBehavior.OnBuildingPreviewStart += (sender, e) =>
        {
            CloseBuidingMenu();
        };
        // build confirm event Triggered -> close building menu
        PlacePeopleManager.OnBuildConfirm += (object sender, PlacePeopleManager.OnBuildConfrimEventArg eventArgs) =>
        {
            buildingMenu.SetActive(false);
        };
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            developerMenu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.F11))
        {
            developerMenu.SetActive(false);
        }
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
    public void OpenRevMenu()
    {
        revMenu.SetActive(true);
        timeManager.PauseOrResume();
    }
    public void CloseRevMenu()
    {
        revMenu.SetActive(false);
        timeManager.PauseOrResume();
    }


}
