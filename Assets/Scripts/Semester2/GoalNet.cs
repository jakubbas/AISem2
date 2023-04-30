using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoalNet : MonoBehaviour, IGoalNet
{
    [SerializeField] public int teamNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IBall ball))
        {

        }
    }

    public void GoalScored(int teamNumber)
    {
        throw new System.NotImplementedException();
    }

    public GoalNet GetGoalNetType()
    {
        return this;
    }
}
