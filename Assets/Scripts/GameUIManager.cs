using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [Header("Panels UI")]
    public GameObject startGamePanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;

    void Start()
    {
        // Par défaut, on démarre sur le Start Panel
        ShowStart();
    }

    // Affiche uniquement le Start Panel
    public void ShowStart()
    {
        startGamePanel.SetActive(true);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    // Affiche uniquement le Game Panel
    public void ShowGame()
    {
        startGamePanel.SetActive(false);
        gamePanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    // Affiche uniquement le Game Over Panel
    public void ShowGameOver()
    {
        startGamePanel.SetActive(false);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
}
