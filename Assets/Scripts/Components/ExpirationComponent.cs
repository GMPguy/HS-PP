using UnityEngine;

public class ExpirationComponent : MonoBehaviour {
    
    public float Lifetime;

    void Start () => Destroy(gameObject, Lifetime);

}
