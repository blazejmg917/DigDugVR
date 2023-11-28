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

    public void Damage(int damage = 1)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void SetStuck(bool stuck)
    {
        stuckWithPump = stuck;
    }
}
