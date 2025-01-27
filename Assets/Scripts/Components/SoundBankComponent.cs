using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBankComponent : MonoBehaviour {

    [SerializeField]
    AudioSource[] Audios;

    [SerializeField]
    SoundBankConfig SoundConfig;

    int currentAudio = 0;

    public void PlayAudio (AudioClip clip, float volume = 1f, float pitch = 1f) {
        Audios[currentAudio].clip = clip;
        Audios[currentAudio].volume = volume;
        Audios[currentAudio].pitch = pitch;
        Audios[currentAudio].Play();

        currentAudio = (currentAudio + 1) % Audios.Length;
    }

    public void PlayAudio (string path, float volume = 1f, float pitch = 1f) =>
        PlayAudio(SoundConfig.GetSound(path), volume, pitch);

}
