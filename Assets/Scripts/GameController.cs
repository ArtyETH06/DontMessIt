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

    [Header("UI Manager")]
    public GameUIManager uiManager;

    [Header("Timing")]
    public float timePerRound = 5f;

    [Header("Effets visuels")]
    public GameObject successParticlesPrefab;

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
        // On attend le démarrage via GameStartManager
    }

    public void StartGameplay()
    {
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

                // ✨ Afficher particules
                SpawnSuccessParticles(currentTargetZone);

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
        if (zoneGenerator.coloredZones.Count == 0)
        {
            Debug.LogError("Aucune zone colorée générée !");
            statusText.text = "❌ Guardian manquant.";
            gameOverPanel.SetActive(true);
            finalScoreText.text = "Score final : " + score;
            retryButton.gameObject.SetActive(false);
            returnToMenuButton.gameObject.SetActive(true);
            yield break;
        }

        yield return new WaitForSeconds(1f);

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
        finalScoreText.text = "Score final: " + score;
        uiManager.ShowGameOver();
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu"); // Remplace si nom différent
    }

    void SpawnSuccessParticles(ColoredZone zone)
    {
        if (successParticlesPrefab == null) return;

        Vector3 p1 = zone.collider.GetPoint(0);
        Vector3 p2 = zone.collider.GetPoint(1);
        Vector3 p3 = zone.collider.GetPoint(2);
        Vector3 center = (p1 + p2 + p3) / 3f;

        GameObject particles = Instantiate(successParticlesPrefab, center, Quaternion.identity);
        Destroy(particles, 2f);
    }
}
