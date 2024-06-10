using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    public void CloseMenu()
    {
        menu.SetActive(false);
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
    }
}
