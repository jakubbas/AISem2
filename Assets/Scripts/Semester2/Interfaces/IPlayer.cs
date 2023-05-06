using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    void SetBallPossessed();

    FootBallAgent GetFootBallAgentType();

    int GetFootBallAgentTeamID();

    void SetMarkAgent(FootBallAgent newMark);

    void AssignState(PlayerState playerState);

    GameObject GetBallHolder();

    void AssignManagerToPlayer(TeamManager manager);

    int GetPositionIndex();
}
