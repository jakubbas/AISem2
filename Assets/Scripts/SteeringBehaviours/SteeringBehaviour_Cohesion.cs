using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Cohesion : SteeringBehaviour
{
    public float m_CohesionRange;
    
    [Range(1,-1)]
    public float m_FOV;
    public override Vector2 CalculateForce()
    {
        Vector2 accumulatedPosition = Vector2.zero;


        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_CohesionRange);
        if (entities.Length == 0)
            return Vector2.zero;

        foreach (Collider2D entity in entities)
        {
            if (Maths.Dot(m_Manager.m_Entity.m_Velocity, (Vector2)transform.position - (Vector2)entity.transform.position) <= m_FOV)
            {
                accumulatedPosition += (Vector2)entity.transform.position;
            }
        }

        Vector2 cohesionForce = accumulatedPosition / entities.Length;

        m_DesiredVelocity = cohesionForce - (Vector2)transform.position;
        m_DesiredVelocity = Maths.Normalise(m_DesiredVelocity) * m_Manager.m_Entity.m_MaxSpeed;
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

        return Maths.Normalise(m_Steering) * m_Weight;
    }
}
