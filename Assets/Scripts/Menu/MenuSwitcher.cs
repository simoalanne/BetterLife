using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject closeThisMenu;
    [SerializeField] private GameObject switchToThisMenu;

    public void SwitchMenu()
    {
        switchToThisMenu.SetActive(true);
        closeThisMenu.SetActive(false);
    }
}
