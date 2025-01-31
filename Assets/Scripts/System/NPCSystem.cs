using System.Collections.Generic;

public static class NPCSystem {

    // Ai variables
    public static List<NPCtemplate> NPCList;
    static int currentNPC = 0;

    public static void SetUp() {
        NPCList = new ();
    }

    public static void CustomUpdate (float delta) {

        // Soldiers
        if (NPCList.Count > 0) {
            for (int s = NPCList.Count - 1; s >= 0; s--) {
                // Check if isn't null
                if (NPCList[s] == null) {
                    NPCList.RemoveAt(s);
                    continue;
                }

                // Think
                NPCList[s].CustomUpdate(delta);
            }

            NPCList[currentNPC = (currentNPC + 1) % NPCList.Count].Think();
        }
    }

}
