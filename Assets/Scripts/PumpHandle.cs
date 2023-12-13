using FMODUnity;
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
    [SerializeField, Tooltip("the join for this object")]private ConfigurableJoint joint;
    [SerializeField, Tooltip("the grab interactable")]private XRGrabInteractable interactable;
    //true if the pump has been pulled back but not yet released
    [SerializeField]private bool primed = false;

    [SerializeField]private bool held = false;

    [SerializeField, Tooltip("this handle's rigidbody")]private Rigidbody rb;
    [Tooltip("the standard rigidbodyconstraints for when the pump isn't held")]private RigidbodyConstraints standardConstraints = RigidbodyConstraints.FreezeAll;
    [ Tooltip("the  rigidbodyconstraints for when the pump is held")]private RigidbodyConstraints heldConstraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

    [SerializeField]
    private StudioEventEmitter airIntake;

    void Start(){
        if(!rb){
            rb = GetComponent<Rigidbody>();
            if(!rb){
                rb = gameObject.AddComponent<Rigidbody>();
            }
        }
        if(!joint){
            joint = GetComponent<ConfigurableJoint>();
            
        }
        // if(joint){
        //     joint.xMotion = ConfigurableJointMotion.Locked;
        // }
        if(!interactable){
            interactable = GetComponent<XRGrabInteractable>();
        }
        Debug.Log("handle start " + standardConstraints + ", max dist = " + Vector3.Distance(restingPosition.position, pulledBackPosition.position) + ", current dist = " + Vector3.Distance(restingPosition.position, handlePoint.position));
        //rb.constraints = standardConstraints;

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(Vector3.Distance(handlePoint.position, pulledBackPosition.position));
        if (held && !primed && Vector3.Distance(handlePoint.position, pulledBackPosition.position) <= .01f)
        {
            Debug.Log("priming handle");
            primed = true;
            airIntake.Play();// play sound of air intake (could add velocity later but who cares)
        }
        else if (held && primed && Vector3.Distance(handlePoint.position, restingPosition.position) <= .01f)
        {
            Debug.Log("finishing pump");
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
        held = false;
        //rb.constraints = standardConstraints;
        transform.localRotation = Quaternion.identity;
        if (primed)
        {
            transform.position = pulledBackPosition.position + (transform.position - handlePoint.position);
        }
        else
        {
            transform.position = restingPosition.position + (transform.position - handlePoint.position);
        }
        //joint.xMotion = ConfigurableJointMotion.Locked;
        
    }

    /// <summary>
    /// called when the player grabs the pump handle
    /// </summary>
    /// <param name="_"></param>
    public void OnPickup(SelectEnterEventArgs _)
    {
        held = true;
        //rb.constraints = heldConstraints;
        //joint.xMotion = ConfigurableJointMotion.Limited;
        //make moveable
    }

    public void setGrabbable(bool grabbable){
        //interactable.enabled = grabbable;
    }
}
