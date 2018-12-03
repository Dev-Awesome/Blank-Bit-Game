using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour {

    public float rise;

    private void Awake()
    {
        rise = this.transform.position.y;
    }

    private void Update()
    {
        if(!GameManager.inMenu)
        {
            rise += Time.deltaTime * 0.75f;

            this.transform.position = new Vector3(this.transform.position.x, rise, this.transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            GameManager.gameOver = true;
        }
        else if (other.tag == "Enemy" || other.tag == "Item" || other.tag == "Platform")
        {
            Destroy(other.gameObject);
        }
    }
}
