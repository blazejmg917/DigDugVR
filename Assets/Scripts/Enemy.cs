using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public enum AIState
    {
        WANDER,
        BURROW,
        SEARCH,
        HUNT
    }
    [SerializeField, Tooltip("this enemy's max health")]
    private int maxHealth = 3;
    //this enemy's current health
    private int health = 0;
    //if this enemy is currently stuck with a pump
    private bool stuckWithPump = false;
    private PumpNozzle nozzle;

    [SerializeField, Tooltip("the time it takes for this enemy to regenerate a point of health while stuck")]
    private float stuckRegenTime = 3f;

    [SerializeField, Tooltip("the time it takes for this enemy to regenerate a point of health while not stuck")]
    private float freeRegenTime = .5f;

    private float regenTimer;

    [SerializeField, Tooltip("If this enemy is currently invisible")]private bool invisible = false;

    [Header("Navmesh")]

    [SerializeField, Tooltip("navmesh agent")]private NavMeshAgent agent;

    [SerializeField, Tooltip("the default enemy type, cannot walk through walls in this form")]
    private int defaultNavMeshAgentType = 0;

    [SerializeField, Tooltip("the invisible enemy type, can walk through walls in this form")]
    private int invisibleNavMeshAgentType = 1;

    [SerializeField, Tooltip("default move speed for the enemy")]
    private float defaultMoveSpeed = 3;

    [SerializeField, Tooltip("this enemy's current AI state")]
    private AIState state;

    [Header("Wander")]
    [SerializeField, Tooltip("the maximum length of time that the enemy can wander to a single point")]
    private float maxWanderTime = 10;

    [SerializeField, Tooltip("the maximum length of time that the enemy will wait at a wander point")]
    private float maxWaitTime = 2;

    private float wanderTimer;

    [SerializeField, Tooltip("the maximum radius for wander point selection")]
    private float maxWanderPointDist = 10;

    [SerializeField, Tooltip("the maximum angle variance allowed when detemining if enemy should walk through blocks.")]
    private float burrowAngle = 10;

    [SerializeField, Tooltip("the distance into a block the agent's destination must be to start burrowing in search mode")]
    private float burrowDistance = 3;

    [SerializeField, Tooltip("if this enemy is currently at their target destination")]
    private bool atDestination = false;

    [SerializeField, Tooltip("if this enemy will allow a wander point to be inside of a wall immediately after exiting a wall")]
    private bool allowSubsequentBurrowing = false;

    [SerializeField, Tooltip("the chance that a burrowing movement is allowed")]
    private float allowBurrowChance = .1f;

    [SerializeField, Tooltip("the number of successive attempts before allowing another wall position to be selected")]
    private int maxAntiBurrowAttempts = 20;

    [SerializeField, Tooltip("the current wander destination")]
    private Vector3 wanderDestination;

    [Header("Burrow")]

    [SerializeField, Tooltip("enemy move speed when they're burrowing")]
    private float invisibleMoveSpeed = 3;

    [SerializeField, Tooltip("the maximum number of blocks this enemy can burrow through in a row when not hunting")]
    private int maxBurrowBlocks = 6;

    [Header("Search")] 

    [SerializeField, Tooltip("the target Transform that the enemy is searching for")]
    private Transform targetTransform;

    [SerializeField, Tooltip("the maximum time an enemy will keep following if they don't see the player")]
    private float maxFollowTime = 5;

    private float followTimer;

    [SerializeField, Tooltip("if they can currently see their target")]
    private bool canSeeSearchTarget;

    [SerializeField, Tooltip("how long the enemy will search without seeing their target before giving up")]
    private float maxSearchTime = 20;

    private float searchTimer = 0;

    [SerializeField, Tooltip("how frequently the target position updates")]
    private float targetUpdateTime = .25f;

    private float targetUpdateTimer;

    [Header("Hunt")] 
    
    [SerializeField, Tooltip("the speed enemies move at in hunt mode")]
    private float huntMoveSpeed = 4;

    [SerializeField, Tooltip("the previous state before entering the hunt state")]
    private AIState prevState = AIState.WANDER;

    [SerializeField, Tooltip("if the enemy will retain its target when the hunt state ends")]
    private bool retainTarget = false;
    //[SerializeField, Tooltip("the previous target before entering the hunt state (if there was one)")]
    //private Transform prevTarget;

    [Space]

    [Header("enemy visuals")]

    [SerializeField, Tooltip("the default enemy appearance")]
    private GameObject normalEnemyAppearance;

    [SerializeField, Tooltip("the enemy appearance when they're invisible")]
    private GameObject invisibleEnemyAppearance;

    [Header("debug")]
    [SerializeField, Tooltip("if the debug processes should run")]bool runDebug = false;
    [SerializeField]private ParticleSystem stuckParticlesDebug;
    [SerializeField]private ParticleSystem damagedParticlesDebug;

    // Start is called before the first frame update
    void Awake()
    {
        health = maxHealth;
    }

    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        defaultNavMeshAgentType = GridSpawner.Instance.GetDefaultSurfaceID();
        invisibleNavMeshAgentType = GridSpawner.Instance.GetInvisibleSurfaceID();

        agent.autoRepath = false;
        EnterWanderState();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (health < maxHealth)
        {
            regenTimer += Time.fixedDeltaTime;
            if ((stuckWithPump && regenTimer > stuckRegenTime) || (!stuckWithPump && regenTimer > freeRegenTime))
            {
                RegenHealth();
                regenTimer = 0;
            }
        }
        if(stuckWithPump){
            return;
        }
        switch (state)
        {
            case AIState.WANDER:
                OnWanderUpdate();
                break;
            case AIState.BURROW:
                OnBurrowUpdate();
                break;
            case AIState.SEARCH:
                OnSearchUpdate();
                break;
            case AIState.HUNT:
                OnHuntUpdate();
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// called to deal damage to this enemy. If it reaches 0 health, it will die
    /// </summary>
    /// <param name="damage">the amount of damage dealt to this enemy. set to 1 by default</param>
    public void Damage(int damage = 1)
    {
        if(runDebug && damagedParticlesDebug){
            damagedParticlesDebug.Play();
        }
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    /// <summary>
    /// called when the enemy hits zero health and breaks. will destroy the enemy
    /// </summary>
    public void Die()
    {
        if(nozzle){
            nozzle.ForceRelease();
        }
        Destroy(gameObject);
    }

    public void RegenHealth()
    {
        health++;
    }

    /// <summary>
    /// called to set whether the enemy is currently stuck with a pump nozzle or not. Will determine whether they can take damage from pumps or not
    /// </summary>
    /// <param name="stuck">if the enemy has been stuck</param>
    public void SetStuck(bool stuck, PumpNozzle nozzle = null)
    {
        if(stuck && runDebug && stuckParticlesDebug){
            stuckParticlesDebug.Play();
        }
        stuckWithPump = stuck;
        agent.isStopped = stuckWithPump;
        if(stuck && nozzle){
            this.nozzle = nozzle;
        }
        else{
            this.nozzle = null;
        }
    }

    public void SwapToInvisible()
    {
        agent.agentTypeID = invisibleNavMeshAgentType;
        normalEnemyAppearance.SetActive(false);
        invisibleEnemyAppearance.SetActive(true);
        agent.SetDestination(agent.destination);
        Debug.Log("enemy going invisible");
        GetComponent<Collider>().enabled = false;
    }

    public void SwapToVisible()
    {
        GetComponent<Collider>().enabled = true;
        agent.agentTypeID = defaultNavMeshAgentType;
        normalEnemyAppearance.SetActive(true);
        invisibleEnemyAppearance.SetActive(false);
        agent.SetDestination(agent.destination);
        Debug.Log("enemy going visible");

    }

    public void TargetSpotted(Transform target)
    {
        if (state == AIState.HUNT)
        {
            targetTransform = target;
        }
        
        if (state == AIState.WANDER)
        {
            EnterSearchState(target);
        }

        if (state == AIState.SEARCH && target == targetTransform)
        {
            searchTimer = 0;
        }
    }

    public void EnterBurrowState(Vector3 newDestination)
    {
        state = AIState.BURROW;
        agent.speed = invisibleMoveSpeed;
        SwapToInvisible();
        agent.SetDestination(newDestination);
        
        Debug.Log("enemy burrowing");
    }

    public void EnterSearchState(Transform target = null, bool maintainTimer = false)
    {
        targetTransform = target;
        state = AIState.SEARCH;
        agent.speed = defaultMoveSpeed;
        if (!maintainTimer)
        {
            searchTimer = maxSearchTime;
        }

        targetUpdateTimer = targetUpdateTime;
        Debug.Log("enemy searching");
    }

    public void EnterWanderState(bool AllowWallDestination = true)
    {
        state = AIState.WANDER;
        targetTransform = null;
        agent.speed = defaultMoveSpeed;
        wanderTimer = 0;
        atDestination = false;
        Debug.Log("enemy wandering");
        SetNewWanderLocation(AllowWallDestination);
        
    }

    public void EnterHuntState(Transform target = null)
    {
        if (state == AIState.HUNT)
        {
            return;
        }
        prevState = state;
        state = AIState.HUNT;
        //prevTarget = targetTransform;
        targetTransform = target;
        agent.speed = huntMoveSpeed;
        agent.SetDestination(targetTransform.position);
        Debug.Log("enemy hunting");
    }

    public void ExitHuntState()
    {
        if (!invisible)
        {
            agent.speed = defaultMoveSpeed;
        }
        else
        {
            agent.speed = invisibleMoveSpeed;
        }
        if (retainTarget)
        {
            EnterSearchState(targetTransform, true);
        }
        else if(invisible)
        {
            EnterBurrowState(targetTransform.position);
        }
        else
        {
            EnterWanderState();
        }
    }

    public void OnSearchUpdate()
    {
        CheckUpdateTarget();
        searchTimer += Time.fixedDeltaTime;
        if (searchTimer > maxSearchTime)
        {
            if (invisible)
            {
                EnterBurrowState(targetTransform.position);
            }
            else
            {
                EnterWanderState();
            }
        }
    }

    private void SetNewWanderLocation(bool allowWallPosition = true)
    {
        Debug.Log("enemy setting new wander location");
        
        Vector3 finalDestination = Vector3.zero;
        for (int i = 0; i < maxAntiBurrowAttempts; i++)
        {
            Vector3 newDestination = Random.insideUnitCircle * maxWanderPointDist;
            newDestination = new Vector3(newDestination.x + transform.position.x, transform.position.y, newDestination.y + transform.position.z);
            NavMeshHit hit;
            float maxDist = maxWanderPointDist;
            if (allowWallPosition)
            {
                maxDist /= (float)Math.Sqrt(2);
            }

            if (allowWallPosition && Random.Range(0.0f, 1.0f) <= allowBurrowChance)
            {
                Debug.Log("allowing burrow location");
                agent.SetDestination(newDestination);
                wanderTimer = 0;
                atDestination = false;
                Debug.DrawLine(transform.position, newDestination, Color.red, 5);
                wanderDestination = newDestination;
                return;
            }

            finalDestination = newDestination;
            if (NavMesh.SamplePosition(newDestination, out hit, maxWanderPointDist, 1))
            {
                finalDestination = hit.position;
                agent.SetDestination(finalDestination);
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(agent.destination, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    wanderTimer = 0;
                    atDestination = false;
                    Debug.DrawLine(transform.position, finalDestination, Color.red, 5);
                    wanderDestination = finalDestination;
                    return;
                }
            }
        }
        agent.SetDestination(finalDestination);
        wanderDestination = finalDestination;
        Debug.DrawLine(transform.position, finalDestination, Color.red, 5);
        wanderTimer = 0;
        atDestination = false;

    }

    public void OnHuntUpdate()
    {
        CheckUpdateTarget();
        if (searchTimer <= maxSearchTime)
        {
            searchTimer += Time.fixedDeltaTime;
            if (searchTimer > maxSearchTime)
            {
                retainTarget = false;
            }
        }
    }

    public void OnBurrowUpdate()
    {
        if (Vector3.Distance(transform.position, new Vector3(agent.destination.x, transform.position.y, agent.destination.z)) < .01f)
        {
            SwapToVisible();
            EnterWanderState();
        }
    }

    public void OnWanderUpdate()
    {
        if (!atDestination && Vector3.Distance(transform.position,new Vector3(wanderDestination.x, transform.position.y, wanderDestination.z)) < .01f)
        {
            Debug.Log("enemy at wander positions");
            atDestination = true;
            wanderTimer = 0;
        }
        wanderTimer += Time.fixedDeltaTime;
        if (!atDestination && wanderTimer > maxWanderTime)
        {
            SetNewWanderLocation();
        }
        else if (atDestination && wanderTimer > maxWaitTime)
        {
            SetNewWanderLocation();
        }

    }

    public void CheckUpdateTarget()
    {
        targetUpdateTimer += Time.fixedDeltaTime;
        if (targetUpdateTimer > targetUpdateTime)
        {
            agent.SetDestination(targetTransform.position);
        }
    }

    public void ExitGround()
    {
        if (state == AIState.BURROW)
        {
            SwapToVisible();
            EnterWanderState(false);
        }
        else if (state == AIState.HUNT)
        {
            SwapToVisible();
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("enemy collision");
        if (invisible)
        {
            return;
        }
        Block block = collision.collider.gameObject.GetComponent<Block>();
        if (!block)
        {
            return;
        }

        if (state == AIState.BURROW || state == AIState.SEARCH)
        {
            return;
        }
        NavMeshPath path = new NavMeshPath();
        
        if (agent.CalculatePath(wanderDestination, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            Debug.Log("path found");
            return;
        }
        Debug.Log("no path found");
        if (state == AIState.HUNT)
        {
            SwapToInvisible();
        }
        else if (state == AIState.WANDER)
        {
            Vector3 newPos;
            bool pathway = false;
            if (CheckMoveThroughBlock(block, collision.contacts[0].point, out newPos, out pathway))
            {
                EnterBurrowState(newPos);
                Debug.Log("enemy deciding to burrow");
            }
            else
            {
                if (!pathway)
                {
                    SetNewWanderLocation();
                    Debug.Log("enemy giving up on current in-wall position");
                }
            }
        }

        
    }

    private bool CheckMoveThroughBlock(Block block, Vector3 collisionPos, out Vector3 newMovePos, out bool viablePathway)
    {
        Debug.Log("checking for burrow");
        viablePathway = true;
        newMovePos = Vector3.zero;
        Vector3 moveVector = wanderDestination - transform.position;
        moveVector = new Vector3(moveVector.x, 0, moveVector.z);
        Vector3 blockVector = collisionPos - transform.position;
        blockVector = new Vector3(blockVector.x, 0, blockVector.z);
        if (Vector3.Angle(moveVector, blockVector) > burrowAngle)
        {
            Debug.Log("invalid angle");
            return false;
        }

        if (moveVector.magnitude < burrowDistance)
        {
            Debug.Log("invalid distance");
            return false;
        }
        viablePathway = false;
        if (Mathf.Abs(moveVector.x) > Mathf.Abs(moveVector.z))
        {
            if (moveVector.x > 0)
            {
                return block.CanWalkThrough(maxBurrowBlocks, Block.Direction.LEFT, out newMovePos);
            }
            else
            {
                return block.CanWalkThrough(maxBurrowBlocks, Block.Direction.RIGHT, out newMovePos);
            }
        }
        else
        {
            if (moveVector.z > 0)
            {
                return block.CanWalkThrough(maxBurrowBlocks, Block.Direction.FRONT, out newMovePos);
            }
            else
            {
                return block.CanWalkThrough(maxBurrowBlocks, Block.Direction.BACK, out newMovePos);
            }
        }

    }
}
