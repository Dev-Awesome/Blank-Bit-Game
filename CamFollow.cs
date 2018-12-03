using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour {

    Transform player = null;
    Transform deathFloor;

    public float desiredPosition;
    float offsetPosition;

    private void Awake()
    {
        deathFloor = GameObject.Find("Death").transform;
    }

    private void Update()
    {
        if(!GameManager.inMenu)
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }
            else
            {
                desiredPosition = player.position.y - this.transform.position.y;
                offsetPosition = player.position.y + 1f;

                if (desiredPosition > 2 || desiredPosition < 0.5f)
                {
                    this.transform.position = new Vector3(this.transform.position.x, Mathf.Lerp(this.transform.position.y, Mathf.Clamp(offsetPosition, deathFloor.position.y, Mathf.Infinity), Time.deltaTime * 1.75f), this.transform.position.z);
                }
            }
        }
    }
}
