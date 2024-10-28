using System.Collections;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI ballText;
    [SerializeField] TextMeshProUGUI infoText;
    private void Awake()
    {
        infoText.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        GameController.OnScoreUpdated += UpdateScore;
        GameController.OnBallsUpdated += UpdateBalls;
        GameController.MercyTime += HandleMercy;
        GameController.GameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        GameController.OnScoreUpdated -= UpdateScore;
        GameController.OnBallsUpdated -= UpdateBalls;
        GameController.MercyTime -= HandleMercy;
        GameController.GameOver -= HandleGameOver;
    }

    private void HandleGameOver()
    {
        StartCoroutine(ShowInfoText("Game over...", 10f));
    }

    private void HandleMercy(bool hasMercy)
    {
        if (hasMercy)
        {
            StartCoroutine(ShowInfoText("Ball restored...", 2f));
        }
        else
        {
            StartCoroutine(ShowInfoText("No ball for you...", 2f));
        }
    }

    // FIXME - will break if two messages get called at once
    IEnumerator ShowInfoText(string text, float duration)
    {
        infoText.text = text;
        infoText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        infoText.gameObject.SetActive(false);
    }
    private void UpdateBalls(int count)
    {
        ballText.text = $"Balls: {count}";
    }

    private void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}
