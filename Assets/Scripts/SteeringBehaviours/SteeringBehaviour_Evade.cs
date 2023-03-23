using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Evade : SteeringBehaviour
{
    public MovingEntity m_EvadingEntity;
    public float m_EvadeRadius;

    public override Vector2 CalculateForce()
    {

        if (m_EvadingEntity == null)
        {
            return Vector2.zero;
        }
        
        Vector2 distanceVector = (Vector2)m_EvadingEntity.transform.position - (Vector2)m_Manager.m_Entity.transform.position;


        float combinedSpeed = Maths.Magnitude(m_EvadingEntity.m_Velocity) + Maths.Magnitude(m_Manager.m_Entity.m_Velocity);
        float predictionTime = Maths.Magnitude(distanceVector) / combinedSpeed;

        Vector2 fleeVector = (Vector2)m_EvadingEntity.transform.position + (Vector2)m_EvadingEntity.m_Velocity * predictionTime;


        //Flee

        m_DesiredVelocity = (Vector2)m_Manager.m_Entity.transform.position - fleeVector;

        Vector2 distance = m_DesiredVelocity;

        m_DesiredVelocity = Maths.Normalise(m_DesiredVelocity) * m_Manager.m_Entity.m_MaxSpeed;

        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;



        return Maths.Normalise(m_Steering) * Mathf.Lerp(m_Weight, 0, Mathf.Min(Maths.Magnitude(distance), m_EvadeRadius) / m_EvadeRadius);







    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, m_EvadeRadius);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
