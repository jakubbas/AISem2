using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Wander : SteeringBehaviour
{
    public float m_WanderRadius = 2; 
    public float m_WanderOffset = 2;
    public float m_AngleDisplacement = 2;

    Vector2 m_CirclePosition;
    Vector2 m_PointOnCircle;
    float m_Angle = 0.0f;

    public override Vector2 CalculateForce()
    {

        //Getting random point on a circle
        float randomRadians = Mathf.Acos(Random.Range(-1f, 1f));
        randomRadians = randomRadians + (Random.value < 0.5f ? 0 : Mathf.PI);
        m_PointOnCircle = new Vector2(m_WanderRadius * Mathf.Cos(randomRadians), m_WanderRadius * Mathf.Sin(randomRadians));

        //Calculating the target vector
        if (Maths.Magnitude(m_Manager.m_Entity.m_Velocity) != 0)
        {
            m_CirclePosition = (Vector2)m_Manager.m_Entity.transform.position + ((Maths.Normalise(m_Manager.m_Entity.m_Velocity)) * m_WanderOffset);
        }
        else
        {
            m_CirclePosition = m_Manager.m_Entity.transform.position;
        }

        Vector2 newTarget = m_PointOnCircle + m_CirclePosition;

        //Seek
        m_DesiredVelocity = newTarget - (Vector2)transform.position;
        m_DesiredVelocity = Maths.Normalise(m_DesiredVelocity) * m_Manager.m_Entity.m_MaxSpeed;
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
        return Maths.Normalise(m_Steering) * m_Weight;
    }

	protected override void OnDrawGizmosSelected()
	{
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(m_CirclePosition, m_WanderRadius);

				Gizmos.color = Color.blue;
				Gizmos.DrawLine(transform.position, m_CirclePosition);

				Gizmos.color = Color.green;
				Gizmos.DrawLine(m_CirclePosition, m_PointOnCircle);

				Gizmos.color = Color.red;
				Gizmos.DrawLine(transform.position, m_PointOnCircle);

                base.OnDrawGizmosSelected();
			}
        }
	}
}
