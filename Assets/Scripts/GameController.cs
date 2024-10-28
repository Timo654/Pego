using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] int balls = 5;
    public static event Action NewTurn;
    private int score;

    private void OnEnable()
    {
        BallControl.OnShot += RemoveBall;
        BallControl.OnBallLost += HandleTurnEnd;
        BasketMove.OnBallCaught += AddBall;
        PegObject.OnPegPopped += AddScore;
    }

    private void HandleTurnEnd()
    {
        if (balls == 0)
        {
            Debug.Log($"GAME OVER!!! Score: {score}");
        }
        else
        {
            NewTurn?.Invoke();
        }
    }

    private void AddScore(PegType type)
    {
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
    }

    private void AddBall()
    {
        balls++;
    }

    private void RemoveBall()
    {
        balls--;
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

    }

    // Update is called once per frame
    void Update()
    {

    }
}
