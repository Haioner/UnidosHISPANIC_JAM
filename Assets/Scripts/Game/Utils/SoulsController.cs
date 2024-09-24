using System.Collections.Generic;
using UnityEngine;

public class SoulsController : MonoBehaviour
{
    [SerializeField] private GameObject jarPrefab;
    [SerializeField] private Transform jarHolder;
    private List<GameObject> jarList = new List<GameObject>();

    public void SpawnJar()
    {
        float randomX = Random.Range(-1f, 1f) + jarHolder.position.x;
        Vector3 randomPosition = new Vector3(randomX, jarHolder.position.y, jarHolder.position.z);
        GameObject currentJar = Instantiate(jarPrefab, randomPosition, Quaternion.identity, jarHolder);
        jarList.Add(currentJar);
    }

    public void RemoveJar()
    {
        if (jarList.Count > 0)
        {
            GameObject lastJar = jarList[jarList.Count - 1];
            jarList.RemoveAt(jarList.Count - 1);
            Destroy(lastJar);
        }
    }
}
