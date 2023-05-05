using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    void SetBallPossessed();

    FootBallAgent GetFootBallAgentType();

    int GetFootBallAgentTeamID();

    void SetMarkAgent(FootBallAgent newMark);

    GameObject GetBallHolder();
}
