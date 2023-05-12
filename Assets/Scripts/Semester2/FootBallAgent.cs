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
    Waiting,
    StrikeOrPass,
    NullState
}

public enum Role
{
    Left,
    Middle,
    Right
}

public class FootBallAgent : MovingEntity, IPlayer
{
    //ui
    public TextMesh playerStateText;
    public TextMesh playerPositionText;
    //player variables

    private float kickAimTimerTemp;
    public float kickAimTimer = 1.5f;

    private float passAimTimerTemp;
    public float passAimTimer = 1f;

    private float defenceSlackPercent = 0.35f;
    private float minDefenceSlackValue = 6.5f;

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

    private List<GameObject> mapPoints = new List<GameObject>();

    public GameObject newMapPoint;

    private GameObject currentMapPoint;

    public Role role;

    private bool inStrikeOrPass = false;

    private Vector2 randomOpenOffset;

    public FootBallAgent teammateToPass;
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
        currentTeamManager.BallGained(this);
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
        //Debug.Log("New mark: " + markAgent.name);
    }

    public GameObject GetBallHolder()
    {
        return ballHolder;
    }

    public void AssignState(PlayerState playerState)
    {
        currentTeamManager.waitingPlayers.Remove(this);
        currentPlayerState = playerState;
    }

    public void AssignManagerToPlayer(TeamManager manager)
    {
        currentTeamManager = manager;
    }

    public int GetPositionIndex()
    {
        currentMapPoint.TryGetComponent<MapPoint>(out MapPoint map);
        return map.mapPositionIndex;
    }

    public PlayerState GetPlayerState()
    {
        return currentPlayerState;
    }

    public void ChangeNewMapPoint()
    {
        currentMapPoint.TryGetComponent<MapPoint>(out MapPoint point);
        int currentMapPointArrayIndex = point.mapPositionIndex;

        switch (teamNumber)
        {
            case 0:
                currentMapPointArrayIndex--;
                newMapPoint = mapPoints[currentMapPointArrayIndex];
                if (point.mapPositionIndex - 1 < 0)
                    Debug.LogError("Out of map index.");

                break;
            case 1:
                //GO UP THE MAPPOINTS ARRAY BY ONE.
                currentMapPointArrayIndex++;
                newMapPoint = mapPoints[currentMapPointArrayIndex];
                if (point.mapPositionIndex + 1 > 2)
                    Debug.LogError("Out of map index.");
                break;
        }
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

        if (!m_SteeringBehaviours)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
        if (!m_Pursuit)
            Debug.LogError("Object doesn't have a Steering Behaviour Pursuit attached", this);
        if (!m_Arrive)
            Debug.LogError("Object doesn't have a Steering Behaviour Arrive attached", this);
        if (!m_Seek)
            Debug.LogError("Object doesn't have a Steering Behaviour Seek attached", this);

        kickAimTimerTemp = kickAimTimer;
        passAimTimerTemp = passAimTimer;
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
        FindMapPointsByRole();
        currentMapPoint = FindClosestMapPoint();
    }

    void FindMapPointsByRole()
    {
        GameObject holder;

        switch (role)
        {
            case Role.Left:
                holder = GameObject.Find("MapPointsLeft");
                foreach (Transform child in holder.transform)
                {
                    mapPoints.Add(child.gameObject);

                }
                break;
            case Role.Middle:
                holder = GameObject.Find("MapPointsMiddle");
                foreach (Transform child in holder.transform)
                {
                    mapPoints.Add(child.gameObject);
                }

                //
                break;
            case Role.Right:
                holder = GameObject.Find("MapPointsRight");
                foreach (Transform child in holder.transform)
                {
                    mapPoints.Add(child.gameObject);
                }
                //
                break;
        }
        
    }

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
        playerPositionText.text = GetPositionIndex().ToString();
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
            case PlayerState.Waiting:
                Waiting();
                break;
            case PlayerState.StrikeOrPass:
                StrikeOrPass();
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
        //Debug.LogError(this.gameObject.name + " has an unassigned task.");
    }

    void GetBall()
    {
        GameObject closestMapPoint;
        LookAtDirection(ball.transform.position);
        //Tells the agent to run at the ball to get possession.
        ArriveToPosition((Vector2)ball.transform.position);
        if (hasBall)
        {
            DisableAllMovement();
            closestMapPoint = FindClosestMapPoint();
            ChangeMapPosition(closestMapPoint);
            currentTeamManager.StateCompleted(PlayerState.GetBall, true, this);
        }
    }

    GameObject FindClosestMapPoint()
    {
        float shortestDistance = Mathf.Infinity;
        int currentIndex = 0;

        for (int i = 0; i < mapPoints.Count; i++)
        {
            if (Vector2.Distance((Vector2)this.transform.position, (Vector2)mapPoints[i].transform.position) < shortestDistance)
            {
                shortestDistance = Vector2.Distance((Vector2)this.transform.position, (Vector2)mapPoints[i].transform.position);
                currentIndex = i;
            }
        }
        return mapPoints[currentIndex];
    }

    void ChangeMapPosition(GameObject newMapPosition)
    {
        currentMapPoint = newMapPosition;
        currentTeamManager.PlayerReachedMapPosition(currentMapPoint, this);
    }    

    void Defend()   
    {
        posToLookAt = markAgent.transform.position;

        Vector2 goToPos;
        goToPos = ownGoal.transform.position - markAgent.transform.position;
        if (Maths.Magnitude(goToPos) < minDefenceSlackValue)
        {
            goToPos = goToPos * (defenceSlackPercent + 0.3f);
        }

        else
        {
            goToPos = goToPos * defenceSlackPercent;
        }
        goToPos = (Vector2)markAgent.transform.position + goToPos;
        SeekToPosition(goToPos);
        //If the player steals the ball during defense.
        if (hasBall)
        {
            currentTeamManager.StateCompleted(PlayerState.GetBall, true, this);
        }

        //Seek to position between the enemy player and your goal, and try to stay in that pocket. Slightly in front of the offensive player, will depend on stats.
    }

    void Waiting()
    {
        if (!currentTeamManager.waitingPlayers.Contains(this))
        {
            currentTeamManager.AddWaitingPlayers(this);
            newMapPoint = null;
        }

        if (currentTeamManager.AllPlayersByGoal())
        {
            currentTeamManager.StateCompleted(PlayerState.Waiting, true, this);
        }    

        //currentTeamManager.StateCompleted(PlayerState.Waiting, true, this);

    }

    void Pass()
    {
        int randomTeammate;
        List<FootBallAgent> teammates = new List<FootBallAgent>();
        if (passAimTimerTemp == passAimTimer)
        {
            teammates = GetTeammateToPass(currentTeamManager.GetTeammates(this));
            //If both teammates are not ready for the pass yet.
            if (teammates.Count != 2)
            {
                currentTeamManager.StateCompleted(PlayerState.Pass, false, this);
                return;
            }

            else
            {
                randomTeammate = Random.Range(0, teammates.Count);
                LookAtDirection(teammates[randomTeammate].transform.position);
                teammateToPass = teammates[randomTeammate];
            };
        }

        if (passAimTimerTemp<=0)
        {
            ChangeMapPosition(FindClosestMapPoint());

            if (Vector2.Distance(this.transform.position, teammateToPass.transform.position) > 10f)
            {
                Debug.Log(Vector2.Distance(this.transform.position, teammateToPass.transform.position));
                Kick(posToLookAt - (Vector2)transform.position, kickPower * 2.2f);
            }

            else
            {
                Debug.Log(Vector2.Distance(this.transform.position, teammateToPass.transform.position));
                Kick(posToLookAt - (Vector2)transform.position, kickPower * 1.3f);
            }





            passAimTimerTemp = passAimTimer;
            currentTeamManager.StateCompleted(PlayerState.Pass, true, this);
            return;
        }

        passAimTimerTemp -= Time.deltaTime;

    }



    List<FootBallAgent> GetTeammateToPass(List<FootBallAgent> teammates)
    {
        //Run fuzzy logic here.
        //Check if the teammates are ahead, if not, don't pass.
        List<FootBallAgent> possibleTeammates = new List<FootBallAgent>();
        //Checks if the teammates are ahead. If not, returns false to teammanager and waits.
        for (int i = 0; i < teammates.Count; i++)
        {
            teammates[i].TryGetComponent(out IPlayer IPlayer);

            if (IPlayer.GetPlayerState() == PlayerState.Waiting || IPlayer.GetPlayerState() == PlayerState.GetOpen)
            {
                possibleTeammates.Add(teammates[i]);
            }
        }
        return possibleTeammates;
    }

    void Run()
    {
        currentMapPoint.TryGetComponent<MapPoint>(out MapPoint point);
        
        if (hasBall)
            Debug.LogError("Trying to run with ball.");

        else
        {
            //If the new map point hasn't been selected yet.
            if (newMapPoint == null || newMapPoint == point)
            {
                ChangeNewMapPoint();
                //Switch on team number. Team 0 goes down the array, Team 1 goes up the array.
                //MAP POINT WAS HEREHR EHREH
            }

            else
            {
                //Debug.Log(newMapPoint.name);
                SeekToPosition(newMapPoint.transform.position);
                //When they have reached the desired position, tell the manager.
                if (!m_Seek.m_Active)
                {
                    ChangeMapPosition(newMapPoint);
                    //currentTeamManager.PlayerReachedMapPosition(currentMapPoint, this);
                    currentTeamManager.StateCompleted(PlayerState.Run, true, this);
                }
            }

        }
        //When your teammate has the ball, run towards the enemy goal. Once close enough, either the ball is passed or GetOpen();
        //Random position in semi-circle coming from the goal. The radius of the semi-circle will depend on kick power.
    }

    GameObject getNewRunPosition()
    {
        return null;
    }

    void GetOpen()
    {
        if (hasBall)
        {
            DisableAllMovement();
            AssignState(PlayerState.StrikeOrPass);
        }


        if (!m_Seek.m_Active && !hasBall)
        {
            Vector2 randomOffset = new Vector2(0, 0);

            if (teamNumber == 0)
            {
                randomOffset.x = Random.RandomRange(-0f, 2f);
                randomOffset.y = Random.RandomRange(-0.5f, 0.5f);

            }

            if (teamNumber == 1)
            {
                randomOffset.x = Random.RandomRange(-2f, 0f);
                randomOffset.y = Random.RandomRange(-0.5f, 0.5f);

            }

            randomOpenOffset = randomOffset;

        }


        SeekToPosition((Vector2)currentMapPoint.transform.position + randomOpenOffset);

        //Find random seek near the currentMapPoint.

        //Move towards it.

        //Wait a second.

        //Move towards a new point.

        //Randomly move left and right to lose the defender.
    }

    void StrikeOrPass()
    {
        if (!ballHolder)
        {
            inStrikeOrPass = false;
            AssignState(PlayerState.Waiting);
        }

        if (inStrikeOrPass)
        {
            return;
        }

        DisableAllMovement();

        inStrikeOrPass = true;

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

        if (openTargets.Count == 0)
        {
            //currentTeamManager.StateCompleted(PlayerState.StrikeOrPass, false, this);
            inStrikeOrPass = false;
            AssignState(PlayerState.Pass);
            return;
            //TELL THE MANAGER THAT IT FAILED.
        }

        float distanceToGoal = Vector2.Distance(this.transform.position, enemyGoal.transform.position);

        //If distanceToGoal > 7, then it is high.
        //If targets count is bigger than 1, it is high.

        float fDistance_Short = FuzzyLogic.ReverseGradient(distanceToGoal, 7.5f, 0.0f);

        float fDistance_High = FuzzyLogic.Gradient(distanceToGoal, 6f, 15f);

        float fTargetLow = FuzzyLogic.ReverseGradient((float)openTargets.Count, 1.5F, 0f);
        float fTargetHigh = FuzzyLogic.Gradient((float)openTargets.Count, 4.0f, 1f);


        Debug.Log("fuzzy: high then short");
        Debug.Log(fDistance_High);
        Debug.Log(fDistance_Short);


        if (openTargets.Count > 0)
        {
            Strike();
            currentPlayerState = PlayerState.Strike;
            return;
        }


        List<FootBallAgent> teammates = currentTeamManager.GetTeammates(this);

        //Fuzzy logic here.

        //Values used:
        //Amount of open targets 1-4. The higher the better.
        //How close the teammates defender is to them. The higher the better.
        //How close the teammate is to the basket. The lower the better.








    }

    void Strike()
    {
        Debug.Log("b4 kick");
        //Actually kicking the ball at a goal to score.
        if (kickAimTimerTemp == kickAimTimer)
        {
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

            int randomIndex;
            randomIndex = Random.Range(0, openTargets.Count);
            LookAtDirection(openTargets[randomIndex].transform.position);
        }

        if (kickAimTimerTemp <= 0)
        {
            Debug.Log("kick");
            Kick(posToLookAt - (Vector2)transform.position, kickPower * 1.5f);
            kickAimTimerTemp = kickAimTimer;
            //TELL TEAMMANAGER HERE THAT YOU KICKED THE BALL.
        }

        kickAimTimerTemp -= Time.deltaTime;

    }

    bool RaycastIsTargetOpen(GameObject expectedHit, Vector2 direction)
    {
        RaycastHit2D hit;
        //Raycasts to the target, and if the raycast hits the target, returns true. Works on gameplay layer only.
        hit = Physics2D.BoxCast((Vector2)transform.position, new Vector2(0.3f, 0.3f), 0, (direction - (Vector2)transform.position), LayerMask.GetMask("Gameplay"));
        Debug.DrawRay((Vector2)transform.position, (direction - (Vector2)transform.position).normalized * 1000, Color.red, 3, false);
        if (hit.transform.gameObject == expectedHit)
        {
            return true;
        }
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
        m_Arrive.m_Active = false;

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
        m_Seek.m_Active = false;
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
