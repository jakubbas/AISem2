using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public int goalTeamNumber;
    public TeamManager enemyManager;

    private List<FootBallAgent> ownTeamPlayers = new List<FootBallAgent>();
    private List<FootBallAgent> enemyTeamPlayers = new List<FootBallAgent>();

    private GoalNet ownGoalNet;
    private GoalNet enemyGoalNet;

    private Ball ball;

    void Start()
    {
        AssignTeams();
    }

    void AssignTeams()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        
    }

    void Update()
    {
        
    }
}
