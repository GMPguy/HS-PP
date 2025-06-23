using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;
using static GameSystem;
using static SettingsSystem;

public class MainMenuUI : UITemplate {

    MenuType currentMenu;

    // References
    [SerializeField]
    RectTransform[] MenuButtons_Transforms;

    Image[] MenuButtons_Images;
    TMP_Text[] MenuButtons_Text;
    Button[] MenuButtons_Buttons;

    // Button variables
    MenuButton[] MenuButtons_Types;


    public override void ClearUp() {Cleared = true;}

    public override void SetUp(int addition) {

        // Set up button references
        MenuButtons_Types = new MenuButton[15];
        MenuButtons_Images = new Image[15];
        MenuButtons_Text = new TMP_Text[15];
        MenuButtons_Buttons = new Button[15];

        for (int ab = 0; ab < 15; ab++) {
            MenuButtons_Transforms[ab] = MenuButtons_Transforms[ab];
            MenuButtons_Images[ab] = MenuButtons_Transforms[ab].GetComponent<Image>();
            MenuButtons_Text[ab] = MenuButtons_Transforms[ab].GetChild(0).GetComponent<TMP_Text>();
            MenuButtons_Buttons[ab] = MenuButtons_Transforms[ab].GetComponent<Button>();
        }

        // Set up sub menus
        if (addition == 1)
            SetSubMenu(MenuType.PauseMenu);
        else
            SetSubMenu(MenuType.MainMenu);
        
    }

    public override void UIUpdate() {}

    public void OnClick (int buttonID) {

        // Do the special code
        if (buttonID < MenuButtons_Types.Length && MenuButtons_Types[buttonID].Active) {
            switch (MenuButtons_Types[buttonID].Type) {

                case ButtonType.Play:
                    UISystem.ChangeMode(UImode.MissionMenu);
                    break;
                case ButtonType.Resume:
                    UISystem.ChangeMode(UImode.PausedMenu);
                    break;
                case ButtonType.Settings:
                    SetSubMenu(MenuType.Settings);
                    break;
                
                case ButtonType.Settings_Language:
                    MainLanguage = (Language) ( (int)(MainLanguage + 1) % 2);
                    SetMenuText();
                    break;
                case ButtonType.Settings_MusicVolume:
                    MusicVolume = (MusicVolume + 5) % 105;
                    SetMenuText();
                    break;
                case ButtonType.Settings_SoundVolume:
                    SoundVolume = (SoundVolume + 5) % 105;
                    SetMenuText();
                    break;
                case ButtonType.Settings_MinimapFOV:
                    MinimapArea = Mathf.Max((MinimapArea + 5f) % 50f, 10f);
                    SetMenuText();
                    break;

                case ButtonType.Back:
                    MenuType main = UISystem.CurrentMode == UImode.PausedMenu ? MenuType.PauseMenu : MenuType.MainMenu;

                    SetSubMenu(currentMenu switch {
                        _ => main
                    });
                    break;

                case ButtonType.Quit:
                    SceneManagmentSystem.LoadScene("MenuScene");
                    break;

                default:
                    Debug.LogError($"No OnClick() code for ButtonType {MenuButtons_Types[buttonID].Type}");
                    break;
            }
        }

        UISystem.eventSystem.SetSelectedGameObject(null);
    }

    /// <summary>
    /// This sets up button options
    /// </summary>
    void SetSubMenu (MenuType which) {

        currentMenu = which;

        // Set up buttons
        MenuButtons_Types = which switch {

            MenuType.PauseMenu => new MenuButton[] {
                new (ButtonType.Quit),
                new (ButtonType.Settings),
                new (ButtonType.Resume),
                new (),
                new (ButtonType.Text_Paused, false, false)
            },

            MenuType.MainMenu => new MenuButton[] {
                new (ButtonType.Exit),
                new (ButtonType.Settings),
                new (ButtonType.Play),
                new (),
                new (ButtonType.Text_MainMenu, false, false)
            },

            MenuType.Settings => new MenuButton[] {
                new (ButtonType.Back),
                new (),
                new (ButtonType.Settings_SoundVolume),
                new (ButtonType.Settings_MusicVolume),
                new (),
                new (ButtonType.Settings_MinimapFOV),
                new (ButtonType.Settings_Language),
            },

            _ => new MenuButton[0]
        };

        // Visualize buttons
        SetMenuText();

        for (int v = 0; v < 15; v++) {
            if (v < MenuButtons_Types.Length) {
                // Button was set - read it's values from MenuButton
                MenuButtons_Images[v].color = MenuButtons_Types[v].ShowBG
                    ? Color.white
                    : new (0f,0f,0f,0f);

                MenuButtons_Buttons[v].interactable = MenuButtons_Types[v].Active;
            } else {
                // Button was not set - hide it
                MenuButtons_Images[v].color = new (0f,0f,0f,0f);
                MenuButtons_Text[v].text = "";
                MenuButtons_Buttons[v].interactable = false;
            }
        }

    }

    /// <summary>
    /// This sets texts of menu buttons
    /// </summary>
    void SetMenuText () {
        for (int v = 0; v < 15; v++)
            if (v < MenuButtons_Types.Length) {
                MenuButtons_Text[v].text = MenuButtons_Types[v].Type switch {
                    ButtonType.Text_Paused => GetString("GAME PAUSED", "GRA ZAPAUZOWANA"),
                    ButtonType.Text_MainMenu => GetString("MAIN MENU", "MENU GŁÓWNE"),

                    ButtonType.Resume => GetString("Resume", "Wznów"),
                    ButtonType.Play => GetString("Play", "Graj"),
                    ButtonType.Settings => GetString("Settings", "Ustawienia"),
                    ButtonType.Quit or ButtonType.Exit => GetString("Quit", "Wyjdź"),

                    ButtonType.Settings_Language => GetString("Change language", "Zmień język"),
                    ButtonType.Settings_MinimapFOV => GetString("Minimap size: ", "Rozmiar minimapy: ") + MinimapArea,
                    ButtonType.Settings_MusicVolume => GetString("Music volume: ", "Głośność muzyki: ") + MusicVolume,
                    ButtonType.Settings_SoundVolume => GetString("Sound volume: ", "Głośność dźwięków: ") + SoundVolume,

                    ButtonType.Back => GetString("Back", "Wróć"),
                    _ => ""
                };
            }
    }

    /// <summary>
    /// This class contains all the needed information to use and view menu buttons
    /// </summary>
    class MenuButton {
        public ButtonType Type;
        public bool ShowBG;
        public bool Active;

        public MenuButton () {}

        public MenuButton (ButtonType newT, bool newS = true, bool newA = true) {
            Type = newT;
            ShowBG = newS;
            Active = newA;
        }
    }

    enum MenuType {
        MainMenu,
        Settings,
        PauseMenu
    }

    enum ButtonType {
        None,

        Resume,
        Play,
        Quit,
        Exit,
        Settings,

        Settings_Language,
        Settings_MusicVolume,
        Settings_SoundVolume,
        Settings_MinimapFOV,

        Back,

        Text_Paused,
        Text_MainMenu
    }
    
}
