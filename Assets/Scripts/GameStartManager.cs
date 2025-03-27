using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStartManager : MonoBehaviour
{
    [Header("Références UI")]
    public Button startButton;                // Bouton "Démarrer"
    public TMP_Text countdownText;           // Texte du décompte
    public GameObject countdownPanel;        // Panel de démarrage

    [Header("Managers externes")]
    public GameUIManager uiManager;          // Contrôle des panels
    public GameController gameController;    // Lancement du jeu

    [Header("Décompte")]
    public int countdownStartValue = 5;      // Valeur de départ
    public float countdownInterval = 1f;     // Temps entre chaque chiffre

    [Header("Effets spéciaux")]
    public Image whiteFlashImage;            // Image blanche plein écran (facultatif)

    void Start()
    {
        countdownText.gameObject.SetActive(false);
        countdownPanel.SetActive(true);
        uiManager.ShowStart(); // Affiche le StartGamePanel uniquement

        startButton.onClick.AddListener(() => StartCoroutine(StartCountdown()));
    }

    IEnumerator StartCountdown()
    {
        startButton.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(true);

        int count = countdownStartValue;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            countdownText.fontSize = 120; // Texte grand dès l'apparition
            countdownText.color = Color.white;
            StartCoroutine(PunchScale(countdownText.transform));
            yield return new WaitForSeconds(countdownInterval);
            count--;
        }

        // GO !!
        countdownText.text = "GO !!";
        countdownText.fontSize = 200;
        countdownText.color = Color.yellow;
        StartCoroutine(PunchScale(countdownText.transform));
        StartCoroutine(FlashWhite()); // (Optionnel)

        yield return new WaitForSeconds(1f);

        countdownPanel.SetActive(false);
        uiManager.ShowGame();                 // Passe au GamePanel
        gameController.StartGameplay();       // Démarre la logique du jeu
    }

    // Effet de "zoom" temporaire sur le texte
    IEnumerator PunchScale(Transform target)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 punchScale = originalScale * 1.5f;
        float duration = 0.2f;

        target.localScale = punchScale;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float factor = t / duration;
            target.localScale = Vector3.Lerp(punchScale, originalScale, factor);
            yield return null;
        }

        target.localScale = originalScale;
    }

    // Flash blanc rapide à l'écran (facultatif)
    IEnumerator FlashWhite()
    {
        if (whiteFlashImage == null) yield break;

        Color originalColor = whiteFlashImage.color;
        originalColor.a = 1f;
        whiteFlashImage.color = originalColor;
        whiteFlashImage.gameObject.SetActive(true);

        float fadeTime = 0.5f;
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            whiteFlashImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        whiteFlashImage.gameObject.SetActive(false);
    }
}
