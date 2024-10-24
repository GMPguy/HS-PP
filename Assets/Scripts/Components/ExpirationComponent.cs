using UnityEngine;

public class ExpirationComponent : MonoBehaviour {
    
    public float Lifetime;

    public bool Expire (float delta) => (Lifetime -= delta) <= 0f;

}
