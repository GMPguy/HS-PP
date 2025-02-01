using UnityEngine;

public interface HitInterface {
    public void Hit (float Damage, Vector3 position, GameObject killer = null);
    public void ListHI ();
    public void UnlistHI ();
    public GameObject GetObject ();
}
