using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerErase : MonoBehaviour {

    bool erased;
    float timer;

    private void Update()
    {
        if(erased)
        {
            timer += Time.deltaTime;

            if(timer >= 0.75f)
            {
                erased = false;
                timer = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!erased)
        {
            if (other.tag == "Enemy")
            {
                if(other.gameObject.name != "AttackCollider")
                {
                    other.GetComponentInChildren<EnemyAnim>().DeathAnim();
                    erased = true;
                    GameManager.byteCount += 15;
                }
            }
            else if (other.tag == "Item")
            {
                Destroy(other.gameObject);
                erased = true;
                GameManager.byteCount += 5;
            }
        }
    }
}
