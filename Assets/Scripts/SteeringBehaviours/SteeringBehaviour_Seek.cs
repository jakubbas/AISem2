using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Seek : SteeringBehaviour
{
    public Vector2 m_TargetPosition;

    public override Vector2 CalculateForce()
    {
        m_DesiredVelocity = m_TargetPosition - (Vector2)transform.position;
        m_DesiredVelocity = Maths.Normalise(m_DesiredVelocity) * m_Manager.m_Entity.m_MaxSpeed;
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

        return Maths.Normalise(m_Steering) * m_Weight;
    }
}
