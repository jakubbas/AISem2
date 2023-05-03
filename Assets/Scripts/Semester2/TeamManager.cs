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
                    Debug.Log(ownTeamPlayers.Count);
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
        Debug.Log(ownTeamPlayers.Count);
        attackState = true;
        Debug.Log(FindPlayerClosestToBall().name);
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

}
