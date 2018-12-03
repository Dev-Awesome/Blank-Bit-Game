using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    public Sprite[] blankMask;
    public SpriteRenderer blankSR;

    public AudioClip hitFX;

    public static bool hit;

    float powerTimer;
    float maskTimer;

    int timerStart;
    int m;

    GameManager gameManager;
    AudioManager audioManager;
    RaycastEngine raycastEngine;
    SpriteMask spriteMask;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        raycastEngine = GetComponent<RaycastEngine>();
        spriteMask = GetComponentInChildren<SpriteMask>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        if(hit)
        {
            gameManager.LoseHealth();
            hit = false;
        }
        else
        {
            if(gameManager.cardCount > 0)
            {
                if(spriteMask.sprite != blankMask[0])
                {
                    maskTimer += Time.deltaTime;

                    if(maskTimer <= 0.1f)
                    {
                        spriteMask.sprite = blankMask[m];
                    }
                    else if (maskTimer > 0.1f)
                    {
                        m--;
                        maskTimer = 0;
                    }
                }
                else
                {
                    spriteMask.enabled = false;
                    blankSR.enabled = false;
                    m = 0;
                    maskTimer = 0;
                }
            }
            else if (gameManager.cardCount == 0)
            {
                spriteMask.enabled = true;
                blankSR.enabled = true;

                if (spriteMask.sprite != blankMask[5])
                {
                    maskTimer += Time.deltaTime;

                    if (maskTimer <= 0.1f)
                    {
                        spriteMask.sprite = blankMask[m];
                    }
                    else if (maskTimer > 0.1f)
                    {
                        m++;
                        maskTimer = 0;
                    }
                }
                else
                {
                    m = 5;
                    maskTimer = 0;
                }
            }
        }

        if(timerStart == 0)
        {
            if(GameManager.isDashing)
            {
                timerStart = 1;
                powerTimer = 0;
            }
            else if(GameManager.isBoosting)
            {
                timerStart = 2;
                powerTimer = 0;
            }
        }
        else if(timerStart == 1)
        {
            powerTimer += Time.deltaTime;

            if (powerTimer >= 0.25f && !GameManager.isDashing)
            {
                powerTimer = 0;
                timerStart = 0;
            }
        }
        else if (timerStart == 2)
        {
            powerTimer += Time.deltaTime;

            if (powerTimer >= 0.25f && !GameManager.isBoosting)
            {
                powerTimer = 0;
                timerStart = 0;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if(!other.gameObject.GetComponent<EnemyAnim>().dead)
            {
                if (GameManager.isDashing && powerTimer > 0.25f || GameManager.isBoosting && powerTimer > 0.25f || !GameManager.isDashing && !GameManager.isBoosting && !GameManager.isErasing && powerTimer <= 0)
                {
                    hit = true;
                    raycastEngine.Stun(other.transform.position.x);
                    audioManager.fx.PlayOneShot(hitFX, 0.75f);
                }
                else
                {
                    other.gameObject.GetComponent<EnemyAnim>().DeathAnim();
                    raycastEngine.Stop();
                    GameManager.byteCount += 15;
                }
            }
        }
    }
}
