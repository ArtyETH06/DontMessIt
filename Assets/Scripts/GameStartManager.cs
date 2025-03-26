using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStartManager : MonoBehaviour
{
    [Header("Références UI")]
    public Button startButton;
    public TMP_Text countdownText;
    public GameObject countdownPanel;

    [Header("Décompte")]
    public int countdownStartValue = 5;
    public float countdownInterval = 1f;

    void Start()
    {
        // Au lancement de la scène
        countdownText.gameObject.SetActive(false);
        countdownPanel.SetActive(true);

        // Lier le bouton
        startButton.onClick.AddListener(() => StartCoroutine(StartCountdown()));
    }

    IEnumerator StartCountdown()
    {
        // Masquer le bouton et activer le texte
        startButton.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(true);

        int count = countdownStartValue;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(countdownInterval);
            count--;
        }

        // Afficher "GO !!"
        countdownText.text = "GO !!";
        yield return new WaitForSeconds(1f);

        // Masquer le panel
        countdownPanel.SetActive(false);

        // TODO : Lancer ici la logique de début de jeu (activer ennemis, timer, etc.)
        Debug.Log("Début du jeu !");
    }
}
