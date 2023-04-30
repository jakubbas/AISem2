using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Player : MovingEntity
{
    private GameObject ownGoal;
    private GameObject enemyGoal;


    [SerializeField] private float kickPower;
    [SerializeField] private int teamNumber;
    public float m_Acceleration;

    //input
    float m_Horizontal;
    float m_Vertical;

    public bool m_CanMoveWhileAttacking;
    bool m_Attacking;

    private Rigidbody2D rb;

    private float rotation;
    private GameObject arrowPoint;

    private Vector2 posToLookAt;


   


    private enum TeamState
    {
        Defense,
        Attack
    }

    void Defense()
    {

    }

    void Attack()
    {

    }

    private enum PlayerState
    {
        Defend,
        Pass,
        Run,
        GetOpen,
        Strike
    }

    void Defend()
    {

    }

    void Pass()
    {

    }
    void Run()
    {

    }

    void GetOpen()
    {

    }

    void Strike()
    {

    }

    void Start()
    {
        foreach (GameObject goal in GameObject.FindGameObjectsWithTag("Goal"))
        {
            if (goal.TryGetComponent(out GoalNet net))
            {
                if (net.teamNumber == teamNumber)
                {
                    ownGoal = goal;
                }
                else
                {
                    enemyGoal = goal;
                }
            }
        }

        rb = GetComponent<Rigidbody2D>();
        arrowPoint = GameObject.Find("ArrowPoint");
    }

    float ArrowDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x);
        angle = 180 * angle/Mathf.PI;
        return angle;
        //return (360 + Mathf.Round(angle)) % 360; //round number, avoid decimal fragments
    }

    float getAngleLookAt()
    {
        Vector2 tempPosToLookAt = posToLookAt;

        tempPosToLookAt -= (Vector2)transform.position;
        float angle = Mathf.Atan2(tempPosToLookAt.y, tempPosToLookAt.x) * Mathf.Rad2Deg;
        return angle;
        
    }

    float getAngle()
    {
        Vector2 tempPosToLookAt = posToLookAt;
        tempPosToLookAt -= (Vector2)transform.position;
        float angle = Mathf.Atan2(tempPosToLookAt.y, tempPosToLookAt.x);
        return angle;
    }


    void Update()
    {
        posToLookAt = (Vector2)GameObject.Find("Ball").transform.position;

        //rotates player and arrow
        arrowPoint.transform.rotation = Quaternion.Euler(new  Vector3(0, 0, getAngleLookAt() - 90));
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, getAngleLookAt() - 90));

        m_Horizontal = Input.GetAxis("Horizontal");
        m_Vertical = Input.GetAxis("Vertical");

        bool IsInRange(float value, float minRotation, float maxRotation)
        {
            if (value > minRotation && value < maxRotation)
                return true;
            else
                return false;  
            
        }

        if (!m_Attacking || (m_Attacking && m_CanMoveWhileAttacking))
        {
            ////up
            //if (IsInRange(transform.rotation.z, -45, 45))
            //{
            //    m_Animator.SetInteger("Direction", 0);
            //}
            ////right
            //else if (IsInRange(transform.rotation.z, -135, -45))
            //{
            //    m_Animator.SetInteger("Direction", 1);
            //}
            ////down
            //else if (IsInRange(transform.rotation.z, -135, -45))
            //{
            //    m_Animator.SetInteger("Direction", 2);
            //}
            ////left
            //else if (IsInRange(transform.rotation.z, -135, -45))
            //{
            //    m_Animator.SetInteger("Direction", 3);
            //}
            ////idle
            //else
            //{
            //    m_Animator.SetInteger("Direction", -1);
            //}
        }
    }

    public void StopAttack()
	{

	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity ent = collision.GetComponent<Entity>();


        if (collision.gameObject.TryGetComponent(out IBall ball))
        {
            ball.KickBall(-(Vector2)transform.position + (Vector2)collision.gameObject.transform.position, kickPower, gameObject);
        }

        //if (ent)
        //{
        //    ent.TakeDamage(m_AttackPower);
        //}
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Entity ent = collision.gameObject.GetComponent<Entity>();

		if (collision.gameObject.TryGetComponent(out IBall ball))
        {

        }
	}

	protected override Vector2 GenerateVelocity()
	{
		return new Vector2(m_Horizontal, m_Vertical) * m_Acceleration;
    }
}
