using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Ball : MonoBehaviour, IBall
{
    private Rigidbody2D rb;

    private float bouncePower = 50f;

    private GameObject lastPlayerToTouch;

    [HideInInspector] public GameObject possessingPlayer;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    public void KickBall(Vector2 direction, float power, GameObject lastTouchPlayer)
    {
        possessingPlayer = null;
        lastPlayerToTouch = lastTouchPlayer;
        Vector2 shotVector;
        shotVector = Maths.Normalise(direction);
        shotVector *= power;
        rb.AddForce(shotVector);
        this.transform.parent = null;

    }

    public void GoalScored(int team)
    {
        Debug.Log("Goal Scored by : " + lastPlayerToTouch);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            lastPlayerToTouch = collision.gameObject;
        }
        //rb.velocity = Vector2.zero;
        Vector2 bounceVector;
        Debug.Log(collision.gameObject.transform);
        float tempBouncePower = bouncePower;
        if (!collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = Vector2.zero;
            bounceVector = (-(Vector2)collision.gameObject.transform.position + (Vector2)gameObject.transform.position) * tempBouncePower;
            rb.AddForce(bounceVector);
        }


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (possessingPlayer == null)
            {
                CaptureBall(collision.gameObject);
            }
        }
    }


    public void CaptureBall(GameObject player)
    {
        lastPlayerToTouch = player;
        player.TryGetComponent(out IPlayer ballref);
        ballref.SetBallPossessed();
        this.transform.parent = player.transform;
    }


}
