using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static bool cardPlayed;

    public static bool isJumping;
    public static bool isDashing;
    public static bool isBoosting;
    public static bool isErasing;
    public static bool isCrouching;
    public static bool isShuffling;

    public static bool actionTrigger;
    public static bool holding;
    public static bool inMenu;
    public static bool gameOver;
    public static bool startClick;
    public static bool paused;

    public static int byteCount;

    public GameObject card;
    public GameObject player;
    public GameObject cardItem;
    public GameObject audioMngr;
    public Camera glitchCam;
    public CameraShake glitchShake;
    public CameraShake mainShake;
    public Image frameUI;
    public Image screenUI;
    public AudioClip click;
    public AudioClip discardFX;
    public AudioClip spawnFX;
    public AudioClip[] collectFX;

    GameObject hand;
    GameObject discard;
    GameObject health;
    GameObject menuUI;
    GameObject restartButton;
    GameObject infoScreen;
    GameObject startSplash;

    Color backgroundColor;
    Text timeText;
    Text byteText;
    Text soundText;
    Text bestText;
    Text worstText;
    Image handBG;
    Vector3 handPos;

    AudioManager audioManager;

    public int cardCount;
    public int hitCounter;
    public int healthCount;

    float hitTimer;
    float gameTimer;
    float splashTimer;

    int scoreCounter;
    bool itemSpawn;

    private void Awake()
    {
        hand = GameObject.Find("Hand");
        discard = GameObject.Find("Discard");
        health = GameObject.Find("Health");
        timeText = GameObject.Find("Timer").GetComponent<Text>();
        byteText = GameObject.Find("Bytes").GetComponent<Text>();
        soundText = GameObject.Find("Sound").GetComponent<Text>();
        bestText = GameObject.Find("BestScore").GetComponent<Text>();
        worstText = GameObject.Find("WorstScore").GetComponent<Text>();
        handBG = GameObject.Find("Hand").GetComponent<Image>();
        menuUI = GameObject.Find("MenuUI");
        restartButton = GameObject.Find("Restart");
        infoScreen = GameObject.Find("InfoScreen");
        startSplash = GameObject.Find("StartSplash");

        backgroundColor = glitchCam.backgroundColor;
        scoreCounter = 0;

        PlayerPrefs.GetInt("Restart", 0);
        PlayerPrefs.GetInt("Bytes", 0);
        PlayerPrefs.GetFloat("Time", 0);
        PlayerPrefs.GetInt("WorstBytes", 0);
        PlayerPrefs.GetFloat("WorstTime", 0);
    }

    private void Start()
    {
        foreach(Transform h in health.transform)
        {
            h.gameObject.SetActive(false);
        }

        if(PlayerPrefs.GetInt("Restart", 0) == 0)
        {
            Instantiate(audioMngr, this.transform.position, Quaternion.identity);
            inMenu = true;
            gameOver = false;
            bestText.enabled = false;
            worstText.enabled = false;
            actionTrigger = false;
            holding = false;
            restartButton.SetActive(false);
            infoScreen.SetActive(false);
            startSplash.SetActive(false);
            frameUI.color = new Color(frameUI.color.r, frameUI.color.g, frameUI.color.b, 0);
            byteCount = 0;
            BoolReset();
        }
        else if (PlayerPrefs.GetInt("Restart", 0) == 1)
        {
            inMenu = false;
            gameOver = false;
            startClick = false;
            bestText.enabled = false;
            worstText.enabled = false;
            actionTrigger = false;
            holding = false;
            restartButton.SetActive(false);
            infoScreen.SetActive(false);
            startSplash.SetActive(false);
            frameUI.color = new Color(frameUI.color.r, frameUI.color.g, frameUI.color.b, 1);
            infoScreen.GetComponent<Image>().raycastTarget = false;
            menuUI.GetComponent<Image>().raycastTarget = false;
            byteCount = 0;
            BoolReset();
            StartCoroutine(RestartGame());
        }

        audioManager = FindObjectOfType<AudioManager>();
        handPos = hand.transform.position;
        handBG.color = new Color(handBG.color.r, handBG.color.g, handBG.color.b, 0);
    }

    private void Update()
    {
        if(!inMenu && !gameOver)
        {
            if(!paused)
            {
                if (!actionTrigger)
                {
                    if (!holding)
                    {
                        if (hand.transform.childCount > 0)
                        {
                            if (Input.GetKeyDown(KeyCode.Space))
                            {
                                actionTrigger = true;
                            }
                            else if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                            {
                                if (!isShuffling)
                                {
                                    StartCoroutine(HandShuffle());
                                }
                            }
                            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                            {
                                if (healthCount > 0)
                                {
                                    audioManager.fx.PlayOneShot(discardFX, 0.8f);
                                    RemoveCard();
                                    health.transform.GetChild(healthCount - 1).gameObject.SetActive(false);
                                    healthCount--;
                                }
                            }
                        }
                        else
                        {
                            if (discard.transform.childCount <= 0)
                            {
                                if (healthCount > 0)
                                {
                                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
                                    {
                                        SpawnCardItem();
                                    }
                                }
                            }
                        }

                        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                        {
                            isCrouching = true;
                        }
                        else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
                        {
                            isCrouching = false;
                        }

                        if(Input.GetKeyDown(KeyCode.Escape))
                        {
                            paused = true;
                        }
                    }
                    else
                        return;
                }
                else if (actionTrigger)
                {
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        actionTrigger = false;
                        holding = false;
                        BoolReset();

                        if (hand.transform.childCount == 0)
                        {
                            StartCoroutine(CheckDiscard());
                        }
                    }
                    else if (Input.GetKey(KeyCode.Space))
                    {
                        holding = true;
                    }
                }

                if (cardCount > 0)
                {
                    if (hitCounter == 0)
                    {
                        if (!isJumping && !isDashing && !isBoosting && !isErasing)
                        {
                            if (glitchCam.backgroundColor != backgroundColor)
                            {
                                glitchCam.backgroundColor = Color.Lerp(glitchCam.backgroundColor, backgroundColor, Time.deltaTime * 5);
                                frameUI.color = new Color(frameUI.color.r, frameUI.color.g, frameUI.color.b, Mathf.Lerp(frameUI.color.a, 1, Time.deltaTime * 5));
                            }

                            if (screenUI.color.a > 0)
                            {
                                screenUI.color = new Color(screenUI.color.r, screenUI.color.g, screenUI.color.b, Mathf.Lerp(screenUI.color.a, 0, Time.deltaTime * 8));
                            }

                            if (Camera.main.backgroundColor != Color.black)
                            {
                                Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, Color.black, Time.deltaTime * 5);
                            }

                            hitTimer = 0;
                        }
                        else if (isJumping)
                        {
                            hitTimer += Time.deltaTime;

                            if (hitTimer < 0.6f)
                            {
                                Camera.main.backgroundColor = new Color(0.117f, 0.654f, 0.882f, 1);
                            }
                            else
                            {
                                Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, Color.black, Time.deltaTime * 5);
                            }
                        }
                        else if (isDashing)
                        {
                            hitTimer += Time.deltaTime;

                            if (hitTimer < 0.6f)
                            {
                                Camera.main.backgroundColor = new Color(1f, 0.8f, 0f, 1);
                            }
                            else
                            {
                                Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, Color.black, Time.deltaTime * 5);
                            }
                        }
                        else if (isBoosting)
                        {
                            hitTimer += Time.deltaTime;

                            if (hitTimer < 0.6f)
                            {
                                Camera.main.backgroundColor = new Color(0.9f, 0.41f, 0.09f, 1);
                            }
                            else
                            {
                                Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, Color.black, Time.deltaTime * 5);
                            }
                        }
                        else if (isErasing)
                        {
                            hitTimer += Time.deltaTime;

                            if (hitTimer < 0.6f)
                            {
                                Camera.main.backgroundColor = new Color(0.45f, 0.8f, 0.29f, 1);
                            }
                            else
                            {
                                Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, Color.black, Time.deltaTime * 5);
                            }
                        }
                    }
                    else if (hitCounter == 1)
                    {
                        hitTimer += Time.deltaTime;
                        glitchCam.backgroundColor = Color.Lerp(glitchCam.backgroundColor, new Color(1, 1, 0, 0), Time.deltaTime * 15);

                        if (hitTimer >= 0.5f)
                        {
                            hitCounter = 0;
                        }
                    }
                    else if (hitCounter == 2)
                    {
                        hitTimer += Time.deltaTime;
                        glitchCam.backgroundColor = Color.Lerp(glitchCam.backgroundColor, new Color(0.085f, 0.085f, 0, 0), Time.deltaTime * 10);

                        if (hitTimer >= 0.25f)
                        {
                            hitCounter = 0;
                        }
                    }
                }
                else
                {
                    if (healthCount <= 0)
                    {
                        glitchCam.backgroundColor = Color.Lerp(glitchCam.backgroundColor, new Color(0.96f, 0.96f, 0.86f, 0), Time.deltaTime * 2);

                        if (glitchCam.backgroundColor.r >= 0.9f)
                        {
                            frameUI.color = new Color(frameUI.color.r, frameUI.color.g, frameUI.color.b, Mathf.Lerp(frameUI.color.a, 0, Time.deltaTime * 2));
                            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, Color.white, Time.deltaTime * 3);
                            screenUI.color = new Color(screenUI.color.r, screenUI.color.g, screenUI.color.b, Mathf.Lerp(screenUI.color.a, 1, Time.deltaTime * 0.75f));

                            if (screenUI.color.a >= 0.9f)
                            {
                                gameOver = true;
                            }
                        }
                        else
                        {
                            if (screenUI.color.a > 0)
                            {
                                screenUI.color = new Color(screenUI.color.r, screenUI.color.g, screenUI.color.b, Mathf.Lerp(screenUI.color.a, 0, Time.deltaTime * 8));
                            }
                        }
                    }
                }
            }
            else if (paused)
            {
                Time.timeScale = 0;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    paused = false;
                    Time.timeScale = 1;
                }
            }
        }
        else if (inMenu && !gameOver)
        {
            if (screenUI.color.a > 0)
            {
                screenUI.color = new Color(screenUI.color.r, screenUI.color.g, screenUI.color.b, Mathf.Lerp(screenUI.color.a, 0, Time.deltaTime * 8));
            }
        }
        else if (!inMenu && gameOver)
        {
            if (screenUI.color.a < 1)
            {
                screenUI.color = new Color(screenUI.color.r, screenUI.color.g, screenUI.color.b, Mathf.Lerp(screenUI.color.a, 1, Time.deltaTime * 2));
            }
        }
    }

    private void LateUpdate()
    {
        if (!inMenu && !gameOver)
        {
            if(!paused)
            {
                if (!startClick)
                {
                    if (byteText.enabled && timeText.enabled)
                    {
                        gameTimer += Time.deltaTime;

                        timeText.text = "TIME = " + gameTimer.ToString("F2");
                        byteText.text = "BYTES = " + byteCount.ToString();

                        if(hand.transform.position.y != handPos.y)
                        {
                            hand.transform.position = Vector3.Lerp(hand.transform.position, handPos, Time.deltaTime * 5);
                            handBG.color = Color.Lerp(handBG.color, new Color(handBG.color.r, handBG.color.g, handBG.color.b, 0), Time.deltaTime * 10);
                            hand.transform.localScale = Vector3.Lerp(hand.transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * 5);
                        }

                        if(startSplash.transform.localPosition.y < 500)
                        {
                            startSplash.transform.localPosition = new Vector3(startSplash.transform.localPosition.x, Mathf.Lerp(startSplash.transform.localPosition.y, 500, Time.deltaTime * 5), startSplash.transform.localPosition.z);
                        }
                    }
                    else
                    {
                        byteText.enabled = true;
                        timeText.enabled = true;
                    }
                }
                else
                {
                    byteText.enabled = false;
                    timeText.enabled = false;
                }
            }
            else if (paused)
            {
                hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(hand.transform.position.x, handPos.y + 350), Time.unscaledDeltaTime * 5);
                handBG.color = Color.Lerp(handBG.color, new Color(handBG.color.r, handBG.color.g, handBG.color.b, 1), Time.unscaledDeltaTime * 10);
                hand.transform.localScale = Vector3.Lerp(hand.transform.localScale, new Vector3(2,2,2), Time.unscaledDeltaTime * 5);
            }
        }
        else if (inMenu && !gameOver)
        {
            if (startClick)
            {
                Text[] menuText = menuUI.GetComponentsInChildren<Text>();

                foreach (Text t in menuText)
                {
                    if (t.color.a > 0)
                    {
                        t.color = new Color(t.color.r, t.color.g, t.color.b, Mathf.Lerp(t.color.a, 0, Time.unscaledDeltaTime * 2));
                    }
                }
            }
            else
            {
                byteText.enabled = false;
                timeText.enabled = false;
                bestText.enabled = false;
                worstText.enabled = false;
            }
        }
        else if (!inMenu && gameOver)
        {
            timeText.transform.position = Vector3.Lerp(timeText.transform.position, new Vector3(550, 575, 0), Time.deltaTime * 2);
            byteText.transform.position = Vector3.Lerp(byteText.transform.position, new Vector3(225, 575, 0), Time.deltaTime * 2);

            if (scoreCounter == 0)
            {
                if (byteCount > PlayerPrefs.GetInt("Bytes", 0))
                {
                    PlayerPrefs.SetInt("Bytes", byteCount);
                    PlayerPrefs.SetFloat("Time", gameTimer);
                    scoreCounter = 1;
                }
                else if (byteCount == PlayerPrefs.GetInt("Bytes", 0))
                {
                    if (gameTimer < PlayerPrefs.GetFloat("Time", 0))
                    {
                        PlayerPrefs.SetInt("Bytes", byteCount);
                        PlayerPrefs.SetFloat("Time", gameTimer);
                        scoreCounter = 1;
                    }
                    else
                    {
                        scoreCounter = 2;
                    }
                }
                else if (byteCount < PlayerPrefs.GetInt("Bytes", 0))
                {
                    if(PlayerPrefs.GetInt("WorstBytes", 0) != 0)
                    {
                        if (byteCount < PlayerPrefs.GetInt("WorstBytes", 0))
                        {
                            PlayerPrefs.SetInt("WorstBytes", byteCount);
                            PlayerPrefs.SetFloat("WorstTime", gameTimer);
                            scoreCounter = 3;
                        }
                        else
                        {
                            if(byteCount == PlayerPrefs.GetInt("WorstBytes", 0))
                            {
                                if(gameTimer > PlayerPrefs.GetFloat("WorstTime", 0))
                                {
                                    PlayerPrefs.SetInt("WorstBytes", byteCount);
                                    PlayerPrefs.SetFloat("WorstTime", gameTimer);
                                    scoreCounter = 3;
                                }
                                else
                                {
                                    scoreCounter = 2;
                                }
                            }
                            else
                            {
                                scoreCounter = 2;
                            }
                        }
                    }
                    else
                    {
                        if(PlayerPrefs.GetFloat("WorstTime", 0) == 0)
                        {
                            PlayerPrefs.SetInt("WorstBytes", byteCount);
                            PlayerPrefs.SetFloat("WorstTime", gameTimer);
                            scoreCounter = 3;
                        }
                        else
                        {
                            scoreCounter = 2;
                        }
                    }
                }
                else
                {
                    scoreCounter = 2;
                }
            }
            else if(scoreCounter == 1)
            {
                bestText.text = "YOUR NEW BEST IS " + PlayerPrefs.GetInt("Bytes", 0).ToString() + " BYTES IN " + PlayerPrefs.GetFloat("Time", 0).ToString("F2") + " SECONDS! B-)";
                worstText.text = "YOUR WORST IS STILL " + PlayerPrefs.GetInt("WorstBytes", 0).ToString() + " BYTES IN " + PlayerPrefs.GetFloat("WorstTime", 0).ToString("F2") + " SECONDS.";
                scoreCounter = 4;
            }
            else if (scoreCounter == 2)
            {
                bestText.text = "YOUR BEST IS STILL " + PlayerPrefs.GetInt("Bytes", 0).ToString() + " BYTES IN " + PlayerPrefs.GetFloat("Time", 0).ToString("F2") + " SECONDS.";
                worstText.text = "YOUR WORST IS STILL " + PlayerPrefs.GetInt("WorstBytes", 0).ToString() + " BYTES IN " + PlayerPrefs.GetFloat("WorstTime", 0).ToString("F2") + " SECONDS.";
                scoreCounter = 4;
            }
            else if(scoreCounter == 3)
            {
                bestText.text = "YOUR BEST IS STILL " + PlayerPrefs.GetInt("Bytes", 0).ToString() + " BYTES IN " + PlayerPrefs.GetFloat("Time", 0).ToString("F2") + " SECONDS.";
                worstText.text = "YOUR NEW WORST IS " + PlayerPrefs.GetInt("WorstBytes", 0).ToString() + " BYTES IN " + PlayerPrefs.GetFloat("WorstTime", 0).ToString("F2") + " SECONDS! B-(";
                scoreCounter = 4;
            }
            else if (scoreCounter == 4)
            {
                if (timeText.transform.position.x >= 500)
                {
                    bestText.enabled = true;
                    
                    if(PlayerPrefs.GetFloat("WorstTime", 0) > 0)
                    {
                        worstText.enabled = true;
                    }

                    if (timeText.transform.position.x >= 545)
                    {
                        restartButton.SetActive(true);
                    }
                }
            }
        }
    }

    public void AddCard(int c)
    {
        if(cardCount < 5)
        {
            audioManager.fx.PlayOneShot(collectFX[Random.Range(0, collectFX.Length)], 0.4f);
            GameObject newCard = Instantiate(card, hand.transform);
            newCard.GetComponent<CardDisplay>().c = c;
            newCard.transform.SetAsLastSibling();
            health.transform.GetChild(cardCount).gameObject.SetActive(true);
            cardCount++;
            byteCount += 10;

            if(healthCount < 5)
            {
                healthCount++;
            }
        }
    }

    public void LoseHealth()
    {
        if (hitCounter == 0)
        {
            glitchShake.ShakeCamera();
            mainShake.ShakeCamera();

            if (healthCount > 0)
            {
                if (discard.transform.childCount > 0)
                {
                    Destroy(discard.transform.GetChild(discard.transform.childCount - 1).gameObject);
                    health.transform.GetChild(healthCount - 1).gameObject.SetActive(false);
                    cardCount--;
                    healthCount--;
                }
                else
                {
                    Destroy(hand.transform.GetChild(hand.transform.childCount - 1).gameObject);
                    health.transform.GetChild(healthCount - 1).gameObject.SetActive(false);
                    cardCount--;
                    healthCount--;
                }

                StartCoroutine(CheckDiscard());
                hitCounter = 1;
            }
            else
            {
                gameOver = true;
            }
        }
    }

    public void RemoveCard()
    {
        if (cardCount > 0)
        {
            glitchShake.ShakeCamera();

            if (hand.transform.childCount > 0)
            {
                Destroy(hand.transform.GetChild(0).gameObject);
                cardCount--;
                byteCount -= 10;
            }
            else
            {
                Destroy(discard.transform.GetChild(discard.transform.childCount - 1).gameObject);
                cardCount--;
                byteCount -= 10;
            }

            StartCoroutine(CheckDiscard());
            hitCounter = 2;
        }
    }

    public void ExchangeCard(int c)
    {
        if (discard.transform.childCount > 0)
        {
            audioManager.fx.PlayOneShot(collectFX[Random.Range(0, collectFX.Length)], 0.4f);
            Destroy(discard.transform.GetChild(discard.transform.childCount - 1).gameObject);
            GameObject newCard = Instantiate(card, discard.transform);
            newCard.GetComponent<CardDisplay>().c = c;
            newCard.transform.SetAsLastSibling();
            byteCount += 10;
        }
        else
        {
            audioManager.fx.PlayOneShot(collectFX[Random.Range(0, collectFX.Length)], 0.4f);
            Destroy(hand.transform.GetChild(hand.transform.childCount - 1).gameObject);
            GameObject newCard = Instantiate(card, hand.transform);
            newCard.GetComponent<CardDisplay>().c = c;
            newCard.transform.SetAsLastSibling();
            byteCount += 10;
        }
    }

    public void AddBytes(int bytesAdded)
    {
        byteCount += bytesAdded;
    }

    private void BoolReset()
    {
        cardPlayed = false;
        isJumping = false;
        isDashing = false;
        isBoosting = false;
        isCrouching = false;
        isErasing = false;
    }

    private void SpawnCardItem()
    {
        if(!itemSpawn)
        {
            Transform playerCurrent = GameObject.FindGameObjectWithTag("Player").transform;
            Vector3 offset = new Vector3(playerCurrent.position.x - 2f, playerCurrent.position.y, playerCurrent.position.z);
            Instantiate(cardItem, offset, Quaternion.identity);
            health.transform.GetChild(healthCount - 1).gameObject.SetActive(false);
            healthCount--;
            itemSpawn = true;
        }
        else if (itemSpawn)
        {
            Transform playerCurrent = GameObject.FindGameObjectWithTag("Player").transform;
            Vector3 offset = new Vector3(playerCurrent.position.x + 2f, playerCurrent.position.y, playerCurrent.position.z);
            Instantiate(cardItem, offset, Quaternion.identity);
            health.transform.GetChild(healthCount - 1).gameObject.SetActive(false);
            healthCount--;
            itemSpawn = false;
        }

        audioManager.fx.PlayOneShot(spawnFX);
    }

    public void StartButton()
    {
        if(inMenu && !startClick && !infoScreen.activeSelf)
        {
            audioManager.fx.PlayOneShot(click, 0.4f);
            StartCoroutine(StartGame());
        }
    }

    public void SoundButton()
    {
        if(inMenu && !infoScreen.activeSelf)
        {
            audioManager.fx.PlayOneShot(click, 0.4f);

            if (!AudioManager.mute)
            {
                AudioManager.mute = true;
                soundText.text = "SOUND OFF";
            }
            else if (AudioManager.mute)
            {
                AudioManager.mute = false;
                soundText.text = "SOUND ON";
            }
        }
    }

    public void InfoButton()
    {
        if(inMenu && !infoScreen.activeSelf)
        {
            audioManager.fx.PlayOneShot(click, 0.4f);
            infoScreen.SetActive(true);
        }
    }

    public void RestartButton()
    {
        if(!inMenu && gameOver)
        {
            audioManager.fx.PlayOneShot(click, 0.4f);
            PlayerPrefs.SetInt("Restart", 1);
            SceneManager.LoadScene(0);
        }
    }

    public void ClickOut()
    {
        if (inMenu && infoScreen.activeSelf)
        {
            audioManager.fx.PlayOneShot(click, 0.4f);
            infoScreen.SetActive(false);
        }
    }

    IEnumerator HandShuffle()
    {
        isShuffling = true;

        if (!actionTrigger && isShuffling)
        {
            if (hand.transform.childCount > 1)
            {
                foreach(Transform h in hand.transform)
                {
                    if(h.GetSiblingIndex() == 0)
                    {
                        h.SetAsLastSibling();
                        break;
                    }
                }
            }
        }

        yield return null;

        isShuffling = false;
    }

    IEnumerator CheckDiscard()
    {
        yield return null;

        if (hand.transform.childCount == 0)
        {
            if (discard.transform.childCount > 0)
            {
                yield return new WaitForSeconds(0.3f);

                Draggable[] cards = discard.GetComponentsInChildren<Draggable>();

                int c = 0;

                while (c < cards.Length)
                {
                    cards[c].transform.SetParent(hand.transform);
                    cards[c].transform.SetSiblingIndex(cards[c].transform.GetSiblingIndex());
                    cards[c].cardType = Draggable.Slot.PLAYABLE;
                    c++;
                }
            }
        }
    }

    IEnumerator StartGame()
    {
        startClick = true;

        Instantiate(player, new Vector3(0, -2f, 0), Quaternion.identity);

        Time.timeScale = 0;

        infoScreen.GetComponent<Image>().raycastTarget = false;
        menuUI.GetComponent<Image>().raycastTarget = false;

        yield return new WaitForSecondsRealtime(0.75f);

        startSplash.SetActive(true);

        yield return new WaitForSecondsRealtime(0.25f);

        startSplash.SetActive(false);

        yield return new WaitForSecondsRealtime(0.25f);

        startSplash.SetActive(true);

        yield return new WaitForSecondsRealtime(0.25f);

        startSplash.SetActive(false);

        yield return new WaitForSecondsRealtime(0.25f);

        startSplash.SetActive(true);

        yield return new WaitForSecondsRealtime(0.25f);

        startSplash.SetActive(false);

        yield return new WaitForSecondsRealtime(0.25f);

        startSplash.SetActive(true);

        yield return new WaitForSecondsRealtime(1.25f);

        PlayerPrefs.SetInt("Restart", 0);
        Time.timeScale = 1;
        scoreCounter = 0;
        inMenu = false;
        startClick = false;
    }

    IEnumerator RestartGame()
    {
        Text[] menuText = menuUI.GetComponentsInChildren<Text>();

        foreach (Text t in menuText)
        {
            if (t.color.a > 0)
            {
                t.color = new Color(t.color.r, t.color.g, t.color.b, 0);
            }
        }

        screenUI.color = new Color(screenUI.color.r, screenUI.color.g, screenUI.color.b, 0);

        startClick = true;

        Instantiate(player, new Vector3(0, -2f, 0), Quaternion.identity);

        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(0.75f);

        startSplash.SetActive(true);

        yield return new WaitForSecondsRealtime(0.25f);

        startSplash.SetActive(false);

        yield return new WaitForSecondsRealtime(0.25f);

        startSplash.SetActive(true);

        yield return new WaitForSecondsRealtime(0.25f);

        startSplash.SetActive(false);

        yield return new WaitForSecondsRealtime(0.25f);

        startSplash.SetActive(true);

        yield return new WaitForSecondsRealtime(0.25f);

        startSplash.SetActive(false);

        yield return new WaitForSecondsRealtime(0.25f);

        startSplash.SetActive(true);

        yield return new WaitForSecondsRealtime(1.25f);

        PlayerPrefs.SetInt("Restart", 0);
        Time.timeScale = 1;
        scoreCounter = 0;
        inMenu = false;
        startClick = false;
    }
}
