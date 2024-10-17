using UnityEngine;

[CreateAssetMenu(fileName = "New sound bank", menuName = "Configs/Sound bank")]
public class SoundBankConfig : ScriptableObject {

    [SerializeField]
    AudioClip[] sounds;

    public AudioClip GetSound (string name) {

        for (int fs = 0; fs < sounds.Length; fs++)
            if (sounds[fs].name == name)
                return sounds[fs];

        Debug.LogError("No audio clip of name '" + name + "' found!");
        return null;
    }

}
