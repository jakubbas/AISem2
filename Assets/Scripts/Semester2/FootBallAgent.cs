using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public enum PlayerState
{
    Defend,
    Pass,
    Run,
    GetOpen,
    GetBall,
    Strike,
    NullState
}

public class FootBallAgent : MovingEntity, IPlayer
{
    //ui
    private TextMesh playerStateText;
    //player variables

    private float kickAimTimerTemp;
    public float kickAimTimer = 1.5f;

    private float defenceSlackPercent = 0.2f;
    private float minDefenceSlackValue = 1.4f;

    //

    public GameObject ballHolder;

    private GameObject ownGoal;
    private GameObject enemyGoal;

    private FootBallAgent markAgent;

    public GameObject ball;
    private bool hasBall = false;

    [SerializeField] private float kickPower;
    [SerializeField] private int teamNumber;

    private Rigidbody2D rb;

    private float rotation;
    public GameObject arrowPoint;

    private Vector2 posToLookAt;

    bool m_Attacking;

    private PlayerState currentPlayerState;

    public TeamManager currentTeamManager;
    //

    SteeringBehaviour_Manager m_SteeringBehaviours;
    SteeringBehaviour_Seek m_Seek;
    SteeringBehaviour_Pursuit m_Pursuit;
    SteeringBehaviour_Arrive m_Arrive;
    SteeringBehaviour_Evade m_Evade;

    public float detectionRadius;
    private bool playerWithinRadius = false;

    //Interfaces
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

    public void SetMarkAgent(FootBallAgent newMark)
    {
        markAgent = newMark;
        Debug.Log("New mark: " + markAgent.name);
    }

    public GameObject GetBallHolder()
    {
        return ballHolder;
    }

    public void AssignState(PlayerState playerState)
    {
        currentPlayerState = playerState;
    }

    public void AssignManagerToPlayer(TeamManager manager)
    {
        currentTeamManager = manager;
    }

    //End

    protected override void Awake()
    {
        base.Awake();
        SwitchPlayerState(PlayerState.NullState);

        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();
        m_Pursuit = GetComponent<SteeringBehaviour_Pursuit>();
        m_Arrive = GetComponent<SteeringBehaviour_Arrive>();
        //m_Evade = GetComponent<SteeringBehaviour_Evade>();
        m_Seek = GetComponent<SteeringBehaviour_Seek>();

        rb = GetComponent<Rigidbody2D>();

        playerStateText = this.GetComponentInChildren<TextMesh>();


        if (!m_SteeringBehaviours)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
        if (!m_Pursuit)
            Debug.LogError("Object doesn't have a Steering Behaviour Pursuit attached", this);
        if (!m_Arrive)
            Debug.LogError("Object doesn't have a Steering Behaviour Arrive attached", this);
        if (!m_Seek)
            Debug.LogError("Object doesn't have a Steering Behaviour Seek attached", this);

        kickAimTimerTemp = kickAimTimer;
    }

    protected void Start()
    {
        foreach (GameObject goal in GameObject.FindGameObjectsWithTag("Goal"))
        {
            if (goal.TryGetComponent(out IGoalNet IGoalNet))
            {
                if (IGoalNet.GetGoalNetType().teamNumber == teamNumber)
                {
                    ownGoal = goal;
                }
                else
                {
                    enemyGoal = goal;
                }
            }
        }
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
            case PlayerState.NullState:
                NullState();
                break;
            default:
                break;
        }

