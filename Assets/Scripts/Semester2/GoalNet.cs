using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GoalNet : MonoBehaviour, IGoalNet
{
    [SerializeField] public int teamNumber;
    public GameObject[] aimTargets = new GameObject[4];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IBall ball))
        {
            StartCoroutine(EndGame());
        }
    }

    IEnumerator EndGame()
    {

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("Sem2Test", LoadSceneMode.Single);

    }





    public void GoalScored(int teamNumber)
    {
        throw new System.NotImplementedException();
    }

    public GoalNet GetGoalNetType()
    {
        return this;
    }

    public GameObject[] GetAimTargets()
    {
        return aimTargets;
    }
}
