using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Ball : MonoBehaviour, IBall
{
    private List<TeamManager> teamManagers = new List<TeamManager>();

    private int possessingTeam = -1;

    private Rigidbody2D rb;

    private float bouncePower = 50f;

    private GameObject lastPlayerToTouch;

    [HideInInspector] public GameObject possessingPlayer;

    public bool isPossessed = false;

    // Start is called before the first frame update
    void Start()
    {
        possessingTeam = -1;
        rb = this.GetComponent<Rigidbody2D>();
    }

    public void KickBall(Vector2 direction, float power, GameObject lastTouchPlayer)
    {
        possessingPlayer = null;
        isPossessed = false;
        lastPlayerToTouch = lastTouchPlayer;
        Vector2 shotVector;
        shotVector = Maths.Normalise(direction);
        shotVector *= power;
        rb.AddForce(shotVector);
        this.transform.parent = null;
        StartCoroutine(BallKicked());
    }

    IEnumerator BallKicked()
    {
        yield return new WaitForSeconds(2);

        if (!isPossessed)
        {
            for (int i = 0; i < teamManagers.Count; i++)
            {
                teamManagers[i].BallIsFree();
            }
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            lastPlayerToTouch = collision.gameObject;
        }
        //rb.velocity = Vector2.zero;
        Vector2 bounceVector;
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
            if (!isPossessed)
            {
                isPossessed = true;
                this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                CaptureBall(collision.gameObject);
            }
        }
    }


    public void CaptureBall(GameObject player)
    {
        lastPlayerToTouch = player;
        player.TryGetComponent(out IPlayer IPlayer);
        IPlayer.SetBallPossessed();
        possessingTeam = IPlayer.GetFootBallAgentTeamID();
        this.transform.parent = IPlayer.GetBallHolder().transform;
    }

    public Ball GetBallType()
    {
        return this;
    }

    public void SetTeamManagers(TeamManager manager)
    {
        teamManagers.Add(manager);
    }

    public FootBallAgent GetBallHandler()
    {

        if (possessingPlayer == null)
        {
            return null;
        }

        possessingPlayer.TryGetComponent<FootBallAgent>(out FootBallAgent agent);

        return agent;
    }

}
