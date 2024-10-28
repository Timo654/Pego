using System;
using UnityEngine;
// https://www.youtube.com/watch?v=RpeRnlLgmv8
public class BallControl : MonoBehaviour
{
    public static event Action OnBallLost;
    public static event Action OnShot;
    public float power = 5f;
    private Rigidbody2D rb;
    private LineRenderer lr;
    Vector2 dragStartPos;
    Vector2 referencePos;
    private bool isShooting = false;
    private bool isGone = false;
    private bool hasBeenVisible = false;
    private Vector3 defaultScale;
    private void Start()
    {
        referencePos = transform.position;
        defaultScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        rb.isKinematic = true;
    }

    private void OnEnable()
    {
        GameController.NewTurn += ResetBall;
        GameController.SpecialMode += TurnBig;
    }

    private void TurnBig(bool enabled)
    {
        if (enabled)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.localScale = defaultScale;
        }
    }

    private void OnDisable()
    {
        GameController.NewTurn -= ResetBall;
        GameController.SpecialMode -= TurnBig;
    }
    private void Update()
    {
        if (!hasBeenVisible && transform.GetChild(0).GetComponent<SpriteRenderer>().isVisible)
        {
            hasBeenVisible = true;
        }
        if (!isShooting)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {

                Vector2 dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 _velocity = (dragEndPos - dragStartPos) * power;
                Vector2[] trajectory = Plot(rb, (Vector2)transform.position, _velocity, 50, 10);
                lr.positionCount = trajectory.Length;
                Vector3[] positions = new Vector3[trajectory.Length];
                for (int i = 0; i < trajectory.Length; i++)
                {
                    positions[i] = trajectory[i];
                }
                lr.SetPositions(positions);
            }

            if (Input.GetMouseButtonUp(0))
            {
                rb.isKinematic = false;
                isShooting = true;
                Vector2 dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 _velocity = (dragEndPos - dragStartPos) * power;
                rb.velocity = _velocity;
                lr.positionCount = 0;
                OnShot?.Invoke();
            }
        }

        if (!isGone && hasBeenVisible && !transform.GetChild(0).GetComponent<SpriteRenderer>().isVisible)
        {
            Debug.Log("gone!");
            isGone = true; // so we don't spam this all the time
            OnBallLost?.Invoke();
        }
    }

    private void ResetBall()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        transform.position = referencePos;
        isShooting = false;
        isGone = false;
    }
    public Vector2[] Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 velocity, int steps, float stepDistance)
    {
        Vector2[] results = new Vector2[steps];
        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations * stepDistance;
        Vector2 gravityAccel = rigidbody.gravityScale * timestep * timestep * Physics2D.gravity;
        float drag = 1f - timestep * rigidbody.drag;
        Vector2 moveStep = velocity * timestep;

        for (int i = 0; i < steps; i++)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            results[i] = pos;
        }
        return results;
    }
}
