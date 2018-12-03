using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByteItem : MonoBehaviour {

    public Sprite[] byteSprites;
    public AudioClip collect;

    SpriteRenderer spriteRenderer;
    AudioManager audioManager;
    GameManager gameManager;

    int b;
    int floatCycle;
    float timer;
    float dist;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        dist = this.transform.position.y - 0.5f;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z);
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer <= 0.25f)
        {
            spriteRenderer.sprite = byteSprites[b];
        }
        else if (timer > 0.25f)
        {
            if (b < 3)
            {
                b++;
                timer = 0;
            }
            else
            {
                b = 0;
                timer = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        if(floatCycle == 0)
        {
            if (this.transform.position.y > dist)
            {
                this.transform.position = new Vector3(this.transform.position.x, Mathf.Lerp(this.transform.position.y, dist, Time.fixedDeltaTime), this.transform.position.z);

                if(this.transform.position.y <= dist + 0.1f)
                {
                    floatCycle++;
                }
            }
        }
        else if(floatCycle == 1)
        {
            this.transform.position = new Vector3(this.transform.position.x, Mathf.Lerp(this.transform.position.y, dist + 1, Time.fixedDeltaTime), this.transform.position.z);

            if(this.transform.position.y >= dist + 0.75f)
            {
                floatCycle--;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            audioManager.fx.PlayOneShot(collect, 0.5f);
            gameManager.AddBytes(25);
            Destroy(this.gameObject);
        }
        else if(other.name == "Death")
        {
            Destroy(this.gameObject);
        }
    }
}
