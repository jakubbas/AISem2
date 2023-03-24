using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoalNet : MonoBehaviour
{
    [SerializeField] public int goalTeamNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IBall ball))
        {
            ball.GoalScored(goalTeamNumber);
        }
    }

}
