using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Seperation : SteeringBehaviour
{
    public float m_SeperationRange;
    
    [Range(1,-1)]
    public float m_FOV;
    public override Vector2 CalculateForce()
    {
        Vector2 accumulatedSeperationForce = Vector2.zero;
        Vector2 accumulatedUnitVector = Vector2.zero;
        Vector2 forceToAdd = Vector2.zero;
        float accumulatedMagnitude = 0f;
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_SeperationRange);
        if (entities.Length == 0)
            return Vector2.zero; 


        foreach (Collider2D entity in entities)
        {
            if (Maths.Dot(m_Manager.m_Entity.m_Velocity, (Vector2)transform.position - (Vector2)entity.transform.position) <= m_FOV)
            {
                accumulatedUnitVector = Maths.Normalise((Vector2)transform.position - (Vector2)entity.gameObject.transform.position);
                accumulatedMagnitude = Maths.Magnitude((Vector2)transform.position - (Vector2)entity.gameObject.transform.position);
                forceToAdd = accumulatedUnitVector / accumulatedMagnitude;
                accumulatedSeperationForce += forceToAdd;
            }
        }


        return accumulatedSeperationForce;
    }
}
