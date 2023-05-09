using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    bool attackState = true;

    public int teamNumber;
    public TeamManager enemyManager;

    private List<FootBallAgent> ownTeamPlayers = new List<FootBallAgent>();
    private List<FootBallAgent> enemyTeamPlayers = new List<FootBallAgent>();

    private GoalNet ownGoalNet;
    private GoalNet enemyGoalNet;

    private Ball ball;

    private FootBallAgent ballHandler;

    private bool passWaitingRunning = false;

    public List<FootBallAgent> waitingPlayers;

    void Start()
    {
        GetTeams();
    }

    //Called when the teams are initialized.
    void Init()
    {
        // THIS IS BROKEN, IT CALLS FREEBALL() BEFORE GETTEAMS() IS FULLY COMPLETED, SO THE OWNTEAMPLAYERS ARRAY DOESN'T GET FILLED UP.
        AssignMarks();
        FreeBall();
        ball.TryGetComponent(out IBall IBall);
        IBall.SetTeamManagers(this);
    }

    //Gets all the required objects and assigns them to their team variables.
    void GetTeams()
    {
        if (GameObject.FindGameObjectWithTag("Ball").TryGetComponent(out IBall IBall))
        {
            ball = IBall.GetBallType();
        }

        else
        {
            Debug.LogError("Unable to find ball in TeamManager");
        }

        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < allPlayers.Length; i++)
        {
            if (allPlayers[i].TryGetComponent(out IPlayer IPlayer))
            {
                if (IPlayer.GetFootBallAgentTeamID() == teamNumber)
                {
                    ownTeamPlayers.Add(IPlayer.GetFootBallAgentType());
                    IPlayer.AssignManagerToPlayer(this);
                }

                else
                {
                    enemyTeamPlayers.Add(IPlayer.GetFootBallAgentType());
                }
            }

            else
            {
                Debug.LogError("Unable to find players for teams in TeamManager");
            }
        }

        GameObject[] goals = GameObject.FindGameObjectsWithTag("Goal");
        for (int i = 0; i < goals.Length; i++)
        {
            if (goals[i].TryGetComponent(out IGoalNet IGoal))
            {
                if (IGoal.GetGoalNetType().teamNumber == teamNumber)
                {
                    ownGoalNet = IGoal.GetGoalNetType();
                }

                else
                {
                    enemyGoalNet = IGoal.GetGoalNetType();
                }
            }

            else
            {
                Debug.LogError("Unable to find goals in TeamManager");
            }
        }

        Init();
    }

    void AssignMarks()
    {
        for (int i = 0; i < ownTeamPlayers.Count; i++)
        {
            FootBallAgent closestOpponent = FindClosestOpponent(ownTeamPlayers[i]);
            ownTeamPlayers[i].SetMarkAgent(closestOpponent);
        }
    }

    FootBallAgent FindClosestOpponent(FootBallAgent currentPlayer)
    {
        float shortestDistance = Mathf.Infinity;
        int currentIndex = 0;
        for (int i = 0; i < enemyTeamPlayers.Count; i++)
        {
            if (Vector2.Distance((Vector2)currentPlayer.transform.position, (Vector2)enemyTeamPlayers[i].transform.position) < shortestDistance)
            {
                shortestDistance = Vector2.Distance((Vector2)currentPlayer.transform.position, (Vector2)enemyTeamPlayers[i].transform.position);
                currentIndex = i;
            }
        }

        return enemyTeamPlayers[currentIndex];
    }

    void FreeBall()
    {
        FootBallAgent closestAgentToBall = FindPlayerClosestToBall();
        for (int i = 0; i < ownTeamPlayers.Count; i++)
        {
            ownTeamPlayers[i].TryGetComponent(out IPlayer IPlayer);
            if (ownTeamPlayers[i] == closestAgentToBall)
            {
                IPlayer.AssignState(PlayerState.GetBall);
            }

            else
            {
                IPlayer.AssignState(PlayerState.Defend);
            }


        }    
    }

    public void BallGained(FootBallAgent newBallHandler)
    {
        ballHandler = newBallHandler;
        enemyManager.OpponentPossessionGained();

        //Remove this maybe?? Added last second.
        //MovePlayersUp();
    }

    public void PlayerReachedMapPosition(GameObject mapPosition, FootBallAgent player)
    {

        List<int> tempIndexList = new List<int>();
        int ballHandlerIndex = 0;
        for (int i = 0; i < ownTeamPlayers.Count; i++)
        {
            ownTeamPlayers[i].TryGetComponent(out IPlayer IPlayer);
            mapPosition.TryGetComponent<MapPoint>(out MapPoint point);
            tempIndexList.Add(IPlayer.GetPositionIndex());
            if (ownTeamPlayers[i] == ballHandler)
                ballHandlerIndex = IPlayer.GetPositionIndex();
        }


        //If all the players are at the same position;
        if (tempIndexList[0] == tempIndexList[1] && tempIndexList[0] == tempIndexList[2])
        {
            //MovePlayersUp();
        }

        //If some of the player's arent in position.
        else
        {
            //Remove ball handler from the tempIndexList.
            //tempIndexList.RemoveAt(ownTeamPlayers.IndexOf(ballHandler));

            //Maybe an issue here as the ball handler might not be the one that is behind but another player.
            //Pass only if the ball handler is the one that is behind.

            //switch (teamNumber)
            //{
            //    case 0:
            //        if (tempIndexList[ownTeamPlayers.IndexOf(ballHandler)] > )
            //        {

            //        }




            //        for (int i = 0; i < ownTeamPlayers.Count; i++)
            //        {
            //            ownTeamPlayers[i].TryGetComponent(out IPlayer IPlayer);
            //            mapPosition.TryGetComponent<MapPoint>(out MapPoint point);
            //        }








            //        break;
            //    case 1:


            //        break;
            //}

            //if none are true, wait.
            ballHandler.AssignState(PlayerState.Pass);
        }
    }

    void MovePlayersUp()
    {
        for (int i = 0; i < ownTeamPlayers.Count; i++)
        {
            ownTeamPlayers[i].TryGetComponent(out IPlayer IPlayer);

            if (ownTeamPlayers[i] != ballHandler)
                IPlayer.AssignState(PlayerState.Run);

            else
            {
                StartCoroutine(SwitchToPassTimer(IPlayer));
            }    
        }
    }

    IEnumerator SwitchToPassTimer(IPlayer IPlayer)
    {
        if (passWaitingRunning)
        {
            yield break;
        }

        passWaitingRunning = true;

        yield return new WaitForSeconds(1);

        IPlayer.AssignState(PlayerState.Pass);

        passWaitingRunning = false;

        yield break;
    }

    FootBallAgent FindPlayerClosestToBall()
    {
        float shortestDistance = Mathf.Infinity;
        int currentIndex = 0;

        for (int i = 0; i < ownTeamPlayers.Count; i++)
        {
            if (Vector2.Distance((Vector2)ball.transform.position, (Vector2)ownTeamPlayers[i].transform.position) < shortestDistance)
            {
                shortestDistance = Vector2.Distance((Vector2)ball.transform.position, (Vector2)ownTeamPlayers[i].transform.position);
                currentIndex = i;
            }
        }
        return ownTeamPlayers[currentIndex];
    }

    public void StateCompleted(PlayerState state, bool successful, FootBallAgent player)
    {
        player.TryGetComponent(out IPlayer IPlayer);

        switch (state)
        {
            case PlayerState.Defend:
                //Shouldn't ever get here.
                break;
            case PlayerState.Strike:
                //If successful, go on defense. If not, pass.
                if (successful)
                {
                    IPlayer.AssignState(PlayerState.Defend);
                }
                else
                {
                    IPlayer.AssignState(PlayerState.Pass);
                }
                break;
            case PlayerState.GetOpen:
                //Shouldn't ever get here.
                break;
            case PlayerState.GetBall:
                if (successful)
                {
                    ballHandler = player;
                    MovePlayersUp();
                    IPlayer.AssignState(PlayerState.Pass);
                    //Decision making to pass.
                }
                else
                {
                    //Change state to defense.
                    IPlayer.AssignState(PlayerState.Defend);
                }
                //If successful, go into decision making for pass/strike. If not, go defense.
                break;
            case PlayerState.Pass:
                //If successful, get open or run depending on current position. If not succesful repeat until it is.
                if (successful)
                {

                    if (!AllPlayersByGoal())
                        IPlayer.AssignState(PlayerState.Run);
                    else
                    {
                        IPlayer.AssignState(PlayerState.Waiting);
                    }
                }
                else
                {
                    IPlayer.AssignState(PlayerState.Pass);
                }
                break;
            case PlayerState.Run:
                //Arrives at the run to location.
                if (successful)
                {
                    IPlayer.AssignState(PlayerState.Waiting);
                }
                break;
            case PlayerState.Waiting:
                break;
        }
    }


    public List<FootBallAgent> GetTeammates(FootBallAgent player)
    {
        List<FootBallAgent> teammates = new List<FootBallAgent>();

        for (int i = 0; i < ownTeamPlayers.Count; i++)
        {
            if (ownTeamPlayers[i] != player)
            {
                teammates.Add(ownTeamPlayers[i]);
            }
        }

        return teammates;

    }


    public void AddWaitingPlayers(FootBallAgent waitingPlayer)
    {
        waitingPlayers.Add(waitingPlayer);
        if (waitingPlayers.Count == ownTeamPlayers.Count)
        {
            if (AllPlayersByGoal())
            {
                ballHandler.AssignState(PlayerState.Strike);
            }

            else
            {
                MovePlayersUp();
            }
        }
    }

    void OpponentPossessionGained()
    {
      for (int i = 0; i < ownTeamPlayers.Count; i++)
        {
            ownTeamPlayers[i].TryGetComponent(out IPlayer IPlayer);
            IPlayer.AssignState(PlayerState.Defend);
        }
    }

    public void BallIsFree()
    {
        FootBallAgent closestPlayer = FindPlayerClosestToBall();
        closestPlayer.TryGetComponent(out IPlayer IPlayer);
        IPlayer.AssignState(PlayerState.GetBall);
    }

    public bool AllPlayersByGoal()
    {
        bool allByGoal = true;

        for (int i = 0; i < ownTeamPlayers.Count; i++)
        {
            ownTeamPlayers[i].TryGetComponent(out IPlayer IPlayer);

            switch (teamNumber)
            {
                case 0:
                    if (IPlayer.GetPositionIndex() != 0)
                        allByGoal = false;
                    break;
                case 1:
                    if (IPlayer.GetPositionIndex() != 2)
                        allByGoal = false;
                    break;
            }

        }

        return allByGoal;
    }

    }
