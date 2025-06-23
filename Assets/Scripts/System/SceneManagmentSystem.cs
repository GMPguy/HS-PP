using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;

public static class SceneManagmentSystem {

    // Main variables
    static string sceneToLoad = "";
    static float timeToLoad;

    public static void CustomUpdate () {

        if (sceneToLoad != "") {
            NPCSystem.SetUp();

            if ((timeToLoad -= Time.unscaledDeltaTime) <= 0f) {
                SceneManager.LoadScene(sceneToLoad);
                sceneToLoad = "";
            }
        }

    }

    public static void LoadScene (string sceneName) {

        UISystem.ChangeMode(UImode.LoadMenu);
        timeToLoad = Random.Range(1f, 3f);
        sceneToLoad = sceneName;

    }

}
