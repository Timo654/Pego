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

    private void HandleGameOver(GameOverType type)
    {
        string msg = "Game over...";
        switch (type)
        {
            case GameOverType.OutOfBalls:
                msg = "Game over...\nRan out of balls.\nPress B to retry...";
                break;
            case GameOverType.GotAllRed:
                msg = "Congratulations!\nGot all reds!\nPress B to continue.";
                break;
            case GameOverType.GotAll:
                msg = "Amazing!\nLevel cleared!\nPress B to continue.";
                break;
        }
        StartCoroutine(ShowInfoText(msg, 10f));
        // TODO - let the user continue somehow
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
