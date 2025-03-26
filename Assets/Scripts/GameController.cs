using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Zone Manager")]
    public GuardianZoneGenerator zoneGenerator;

    [Header("UI Elements")]
    public TMP_Text scoreText;
    public TMP_Text colorTargetText;
    public TMP_Text timerText;
    public TMP_Text statusText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public Button retryButton;
    public Button returnToMenuButton;

    [Header("Timing")]
    public float timePerRound = 5f;

    private int score = 0;
    private float timer = 0f;
    private bool isRoundActive = false;

    private ColoredZone currentTargetZone;

    void Start()
    {
        retryButton.onClick.AddListener(RestartGame);
        returnToMenuButton.onClick.AddListener(ReturnToMenu);

        gameOverPanel.SetActive(false);
        UpdateScoreUI();
        StartCoroutine(StartNewRound());
    }

    void Update()
    {
        if (!isRoundActive) return;

        timer -= Time.deltaTime;
        timerText.text = "Temps restant : " + Mathf.CeilToInt(timer).ToString();

        if (timer <= 0f)
        {
            bool isInZone = currentTargetZone.collider.Contains(Camera.main.transform.position);

            if (isInZone)
            {
                statusText.text = "✅ Bien joué !";
                score++;
                UpdateScoreUI();
                StartCoroutine(StartNewRound());
            }
            else
            {
                statusText.text = "❌ Raté !";
                GameOver();
            }

            isRoundActive = false;
        }
    }

    IEnumerator StartNewRound()
    {
        yield return new WaitForSeconds(1f); // Petite pause avant le prochain round

        currentTargetZone = zoneGenerator.coloredZones[Random.Range(0, zoneGenerator.coloredZones.Count)];

        string colorHex = ColorUtility.ToHtmlStringRGB(currentTargetZone.color);
        colorTargetText.text = $"Couleur cible : <color=#{colorHex}>■</color>";

        statusText.text = "";
        timer = timePerRound;
        isRoundActive = true;
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score : " + score;
    }

    void GameOver()
    {
        isRoundActive = false;
        gameOverPanel.SetActive(true);
        finalScoreText.text = "Score final : " + score;
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene"); // Remplace par le nom exact de ta scène de menu
    }
}
