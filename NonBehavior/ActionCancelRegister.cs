using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static BuidableBehavior;

public enum PanelType
{
    Build,
    PersonInspect,
    None
}

public class ActionCancelRegister : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject button;
    private PanelType menuOnHold = PanelType.None;
    public static event EventHandler OnBuildCancel;
    public static event EventHandler OnPersonInspectClosing;

    public void SetActionInstance(PanelType menuType)
    {
        if (this.menuOnHold != PanelType.None && menuType != this.menuOnHold)
        {
            SendClosingSignal();
        }
        this.menuOnHold = menuType;
        button.SetActive(true);
    }
    public void RemoveActionInstance()
    {
        this.menuOnHold = PanelType.None;
        button.SetActive(false);
    }


    public void SendClosingSignal()
    {
        switch (menuOnHold)
        {
            //case MenuType.None: { return; }
            case PanelType.Build:
                {
                    OnBuildCancel?.Invoke(this, EventArgs.Empty);
                    RemoveActionInstance();
                    return;
                }
            case PanelType.PersonInspect:
                {
                    OnPersonInspectClosing?.Invoke(this, EventArgs.Empty);
                    RemoveActionInstance();
                    return;
                }
            default: { return; }
        }
    }

    private void Start()
    {
        button = transform.GetChild(0).gameObject;
        PlacePeopleManager.OnBuildConfirm += (object sender, PlacePeopleManager.OnBuildConfrimEventArg eventArgs) =>
        {
            button.SetActive(false);
        };
    }
}
