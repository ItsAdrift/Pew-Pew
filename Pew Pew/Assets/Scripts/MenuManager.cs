using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    void Awake()
    {
        instance = this;
    }

    [SerializeField] Menu[] menus;

    public void OpenMenu(string menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName.Equals(menuName))
            {
                menus[i].Open();
            }
            else if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void CloseAll()
    {
        for (int i = 0; i < menus.Length; i++)
        {
            CloseMenu(menus[i]);
        }
    }

    public void ThrowError(string menuName, string errorMessage)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName.Equals(menuName))
            {
                if (menus[i] is ErrorMenu)
                {
                    ErrorMenu errorMenu = (ErrorMenu)menus[i];
                    menus[i].Open();
                    errorMenu.errorText.text = errorMessage;
                }
            }
            else if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
    }

}
