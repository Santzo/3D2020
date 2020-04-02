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
    public GameObject menuItem, scoreEntry, leftRight, slider;
    private GameObject leftRightMenu, sliderMenu;
    private TextMeshProUGUI leftRightText;
    internal GameObject highScores, loading, content;
    internal Image[] helpBarImages;
    internal TextMeshProUGUI[] helpBarTexts;

    private string[] main = new string[] { "New Game", "High Scores", "Settings", "Quit Game" };
    private MenuItemType[] mainTypes = new MenuItemType[] { MenuItemType.normal, MenuItemType.normal, MenuItemType.normal, MenuItemType.normal };

    private string[] settings = new string[] { "Graphics", "Audio", "Controls" };
    private MenuItemType[] settingsTypes = new MenuItemType[] { MenuItemType.normal, MenuItemType.normal, MenuItemType.normal };

    private string[] graphics = new string[] { "Resolution", "Screen Mode", "Lighting" };
    private MenuItemType[] graphicsTypes = new MenuItemType[] { MenuItemType.leftright, MenuItemType.leftright, MenuItemType.leftright };

    private string[] audio = new string[] { "Master Volume", "SFX Volume", "Music Volume" };
    private MenuItemType[] audioTypes = new MenuItemType[] { MenuItemType.slider, MenuItemType.slider, MenuItemType.slider };

    private string[] currentMenu, currentLeftRight;
    private int selectedLeftRight;
    private MenuItemType[] currentMenuTypes;

    private MainMenuItem[] currentMenuItems;
    Vector2Int menuPosition = new Vector2Int(0, 200);
    int spacing = 110, menuEnd, currentSelected, highScoreSpacing = 60;
    MenuState menuState
    {
        get
        {
            return _menuState;
        }
        set
        {
            _menuState = value;
            ChangeMenu();
        }
    }

    private void Awake()
    {
        highScores = transform.Find("HighScores").gameObject;
        loading = highScores.transform.Find("Loading").gameObject;
        content = highScores.transform.Find("Table").Find("Content").gameObject;
        helpBarImages = transform.Find("HelpBar").GetComponentsInChildren<Image>();
        helpBarTexts = transform.Find("HelpBar").GetComponentsInChildren<TextMeshProUGUI>();
        highScores.SetActive(false);
        JoystickHandler.DetectControllerType();
        GetHelpBarButtonImages();
        leftRightMenu = Instantiate(leftRight);
        leftRightMenu.transform.SetParent(transform);
        leftRightMenu.SetActive(false);
        sliderMenu = Instantiate(slider);
        sliderMenu.transform.SetParent(transform);
        sliderMenu.SetActive(false);
        leftRightText = leftRightMenu.GetComponentInChildren<TextMeshProUGUI>();
        menuState = MenuState.main;
    }

    private void GetHelpBarButtonImages()
    {
        helpBarImages[0].sprite = JoystickHandler.ButtonSprite("dpad");
        helpBarImages[1].sprite = JoystickHandler.ButtonSprite("buttonSouth");
        helpBarImages[2].sprite = JoystickHandler.ButtonSprite("buttonEast");
    }
    private void Start()
    {
        var res = Settings.ResolutionToVector(Settings.resolutionCurrent);
        Settings.SetResolution(res, Settings.StringToScreenMode(Settings.screenmodeCurrent));
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
        if (JoystickHandler.MenuMovementVertical != 0) MoveSelector(JoystickHandler.MenuMovementVertical);
        if (JoystickHandler.MenuMovementHorizontal != 0 && currentMenuTypes[currentSelected] != MenuItemType.normal)
        {
            SetMenuValue(JoystickHandler.MenuMovementHorizontal);
        }
        if (JoystickHandler.Interact)
        {
            InteractButton();
        }
        if (JoystickHandler.Cancel)
        {
            BackButton();
        }
    }
    private void BackButton()
    {
        switch (menuState)
        {
            case MenuState.graphics:
            case MenuState.audio:
            case MenuState.controls:
                menuState = MenuState.settings;
                break;
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
    private void InteractButton()
    {
        switch (menuState)
        {
            case MenuState.audio:
            case MenuState.graphics:
            case MenuState.controls:
                Settings.SaveSettings();
                BackButton();
                break;
            default:
                var sel = currentMenu[currentSelected];
                menuState = (MenuState)Enum.Parse(typeof(MenuState), ConvertToState(sel), true);
                break;
        }
    }
    private void SetMenuValue(int move)
    {
        if (currentMenuTypes[currentSelected] == MenuItemType.leftright)
        {
            selectedLeftRight = (int)Mathf.Clamp(selectedLeftRight += move, 0, currentLeftRight.Length - 1);
            leftRightText.text = currentLeftRight[selectedLeftRight];
            var item = ConvertToState(currentMenu[currentSelected]);
            var text = typeof(Settings).GetField($"{item}Save");
            text.SetValue(null, leftRightText.text);
        }
        else
        {
            var text = ReturnCurrentSaveValue;
            var value = Int32.Parse(text);
            Slider slider = sliderMenu.GetComponent<Slider>();
            value = (int) Mathf.Clamp(value += move, slider.minValue, slider.maxValue);
            slider.value = value;
            SetCurrentSaveValue(value.ToString());
        }
    }

    private void MoveSelector(int menuMovement)
    {
        currentSelected += menuMovement;
        if (currentSelected < 0) currentSelected = menuEnd;
        if (currentSelected > menuEnd) currentSelected = 0;
        SetSelected();
    }

    private void ChangeMenu()
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
            case MenuState.highscores:
                currentMenuItems = null;
                highScores.SetActive(true);
                loading.SetActive(true);
                HighScoreManager.GetHighScores();
                break;
            case MenuState.quitgame:
                Application.Quit();
                break;
            case MenuState.main:
            case MenuState.settings:
                helpBarTexts[1].text = "Select";
                helpBarTexts[2].text = "Back";
                BuildMenu();
                break;
            default:
                helpBarTexts[1].text = "Save";
                helpBarTexts[2].text = "Cancel";
                BuildMenu();
                break;
        }
    }
    private void BuildMenu()
    {
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
        SetSelected(true);
    }

    private void SetSelected(bool init = false)
    {
        for (int i = 0; i < currentMenu.Length; i++)
        {
            currentMenuItems[i].anim.SetBool(currentMenuItems[i].selected, i == currentSelected);
        }
        if (currentMenuTypes[currentSelected] == MenuItemType.normal)
        {
            leftRightMenu.SetActive(false);
            sliderMenu.SetActive(false);
        }
        else if (currentMenuTypes[currentSelected] == MenuItemType.leftright)
        {
            if (init) Settings.RestoreValues();
            leftRightMenu.SetActive(true);
            leftRightMenu.transform.localPosition = new Vector2(menuPosition.x, menuPosition.y - currentSelected * spacing - (spacing * 0.4f));
            var item = ConvertToState(currentMenuItems[currentSelected].text.text);
            var text = ReturnCurrentSaveValue;
            leftRightText.text = text ?? "nothing";
            currentLeftRight = typeof(Settings).GetField($"{item}s")?.GetValue(null) as string[];
            selectedLeftRight = Array.IndexOf(currentLeftRight, text);
        }
        else if (currentMenuTypes[currentSelected] == MenuItemType.slider)
        {
            if (init) Settings.RestoreValues();
            sliderMenu.SetActive(true);
            sliderMenu.transform.localPosition = new Vector2(menuPosition.x, menuPosition.y - currentSelected * spacing - (spacing * 0.4f));
            var text = ReturnCurrentSaveValue;
            Slider sliderValue = sliderMenu.GetComponent<Slider>();
            sliderValue.value = Int32.Parse(text);
        }
    }
    private string ReturnCurrentSaveValue
    {
        get
        {
            var item = ConvertToState(currentMenuItems[currentSelected].text.text);
            var text = typeof(Settings).GetField($"{item}Save")?.GetValue(null) as string;
            return text;
        }
    }
    private void SetCurrentSaveValue(string value)
    {
        var item = ConvertToState(currentMenuItems[currentSelected].text.text);
        var text = typeof(Settings).GetField($"{item}Save");
        text.SetValue(null, value);
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
    newgame,
    quitgame,
    graphics,
    audio,
    controls
}
enum MenuItemType
{
    normal,
    leftright,
    slider
}
