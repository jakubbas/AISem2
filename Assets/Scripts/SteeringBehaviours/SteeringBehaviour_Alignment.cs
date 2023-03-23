using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Alignment : SteeringBehaviour
{
    public float m_AlignmentRange;
    
    [Range(1,-1)]
    public float m_FOV;
    public override Vector2 CalculateForce()
    {
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_AlignmentRange);
        Vector2 entityRotation = Vector2.zero;

        if (entities.Length == 0)
        {
            return Vector2.zero;
        }

        foreach (Collider2D entity in entities)
        {
            if (Maths.Dot(m_Manager.m_Entity.m_Velocity, (Vector2)transform.position - (Vector2)entity.transform.position) <= m_FOV)
            {
                entityRotation += (Vector2)entity.transform.forward;
            }

        }

        Vector2 alignForce = entityRotation / entities.Length;


        alignForce -= (Vector2)transform.forward;

        return alignForce;
    }
}
