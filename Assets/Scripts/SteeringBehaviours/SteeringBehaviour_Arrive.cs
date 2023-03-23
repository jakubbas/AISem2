using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Arrive : SteeringBehaviour
{
    public Vector2 m_TargetPosition;
    public float m_SlowingRadius; 

    public override Vector2 CalculateForce()
    {
        Vector2 vector = m_TargetPosition - (Vector2)transform.position;
        float vectorMagnitude = Maths.Magnitude(vector);
        float arriveSpeed = vectorMagnitude * m_SlowingRadius;
        if (arriveSpeed > m_Manager.m_Entity.m_MaxSpeed)
        {
            arriveSpeed = m_Manager.m_Entity.m_MaxSpeed;
        }

        Vector2 unitVector = vector / vectorMagnitude;

        m_DesiredVelocity = unitVector * arriveSpeed;

        Vector2 arriveForce = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

        m_Steering = arriveForce;

        return Maths.Normalise(m_Steering) * m_Weight;
    }
}
