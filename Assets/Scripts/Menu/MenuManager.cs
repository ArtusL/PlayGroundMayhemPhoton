using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] MainMenu[] menus;

    void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            //if (menus[i] == null)
            //{
            //    Debug.LogWarning($"MainMenu at index {i} is null!");
            //    continue; 
            //}

            if (menus[i].menuName == menuName)
            {
                menus[i].Open();
            }
            else if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
    }

    public void OpenMenu(MainMenu menu)
    {
        if (menu == null)
        {
            Debug.LogError("Menu is null!");
            return;
        }

        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i] != null && menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }

    public void CloseMenu(MainMenu menu)
    {
        if (menu == null)
        {
            Debug.LogError("Menu is null!");
            return;
        }

        menu.Close();
    }
}
