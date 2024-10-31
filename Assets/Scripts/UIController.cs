using DG.Tweening;
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
    private int currentScore = 0;
    private void Awake()
    {
        infoText.gameObject.SetActive(false);
    }
    private void Start()
    {
        if (!SaveManager.Instance.runtimeData.seenTutorial)
        {
            StartCoroutine(ShowInfoText("Hold left click and drag your mouse to shoot the ball.\nClear all the red pegs to proceed.\n\nMash + and - to change the music volume, or R to randomize it.\nGood luck.", 11037f));
        }

    }
    private void OnEnable()
    {
        GameController.OnScoreUpdated += UpdateScore;
        GameController.MercyTime += HandleMercy;
        GameController.GameOver += HandleGameOver;
        BallControl.OnShot += DisableTutorial;
    }

    private void OnDisable()
    {
        GameController.OnScoreUpdated -= UpdateScore;
        GameController.MercyTime -= HandleMercy;
        GameController.GameOver -= HandleGameOver;
        BallControl.OnShot -= DisableTutorial;
    }

    private void DisableTutorial()
    {
        if (!SaveManager.Instance.runtimeData.seenTutorial)
        {
            SaveManager.Instance.runtimeData.seenTutorial = true;
            StartCoroutine(ShowInfoText("", 0f));
        }
        BallControl.OnShot -= DisableTutorial;
    }

    private void HandleGameOver(GameOverType type)
    {
        string msg = "";
        var levelData = SaveManager.Instance.GetLevelSave(SaveManager.Instance.runtimeData.currentLevel.levelID);
        if (currentScore > levelData.score && levelData.score > 0)
        {
            msg += $"New high score! Previous: {levelData.score}\n";
        }
        else if (levelData.score > 0)
        {
            msg += $"Current high score is still {levelData.score}...\n";
        }
        switch (type)
        {
            case GameOverType.OutOfBalls:
                msg += "Game over...\nRan out of balls.\nPress SPACE to retry...";
                break;
            case GameOverType.GotAllRed:
                msg += "Congratulations!\nGot all reds!\nPress SPACE to continue.";
                break;
            case GameOverType.GotAll:
                msg += "Amazing!\nLevel cleared!\nPress SPACE to continue.";
                break;
        }
        StartCoroutine(ShowInfoText(msg, 600f));
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
        DOVirtual.Int(currentScore, score, 1f, v => scoreText.text = $"SCORE\n{v:000000000}");
        currentScore = score;
    }
}
