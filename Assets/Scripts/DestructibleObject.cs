using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DestructibleObject : MonoBehaviour
{
    [SerializeField, Tooltip("if this block can be broken")]private bool canBreak = true;
    [SerializeField, Tooltip("the maximum durability of this object. This will break once it reaches 0")]
    private int maxDurability = 3;
    //current durability
    [ReadOnly]private int durabilty;
    [SerializeField, Tooltip("the velocity of swing needed to break this object")]
    private float breakVelocity = 3;

    [SerializeField, Tooltip("the Block component for this object")]
    private Block block;
    [SerializeField, Tooltip("the collider for this object")]private Collider col;
    [SerializeField, Tooltip("the renderer for this object")]private Renderer render;
    [Header("Particles")]
    [SerializeField, Tooltip("the particle system that will be played when this object is damaged")]
    private ParticleSystem damageParticles;
    [SerializeField, Tooltip("the particle system that will be played when this object is destroyed")]
    private ParticleSystem destroyParticles;
    [Header("Haptics")]
    [SerializeField, Tooltip("the haptic for hitting this object")]
    private HapticSource hitHaptic;
    [SerializeField, Tooltip("the haptic for breaking this object")]
    private HapticSource breakHaptic;
    //[Header("Audio")]
    //put audio for block break here

    [SerializeField]
    private int blockType;

    // Start is called before the first frame update
    void Awake()
    {
        durabilty = maxDurability;
    }

    void Start()
    {
        if (!block)
        {
            block = GetComponent<Block>();
        }
        if(!col){
            col = GetComponent<Collider>();
        }
        if(!render){
            render = GetComponent<Renderer>();
        }

        block.GetComponent<FMODUnity.StudioEventEmitter>().EventReference = FMODUnity.RuntimeManager.PathToEventReference("event:/Shovel/DigDirt " + blockType);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// called to deal damage to this object. If it reaches 0 durability, it will break
    /// </summary>
    /// <param name="damage">the amount of damage dealt to this object. set to 1 by default</param>
    public void TakeDamage(int damage = 1, Shovel shovel = null)
    {
        Debug.Log("Taking Damage");
        GetComponent<FMODUnity.StudioEventEmitter>().Play();
        if (canBreak){
            durabilty -= 1;
            if (durabilty <= 0)
            {
                Break(shovel);
            }
            else{
                if(shovel && hitHaptic){
                    shovel.ReceiveHaptic(hitHaptic);
                }
            }
        }
        else{
            if(shovel && hitHaptic){
                shovel.ReceiveHaptic(hitHaptic);
            }
        }
    }

    /// <summary>
    /// called when the object hits zero durability and breaks. will destroy the object
    /// </summary>
    public void Break(Shovel shovel = null)
    {
        if(shovel && breakHaptic){
            shovel.ReceiveHaptic(breakHaptic);
        }

        if (block)
        {
            block.OnBreak();
        }
        if(shovel){
            destroyParticles.Play(true);
            destroyParticles.transform.parent = null;
        }
        if(!render){
            render = GetComponent<Renderer>();
        }
        if(render){
            render.enabled = false;
        }
        if(!col){
            col = GetComponent<Collider>();
        }
        if(col){
            col.enabled = false;
        }
        //Destroy(gameObject);
    }

    public void Unbreak(){
        render.enabled = true;
        col.enabled = true;
        if(block){
            block.SetBroken(false);
        }
    }

    public void OnCollisionEnter(Collision col){
        Shovel shovel = col.collider.gameObject.GetComponent<Shovel>();
        //Debug.Log("object hit, " + (col.relativeVelocity.magnitude >= breakVelocity) + ", " + shovel);
        
        if(col.relativeVelocity.magnitude >= breakVelocity && shovel && shovel.CanBreak(gameObject, col.contacts[0].point)){
            if (canBreak)
            {
                damageParticles.transform.position = col.contacts[0].point;
                damageParticles.Play(true);
            }
            
            TakeDamage();
        }
    }

    public void SetParticleMaterial(Material mat1, Material mat2) 
    {
        damageParticles.GetComponent<ParticleSystemRenderer>().material = mat1;
        damageParticles.GetComponentInChildren<ParticleSystemRenderer>().material = mat2;
        destroyParticles.GetComponent<ParticleSystemRenderer>().material = mat1;
        destroyParticles.GetComponentInChildren<ParticleSystemRenderer>().material = mat2;
    }

    public void SetBlockType(int a )
    {
        blockType = a;
    }

    public bool CanBreak(){
        return canBreak;
    }
}
