using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HumanoidComponent : MonoBehaviour {
    
    // References
    public Animator AnimationController;
    public Transform HandRoot, HeadRoot;
    public SkinnedMeshRenderer HumanoidMesh;

    // Start values
    public string CurrentStance, CurrentAnim;
    public Texture[] Uniforms, Faces;

    public void PlayAnim (string name) => AnimationController.Play(name, 1, 0f);
    public void ChangeState (string name) => AnimationController.Play(name, 0);
    public void ChangePace (float value) => AnimationController.SetFloat("Pace", value);

    public void ChangeTool (GameObject tool) {
        for (int gc = HandRoot.childCount - 1; gc >= 0; gc--)
            Destroy(HandRoot.GetChild(gc).gameObject);
        
        if (!tool)
            return;
        
        Transform newTool = GameObject.Instantiate(tool).transform;
        newTool.SetParent(HandRoot);
        newTool.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    void Start () {

        if (CurrentStance != "")
            ChangeState(CurrentStance);

        if (CurrentAnim != "")
            PlayAnim(CurrentAnim);
        
        if (Faces.Length > 0 || Uniforms.Length > 0)
            foreach (Material mat in HumanoidMesh.materials)
                mat.SetTexture("_MainTex",
                    mat.name switch {
                        "Soldier1 (Instance)" => Uniforms[(int)Random.Range(0, Uniforms.Length - .1f)],
                        _ => Faces[(int)Random.Range(0, Faces.Length - .1f)]
                    }
                );

    }

}
