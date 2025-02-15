using UnityEngine;
using static Enums;

public abstract class UITemplate : MonoBehaviour {

    public bool Sacred;
    public bool Cleared, InstantDestroyOnClear;
    protected float Lifetime;
    
    public void UpdateLifetime () => Lifetime += Time.deltaTime;

    abstract public void SetUp (int addition);

    abstract public void UIUpdate ();

    abstract public void ClearUp ();

}

