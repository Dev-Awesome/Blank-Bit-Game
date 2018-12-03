using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnim : MonoBehaviour {

    public Sprite[] walkLE;
    public Sprite[] walkDE;
    public Sprite[] attackLE;
    public Sprite[] attackDE;
    public Sprite[] maskSprites;
    public SpriteRenderer fxSR;
    public AudioClip enemyDeath;
    public BoxCollider2D attackCollider;

    SpriteRenderer spriteRenderer;
    SpriteMask spriteMask;
    CameraShake glitchShake;
    AudioManager audioManager;

    public int cycleCounter;
    public bool flip;
    public bool dead;

    Vector3 lastPos;
    float timer;
    int s;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteMask = GetComponentInChildren<SpriteMask>();
        glitchShake = GameObject.Find("GlitchCamera").GetComponent<CameraShake>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        spriteMask.enabled = false;
    }

    private void Update()
    {
        if(!spriteMask.enabled)
        {
            if (!flip)
            {
                timer += Time.deltaTime;

                if (cycleCounter == 0)
                {
                    if (timer < 0.2f)
                    {
                        spriteRenderer.sprite = walkLE[s];
                        fxSR.sprite = walkDE[s];
                        lastPos = this.transform.position;
                    }
                    else if (timer >= 0.2f)
                    {
                        if (this.transform.position.x > lastPos.x)
                        {
                            spriteRenderer.flipX = false;
                            fxSR.flipX = false;
                            attackCollider.offset = new Vector2(0.6f, 0);
                        }
                        else if (this.transform.position.x < lastPos.x)
                        {
                            spriteRenderer.flipX = true;
                            fxSR.flipX = true;
                            attackCollider.offset = new Vector2(-0.6f, 0);
                        }

                        if (s < 2)
                        {
                            s++;
                            timer = 0;
                        }
                        else
                        {
                            s = 0;
                            timer = 0;
                        }
                    }
                }
                else if (cycleCounter == 1)
                {
                    if (timer < 0.1f)
                    {
                        spriteRenderer.sprite = attackLE[s];
                        fxSR.sprite = attackDE[s];
                    }
                    else if (timer >= 0.1f)
                    {
                        if (s < 2)
                        {
                            s++;
                            timer = 0;
                            attackCollider.enabled = false;
                        }
                        else
                        {
                            attackCollider.enabled = true;
                            timer = 0;
                        }
                    }
                }
                else if (cycleCounter == 2)
                {
                    spriteRenderer.sprite = walkLE[0];
                    fxSR.sprite = walkDE[0];
                    s = 0;
                    timer = 0;
                }
            }
            else
            {
                s = 0;
                timer = 0;
                flip = false;
                attackCollider.enabled = false;
            }
        }
        else
        {
            timer += Time.deltaTime;

            if(timer <= 0.1f)
            {
                spriteMask.sprite = maskSprites[s];
            }
            else if (timer > 0.1f)
            {
                if (s < 5)
                {
                    s++;
                    timer = 0;
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.name == "Death")
        {
            Destroy(this.gameObject);
        }
    }

    public void DeathAnim()
    {
        dead = true;
        audioManager.fx.PlayOneShot(enemyDeath, 0.5f);
        glitchShake.ShakeCamera();
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        spriteMask.sprite = maskSprites[0];

        yield return null;

        s = 0;
        timer = 0;
        attackCollider.enabled = false;
        spriteMask.enabled = true;
    }
}
