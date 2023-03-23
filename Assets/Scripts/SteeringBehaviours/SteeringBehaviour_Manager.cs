using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Manager : MonoBehaviour
{
    public MovingEntity m_Entity { get; private set; }
    public float m_MaxForce = 100;
    public float m_RemainingForce;
    public List<SteeringBehaviour> m_SteeringBehaviours;

	private void Awake()
	{
        m_Entity = GetComponent<MovingEntity>();

        if(!m_Entity)
            Debug.LogError("Steering Behaviours only working on type moving entity", this);
    }

	public Vector2 GenerateSteeringForce()
    {
        //if (m_SteeringBehaviours.Count > 0)
        //{
        //    return m_SteeringBehaviours[0].CalculateForce();
        //}

        //else
        //{
        //    return Vector2.zero;
        //}

        m_RemainingForce = m_MaxForce;

        Vector2 combinedForce = Vector2.zero;

        foreach (SteeringBehaviour sb in m_SteeringBehaviours)
        {
            if (m_RemainingForce > 0 && sb.m_Active)
            {
                Vector2 tempForce = sb.CalculateForce();
                if (tempForce.magnitude > m_RemainingForce)
                {
                    tempForce = Maths.Normalise(tempForce) * m_RemainingForce;
                }

                m_RemainingForce -= Maths.Magnitude(tempForce);
                combinedForce += tempForce;
            }
        }

        return combinedForce;



    }

    public void EnableExclusive(SteeringBehaviour behaviour)
	{
        if(m_SteeringBehaviours.Contains(behaviour))
		{
            foreach(SteeringBehaviour sb in m_SteeringBehaviours)
			{
                sb.m_Active = false;
			}

            behaviour.m_Active = true;
		}
        else
		{
            Debug.Log(behaviour + " doesn't not exist on object", this);
		}
	}
    public void DisableAllSteeringBehaviours()
    {
        foreach (SteeringBehaviour sb in m_SteeringBehaviours)
        {
            sb.m_Active = false;
        }
    }
}
