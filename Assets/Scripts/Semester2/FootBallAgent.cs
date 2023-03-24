using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootBallAgent : MovingEntity
{
    //
    private GameObject ownGoal;
    private GameObject enemyGoal;


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


    private TeamState currentTeamState;
    private enum TeamState
    {
        Defense,
        Attack
    }

    void Defense()
    {
        //When the enemy team has possession of the ball.
    }

    void Attack()
    {
        //When the player team gets possession of the ball.

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
        //Choose one of the 4 spots on the goal which aren't covered by an enemy player. Position yourself behind the ball in that direction, and kick().
    }

    void GetBall()
    {
        //When neither team has possession of the ball, run to get it.
    }

    void GetBehindBall()
    {
        //When the player gets possession of the ball, position yourself so that the ball is closer towards the enemy goal than the player.
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

    protected override void Awake()
    {
        base.Awake();

        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();
        m_Pursuit = GetComponent<SteeringBehaviour_Pursuit>();
        m_Arrive = GetComponent<SteeringBehaviour_Arrive>();
        //m_Evade = GetComponent<SteeringBehaviour_Evade>();
        m_Seek = GetComponent<SteeringBehaviour_Seek>();

        rb = GetComponent<Rigidbody2D>();
        arrowPoint = GameObject.Find("ArrowPoint");


        if (!m_SteeringBehaviours)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
        if (!m_Pursuit)
            Debug.LogError("Object doesn't have a Steering Behaviour Pursuit attached", this);
        if (!m_Arrive)
            Debug.LogError("Object doesn't have a Steering Behaviour Arrive attached", this);
        if (!m_Evade)
            Debug.LogError("Object doesn't have a Steering Behaviour Evade attached", this);
        if (!m_Seek)
            Debug.LogError("Object doesn't have a Steering Behaviour Seek attached", this);




    }

    protected void Start()
    {

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

        //m_Evade.m_EvadingEntity = GameObject.Find("Player").GetComponent<MovingEntity>();
        m_Pursuit.m_PursuingEntity = GameObject.Find("Player").GetComponent<MovingEntity>();
        m_Evade.m_Active = false;
        m_Pursuit.m_Active = false;
        m_Seek.m_Active = false;

    }
    // Update is called once per frame

    float getAngleLookAt()
    {
        Vector2 tempPosToLookAt = posToLookAt;

        tempPosToLookAt -= (Vector2)transform.position;
        float angle = Mathf.Atan2(tempPosToLookAt.y, tempPosToLookAt.x) * Mathf.Rad2Deg;
        return angle;

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        posToLookAt = (Vector2)GameObject.Find("Ball").transform.position;

        arrowPoint.transform.rotation = Quaternion.Euler(new Vector3(0, 0, getAngleLookAt() - 90));
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, getAngleLookAt() - 90));


        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Animator.SetTrigger("Attack");
            m_Attacking = true;
        }

        DetectionCheck();


    }

    private void DetectionCheck()
    {
        float distanceToPlayer;

        distanceToPlayer = Mathf.Abs(Maths.Magnitude((Vector2)transform.position - (Vector2)m_Pursuit.m_PursuingEntity.gameObject.transform.position));

        if (distanceToPlayer <= detectionRadius)
        {
            playerWithinRadius = true;
        }

        else
        {
            playerWithinRadius = false;
        }
    }

    protected override Vector2 GenerateVelocity()
    {
        return m_SteeringBehaviours.GenerateSteeringForce();
    }

    private void SwitchTeamState(TeamState newState)
    {
        currentTeamState = newState;

        switch (currentTeamState)
        {
            case TeamState.Attack:
                break;
            case TeamState.Defense:
                break;
            default:
                break;

        }
    }

    private void SwitchPlayerState(PlayerState newState)
    {
        currentPlayerState = newState;

        switch (currentPlayerState)
        {
            case PlayerState.Defend:
                break;
            case PlayerState.Strike:
                break;
            case PlayerState.GetOpen:
                break;
            case PlayerState.GetBall:
                break;
            case PlayerState.Pass:
                break;
            case PlayerState.Run:
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IBall ball))
        {
            ball.KickBall(-(Vector2)transform.position + (Vector2)collision.gameObject.transform.position, kickPower, gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
  
    }

}
