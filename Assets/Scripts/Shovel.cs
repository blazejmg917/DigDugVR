using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Shovel : MonoBehaviour
{
    [SerializeField, Tooltip("the speed the shovel needs to be swinging at to damage a block")]
    private float minSwingSpeed = 5f;
    [SerializeField, Tooltip("the destructible layer")]
    private LayerMask destructibleLayer;
    //if this shovel is currently being held by the player
    private bool held;

    [SerializeField,Tooltip("how far away from the original point of collision this object has to go before it can damage it again")]
    private float breakRefreshDist = 1f;
    //used to prevent jiggling to break blocks
    private bool breakRefreshed = true;
    private Vector3 lastHitPositon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!breakRefreshed && Vector3.Distance(transform.position, lastHitPositon) > breakRefreshDist)
        {
            breakRefreshed = true;
        }
    }

    /// <summary>
    /// Called when the shovel is picked up. Used to know if the shovel can dig blocks or not
    /// </summary>
    /// <param name="_"></param>
    public void OnPickup(SelectEnterEventArgs _)
    {
        held = true;
    }

    /// <summary>
    /// Called when the shovel is released. Used to know if the shovel can dig blocks or not
    /// </summary>
    /// <param name="_"></param>
    public void OnRelease(SelectExitEventArgs _)
    {
        held = false;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.relativeVelocity.magnitude >= minSwingSpeed && col.gameObject.layer == destructibleLayer)
        {
            col.gameObject.GetComponent<DestructibleObject>().TakeDamage();
            breakRefreshed = false;
            lastHitPositon = col.contacts[0].point;
        }
    }
}
