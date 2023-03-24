using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBall
{
    void KickBall(Vector2 direction, float power, GameObject playerToKick);

    void GoalScored(int team);

}
