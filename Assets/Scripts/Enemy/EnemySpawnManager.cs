using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField]
    private List<Pair<int>> enemyGroups;

    private List<int> spawnedGroups = new();
    private bool spawningEnemies = false;
    private GameObject currentGroup = null;
    private float spawnTime = 1.5f;
    private float spawnStarted = 0f;

    private Vector3 spawnPosition = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;
    private Vector3 lerpPosition = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Pair<int> pair in enemyGroups)
        {
            foreach (Transform t in pair.Value.transform)
            {
                t.GetComponent<NavMeshAgent>().enabled = false;
            }

            pair.Value.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float dist = TrainManager.main.Goal - TrainManager.main.Distance;

        Pair<int> firstNewGroup = enemyGroups.Where(x => !spawnedGroups.Contains(x.Key)).OrderBy(x => x.Key).FirstOrDefault();

        if (firstNewGroup != null && firstNewGroup.Key < dist)
        {
            spawnedGroups.Add(firstNewGroup.Key);
            firstNewGroup.Value.SetActive(true);
            spawningEnemies = true;
            currentGroup = firstNewGroup.Value;
            spawnPosition = currentGroup.transform.position;
            targetPosition = new Vector3(spawnPosition.x, 0f, spawnPosition.z);
            spawnStarted = Time.time;
        }

        if (spawningEnemies && currentGroup != null) {
            float lerp = (Time.time - spawnStarted) / spawnTime;
            currentGroup.transform.position = Vector3.Lerp(spawnPosition, targetPosition, lerp);

            if (lerp >= 1f)
            {
                foreach(Transform t in currentGroup.transform)
                {
                    t.GetComponent<NavMeshAgent>().enabled = true;
                }

                spawningEnemies = false;
                currentGroup = null;
            }
        }
    }
}
