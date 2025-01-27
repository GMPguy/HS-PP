using UnityEngine;
using Unity.Mathematics;
using Random=UnityEngine.Random;

public class EffectComponent : MonoBehaviour {

    public float Lifetime;
    public AudioSource Audio;
    public AudioClip[] Clips;

    public float2 PitchVariation = new (1f, 1f);

    void Start () {
        Audio.clip = Clips[(int)Random.Range(0, Clips.Length - .1f)];
        Audio.pitch = Random.Range(PitchVariation.x, PitchVariation.y);
        Audio.Play();
        Destroy(gameObject, Lifetime);
    }

}
