using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Entity>().m_CurrentHealth += other.gameObject.GetComponent<Entity>().m_MaxHealth / 2;
            //other.gameObject.GetComponent<DecisionMakingAgent>().closestHealthPack = null;
            this.gameObject.SetActive(false);
        }
    }
}
