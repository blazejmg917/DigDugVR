using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField, Tooltip("this enemy's max health")]
    private int maxHealth = 3;
    //this enemy's current health
    private int health = 0;
    //if this enemy is currently stuck with a pump
    private bool stuckWithPump = false;
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
}
