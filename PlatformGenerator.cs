using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour {

    List<Transform> plats = new List<Transform>();

    public GameObject[] platforms;
    public Transform[] genPoints;
    public Transform genLine;

    public float distBetween;

    bool spawning;
    int r;

    private void Update()
    {
        if(!GameManager.gameOver)
        {
            if (transform.position.y < genLine.position.y && !spawning)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + distBetween, transform.position.z);
                StartCoroutine(PlatformSpawning());
            }
        }
    }

    IEnumerator PlatformSpawning()
    {
        spawning = true;

        plats.Clear();
        r = 0;

        foreach (Transform t in genPoints)
        {
            plats.Add(t);
        }

        r = Random.Range(1, 2);

        int s = 0;

        while (s < r)
        {
            int p = Random.Range(0, plats.Count);
            Vector3 spawnPoint = plats[p].position;
            Instantiate(platforms[Random.Range(0, platforms.Length)], spawnPoint, Quaternion.identity);
            plats.Remove(plats[p]);
            s++;
        }

        yield return null;

        spawning = false;
    }
}
