using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    MenuState _menuState;
    public GameObject menuItem, scoreEntry;
    internal GameObject highScores, loading, content;
    internal Image[] helpBarImages;

    private string[] main = new string[] { "New Game", "High Scores", "Settings", "Quit Game" };
    private MenuItemType[] mainTypes = new MenuItemType[] { MenuItemType.normal, MenuItemType.normal, MenuItemType.normal, MenuItemType.normal };

    private string[] settings = new string[] { "Graphics", "Audio", "Controls" };
    private MenuItemType[] settingsTypes = new MenuItemType[] { MenuItemType.normal, MenuItemType.normal, MenuItemType.normal };

    private string[] currentMenu;
    private MenuItemType[] currentMenuTypes;

    private MainMenuItem[] currentMenuItems;
    Vector2Int menuPosition = new Vector2Int(0, 200);
    int spacing = 100, menuEnd, currentSelected, highScoreSpacing = 60;
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
        highScores = transform.Find("HighScores").gameObject;
        loading = highScores.transform.Find("Loading").gameObject;
        content = highScores.transform.Find("Table").Find("Content").gameObject;
        helpBarImages = transform.Find("HelpBar").GetComponentsInChildren<Image>();
        highScores.SetActive(false);
        JoystickHandler.DetectControllerType();
        GetHelpBarButtonImages();
    }

    private void GetHelpBarButtonImages()
    {
        helpBarImages[0].sprite = JoystickHandler.ButtonSprite("dpad");
        helpBarImages[1].sprite = JoystickHandler.ButtonSprite("buttonSouth");
        helpBarImages[2].sprite = JoystickHandler.ButtonSprite("buttonEast");
    }
    private void Start()
    {
        HighScoreManager.instance.OnScoresLoaded += ScoresLoaded;
    }

    private void ScoresLoaded(bool loaded)
    {
        if (loaded)
        {
            loading.SetActive(false);
            var scores = HighScoreManager.instance.currentHighscores;
            for (int i = 0; i < scores.Count; i++)
            {
                content.SetActive(true);
                GameObject entry = Instantiate(scoreEntry);
                TextMeshProUGUI[] fields = entry.GetComponentsInChildren<TextMeshProUGUI>();
                fields[0].text = $"{i + 1}.";
                fields[1].text = scores[i].name;
                fields[2].text = scores[i].score.ToString();
                entry.transform.SetParent(content.transform, false);
                entry.transform.localPosition = new Vector2(0f, -highScoreSpacing * i);
            }
        }
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
                case MenuState.highscores:
                    content.SetActive(false);
                    highScores.SetActive(false);
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
        switch (menuState)
        {
            case MenuState.newgame:
                SceneManager.LoadScene("Game", LoadSceneMode.Single);
                break;
            case MenuState.main:
            case MenuState.settings:
                currentMenu = (string[])GetType().GetField(menuState.ToString(), BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                currentMenuTypes = (MenuItemType[])GetType().GetField(menuState.ToString() + "Types", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);

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
                currentSelected = 0;
                SetSelected();
                break;
            case MenuState.highscores:
                currentMenuItems = null;
                highScores.SetActive(true);
                loading.SetActive(true);
                HighScoreManager.GetHighScores();
                break;
        }
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
        return newString;
    }
}
enum MenuState
{
    main,
    highscores,
    settings,
    newgame
}
enum MenuItemType
{
    normal,
    leftright,
    slider
}