        arrowPoint.transform.rotation = Quaternion.Euler(new Vector3(0, 0, GetAngleLookAt() - 90));
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, GetAngleLookAt() - 90));


        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Animator.SetTrigger("Attack");
            m_Attacking = true;
        }

    }

    protected override Vector2 GenerateVelocity()
    {
        return m_SteeringBehaviours.GenerateSteeringForce();
    }

    void NullState()
    {
        //Used for debugging so I know when an agent doesn't have a task.
        Debug.LogError(this.gameObject.name + " has an unassigned task.");
    }

    void GetBall()
    {
        LookAtDirection(ball.transform.position);
        //Tells the agent to run at the ball to get possession.
        ArriveToPosition((Vector2)ball.transform.position);
        if (hasBall)
        {
            DisableAllMovement();
            SwitchPlayerState(PlayerState.NullState);
            //TELL TEAMMANAGER HERE THAT YOU HAVE THE BALL.

        }
    }

    void Defend()
    {
        posToLookAt = markAgent.transform.position;

        Vector2 goToPos;
        goToPos = ownGoal.transform.position - markAgent.transform.position;
        goToPos = goToPos * defenceSlackPercent;
        if (Maths.Magnitude(goToPos) < minDefenceSlackValue)
        {
            Debug.Log("ehrhere");
            Debug.Log(Maths.Magnitude(goToPos));
        }
        goToPos = (Vector2)markAgent.transform.position + goToPos;
        SeekToPosition(goToPos);

        //Seek to position between the enemy player and your goal, and try to stay in that pocket. Slightly in front of the offensive player, will depend on stats.
    }

    void Pass()
    {
        //Find a position to pass to.
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
        if (kickAimTimerTemp == kickAimTimer)
        {
            //Gets the targets from the opponent goal.
            enemyGoal.TryGetComponent(out IGoalNet IGoalNet);
            GameObject[] targets = IGoalNet.GetAimTargets();
            List<GameObject> openTargets = new List<GameObject>();

            //For each target
            for (int i = 0; i < targets.Length; i++)
            {
                //Check if there is nothing between the player and each target.
                if (RaycastIsTargetOpen(enemyGoal, (Vector2)targets[i].transform.position))
                {
                    openTargets.Add(targets[i]);
                }
            }

            //If every target is hidden away.
            if (openTargets.Count == 0)
            {
                Debug.Log("No valid target to strike.");

                //TELL THE MANAGER THAT IT FAILED.
            }

            //If there is an available target, pick a random one and strike.
            else
            {
                int randomIndex;
                randomIndex = Random.Range(0, openTargets.Count);
                LookAtDirection(openTargets[randomIndex].transform.position);

            }
        }

        if (kickAimTimerTemp <= 0)
        {
            Kick((posToLookAt - (Vector2)transform.position), kickPower * 1.5f);
            kickAimTimerTemp = kickAimTimer;

            //TELL TEAMMANAGER HERE THAT YOU KICKED THE BALL.

        }

        kickAimTimerTemp -= Time.deltaTime;


    }

    bool RaycastIsTargetOpen(GameObject expectedHit, Vector2 direction)
    {
        RaycastHit2D hit;
        //Raycasts to the target, and if the raycast hits the target, returns true. Works on gameplay layer only.
        hit = Physics2D.BoxCast((Vector2)transform.position, new Vector2(1, 1), 0, (direction - (Vector2)transform.position), LayerMask.GetMask("Gameplay"));
        Debug.DrawRay((Vector2)transform.position, (direction - (Vector2)transform.position).normalized * 1000, Color.red, 3, false);
        if (hit.transform.gameObject == expectedHit)
        {
            return true;
        }


        Debug.Log("Strike obstructed by: " + hit.transform.gameObject.name);
        return false;
    }

    void DisableAllMovement()
    {
        m_Seek.m_Active = false;
        m_Pursuit.m_Active = false;
        m_Arrive.m_Active=false;
    }

    void SeekToPosition(Vector2 position)
    {
        //Actually handles seeking to specific locations. Other states will call this.
        m_Seek.m_TargetPosition = position;
        m_Seek.m_Active = true;
        if (Maths.Magnitude((Vector2)transform.position - m_Seek.m_TargetPosition) < 0.1f)
        {
            m_Seek.m_Active = false;
        }    

    }
    void ArriveToPosition(Vector2 position)
    {
        //Actually handles seeking to specific locations. Other states will call this.
        m_Arrive.m_TargetPosition = position;
        m_Arrive.m_Active = true;
        if (Maths.Magnitude((Vector2)transform.position - m_Arrive.m_TargetPosition) < 0.1f)
        {
            m_Arrive.m_Active = false;
        }

    }

    void LookAtDirection(Vector2 direction)
    {
        //Determines what the player is looking at.
        //Consider changing system to have this determine kick direction, not the player location.
        posToLookAt = (Vector2)direction;
    }

    void Kick(Vector2 direction,float kickPower)
    {
        //Only handles kicking the ball in a provided direction and power.
        if (ball.TryGetComponent(out IBall IBall))
        {
            IBall.KickBall(direction, kickPower, this.gameObject);
            hasBall = false;
        }
    }

    private void SwitchPlayerState(PlayerState newState)
    {
        currentPlayerState = newState;
    }


}
