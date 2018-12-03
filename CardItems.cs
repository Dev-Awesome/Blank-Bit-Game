using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardItems : MonoBehaviour {

    public Sprite[] colors;
    public Sprite remove;
    public AudioClip removeFX;

    public int r;

    SpriteRenderer spriteRenderer;
    AudioManager audioManager;

    int c;
    float timer;

    GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        if(r == 0)
        {
            timer += Time.deltaTime;

            if (timer <= 0.25f)
            {
                spriteRenderer.sprite = colors[c];
            }
            else if (timer > 0.25f)
            {
                if (c < 3)
                {
                    c++;
                    timer = 0;
                }
                else
                {
                    c = 0;
                    timer = 0;
                }
            }
        }
        else if (r == 1)
        {
            spriteRenderer.sprite = colors[0];
        }
        else if (r == 2)
        {
            spriteRenderer.sprite = colors[1];
        }
        else if (r == 3)
        {
            spriteRenderer.sprite = colors[2];
        }
        else if (r == 4)
        {
            spriteRenderer.sprite = colors[3];
        }
        else if (r == 5)
        {
            spriteRenderer.sprite = remove;
        }
    }

    private void FixedUpdate()
    {
        if(gameObject.name == "RemoveItem(Clone)")
        {
            this.transform.Rotate(0,0,3);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if(gameObject.name != "RemoveItem(Clone)")
            {
                if (gameManager.cardCount < 5)
                {
                    gameManager.AddCard(r);
                    Destroy(this.gameObject);
                }
                else
                {
                    gameManager.ExchangeCard(r);
                    Destroy(this.gameObject);
                }
            }
            else
            {
                if(gameManager.cardCount > 0)
                {
                    audioManager.fx.PlayOneShot(removeFX, 0.8f);
                    gameManager.RemoveCard();
                    Destroy(this.gameObject);
                }
            }
        }
        else if (other.name == "Death")
        {
            Destroy(this.gameObject);
        }
    }
}
