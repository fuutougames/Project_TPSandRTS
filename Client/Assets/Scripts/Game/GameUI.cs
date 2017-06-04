using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBase
{
    private Image fadePlane;
    private Button playButton;
    private Button returnButton;
    private Transform gameOverUI;

    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    public Text scoreUI;
    public RectTransform healthBar;
    public Text gameOverScoreUI;

    Spawner spawner;
    Player player;

    protected override void OnAwake()
    {
        base.OnAwake();

        fadePlane = this.transform.Find("Fade").GetComponent<Image>();
        gameOverUI = this.transform.Find("GameOverUI");
        playButton = gameOverUI.Find("PlayAgainButton").GetComponent<Button>();
        playButton.onClick.AddListener(StartNewGame);
        returnButton = gameOverUI.Find("ReturnToMenuButton").GetComponent<Button>();
        returnButton.onClick.AddListener(ReturnToMenu);
        gameOverScoreUI = gameOverUI.Find("GameOverScoreUI").GetComponent<Text>();

        newWaveBanner = this.transform.Find("NewwaveBanner").GetComponent<RectTransform>();
        newWaveTitle = newWaveBanner.Find("Title").GetComponent<Text>();
        newWaveEnemyCount = newWaveBanner.Find("EnemyCount").GetComponent<Text>();

        scoreUI = this.transform.Find("ScoreUI").GetComponent<Text>();
        healthBar = this.transform.Find("HealthBar/Bar").GetComponent<RectTransform>();
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        scoreUI.text = ScoreKeeper.score.ToString("D6");
        float healthPercent = 0;
        if (player != null)
        {
            healthPercent = player.health / player.startingHealth;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
    }

    private void OnNewWave(int waveNumber)
    {
        string[] numbers = { "One", "Two", "Three", "Four", "Five" };
        newWaveTitle.text = "- Wave " + numbers[waveNumber - 1] + " -";
        string enemyCountString = (spawner.waves[waveNumber - 1].infinite) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "";
        newWaveEnemyCount.text = "Enemies : " + enemyCountString;

        StopCoroutine("AnimateNewWaveBanner");
        //StopAllCoroutines();
        StartCoroutine("AnimateNewWaveBanner");
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 1.5f;
        float speed = 2.5f;
        float animaterPercent = 0;
        int dir = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime;

        while(animaterPercent >= 0)
        {
            animaterPercent += Time.deltaTime * speed * dir;

            if(animaterPercent >= 1)
            {
                animaterPercent = 1;
                if(Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-170, 45, animaterPercent);
            yield return null;
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
    }

    private void OnGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, .95f), 1));
        gameOverScoreUI.text = scoreUI.text;
        scoreUI.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.gameObject.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    // UI Input

    private void StartNewGame()
    {
        SceneManager.LoadSceneAsync("Game");
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }
}
