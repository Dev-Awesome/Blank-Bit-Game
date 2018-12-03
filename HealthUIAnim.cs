using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIAnim : MonoBehaviour {

    public Sprite[] healthSprites;

    Image healthVis;

    float timer;
    int cycleConuter = 2;
    int s;

    private void Awake()
    {
        healthVis = GetComponent<Image>();
    }

    private void Update()
    {
        if(cycleConuter == 0)
        {
            timer = 0;
            s = 0;
            cycleConuter++;
        }
        else if (cycleConuter == 1)
        {
            timer += Time.deltaTime;

            if(timer <= 0.15f)
            {
                healthVis.sprite = healthSprites[s];
            }
            else if (timer > 0.15f)
            {
                if(s < 3)
                {
                    s++;
                    timer = 0;
                }
                else
                {
                    cycleConuter--;
                }
            }
        }
    }

    private void OnEnable()
    {
        cycleConuter = 0;
    }
}
