using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionMakingAgent : MovingEntity
{
    private List<GameObject> healthPacks = new List<GameObject>();
    [HideInInspector]public GameObject closestHealthPack;

    SteeringBehaviour_Manager m_SteeringBehaviours;
    SteeringBehaviour_Pursuit m_Pursuit;
    SteeringBehaviour_Arrive m_Arrive;
    SteeringBehaviour_Evade m_Evade;
    SteeringBehaviour_Cohesion m_Cohesion;
    SteeringBehaviour_Seperation m_Separation;
    SteeringBehaviour_Alignment m_Alignment;

    public float detectionRadius;
    private bool playerWithinRadius = false;

    private State currentState;
    private enum State
    {
        Patrol,
        Chase,
        Escape,
        GetHealth
    }


    protected override void Awake()
    {
        base.Awake();
        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

        m_Pursuit = GetComponent<SteeringBehaviour_Pursuit>();
        m_Arrive = GetComponent<SteeringBehaviour_Arrive>();
        m_Evade = GetComponent<SteeringBehaviour_Evade>();
        m_Cohesion = GetComponent<SteeringBehaviour_Cohesion>();
        m_Separation = GetComponent<SteeringBehaviour_Seperation>();
        m_Alignment = GetComponent<SteeringBehaviour_Alignment>();

        if (!m_SteeringBehaviours)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
        if (!m_Pursuit)
            Debug.LogError("Object doesn't have a Steering Behaviour Pursuit attached", this);
        if (!m_Arrive)
            Debug.LogError("Object doesn't have a Steering Behaviour Arrive attached", this);
        if (!m_Evade)
            Debug.LogError("Object doesn't have a Steering Behaviour Evade attached", this);
        if (!m_Cohesion)
            Debug.LogError("Object doesn't have a Steering Behaviour Cohesion attached", this);
        if (!m_Separation)
            Debug.LogError("Object doesn't have a Steering Behaviour Separation attached", this);
        if (!m_Alignment)
            Debug.LogError("Object doesn't have a Steering Behaviour Alignment attached", this);

        currentState = State.Patrol;

    }

    protected void Start()
    {
        foreach(GameObject health in GameObject.FindGameObjectsWithTag("HealthPack"))
        {
            healthPacks.Add(health);
        }

        m_Evade.m_EvadingEntity = GameObject.Find("Player").GetComponent<MovingEntity>();
        m_Pursuit.m_PursuingEntity = GameObject.Find("Player").GetComponent<MovingEntity>();
        m_Evade.m_Active = false;
        m_Pursuit.m_Active = false;

    }
    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //Debug.Log(currentState);
        //Debug.Log(m_Arrive.m_TargetPosition);
        //Debug.Log(closestHealthPack);

        DetectionCheck();

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Escape:
                Escape();
                break;
            case State.GetHealth:
                GetHealth();
                break;
            default:
                Debug.Log("No state selected. Patrolling");
                Patrol();
                break;
        }


    }

    private void DetectionCheck()
    {
        float distanceToPlayer;

        distanceToPlayer = Mathf.Abs(Maths.Magnitude((Vector2)transform.position - (Vector2)m_Pursuit.m_PursuingEntity.gameObject.transform.position));

        if (distanceToPlayer <= detectionRadius)
        {
            playerWithinRadius = true;
        }

        else
        {
            playerWithinRadius = false;
        }
    }

    protected override Vector2 GenerateVelocity()
    {
        return m_SteeringBehaviours.GenerateSteeringForce();
    }

    private void SwitchState(State newState)
    {
        m_Pursuit.m_Active = false;
        m_Arrive.m_Active = false;
        m_Evade.m_Active = false;
        switch(newState)
        {
            case State.Patrol:
                m_Arrive.m_Active = true;
                break;
            case State.Chase:
                m_Pursuit.m_Active = true;
                break;
            case State.Escape:
                m_Evade.m_Active = true;
                break;
            case State.GetHealth:
                m_Arrive.m_Active = true;
                break;

        }

        currentState = newState;
    }

    private void Patrol()
    {
        if (Maths.Magnitude((Vector2)transform.position - m_Arrive.m_TargetPosition) < 0.1f)
        {
            m_Arrive.m_TargetPosition = Grid.GetRandomWalkableTile().transform.position;
        }

        if (playerWithinRadius)
        {
            SwitchState(State.Chase);
        }
    }

    private void Chase()
    {
        if (!playerWithinRadius)
        {
            SwitchState(State.Patrol);
        }

        else if (this.m_CurrentHealth < this.m_MaxHealth / 2)
        {
            SwitchState(State.Escape);
        }

    }

    private void Escape()
    {
        if (this.m_CurrentHealth == this.m_MaxHealth && playerWithinRadius)
        {
            SwitchState(State.Chase);
        }
        else if (!playerWithinRadius)
        {
            SwitchState(State.GetHealth);
        }

    }

    private void GetHealth()
    {
        if (this.m_CurrentHealth >= this.m_MaxHealth)
        {
            SwitchState(State.Patrol);
        }

        if (closestHealthPack == null)
        {
            closestHealthPack = FindClosestHealthPack();
            m_Arrive.m_TargetPosition = (Vector2)closestHealthPack.transform.position;
        }
        else
        {
            //Debug.Log("here1");
            if (Maths.Magnitude((Vector2)transform.position - (Vector2)closestHealthPack.transform.position) < 0.1f)
            {
                //Debug.Log("here");
                healthPacks.Remove(closestHealthPack);
                closestHealthPack = null;
                SwitchState(State.Patrol);
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Weapon"))
        {
            this.TakeDamage(m_Pursuit.m_PursuingEntity.m_AttackPower);
        }

        //if (other.gameObject.CompareTag("HealthPack"))
        //{
        //    Debug.Log("here");
        //    closestHealthPack = null;
        //    healthPacks.Remove(other.gameObject);
        //}
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("HealthPack"))
        {
            healthPacks.Remove(closestHealthPack);
            closestHealthPack = null;
        }
    }

    GameObject FindClosestHealthPack()
    {
        foreach (GameObject healthPack in healthPacks)
        {

            if (healthPack == healthPacks[0])
                closestHealthPack = healthPack;

            if (Vector2.Distance((Vector2)transform.position, (Vector2)closestHealthPack.transform.position) >= Vector2.Distance((Vector2)transform.position, (Vector2)healthPack.transform.position))
            {
                closestHealthPack = healthPack;
            }
        }

        return closestHealthPack;
    }

    //void FoundHealthPack()
    //{
    //    healthPacks.Clear();
    //    foreach (GameObject health in GameObject.FindGameObjectsWithTag("HealthPack"))
    //    {
    //        healthPacks.Add(health);
    //    }
    //}

}
