using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [SerializeField, Tooltip("the maximum durability of this object. This will break once it reaches 0")]
    private int maxDurability = 3;
    //current durability
    private int durabilty;
    [Header("Particles")]
    [SerializeField, Tooltip("the particle system that will be played when this object is damaged")]
    private ParticleSystem damageParticles;
    [SerializeField, Tooltip("the particle system that will be played when this object is destroyed")]
    private ParticleSystem destroyParticles;
    //[Header("Audio")]
    //[SerializeField, Tooltip("the audio source for this ")]
    // Start is called before the first frame update
    void Awake()
    {
        durabilty = maxDurability;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// called to deal damage to this object. If it reaches 0 durability, it will break
    /// </summary>
    /// <param name="damage">the amount of damage dealt to this object. set to 1 by default</param>
    public void TakeDamage(int damage = 1)
    {
        durabilty -= 1;
        if (durabilty < 0)
        {
            Break();
        }
    }

    /// <summary>
    /// called when the object hits zero durability and breaks. will destroy the object
    /// </summary>
    public void Break()
    {
        Destroy(gameObject);
    }
}
