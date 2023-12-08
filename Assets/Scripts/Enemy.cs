using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField, Tooltip("this enemy's max health")]
    private int maxHealth = 3;
    //this enemy's current health
    private int health = 0;
    //if this enemy is currently stuck with a pump
    private bool stuckWithPump = false;
    [Header("Navmesh")]
    [SerializeField, Tooltip("navmesh agent")]private NavMeshAgent agent;

    [SerializeField, Tooltip("the default enemy type, cannot walk through walls in this form")]
    private int defaultNavMeshAgentType = 0;
    [SerializeField, Tooltip("the invisible enemy type, can walk through walls in this form")]
    private int invisibleNavMeshAgentType = 1;
    [Header("enemy visuals")]
    [SerializeField, Tooltip("the default enemy appearance")]private GameObject normalEnemyAppearance;
    [SerializeField, Tooltip("the enemy appearance when they're invisible")]private GameObject invisibleEnemyAppearance;
    // Start is called before the first frame update
    void Awake()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// called to deal damage to this enemy. If it reaches 0 health, it will die
    /// </summary>
    /// <param name="damage">the amount of damage dealt to this enemy. set to 1 by default</param>
    public void Damage(int damage = 1)
    {
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
        Destroy(gameObject);
    }

    /// <summary>
    /// called to set whether the enemy is currently stuck with a pump nozzle or not. Will determine whether they can take damage from pumps or not
    /// </summary>
    /// <param name="stuck">if the enemy has been stuck</param>
    public void SetStuck(bool stuck)
    {
        stuckWithPump = stuck;
    }

    public void SwapToInvisible()
    {
        agent.agentTypeID = invisibleNavMeshAgentType;
        normalEnemyAppearance.SetActive(false);
        invisibleEnemyAppearance.SetActive(true);
    }

    public void SwapToVisible()
    {
        agent.agentTypeID = defaultNavMeshAgentType;
        normalEnemyAppearance.SetActive(true);
        invisibleEnemyAppearance.SetActive(false);
    }
}
