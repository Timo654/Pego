using System;
using UnityEngine;

public class BasketMove : MonoBehaviour
{
    // Start is called before the first frame update
    private float speed = 2f;
    public static event Action OnBallCaught;
    private Vector2 leftTarget;
    private Vector2 rightTarget;
    private bool goingLeft;
    void Start()
    {
        var hitLeft = Physics2D.Raycast(transform.position, Vector2.left);
        var hitRight = Physics2D.Raycast(transform.position, Vector2.right);
        leftTarget = new Vector2(hitLeft.point.x + 0.3f, transform.position.y);
        rightTarget = new Vector2(hitRight.point.x - 1.2f, transform.position.y);
        var colliders = GetComponents<BoxCollider2D>();
        foreach (var collider in colliders)
            collider.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (goingLeft)
        {
            transform.position = Vector3.MoveTowards(transform.position, leftTarget, speed * Time.deltaTime);
            if (transform.position.x <= leftTarget.x + 0.2f) goingLeft = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, rightTarget, speed * Time.deltaTime);
            if (transform.position.x >= rightTarget.x - 0.2f) goingLeft = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name + " has entered");
        if (collision.name.ToLower() == "ball")
        {
            OnBallCaught?.Invoke();
        }
    }
}
