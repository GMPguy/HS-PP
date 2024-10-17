using Unity.Mathematics;
using UnityEngine;
using static Enums;

[CreateAssetMenu(fileName = "New Gun", menuName = "Configs/Gun")]
public class GunConfig : ItemConfig {

    public string Animation_Fire;

    public GunFireMode FireMode;
    public float2 Damage;
    public float2 Accuracy;
    public float RecoilTime;
    public float2 Recoil;

}
