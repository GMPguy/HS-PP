using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class NPCsoldierComponent : NPCtemplate {

    // References
    public GameObject BloodSplat;
    public HumanoidComponent Humanoid;

    public override void CustomUpdate(float delta) {}

    public override void HitEffect(float damage, Vector3 position) {
        Transform splash = GameObject.Instantiate(BloodSplat).transform;
        splash.position = position;
        splash.Rotate(Vector3.one * Random.Range(0f, 360f));
    }

    public override void Dead() {
        CurrentState = AIstate.Dead;
        Humanoid.PlayAnim("Dead");

        CapsuleCollider col = GetComponent<CapsuleCollider>();
        col.height = col.radius = .1f;
        col.center = Vector3.down * .8f;
    }

    void Start () {
        ListHI();
    }

    void OnDestroy () {
        UnlistHI();
    }
    
}
