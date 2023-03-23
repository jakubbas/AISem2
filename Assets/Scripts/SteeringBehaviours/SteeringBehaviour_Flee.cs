using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Flee : SteeringBehaviour
{
    public Transform m_FleeTarget;
    public float m_FleeRadius;

    public override Vector2 CalculateForce()
    {
        if (m_FleeTarget == null)
        {
            return Vector2.zero;
        }

        else
        {
            m_DesiredVelocity = (Vector2)m_Manager.m_Entity.transform.position - (Vector2)m_FleeTarget.position;

            Vector2 distance = m_DesiredVelocity;

            m_DesiredVelocity = Maths.Normalise(m_DesiredVelocity) * m_Manager.m_Entity.m_MaxSpeed;

            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

            

            return Maths.Normalise(m_Steering) * Mathf.Lerp(m_Weight, 0, Mathf.Min(Maths.Magnitude(distance), m_FleeRadius) / m_FleeRadius);



        }


    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, m_FleeRadius);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
