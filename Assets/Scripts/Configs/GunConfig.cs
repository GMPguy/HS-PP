using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Configs/Gun")]
public class GunConfig : ItemConfig {

    public float2 Damage;
    public float2 Accuracy;
    public float RecoilTime;
    public float2 Recoil;

}
