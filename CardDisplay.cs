using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour {

    public Card[] types;

    public Text nameText;
    public Image art;
    public Image border;
    public Transform indicator;

    public int c;

    float lerpTimer;

    Card card;
    Draggable draggable;
    GameObject discard;

    private void Awake()
    {
        draggable = GetComponent<Draggable>();
        discard = GameObject.Find("Discard");
    }

    private void Update()
    {
        if(!GameManager.paused)
        {
            if (this.transform.parent.name == "Hand")
            {
                if (GameManager.actionTrigger)
                {
                    if (this.transform.GetSiblingIndex() == 0)
                    {
                        if (draggable.cardType == Draggable.Slot.PLAYABLE)
                        {
                            if (nameText.text == "JUMP")
                            {
                                if (!GameManager.cardPlayed)
                                {
                                    GameManager.cardPlayed = true;
                                    GameManager.isJumping = true;
                                    draggable.cardType = Draggable.Slot.UNPLAYABLE;
                                }
                            }
                            else if (nameText.text == "DASH")
                            {
                                if (!GameManager.cardPlayed)
                                {
                                    GameManager.cardPlayed = true;
                                    GameManager.isDashing = true;
                                    draggable.cardType = Draggable.Slot.UNPLAYABLE;
                                }
                            }
                            else if (nameText.text == "BOOST")
                            {
                                if (!GameManager.cardPlayed)
                                {
                                    GameManager.cardPlayed = true;
                                    GameManager.isBoosting = true;
                                    draggable.cardType = Draggable.Slot.UNPLAYABLE;
                                }
                            }
                            else if (nameText.text == "ERASE")
                            {
                                if (!GameManager.cardPlayed)
                                {
                                    GameManager.cardPlayed = true;
                                    GameManager.isErasing = true;
                                    draggable.cardType = Draggable.Slot.UNPLAYABLE;
                                }
                            }

                            if (indicator.gameObject.activeSelf)
                            {
                                indicator.gameObject.SetActive(false);
                            }
                            else
                            {
                                indicator.localPosition = new Vector3(-40f, indicator.localPosition.y, indicator.localPosition.z);
                            }
                        }
                        else
                        {
                            if (GameManager.cardPlayed)
                            {
                                Discarding();
                            }
                        }
                    }
                }
                else
                {
                    if (this.transform.GetSiblingIndex() == 0)
                    {
                        if (!indicator.gameObject.activeSelf)
                        {
                            indicator.gameObject.SetActive(true);
                            lerpTimer = 0;
                        }
                        else
                        {
                            lerpTimer += Time.deltaTime;

                            if (lerpTimer <= 0.5f)
                            {
                                indicator.localPosition = new Vector3(Mathf.Lerp(indicator.localPosition.x, -55f, Time.deltaTime * 2), indicator.localPosition.y, indicator.localPosition.z);
                            }
                            else if (lerpTimer > 0.5f)
                            {
                                indicator.localPosition = new Vector3(Mathf.Lerp(indicator.localPosition.x, -40f, Time.deltaTime * 2), indicator.localPosition.y, indicator.localPosition.z);

                                if (lerpTimer >= 1)
                                {
                                    lerpTimer = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (indicator.gameObject.activeSelf)
                        {
                            indicator.gameObject.SetActive(false);
                        }
                        else
                        {
                            indicator.localPosition = new Vector3(-40f, indicator.localPosition.y, indicator.localPosition.z);
                        }
                    }
                }
            }
        }
        else if(GameManager.paused)
        {
            if (indicator.gameObject.activeSelf)
            {
                indicator.gameObject.SetActive(false);
            }
        }
    }

    private void LateUpdate()
    {
        if(!GameManager.paused)
        {
            if (this.transform.localScale != Vector3.one)
            {
                this.transform.localScale = Vector3.one;
            }
        }
    }

    private void OnEnable()
    {
        StartCoroutine(Activate());
    }

    IEnumerator Activate()
    {
        yield return null;

        if(c == 0)
        {
            card = types[Random.Range(0, types.Length)];
        }
        else if (c == 1)
        {
            card = types[0];
        }
        else if (c == 2)
        {
            card = types[3];
        }
        else if (c == 3)
        {
            card = types[2];
        }
        else if (c == 4)
        {
            card = types[1];
        }

        nameText.text = card.name;
        art.sprite = card.artwork;
        art.enabled = false;
        border.color = card.color;
        draggable.cardType = Draggable.Slot.PLAYABLE;
    }

    void Discarding()
    {
        this.transform.SetParent(discard.transform);
        this.transform.SetAsLastSibling();
    }
}
