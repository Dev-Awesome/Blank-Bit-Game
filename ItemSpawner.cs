using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {

    public GameObject[] items;
    public GameObject[] enemies;

    Vector3 itemSpawn;
    Vector3 enemySpawn;
    GameManager gameManager;

    int r;

    private void Awake()
    {
        r = Random.Range(1, 100);
        itemSpawn = new Vector3(this.transform.position.x + 1.05f, this.transform.position.y + 1.5f, this.transform.position.z);
        enemySpawn = new Vector3(this.transform.position.x + 1.05f, this.transform.position.y + 1.25f, this.transform.position.z + 0.15f);
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        if(gameObject.name != "MovingPlatform(Clone)" && gameObject.name != "MovingPlatform")
        {
            if (gameManager.cardCount >= 3)
            {
                if (r <= 25)
                {
                    Instantiate(items[Random.Range(0, items.Length)], itemSpawn, Quaternion.identity);
                }
                else if (r > 25 && r <= 75)
                {
                    Instantiate(enemies[Random.Range(0, enemies.Length)], enemySpawn, Quaternion.identity);
                }
            }
            else if (gameManager.cardCount < 3)
            {
                if (r <= 45)
                {
                    Instantiate(items[Random.Range(0, items.Length)], itemSpawn, Quaternion.identity);
                }
                else if (r > 45 && r <= 75)
                {
                    Instantiate(enemies[Random.Range(0, enemies.Length)], enemySpawn, Quaternion.identity);
                }
            }
        }
        else
        {
            if (gameManager.cardCount >= 3)
            {
                if (r <= 15)
                {
                    Instantiate(items[Random.Range(0, items.Length)], itemSpawn, Quaternion.identity);
                }
            }
            else if (gameManager.cardCount < 3)
            {
                if (r <= 30)
                {
                    Instantiate(items[Random.Range(0, items.Length)], itemSpawn, Quaternion.identity);
                }
            }
        }
    }
}
