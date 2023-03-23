using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Pursuit : SteeringBehaviour
{
    public MovingEntity m_PursuingEntity;

    public override Vector2 CalculateForce()
    {
        Vector2 distanceVector = (Vector2)m_Manager.m_Entity.transform.position - (Vector2)m_PursuingEntity.transform.position;

        float combinedSpeed = Maths.Magnitude(m_PursuingEntity.m_Velocity) + Maths.Magnitude(m_Manager.m_Entity.m_Velocity);

        float predictionTime = Maths.Magnitude(distanceVector) / combinedSpeed;

        Vector2 seekVector = (Vector2)m_PursuingEntity.transform.position + m_PursuingEntity.m_Velocity * predictionTime;

        m_DesiredVelocity = seekVector - (Vector2)transform.position;
        m_DesiredVelocity = Maths.Normalise(m_DesiredVelocity) * m_Manager.m_Entity.m_MaxSpeed;
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
        return Maths.Normalise(m_Steering) * m_Weight;
    }
}
