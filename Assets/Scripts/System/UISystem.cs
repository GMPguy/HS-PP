using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Mathematics;
using static Enums;

public static class UISystem {

    // Main variables
    static public UImode CurrentMode;

    // Pause menu variables
    static public bool IsPaused = false;
    static bool allowedToPause = false;
    static UImode prevPauseMode;

    // Misc
    static float lockCursor = 0f;
    static float prevTimeScale = 0f;

    // References
    public static Transform MainCanvas;
    static ObjectsConfig windowses;
    static List<UITemplate> spawnedWindowses;
    static List<UITemplate> clearedWindowses;

    public static List<CommentContainer> Comments;
    public static EventSystem eventSystem;

    // Specific window reference
    static AliveMenuUI aliveMenuUI;
    public static bool TryGetAliveMenu (out AliveMenuUI ui) {
        ui = aliveMenuUI;
        return ui;
    }

    static CommentsMenuUI commentsMenuUI;
    public static bool TryGetCommentMenu (out CommentsMenuUI ui) {
        ui = commentsMenuUI;
        return ui;
    }

    /// <summary>
    /// This function sets up a few variables
    /// </summary>
    public static void RecallCanvas (Transform ourCanvas, UImode defaultMode) {

        // Find event system
        eventSystem = GameObject.FindObjectOfType<EventSystem>();

        // If we already had Canvas, destroy it, and spawnedWindowses
        if (MainCanvas) 
            Object.Destroy(MainCanvas.gameObject);

        Comments = new ();

        spawnedWindowses = new List<UITemplate>();
        clearedWindowses = new List<UITemplate>();

        // If we don't have windowses config, load it
        if (!windowses)
            windowses = Resources.Load <ObjectsConfig> ("Configs/Windowses");
        
        MainCanvas = ourCanvas.transform;

        // Sacred windowses
        ClearWindow("all", true);
        SpawnWindow("UI_Comments");

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
                }
        }

        // Hide cursor
        if ((lockCursor -= Time.unscaledDeltaTime) > 0f) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }

    /// <summary>
    /// Use this function to change UI main mode
    /// </summary>
    public static void ChangeMode (UImode newMode) {

        if (newMode != CurrentMode || newMode == UImode.PausedMenu) {

            switch (newMode) {

                // This is triggered whenever scene loading is initialized
                case UImode.LoadMenu:

                    allowedToPause = false;
                    SpawnWindow("UI_LoadMenu", 1);
                    ClearWindow("UI_LoadMenu", true);

                    CurrentMode = UImode.LoadMenu;
                    Time.timeScale = 0f;

                    break;

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
                    Time.timeScale = 1f;

                    break;

                // This creates dead menu
                case UImode.DeadMenu:

                    allowedToPause = false;

                    SpawnWindow("UI_DeadMenu");
                    ClearWindow("UI_DeadMenu", true);

                    CurrentMode = UImode.AliveMenu;

                    break;

            }
        }

    }

    public static void LockCursor (float howLong) => lockCursor = howLong;

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

                switch (windowName) {
                    case "UI_AliveMenu":
                        aliveMenuUI = newWindow.GetComponent<AliveMenuUI>();
                        break;
                    case "UI_Comments":
                        commentsMenuUI = newWindow.GetComponent<CommentsMenuUI>();
                        break;
                }

                break;
            }
            else if (spawnedWindowses[find].gameObject.name == windowName)
                Debug.LogError(windowName + " has already been spawned. No duplicates allowed");
        }

    }

    /// <summary>
    /// This function creates a new CommentContainer, and adds it to comments
    /// </summary>
    public static void AddComment (string text, float time, Color color) =>
        Comments.Add(new(text, color, time));

    public static void FlashImage (Color color, float2 time, int importance = 0) {
        if (TryGetCommentMenu(out CommentsMenuUI ui))
            ui.FlashTheImage(color, time, importance);
    }

    /// <summary>
    /// Checks spawned windowses, and removes those whose name is windowName (or, those whose name is not windowname,
    /// if keep is true). Removed windowses get moved to clearedWindowses, where they await removal
    /// </summary>
    static void ClearWindow (string windowName, bool keep = false) {

        for (int find = 0; find < spawnedWindowses.Count; find++) {
            if (spawnedWindowses[find] && !(spawnedWindowses[find].Sacred && windowName != "all")) {
                string targetName = spawnedWindowses[find].gameObject.name;

                if ( (!keep && targetName == windowName) || (keep && targetName != windowName) ) {

                    if (spawnedWindowses[find].InstantDestroyOnClear) {
                        GameObject.Destroy(spawnedWindowses[find].gameObject);
                        spawnedWindowses.RemoveAt(find);
                    } else {
                        clearedWindowses.Add(spawnedWindowses[find]);
                        spawnedWindowses.RemoveAt(find);
                    }

                    find--;

                    if (!keep)
                        break;
                }
            }
        }

    }

    public class CommentContainer {
        public string Text;
        public Color TextColor;
        public float TextTime;

        public CommentContainer (string t, Color c, float f) {
            Text = t;
            TextColor = c;
            TextTime = f;
        }
    }

}
