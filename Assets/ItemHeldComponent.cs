using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHeldComponent : MonoBehaviour {

    public static GrenadeComponent PulledGrenade;
    public GameObject Grenade;
    public Transform Socket;

    public void Pinout () {
        Transform newNade = Instantiate(Grenade).transform;
        newNade.SetParent(Socket);
        newNade.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void BombsAway () =>
        PulledGrenade.BombsAway(CameraSystem.MainCamera.transform.forward);
}
