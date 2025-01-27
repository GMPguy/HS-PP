using UnityEngine;
using UnityEngine.Audio;
using Unity.Mathematics;

public static class SoundSystem {

    public static AudioMixer MainMixer;

    // Independent audio variables
    static AudioSource[] indieAudios;
    static int indieAudioID = 0;

    public static void SetUp () {

        // Reference:
        MainMixer = Resources.Load<AudioMixer>("Settings/MainMixer");

        // Independent audio sources
        if (indieAudios == null) {
            Transform AudiosRoot = new GameObject("AudiosRoot").transform;
            indieAudios = new AudioSource[10];

            for (int ca = 0; ca < indieAudios.Length; ca++) {
                GameObject newAudio = new ("Audio_" + ca);
                AudioSource newSource = newAudio.AddComponent<AudioSource>();

                indieAudios[ca] = newSource;

                indieAudios[ca].rolloffMode = AudioRolloffMode.Linear;
                indieAudios[ca].spatialBlend = 1f;

                indieAudios[ca].outputAudioMixerGroup = MainMixer.FindMatchingGroups("Master/Sounds/Environment/ActualEnvironment")[0];

                newAudio.transform.SetParent(AudiosRoot);
            }

            Object.DontDestroyOnLoad(AudiosRoot.gameObject);
        }
    }

    /// <summary>
    /// Uses one of the independent audio source to be played at any given position
    /// </summary>
    public static void PlayAudioAt (AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f, float2 dist = default) {
        
        indieAudios[indieAudioID].transform.position = position;
        indieAudios[indieAudioID].clip = clip;

        indieAudios[indieAudioID].volume = volume;
        indieAudios[indieAudioID].pitch = pitch;

        float2 audioDistance = (dist.x == default && dist.x == default) ? new (0f, 250f) : dist;

        indieAudios[indieAudioID].minDistance = audioDistance.x;
        indieAudios[indieAudioID].maxDistance = audioDistance.y;

        indieAudios[indieAudioID].Play();
        indieAudioID = (indieAudioID + 1) % indieAudios.Length;

    }

    public static void CustomUpdate () {

        // Set volumes
        MainMixer.SetFloat("Music", Mathf.Log10(Mathf.Max(SettingsSystem.MusicVolume, 0.001f)) * 20f);
        MainMixer.SetFloat("Sounds", Mathf.Log10(Mathf.Max(SettingsSystem.SoundVolume, 0.001f)) * 20f);

    }

}
