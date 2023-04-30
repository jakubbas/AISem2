using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    public void SetBallPossessed();

    public FootBallAgent GetFootBallAgentType();

    public int GetFootBallAgentTeamID();
}
