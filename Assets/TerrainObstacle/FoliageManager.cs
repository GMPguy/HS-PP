using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class FoliageManager : MonoBehaviour
{
    public Terrain terrain; // Referencja do terenu
    public Transform terrainObstacleContainer; // Referencja do kontenera na przeszkody
    public GameObject terrainObstaclePref; // Referencja do prefabu przeszkody

    public GameObject go;

    void Start()
    {
        var navob = go.GetComponent<NavMeshObstacle>();
        if (terrain == null || terrainObstacleContainer == null || terrainObstaclePref == null)
        {
            Debug.Log("Brak referencji");
            return;
        }

        TreeInstance[] trees = terrain.terrainData.treeInstances; // Pobranie informacji o instancjach drzew
        foreach (var tree in trees)
        {
            // Obliczene lokalizacji drzewa
            Vector3 treeWorldPosition = Vector3.Scale(tree.position, terrain.terrainData.size) + terrain.transform.position;

            // Zmienna zawieraj�ca wyliczony obrot drzewa
            // tree.rotation to tylko obr�t wzgl�dem osi Y!!!
            Quaternion treeWorldRotation = Quaternion.AngleAxis(tree.rotation * Mathf.Rad2Deg, Vector3.up);

            //Pobranie referencji do prefabu danego drzewa
            GameObject treePref = terrain.terrainData.treePrototypes[tree.prototypeIndex].prefab;

            //Pobranie wszystkich NavMeshObstacle znajduj�cych si� w prefabie
            var treePrefObstacles = treePref.GetComponentsInChildren<NavMeshObstacle>();
            foreach (var treePrefObstacle in treePrefObstacles)
            {
                //Obliczenie pozycji NavMeshObstacle w �wiecie
                var navWorldPosition = treeWorldPosition + treePrefObstacle.transform.localPosition; //Uwzgl�dnienie przesuni�cia
                Vector3 direction = navWorldPosition - treeWorldPosition;
                navWorldPosition = treeWorldPosition + treeWorldRotation * direction;

                var navWorldRotation = treeWorldRotation;

                // Uwzgl�dnienie obrotu, je�li komponent znajduje si� w zagnie�d�onym obiekcie
                if (treePrefObstacle.gameObject != treePref) 
                {
                    navWorldRotation = navWorldRotation * treePrefObstacle.transform.localRotation; 
                }

                //Tworzy instancj� przeszkody w obliczonej pozycji i rotacji oraz z przypisanym rodzicem 
                GameObject obstacleInstance = Instantiate(terrainObstaclePref, navWorldPosition, navWorldRotation, terrainObstacleContainer);

                //Konfiguracja parametr�w NavMeshObstacle z uwzgl�dnieniem kszta�tu
                var navMeshObstacle = obstacleInstance.GetComponent<NavMeshObstacle>();
                navMeshObstacle.shape = treePrefObstacle.shape;
                navMeshObstacle.center = treePrefObstacle.center;

                if (navMeshObstacle.shape == NavMeshObstacleShape.Capsule)
                {
                    navMeshObstacle.radius = treePrefObstacle.radius;
                    navMeshObstacle.height = treePrefObstacle.height * 2;
                }
                else
                {
                    navMeshObstacle.size = treePrefObstacle.size;
                }

                navMeshObstacle.carving = treePrefObstacle.carving;
                navMeshObstacle.carvingMoveThreshold = treePrefObstacle.carvingMoveThreshold;
                navMeshObstacle.carvingTimeToStationary = treePrefObstacle.carvingTimeToStationary;
                navMeshObstacle.carveOnlyStationary = treePrefObstacle.carveOnlyStationary;
            }
        }
    }
}
