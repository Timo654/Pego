using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] Transform pegs;
    [SerializeField] SpriteRenderer background;
    private int balls;
    private int bonusCount = 1; // can always be only one
    public static event Action NewTurn;
    public static Action<bool> SpecialMode;
    public static event Action<int> OnBallsUpdated;
    public static event Action<int> OnScoreUpdated;
    public static event Action<bool> MercyTime;
    public static event Action<GameOverType> GameOver;
    private int score;
    private bool hasHitDuringRound = false;
    private PegObject currentBonus;

    private int pegsLeft;
    private int redsLeft;
    private int specialRounds = 0;
    private bool gameOver = false;
    private bool isBadEnd = false;
    private int combo;
    private int roundScore;
    private void OnEnable()
    {
        BallControl.OnShot += RemoveBall;
        BallControl.OnBallLost += HandleTurnEnd;
        BasketMove.OnBallCaught += BasketBall;
        PegObject.OnPegPopped += AddScore;
        LevelChanger.OnGameplayLevelLoaded += SetupGame;
        GameOver += SetGameOver;
    }

    private void BasketBall()
    {
        AddBall(true);
    }

    private void SetGameOver(GameOverType type)
    {
        isBadEnd = type == GameOverType.OutOfBalls;
        gameOver = true;
        StartCoroutine(DelayScoreSet());
    }

    IEnumerator DelayScoreSet()
    {
        yield return null;
        var currentLevel = SaveManager.Instance.runtimeData.currentLevel;
        var levelSave = SaveManager.Instance.GetLevelSave(currentLevel.levelID);
        if (score > levelSave.score)
        {
            levelSave.score = score;
        }
    }
    private void HandleTurnEnd()
    {
        if (!hasHitDuringRound) // random mercy chance
        {
            bool hasMercy = UnityEngine.Random.value > 0.5f;
            if (hasMercy)
            {
                AddBall();
            }
            MercyTime?.Invoke(hasMercy);
        }


        if (redsLeft == 0) // could also do with red?
        {
            AddScoreForBalls();
            if (pegsLeft == 0) GameOver?.Invoke(GameOverType.GotAll);
            else GameOver?.Invoke(GameOverType.GotAllRed);
        }
        else if (balls == 0)
        {
            Debug.Log($"GAME OVER!!! Score: {score}, red {redsLeft}, all {pegsLeft}");
            GameOver?.Invoke(GameOverType.OutOfBalls);
        }
        else
        {
            NewTurn?.Invoke();
            if (specialRounds > 0)
            {
                specialRounds--;
                if (specialRounds == 0) SpecialMode?.Invoke(false);
            }
            hasHitDuringRound = false;
            combo = 0;
            roundScore = 0;
            RandomizeBonus();
        }
    }

    private void AddScoreForBalls()
    {
        score += balls * 200;
        OnScoreUpdated?.Invoke(score);
    }
    private void AddScore(PegType type)
    {
        hasHitDuringRound = true;
        combo++;
        pegsLeft--;
        var multiplier = combo / 5;
        int currentHit = 0;
        switch (type)
        {
            case PegType.Normal:
                currentHit += 25;
                break;
            case PegType.Red:
                redsLeft--;
                currentHit += 75;
                combo += 2;
                break;
            case PegType.Bonus:
                currentHit += 150;
                combo += 3;
                break;
            case PegType.Special:
                // we only have one special: big mode
                specialRounds += 3;
                combo++;
                SpecialMode?.Invoke(true);
                currentHit += 50;
                break;
        }
        currentHit = currentHit + currentHit * combo;
        score += currentHit;
        roundScore += currentHit;
        if (roundScore > 15000)
        {
            roundScore = 0;
            AddBall(); // free ball for good score
        }
        OnScoreUpdated?.Invoke(score);
    }

    private void AddBall(bool isBasket = false)
    {
        if (isBasket)
        {
            hasHitDuringRound = true; // basket
            score += 10000;
            OnScoreUpdated?.Invoke(score);
        }
        balls++;
        OnBallsUpdated?.Invoke(balls);
    }

    private void RemoveBall()
    {
        balls--;
        OnBallsUpdated?.Invoke(balls);
    }

    private void OnDisable()
    {
        BallControl.OnShot -= RemoveBall;
        BallControl.OnBallLost -= HandleTurnEnd;
        BasketMove.OnBallCaught -= BasketBall;
        PegObject.OnPegPopped -= AddScore;
        LevelChanger.OnGameplayLevelLoaded -= SetupGame;
        GameOver -= SetGameOver;
    }

    private void RandomizePeg(int pegCount, PegType pegType)
    {
        int failedAttempts = 0;
        for (int i = 0; i < pegCount; i++)
        {
            var pegScript = pegs.GetChild(UnityEngine.Random.Range(0, pegs.childCount)).GetComponent<PegObject>();
            if (pegScript.GetPegType() == PegType.Normal)
            {
                pegScript.SetPegType(pegType);
                failedAttempts = 0;
            }
            else
            {
                failedAttempts++;
                if (failedAttempts > 10) break; // just give up, we dont want to get stuck in an infinite loop
                i--; // try again
                continue;
            }
            if (pegType == PegType.Bonus)
            {
                currentBonus = pegScript;
            }
        }
    }

    private void Start()
    {
        LevelChanger.Instance.HandleLevelLoad();
    }
    private void RandomizeBonus()
    {
        if (currentBonus != null)
            currentBonus.SetPegType(PegType.Normal);
        RandomizePeg(bonusCount, PegType.Bonus);
    }

    private void SetupGame(LevelData levelData)
    {
        background.sprite = levelData.backgroundImage;
        background.color = levelData.backgroundTint;
        balls = levelData.ballCount;
        if (pegs != null)
        {
            Destroy(pegs.gameObject);
        }
        pegs = Instantiate(levelData.levelObjects).transform;
        OnBallsUpdated?.Invoke(balls);
        OnScoreUpdated?.Invoke(score);
        pegsLeft = pegs.childCount;
        redsLeft = pegs.childCount / 2;
        RandomizePeg(redsLeft, PegType.Red);
        RandomizePeg(levelData.specialCount, PegType.Special);
        RandomizeBonus();
    }


    // TODO - use new input system maybe to avoid this mess !!!
    private void Update()
    {
        if (gameOver && Input.GetKeyDown(KeyCode.B))
        {
            var currentLevel = SaveManager.Instance.runtimeData.currentLevel;
            if (!isBadEnd) SaveManager.Instance.runtimeData.currentLevel = LevelChanger.Instance.GetNextLevel(currentLevel);
            gameOver = false;

            if (SaveManager.Instance.runtimeData.currentLevel != null)
            {
                LevelChanger.Instance.FadeToLevel(SceneManager.GetActiveScene().name);
            }
            else
            {
                LevelChanger.Instance.FadeToLevel("EndingScene");
            }
        }
    }
}

public enum GameOverType
{
    OutOfBalls,
    GotAllRed,
    GotAll
}