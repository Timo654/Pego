using System.Collections;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI ballText;
    [SerializeField] TextMeshProUGUI infoText;
    private float textEndTime;
    private bool isShowingText;
    private void Awake()
    {
        infoText.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        GameController.OnScoreUpdated += UpdateScore;
        GameController.MercyTime += HandleMercy;
        GameController.GameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        GameController.OnScoreUpdated -= UpdateScore;
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

    IEnumerator ShowInfoText(string text, float duration)
    {
        infoText.text = text;
        textEndTime = Time.time + duration;
        if (isShowingText) yield break;
        isShowingText = true;
        infoText.gameObject.SetActive(true);
        while (textEndTime > Time.time)
            yield return null;
        infoText.gameObject.SetActive(false);
        isShowingText = false;
    }

    private void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}
