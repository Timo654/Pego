using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] int balls = 5;
    public static event Action NewTurn;
    public static event Action<int> OnBallsUpdated;
    public static event Action<int> OnScoreUpdated;
    public static event Action<bool> MercyTime;
    public static event Action GameOver;
    private int score;
    private bool hasHitDuringRound = false;
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
            Debug.Log($"GAME OVER!!! Score: {score}");
            GameOver?.Invoke();
        }
        else
        {
            NewTurn?.Invoke();
        }
        hasHitDuringRound = false;
    }

    private void AddScore(PegType type)
    {
        hasHitDuringRound = true;
        switch (type)
        {
            case PegType.Normal:
                score += 50;
                break;
            case PegType.Bonus:
                score += 200;
                break;
            case PegType.Special:
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
    // Start is called before the first frame update
    void Start()
    {
        // reset just in case
        OnBallsUpdated?.Invoke(balls);
        OnScoreUpdated?.Invoke(score);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
