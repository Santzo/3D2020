using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MainMenuHandler : MonoBehaviour
{
    MenuState _menuState;
    public GameObject menuItem;

    private string[] main = new string[] { "New Game", "High Scores", "Settings", "Quit Game" };
    private MenuItemType[] mainTypes = new MenuItemType[] { MenuItemType.normal, MenuItemType.normal, MenuItemType.normal, MenuItemType.normal };

    private string[] settings = new string[] { "Graphics", "Audio", "Controls" };
    private MenuItemType[] settingsTypes = new MenuItemType[] { MenuItemType.normal, MenuItemType.normal, MenuItemType.normal };

    private string[] currentMenu;
    private MenuItemType[] currentMenuTypes;

    private MainMenuItem[] currentMenuItems;
    Vector2Int menuPosition = new Vector2Int(0, 200);
    int spacing = 100, menuEnd, currentSelected;
    MenuState menuState
    {
        get
        {
            return _menuState;
        }
        set
        {
            _menuState = value;
            BuildMenu();
        }
    }

    private void Awake()
    {
        menuState = MenuState.main;
        JoystickHandler.DetectControllerType();
    }
    private void Start()
    {
        HighScoreManager.instance.OnScoresLoaded += ScoresLoaded;
    }

    private void ScoresLoaded(bool obj)
    {
        Debug.Log(obj);
    }

    private void Update()
    {
        if (JoystickHandler.MenuMovement != 0) MoveSelector(JoystickHandler.MenuMovement);
        if (JoystickHandler.Interact)
        {
            var sel = currentMenu[currentSelected];
            menuState = (MenuState)Enum.Parse(typeof(MenuState), ConvertToState(sel), true);
        }
        if (JoystickHandler.Cancel)
        {
            switch (menuState)
            {
                case MenuState.settings:
                    menuState = MenuState.main;
                    break;
            }
        }
    }
    private void MoveSelector(int menuMovement)
    {
        currentSelected += menuMovement;
        if (currentSelected < 0) currentSelected = menuEnd;
        if (currentSelected > menuEnd) currentSelected = 0;
        SetSelected();
    }

    private void BuildMenu()
    {
        if (currentMenuItems?.Length > 0)
        {
            for (int i = 0; i < currentMenuItems.Length; i++)
            {
                Destroy(currentMenuItems[i].gameObject);
            }
        }
        if (menuState != MenuState.highscores)
        {
            currentMenu = (string[])GetType().GetField(menuState.ToString(), BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
            currentMenuTypes = (MenuItemType[])GetType().GetField(menuState.ToString() + "Types", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
        }
        else
        {
            HighScoreManager.GetHighScores();
        }
        menuEnd = currentMenu.Length - 1;
        currentMenuItems = new MainMenuItem[currentMenu.Length];
        for (int i = 0; i < currentMenu.Length; i++)
        {
            var obj = Instantiate(menuItem);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = new Vector2(menuPosition.x, menuPosition.y - i * spacing);
            MainMenuItem item = obj.GetComponent<MainMenuItem>();
            item.text.text = currentMenu[i] + " " + currentMenuTypes[i];
            currentMenuItems[i] = item;
        }
        currentSelected = 0;
        SetSelected();
    }
    private void SetSelected()
    {
        for (int i = 0; i < currentMenu.Length; i++)
        {
            currentMenuItems[i].anim.SetBool(currentMenuItems[i].selected, i == currentSelected);
        }
    }
    private string ConvertToState(string obj)
    {
        string newString = "";
        obj = obj.ToLower();
        for (int i = 0; i < obj.Length; i++)
        {
            if (obj[i] != ' ') newString += obj[i].ToString();   
        }
        Debug.Log(newString);
        return newString;
    }
}
enum MenuState
{
    main,
    highscores,
    settings
}
enum MenuItemType
{
    normal,
    leftright,
    slider
}
