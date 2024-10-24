using Unity.Mathematics;
using UnityEngine;
using static Enums;

[CreateAssetMenu(fileName = "New Gun", menuName = "Configs/Gun")]
public class GunConfig : ItemConfig {

    public string Animation_Fire;
    public string Animation_Reload;

    public GunFireMode FireMode;
    public float2 Damage;
    public AnimationCurve Accuracy;
    public float RecoilTime;

    public float Recoil;
    public AnimationCurve[] RecoilPatterns;

    public float ReloadTime;

    public GameObject GunFire;

}
