using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PumpHandle : MonoBehaviour
{
    [SerializeField, Tooltip("the point on the handle to be used when checking location for pumping")]
    private Transform handlePoint;
    [SerializeField, Tooltip("the starting point for the pump, and the final position to complete the pump")]
    private Transform restingPosition;
    [SerializeField, Tooltip("the fully pulled back position for the pump")]
    private Transform pulledBackPosition;

    [SerializeField, Tooltip("the pump script")]
    private Pump pump;
    //true if the pump has been pulled back but not yet released
    private bool primed = false;

    [SerializeField, Tooltip("this handle's rigidbody")]private Rigidbody rb;
    // [SerializeField, Tooltip("the standard rigidbodyconstraints for when the pump isn't held")]private RigidbodyConstraints standardConstraints = RigidbodyConstraints.FreezeAll;
    [SerializeField, Tooltip("the  rigidbodyconstraints for when the pump is held")]private RigidbodyConstraints heldConstraints = RigidbodyConstraints.FreezeAll & ~RigidbodyConstraints.FreezePositionX;

    void Start(){
        if(!rb){
            rb = GetComponent<Rigidbody>();
            if(!rb){
                rb = gameObject.AddComponent<Rigidbody>();
            }
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!primed && Vector3.Distance(handlePoint.position, pulledBackPosition.position) <= .001f)
        {
            primed = true;
        }
        else if (primed && Vector3.Distance(handlePoint.position, restingPosition.position) <= .001f)
        {
            primed = false;
            pump.CompletePump();
        }
    }

    /// <summary>
    /// Called when the player lets go of the pump handle
    /// </summary>
    /// <param name="_"></param>
    public void OnRelease(SelectExitEventArgs _)
    {
        if (primed)
        {
            transform.position = pulledBackPosition.position;
        }
        else
        {
            transform.position = restingPosition.position;
        }
    }

    /// <summary>
    /// called when the player grabs the pump handle
    /// </summary>
    /// <param name="_"></param>
    public void OnPickup(SelectEnterEventArgs _)
    {
        //make moveable
    }
}
