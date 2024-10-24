using System.Collections.Generic;
using UnityEngine;
using static Enums;

public static class UISystem {

    // Main variables
    static public UImode CurrentMode;

    // Pause menu variables
    static public bool IsPaused = false;
    static bool allowedToPause = false;
    static UImode prevPauseMode;

    // Misc
    static float prevTimeScale = 0f;

    // References
    public static Transform MainCanvas;
    static ObjectsConfig windowses;
    static List<UITemplate> spawnedWindowses;
    static List<UITemplate> clearedWindowses;

    /// <summary>
    /// This function sets up a few variables
    /// </summary>
    public static void RecallCanvas (Transform ourCanvas, UImode defaultMode) {

        // If we already had Canvas, destroy it, and spawnedWindowses
        if (MainCanvas) 
            Object.Destroy(MainCanvas.gameObject);

        spawnedWindowses = new List<UITemplate>();
        clearedWindowses = new List<UITemplate>();

        // If we don't have windowses config, load it
        if (!windowses)
            windowses = Resources.Load <ObjectsConfig> ("Configs/Windowses");
        
        MainCanvas = ourCanvas.transform;

        ChangeMode(defaultMode);
    }

    public static void CustomUpdate () {

        // Pauses
        if (Input.GetKeyDown (KeyCode.Escape) && allowedToPause)
            ChangeMode(UImode.PausedMenu);
        
        // Activate custom updates
        for (int fw = 0; fw < spawnedWindowses.Count; fw++) {
            if (spawnedWindowses[fw])
                spawnedWindowses[fw].UIUpdate();
        }

        // Activate clear updates
        for (int fw = 0; fw < clearedWindowses.Count; fw++) {
            if (clearedWindowses[fw])
                if (!clearedWindowses[fw].Cleared)
                    clearedWindowses[fw].ClearUp();
                else {
                    GameObject.Destroy(clearedWindowses[fw].gameObject);
                    clearedWindowses.RemoveAt(fw);
                    clearedWindowses.TrimExcess();
                }
        }

    }

    /// <summary>
    /// Use this function to change UI main mode
    /// </summary>
    public static void ChangeMode (UImode newMode) {

        if (newMode != CurrentMode || newMode == UImode.PausedMenu) {

            switch (newMode) {

                // This can either turn on or off a pause menu
                case UImode.PausedMenu:

                    allowedToPause = true;

                    if (!IsPaused) {
                        prevPauseMode = CurrentMode;
                        SpawnWindow("UI_MainMenu", 1);
                        ClearWindow("UI_MainMenu", true);
                        prevTimeScale = Time.timeScale;
                        Time.timeScale = 0f;

                        CurrentMode = UImode.PausedMenu;
                    } else {
                        ClearWindow("UI_MainMenu");
                        Time.timeScale = prevTimeScale;
                        ChangeMode(prevPauseMode);
                    }
                        
                    IsPaused = !IsPaused;

                    break;

                // This creates all of our instruments and whatnot
                case UImode.AliveMenu:

                    allowedToPause = true;

                    SpawnWindow("UI_AliveMenu");
                    ClearWindow("UI_AliveMenu", true);

                    CurrentMode = UImode.AliveMenu;

                    break;

            }
        }

    }

    /// <summary>
    /// This function allows us to remotely call functions in spawnedWindows, without the need to reference them
    /// </summary>
    public static void UIEventCall (UIevent what, int bonus = 0) {
        foreach (UITemplate listeners in spawnedWindowses)
            listeners.EventTrigger(what, bonus);
    }

    /// <summary>
    /// Spawns a window from windowses config, by fetching them, by a name
    /// </summary>
    static void SpawnWindow (string windowName, int addition = 0) {

        for (int find = 0; find <= spawnedWindowses.Count; find++) {
            if (find == spawnedWindowses.Count) {

                // If there are no duplicates, spawn the window
                GameObject newWindow = GameObject.Instantiate( windowses.Fetch(windowName) );
                newWindow.name = windowName;
                spawnedWindowses.Add(newWindow.GetComponent<UITemplate>());
                
                newWindow.transform.SetParent(MainCanvas);
                newWindow.transform.localScale = Vector3.one;

                newWindow.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                newWindow.GetComponent<RectTransform>().offsetMax = Vector2.zero;

                newWindow.GetComponent<UITemplate>().SetUp(addition);

                break;
            }
            else if (spawnedWindowses[find].gameObject.name == windowName)
                Debug.LogError(windowName + " has already been spawned. No duplicates allowed");
        }

    }

    /// <summary>
    /// Checks spawned windowses, and removes those whose name is windowName (or, those whose name is not windowname,
    /// if keep is true). Removed windowses get moved to clearedWindowses, where they await removal
    /// </summary>
    static void ClearWindow (string windowName, bool keep = false) {

        for (int find = 0; find < spawnedWindowses.Count; find++) {
            if (spawnedWindowses[find]) {
                string targetName = spawnedWindowses[find].gameObject.name;

                if ( (!keep && targetName == windowName) || (keep && targetName != windowName) ) {
                    clearedWindowses.Add(spawnedWindowses[find]);
                    spawnedWindowses.RemoveAt(find);

                    find--;

                    if (!keep)
                        break;
                }
            }
        }

        spawnedWindowses.TrimExcess();

    }

}
