using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private enum MusicState
    {
        None = 0, Menu, Level, GameOver,
    }

    private enum FXState
    {
        None = 0, Boost, Dash, Jump, Erase,
    }

    public static AudioManager instance = null;
    public static bool mute;

    public AudioClip menuMusic;
    public AudioClip levelMusic;
    public AudioClip levelMusicTwo;
    public AudioClip levelMusicThree;
    public AudioClip levelMusicFour;
    public AudioClip gameOverMusic;
    public AudioClip jumpFX;
    public AudioClip dashFX;
    public AudioClip boostFX;
    public AudioClip eraseFX;
    public AudioClip startFX;

    public AudioSource music;
    public AudioSource fx;

    private MusicState musicState;
    private FXState fxState;

    int levelMusicCounter;
    float levelMusicTimer;
    float musicVol;
    float fxVol;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicVol = music.volume;
        fxVol = fx.volume;
    }

    private void Update()
    {
        switch(musicState)
        {
            case MusicState.None:
                if(GameManager.inMenu && !GameManager.startClick)
                {
                    musicState = MusicState.Menu;
                }
                break;

            case MusicState.Menu:
                if (GameManager.startClick)
                {
                    musicState = MusicState.Level;
                }
                else
                {
                    if(music.clip != menuMusic)
                    {
                        music.clip = menuMusic;
                        music.loop = true;
                        music.Play();
                    }
                }
                break;

            case MusicState.Level:
                if(GameManager.gameOver)
                {
                    musicState = MusicState.GameOver;
                }
                else
                {
                    if(levelMusicCounter == 0)
                    {
                        if (music.clip != levelMusic)
                        {
                            levelMusicTimer = 0;
                            music.clip = levelMusic;
                            music.loop = false;
                            music.Play();
                        }
                        else
                        {
                            levelMusicTimer += Time.unscaledDeltaTime;

                            if(levelMusicTimer >= levelMusic.length)
                            {
                                levelMusicCounter++;
                            }
                        }
                    }
                    else if (levelMusicCounter == 1)
                    {
                        if (music.clip != levelMusicTwo)
                        {
                            levelMusicTimer = 0;
                            music.clip = levelMusicTwo;
                            music.loop = false;
                            music.Play();
                        }
                        else
                        {
                            levelMusicTimer += Time.unscaledDeltaTime;

                            if (levelMusicTimer >= levelMusicTwo.length)
                            {
                                levelMusicCounter++;
                            }
                        }
                    }
                    else if (levelMusicCounter == 2)
                    {
                        if (music.clip != levelMusicThree)
                        {
                            levelMusicTimer = 0;
                            music.clip = levelMusicThree;
                            music.loop = false;
                            music.Play();
                        }
                        else
                        {
                            levelMusicTimer += Time.unscaledDeltaTime;

                            if (levelMusicTimer >= levelMusicThree.length)
                            {
                                levelMusicCounter++;
                            }
                        }
                    }
                    else if (levelMusicCounter == 3)
                    {
                        if (music.clip != levelMusicFour)
                        {
                            levelMusicTimer = 0;
                            music.clip = levelMusicFour;
                            music.loop = false;
                            music.Play();
                        }
                        else
                        {
                            levelMusicTimer += Time.unscaledDeltaTime;

                            if (levelMusicTimer >= levelMusicFour.length)
                            {
                                levelMusicCounter++;
                            }
                        }
                    }
                    else if (levelMusicCounter == 4)
                    {
                        //You win?
                    }
                }
                break;

            case MusicState.GameOver:
                if (!GameManager.inMenu && GameManager.startClick)
                {
                    musicState = MusicState.Level;
                }
                else
                {
                    if (music.clip != gameOverMusic)
                    {
                        levelMusicCounter = 0;
                        music.clip = gameOverMusic;
                        music.loop = true;
                        music.Play();
                    }
                }
                break;
        }

        switch(fxState)
        {
            case FXState.None:
                if (GameManager.isJumping)
                {
                    fx.pitch = Random.Range(0.75f, 1.25f);
                    fx.PlayOneShot(jumpFX, 0.5f);
                    fxState = FXState.Jump;
                }
                else if (GameManager.isDashing)
                {
                    fx.pitch = Random.Range(0.75f, 1.25f);
                    fx.PlayOneShot(dashFX, 0.5f);
                    fxState = FXState.Dash;
                }
                else if (GameManager.isBoosting)
                {
                    fx.pitch = Random.Range(0.75f, 1.25f);
                    fx.PlayOneShot(boostFX, 0.6f);
                    fxState = FXState.Boost;
                }
                else if (GameManager.isErasing)
                {
                    fx.pitch = Random.Range(0.75f, 1.25f);
                    fx.PlayOneShot(eraseFX, 0.5f);
                    fxState = FXState.Erase;
                }

                if(!GameManager.actionTrigger)
                {
                    if(fx.pitch != 1)
                    {
                        fx.pitch = Mathf.Lerp(fx.pitch, 1, Time.deltaTime * 2);
                    }
                }

                break;

            case FXState.Jump:
                if(!GameManager.actionTrigger)
                {
                    fxState = FXState.None;
                }
                break;

            case FXState.Dash:
                if (!GameManager.actionTrigger)
                {
                    fxState = FXState.None;
                }
                break;

            case FXState.Boost:
                if (!GameManager.actionTrigger)
                {
                    fxState = FXState.None;
                }
                break;

            case FXState.Erase:
                if (!GameManager.actionTrigger)
                {
                    fxState = FXState.None;
                }
                break;
        }
    }

    private void LateUpdate()
    {
        if(mute)
        {
            music.volume = 0;
            fx.volume = 0;
        }
        else if (!mute)
        {
            if(GameManager.startClick)
            {
                if(fx.clip != startFX)
                {
                    fx.clip = startFX;
                    fx.loop = false;
                    fx.pitch = 1;
                    //fx.PlayOneShot(startFX, 0.2f);
                }
            }
            else if(!GameManager.startClick)
            {
                fx.clip = null;

                if (GameManager.gameOver)
                {
                    fx.volume = 0;
                }
            }

            if(!GameManager.paused)
            {
                if(music.volume != musicVol)
                {
                    music.volume = Mathf.Lerp(music.volume, musicVol, Time.deltaTime * 10);
                }

                fx.volume = fxVol;
            }
            else if (GameManager.paused)
            {
                if(music.volume != 0.1f)
                {
                    music.volume = Mathf.Lerp(music.volume, 0.1f, Time.unscaledDeltaTime * 10);

                    if(music.volume <= 0.15f)
                    {
                        music.volume = 0.1f;
                    }
                }
            }
        }
    }
}
