using System;
using UnityEngine;
// https://www.youtube.com/watch?v=RpeRnlLgmv8
public class BallControl : MonoBehaviour
{
    public static event Action OnBallLost;
    public float power = 5f;
    private Rigidbody2D rb;
    private LineRenderer lr;
    Vector2 dragStartPos;
    Vector2 referencePos;
    private bool isShooting = false;
    private void Start()
    {
        referencePos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        rb.isKinematic = true;
    }

    private void Update()
    {
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
            }
        }
        
        if (!transform.GetChild(0).GetComponent<SpriteRenderer>().isVisible)
        {
            ResetBall();
        }
    }

    private void ResetBall()
    {
        Debug.Log("gone!");
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        transform.position = referencePos;
        OnBallLost?.Invoke();
        isShooting = false;
    }
    public Vector2[] Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 velocity, int steps, float stepDistance)
    {
        Vector2[] results = new Vector2[steps];
        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations * stepDistance;
        Vector2 gravityAccel = Physics2D.gravity * rigidbody.gravityScale * timestep * timestep;
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
