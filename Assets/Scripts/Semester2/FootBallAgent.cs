using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootBallAgent : MovingEntity, IPlayer
{
    //ui
    private TextMesh playerStateText;
    //timers

    private float timer;

    public float kickAimTimer = 1.5f;

    //

    private GameObject ownGoal;
    private GameObject enemyGoal;
    public GameObject ball;
    private bool hasBall = false;

    [SerializeField] private float kickPower;
    [SerializeField] private int teamNumber;

    private Rigidbody2D rb;

    private float rotation;
    private GameObject arrowPoint;

    private Vector2 posToLookAt;

    bool m_Attacking;


    //

    SteeringBehaviour_Manager m_SteeringBehaviours;
    SteeringBehaviour_Seek m_Seek;
    SteeringBehaviour_Pursuit m_Pursuit;
    SteeringBehaviour_Arrive m_Arrive;
    SteeringBehaviour_Evade m_Evade;

    public float detectionRadius;
    private bool playerWithinRadius = false;


    public void SetBallPossessed()
    {
        hasBall = true;
    }

    public FootBallAgent GetFootBallAgentType()
    {
        return this;
    }

    public int GetFootBallAgentTeamID()
    {
        return teamNumber;
    }


    protected override void Awake()
    {
        base.Awake();
        SwitchPlayerState(PlayerState.GetBall);

        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();
        m_Pursuit = GetComponent<SteeringBehaviour_Pursuit>();
        m_Arrive = GetComponent<SteeringBehaviour_Arrive>();
        //m_Evade = GetComponent<SteeringBehaviour_Evade>();
        m_Seek = GetComponent<SteeringBehaviour_Seek>();

        rb = GetComponent<Rigidbody2D>();
        arrowPoint = GameObject.Find("ArrowPoint");

        playerStateText = this.GetComponentInChildren<TextMesh>();


        if (!m_SteeringBehaviours)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
        if (!m_Pursuit)
            Debug.LogError("Object doesn't have a Steering Behaviour Pursuit attached", this);
        if (!m_Arrive)
            Debug.LogError("Object doesn't have a Steering Behaviour Arrive attached", this);
        if (!m_Seek)
            Debug.LogError("Object doesn't have a Steering Behaviour Seek attached", this);

        timer = kickAimTimer;


    }

    protected void Start()
    {
        //ball = GameObject.FindGameObjectWithTag("Ball");
        posToLookAt = ball.transform.position;

        foreach (GameObject goal in GameObject.FindGameObjectsWithTag("Goal"))
        {
            if (goal.TryGetComponent(out GoalNet net))
            {
                if (net.goalTeamNumber == teamNumber)
                {
                    ownGoal = goal;
                }
                else
                {
                    enemyGoal = goal;
                }
            }
        }
        m_Evade.m_Active = false;
        m_Pursuit.m_Active = false;
        m_Seek.m_Active = false;



    }
    // Update is called once per frame

    float GetAngleLookAt()
    {
        Vector2 tempPosToLookAt = posToLookAt;
        tempPosToLookAt -= (Vector2)transform.position;
        float angle = Mathf.Atan2(tempPosToLookAt.y, tempPosToLookAt.x) * Mathf.Rad2Deg;
        return angle;

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        playerStateText.text = currentPlayerState.ToString();
        switch (currentPlayerState)
        {
            case PlayerState.Defend:
                Defend();
                break;
            case PlayerState.Strike:
                Strike();
                break;
            case PlayerState.GetOpen:
                GetOpen();
                break;
            case PlayerState.GetBall:
                GetBall();
                break;
            case PlayerState.Pass:
                Pass();
                break;
            case PlayerState.Run:
                Run();
                break;
            default:
                break;
        }

        arrowPoint.transform.rotation = Quaternion.Euler(new Vector3(0, 0, GetAngleLookAt() - 90));
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, GetAngleLookAt() - 90));


        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Animator.SetTrigger("Attack");
            m_Attacking = true;
        }

        //DetectionCheck();


    }

    //private void DetectionCheck()
    //{
    //    float distanceToPlayer;

    //    distanceToPlayer = Mathf.Abs(Maths.Magnitude((Vector2)transform.position - (Vector2)m_Pursuit.m_PursuingEntity.gameObject.transform.position));

    //    if (distanceToPlayer <= detectionRadius)
    //    {
    //        playerWithinRadius = true;
    //    }

    //    else
    //    {
    //        playerWithinRadius = false;
    //    }
    //}

    protected override Vector2 GenerateVelocity()
    {
        return m_SteeringBehaviours.GenerateSteeringForce();
    }

    private PlayerState currentPlayerState;
    private enum PlayerState
    {
        Defend,
        Pass,
        Run,
        GetOpen,
        GetBall,
        Strike
    }

    void Defend()
    {
        //Seek to position between the enemy player and your goal, and try to stay in that pocket. Slightly in front of the offensive player, will depend on stats.
    }

    void Pass()
    {
        //Find a position to pass to.
        //Move the behind the ball on the same line as that position, and then Kick().
        //Fuzzy Logic : Openness and Distance and HowCloseToGoal and TeammateSpeed.
        //Will need a reference to the teammate and their defender.

    }
    void Run()
    {
        //When your teammate has the ball, run towards the enemy goal. Once close enough, either the ball is passed or GetOpen();
        //Random position in semi-circle coming from the goal. The radius of the semi-circle will depend on kick power.
    }

    void GetOpen()
    {
        //Randomly move left and right to lose the defender.
    }

    void Strike()
    {
        //Actually kicking the ball at a goal to score.
        LookAtDirection(enemyGoal.transform.position);
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Kick(posToLookAt, kickPower * 1.5f);
            SwitchPlayerState(PlayerState.Run);
            timer = kickAimTimer;
        }
        //Choose one of the 4 spots on the goal which aren't covered by an enemy player. Position yourself behind the ball in that direction, and kick().
    }

    void GetBall()
    {
        //Tells the agent to run at the ball to get possession.
        m_Seek.m_Active = true;
        m_Seek.m_TargetPosition = (Vector2)ball.transform.position;
        if (hasBall)
        {
            Debug.Log("HAS BALL");
            m_Seek.m_Active = false;
            SwitchPlayerState(PlayerState.Strike);
        }
    }

    void MoveToPosition(Vector2 position)
    {
        //Actually handles seeking to specific locations. Other states will call this.

    }

    void LookAtDirection(Vector2 direction)
    {
        //Determines what the player is looking at.
        //Consider changing system to have this determine kick direction, not the player location.
        posToLookAt = direction;
    }

    void Kick(Vector2 direction,float kickPower)
    {
        //Only handles kicking the ball in a provided direction and power.
        if (ball.TryGetComponent(out IBall ballref))
        {
            ballref.KickBall(direction, kickPower, this.gameObject);
            hasBall = false;
        }
    }

    private void SwitchPlayerState(PlayerState newState)
    {
        currentPlayerState = newState;
    }


}
