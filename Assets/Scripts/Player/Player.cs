using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingEntity
{
    public float m_Acceleration;

    //input
    float m_Horizontal;
    float m_Vertical;

    public bool m_CanMoveWhileAttacking;
    bool m_Attacking;

    private Rigidbody2D rb;

    private float rotation;
    private GameObject arrowPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        arrowPoint = GameObject.Find("ArrowPoint");
    }
    void Update()
    {
        rotation = Quaternion.LookRotation((Vector2)rb.velocity, Vector2.up).normalized.z * 360;
        arrowPoint.transform.rotation = new Quaternion(0,0, rotation, 1);
        m_Horizontal = Input.GetAxis("Horizontal");
        m_Vertical = Input.GetAxis("Vertical");
        
        if(Input.GetKeyDown(KeyCode.Space))
		{
            m_Animator.SetTrigger("Attack");
            m_Attacking = true;
        }

        if (!m_Attacking || (m_Attacking && m_CanMoveWhileAttacking))
        {
            //up
            if (m_Vertical > 0)
            {
                m_Animator.SetInteger("Direction", 0);
            }
            //right
            else if (m_Horizontal > 0)
            {
                m_Animator.SetInteger("Direction", 1);
            }
            //down
            else if (m_Vertical < 0)
            {
                m_Animator.SetInteger("Direction", 2);
            }
            //left
            else if (m_Horizontal < 0)
            {
                m_Animator.SetInteger("Direction", 3);
            }
            //idle
            else
            {
                m_Animator.SetInteger("Direction", -1);
            }
        }
    }

    public void StopAttack()
	{
        m_Attacking = false;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity ent = collision.GetComponent<Entity>();


        if (collision.gameObject.TryGetComponent(out IBall ball))
        {
            ball.KickBall(-(Vector2)transform.position + (Vector2)collision.gameObject.transform.position, 500f);
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
