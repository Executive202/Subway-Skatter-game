using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

    private const int COIN_SCORE_AMOUNT = 5;

    public static GameManager Instance { set; get; }

    public bool IsDead { set; get; }
    public GameObject[] characters;
    public int selectedchar = 0;

    private bool isGameStarted = false;
    private PlayerMotor motor;
    private CameraMotor cam;
    private GlacierSpawner spawn;
    private FollowPlayer follow;
    public AudioManager audio;
    private AudioSource source;
    private bool soundRed = false;
    public bool mute = false;

    //UI and UI fields
    public Animator gameCanvasAnim,menuAnim,diamondAnim;
    public Text scoreText, coinText, modifierText, highscoreText,menuHighScore;
    private float score, coinScore, modifierScore;
    private int lastScore;

    // Death Menu
    public Animator deathMenuAnim;
    public Text deadScoreText, deadCoinText,muteText;

    private void Awake()
    {
        Instance = this;
        //motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        modifierScore = 1;

        highscoreText.text = PlayerPrefs.GetInt("Highscore").ToString();
        menuHighScore.text = PlayerPrefs.GetInt("Highscore").ToString();
    }
    private void Start()
    {
        CharacterReset();
        cam = GameObject.FindObjectOfType<CameraMotor>();
        spawn = GameObject.FindObjectOfType<GlacierSpawner>();
        follow = GameObject.FindObjectOfType<FollowPlayer>();
        source = GetComponent<AudioSource>();
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        soundRed = true;
    }

    private void Update()
    {
        if (motor == null)
        {
            MotorUpdate();
        }

        if (MobileInput.Instance.Tap && !isGameStarted)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            isGameStarted = true;
            if(soundRed)
            {
                source.volume = 0.75f;
                soundRed = false;
            }
            motor.StartRunning();

            FindObjectOfType<GlacierSpawner>().IsScrolling = true;
            FindObjectOfType<CameraMotor>().IsMoving = true;

            gameCanvasAnim.SetTrigger("Show");
            menuAnim.SetTrigger("Hide");
        }

        if (isGameStarted && !IsDead)
        {
            //Increase score
            score += (Time.deltaTime * modifierScore);
            if (lastScore != (int)score)
            {
                lastScore = (int)score;
                scoreText.text = score.ToString("0");
            }
        }
    }

    public void GetCoin()
    {
        diamondAnim.SetTrigger("Collect");
        audio.Play("coin");
        coinScore ++;
        score += COIN_SCORE_AMOUNT;
        coinText.text = coinScore.ToString("0");
        scoreText.text = score.ToString("0");
    }

    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;

        modifierText.text = "x" + modifierScore.ToString("0.0");
    }

    public void OnPlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void OnDeath()
    {
        IsDead = true;
        audio.Play("gameover");
        deadCoinText.text = score.ToString("0");
        deadScoreText.text = coinScore.ToString("0");

        deathMenuAnim.SetTrigger("Dead");

        FindObjectOfType<GlacierSpawner>().IsScrolling = false;

        gameCanvasAnim.SetTrigger("Hide");

        // Check if this is a highscore
        if (score > PlayerPrefs.GetInt("Highscore"))
        {
            float s = score;
            if (s % 1 == 0)
                s++;

            PlayerPrefs.SetInt("Highscore", (int)s);
        }
    }
    public void CharacterReset()
    {
        foreach (var ch in characters)
        {
            ch.SetActive(false);
        }
        characters[selectedchar].SetActive(true);
    }

    // change char
    public void ChangeCharacter(int newChar)
    {
        //audio.Play("click");
        characters[selectedchar].SetActive(false);
        characters[newChar].SetActive(true);
        selectedchar = newChar;
        RefreshPlayer();
    }
    private void RefreshPlayer()
    {
        cam.RefreshPlayer();
        spawn.RefreshPlayer();
        follow.RefreshPlayer();
        MotorUpdate();
    }
    public void GameMute()
    {
        if(mute == false)
        {
            mute = true;
            muteText.text = " /";
            source.Stop();
        }else if(mute == true)
        {
            mute = false;
            muteText.text = "";
            source.Play();
        }
    }
    private void  MotorUpdate()
    {
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
    }

}
