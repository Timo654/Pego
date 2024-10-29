using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] Transform pegs;
    [SerializeField] SpriteRenderer background;
    private int balls;
    [SerializeField] private LevelData debugLevel;
    private int bonusCount = 1; // can always be only one
    public static event Action NewTurn;
    public static Action<bool> SpecialMode;
    public static event Action<int> OnBallsUpdated;
    public static event Action<int> OnScoreUpdated;
    public static event Action<bool> MercyTime;
    public static event Action GameOver;
    private int score;
    private bool hasHitDuringRound = false;
    private PegObject currentBonus;

    private int pegsLeft;
    private int redsLeft;
    private int specialRounds = 0;
    
    private void OnEnable()
    {
        BallControl.OnShot += RemoveBall;
        BallControl.OnBallLost += HandleTurnEnd;
        BasketMove.OnBallCaught += AddBall;
        PegObject.OnPegPopped += AddScore;
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

        if (balls == 0)
        {
            Debug.Log($"GAME OVER!!! Score: {score}, red {redsLeft}, all {pegsLeft}");
            GameOver?.Invoke();
        }
        else if (pegsLeft == 0) // could also do with red?
        {
            AddScoreForBalls();
            GameOver?.Invoke();
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
        pegsLeft--;
        switch (type)
        {
            case PegType.Normal:
                score += 50;
                break;
            case PegType.Red:
                redsLeft--;
                score += 100;
                break;
            case PegType.Bonus:
                score += 200;
                break;
            case PegType.Special:
                // we only have one special: big mode
                specialRounds += 3;
                SpecialMode?.Invoke(true);
                score += 100;
                break;
        }
        OnScoreUpdated?.Invoke(score);
    }

    private void AddBall()
    {
        hasHitDuringRound = true; // basket
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
        BasketMove.OnBallCaught -= AddBall;
        PegObject.OnPegPopped -= AddScore;
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


    private void RandomizeBonus()
    {
        if (currentBonus != null)
            currentBonus.SetPegType(PegType.Normal);
        RandomizePeg(bonusCount, PegType.Bonus);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupGame(debugLevel);
    }

    private void SetupGame(LevelData levelData)
    {
        background.sprite = levelData.backgroundImage;
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
}
