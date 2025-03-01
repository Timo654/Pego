using DG.Tweening;
using System;
using System.Collections;
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
    private bool hasShot = false;
    private bool isGone = false;
    private bool hasBeenVisible = false;
    private Vector3 defaultScale;
    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        referencePos = transform.position;
        defaultScale = transform.localScale;
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnEnable()
    {
        GameController.NewTurn += ResetBall;
        GameController.SpecialMode += TurnBig;
        BasketMove.OnBallCaught += TurnInvisible;
    }

    private void TurnInvisible()
    {
        spriteRenderer.DOFade(0f, 0.1f);
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
        BasketMove.OnBallCaught -= TurnInvisible;
    }
    private void Update()
    {
        if (!hasBeenVisible && transform.GetChild(0).GetComponent<SpriteRenderer>().isVisible)
        {
            hasBeenVisible = true;
        }
        if (!hasShot)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {

                Vector2 dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 _velocity = Vector2.ClampMagnitude((dragEndPos - dragStartPos) * power, 15f);
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
                rb.bodyType = RigidbodyType2D.Dynamic;
                hasShot = true;
                Vector2 dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 _velocity = Vector2.ClampMagnitude((dragEndPos - dragStartPos) * power, 15f);
                rb.linearVelocity = _velocity;
                lr.positionCount = 0;
                rb.constraints = RigidbodyConstraints2D.None;
                OnShot?.Invoke();
            }
        }

        if (!isGone && hasBeenVisible && !transform.GetChild(0).GetComponent<SpriteRenderer>().isVisible)
        {
            isGone = true; // so we don't spam this all the time
            StartCoroutine(DelayBallLoss());
        }
    }

    IEnumerator DelayBallLoss()
    {
        yield return new WaitForSeconds(0.2f);
        OnBallLost?.Invoke();
    }
    private void ResetBall()
    {
        spriteRenderer.DOFade(0f, 0f);
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.rotation = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.position = referencePos;
        isGone = false;
        spriteRenderer.DOFade(1f, 0.2f).OnComplete(() => hasShot = false);
    }
    public Vector2[] Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 velocity, int steps, float stepDistance)
    {
        Vector2[] results = new Vector2[steps];
        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations * stepDistance;
        Vector2 gravityAccel = rigidbody.gravityScale * timestep * timestep * Physics2D.gravity;
        float drag = 1f - timestep * rigidbody.linearDamping;
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
