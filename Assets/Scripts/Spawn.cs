using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public Transform plane;
    public GameObject[] prefabObject;

    public float minX = -50f;
    public float maxX = 50f;

    public float spawnDistance = 100f;
    public float spawnTime = 25f;

    public float maxY = 6f;
    public float minY = 0f;

    public float despawnDistance = 30f;

    private float nextSpawnZ;

    private List<GameObject> spawnedObstacles =
        new List<GameObject>();


    void Start()
    {
        if (plane == null || prefabObject.Length == 0)
        {
            Debug.LogError("Missing aircraft or obstacle prefabs!");
            enabled = false;
            return;
        }

        nextSpawnZ = plane.position.z + spawnDistance;
    }

    // Update is called once per frame
    void Update()
    {
        SpawnObstacles();
        DespawnObstacles();
    }

    void SpawnObstacles()
    {
        while (nextSpawnZ <= plane.position.z + spawnDistance)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            Vector3 spawnPosition = new Vector3(
                randomX,
                randomY,
                nextSpawnZ
            );

            int randomIndex =
                Random.Range(0, prefabObject.Length);

            GameObject obstacle = Instantiate(
    prefabObject[randomIndex],
    spawnPosition,
    Quaternion.identity
);

            PlaneController controller =
                plane.GetComponent<PlaneController>();

            PassTrigger[] triggers =
                obstacle.GetComponentsInChildren<PassTrigger>(true);

            foreach (PassTrigger trigger in triggers)
            {
                trigger.Initialize(controller);
            }

            spawnedObstacles.Add(obstacle);

            nextSpawnZ += spawnTime;
        }
    }

    void DespawnObstacles()
    {
        for (int i = spawnedObstacles.Count - 1; i >= 0; i--)
        {
            GameObject obstacle = spawnedObstacles[i];

            if (obstacle == null)
            {
                spawnedObstacles.RemoveAt(i);
                continue;
            }

            // Remove obstacle after aircraft passes it
            if (plane.position.z >
                obstacle.transform.position.z + despawnDistance)
            {
                Destroy(obstacle);
                spawnedObstacles.RemoveAt(i);
            }
        }
    }
}
