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
    private string[] currentMenu;
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

    private void Update()
    {
        if (JoystickHandler.MenuMovement != 0) MoveSelector(JoystickHandler.MenuMovement);  
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
        currentMenu = (string[])GetType().GetField(menuState.ToString(), BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
        menuEnd = currentMenu.Length - 1;
        currentMenuItems = new MainMenuItem[currentMenu.Length];
        for (int i = 0; i < currentMenu.Length; i++)
        {
            var obj = Instantiate(menuItem);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = new Vector2(menuPosition.x, menuPosition.y - i * spacing);
            MainMenuItem item = obj.GetComponent<MainMenuItem>();
            item.text.text = currentMenu[i];
            currentMenuItems[i] = item;
        }
        SetSelected();
    }
    private void SetSelected()
    {
        for (int i = 0; i < currentMenu.Length; i++)
        {
            currentMenuItems[i].anim.SetBool(currentMenuItems[i].selected, i == currentSelected);
        }
    }
}
enum MenuState
{
    main,
    highScores,
    settings
}
