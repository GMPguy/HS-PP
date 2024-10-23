using UnityEngine;

[CreateAssetMenu(fileName = "New objects", menuName = "Configs/Objects")]
public class ObjectsConfig : ScriptableObject {

    [SerializeField]
    GameObject[] Objects;

    public GameObject Fetch (string objectName) {

        for (int find = 0; find <= Objects.Length; find++) {
            if (find == Objects.Length)
                Debug.LogError("Could not find object of name " + objectName);
            else if (Objects[find].name == objectName)
                return Objects[find];
        }

        return null;

    }

}
