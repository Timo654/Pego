using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHolder : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    private List<GameObject> balls = new();
    // Start is called before the first frame update
    private void Awake()
    {
        foreach (Transform item in transform)
        {
            balls.Add(item.gameObject);
            item.gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        GameController.OnBallsUpdated += HandleBalls;
    }

    private void OnDisable()
    {
        GameController.OnBallsUpdated -= HandleBalls;
    }

    IEnumerator SpawnBalls(int ballCount)
    {
        for (int i = 0; i < balls.Count; i++)
        {
            int currentBall = balls.Count - i - 1;
            if (currentBall >= ballCount)
            {
                balls[currentBall].SetActive(false);
            }
            else
            {
                if (balls[currentBall].activeSelf) continue; // ignore if spawned in already
                balls[currentBall].transform.position = new Vector2(transform.position.x, transform.position.y + 200f);
                balls[currentBall].SetActive(true);
                yield return new WaitForSeconds(0.3f);
            }
        }

    }
    private void HandleBalls(int ballCount)
    {
        if (ballCount > balls.Count)
        {
            var currentBalls = balls.Count;
            for (int i = 0; i < ballCount - currentBalls; i++)
            {
                GameObject ball = Instantiate(ballPrefab, transform);
                ball.SetActive(false);
                balls.Add(ball);
            }
        }

        StartCoroutine(SpawnBalls(ballCount));
    }
}
