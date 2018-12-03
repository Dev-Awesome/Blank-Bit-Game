using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour {

    public Sprite[] idle;
    public Sprite[] walk;
    public Sprite[] fall;
    public Sprite dash;
    public Sprite jump;
    public Sprite boost;
    public Sprite crouch;
    public Sprite stun;
    public Sprite erase;
    public Sprite[] blankIdle;
    public Sprite[] blankWalk;
    public Sprite[] blankFall;
    public Sprite blankCrouch;
    public Sprite boostFX;
    public Sprite dashFX;
    public Sprite eraseFX;

    public SpriteRenderer blankSR;
    public SpriteRenderer moveFX;
    public ParticleSystem boostParticleFX;
    public ParticleSystem dashParticleFX;
    public ParticleSystem eraseParticleFX;
    public ParticleSystem jumpParticleFX;
    public Material boostL;
    public Material boostR;
    public Material dashL;
    public Material dashR;
    public Material eraseL;
    public Material eraseR;

    public BoxCollider2D hitBox;

    public int cycleCounter;
    public bool flip;

    int s;
    float timer;

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(!flip)
        {
            timer += Time.deltaTime;

            if(spriteRenderer.flipX)
            {
                moveFX.flipX = true;
            }
            else if(!spriteRenderer.flipX)
            {
                moveFX.flipX = false;
            }

            if (cycleCounter == 0)
            {
                if (timer < 0.15f)
                {
                    spriteRenderer.sprite = idle[s];
                    blankSR.sprite = blankIdle[s];
                }
                else if (timer >= 0.15f)
                {
                    if(s < 2)
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
                if (timer < 0.05f)
                {
                    spriteRenderer.sprite = walk[s];
                    blankSR.sprite = blankWalk[s];
                }
                else if (timer >= 0.05f)
                {
                    if (s < 3)
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
            else if (cycleCounter == 2)
            {
                if (timer < 0.1f)
                {
                    spriteRenderer.sprite = fall[s];
                    blankSR.sprite = blankFall[s];
                }
                else if (timer >= 0.1f)
                {
                    if (s < 1)
                    {
                        s++;
                        timer = 0;
                    }
                    else
                    {
                        timer = 0;
                    }
                }
            }
            else if (cycleCounter == 3)
            {
                spriteRenderer.sprite = jump;
                jumpParticleFX.Play();
                s = 0;
                timer = 0;
            }
            else if (cycleCounter == 4)
            {
                if(spriteRenderer.sprite != dash)
                {
                    spriteRenderer.sprite = dash;

                    if (moveFX.flipX)
                    {
                        moveFX.transform.localPosition = new Vector3(-0.1f, 0, 0);
                        dashParticleFX.GetComponent<ParticleSystemRenderer>().material = dashL;
                    }
                    else if (!moveFX.flipX)
                    {
                        moveFX.transform.localPosition = new Vector3(0.1f, 0, 0);
                        dashParticleFX.GetComponent<ParticleSystemRenderer>().material = dashR;
                    }

                    moveFX.sprite = dashFX;
                    moveFX.enabled = true;
                }

                dashParticleFX.Play();
                s = 0;
                timer = 0;
            }
            else if (cycleCounter == 5)
            {
                if(spriteRenderer.sprite != boost)
                {
                    spriteRenderer.sprite = boost;

                    if (spriteRenderer.flipX)
                    {
                        boostParticleFX.GetComponent<ParticleSystemRenderer>().material = boostL;
                    }
                    else if (!spriteRenderer.flipX)
                    {
                        boostParticleFX.GetComponent<ParticleSystemRenderer>().material = boostR;
                    }

                    moveFX.sprite = boostFX;
                    moveFX.transform.localPosition = new Vector3(0, 0.1f, 0);
                    moveFX.enabled = true;
                }

                boostParticleFX.Play();
                s = 0;
                timer = 0;
            }
            else if (cycleCounter == 6)
            {
                spriteRenderer.sprite = crouch;
                blankSR.sprite = blankCrouch;
                s = 0;
                timer = 0;
            }
            else if (cycleCounter == 7)
            {
                spriteRenderer.sprite = stun;
                s = 0;
                timer = 0;
            }
            else if(cycleCounter == 8)
            {
                if(spriteRenderer.sprite != erase)
                {
                    spriteRenderer.sprite = erase;

                    if (moveFX.flipX)
                    {
                        moveFX.transform.localPosition = new Vector3(-0.1f, 0, 0);
                        hitBox.offset = new Vector2(-1.25f, 0);
                        eraseParticleFX.GetComponent<ParticleSystemRenderer>().material = eraseL;
                    }
                    else if (!moveFX.flipX)
                    {
                        moveFX.transform.localPosition = new Vector3(0.1f, 0, 0);
                        hitBox.offset = new Vector2(1.25f, 0);
                        eraseParticleFX.GetComponent<ParticleSystemRenderer>().material = eraseR;
                    }

                    moveFX.sprite = eraseFX;
                    moveFX.enabled = true;
                    hitBox.enabled = true;
                }

                eraseParticleFX.Play();
                s = 0;
                timer = 0;
            }
        }
        else
        {
            s = 0;
            timer = 0;
            flip = false;
            moveFX.enabled = false;
            hitBox.enabled = false;
        }
    }
}
