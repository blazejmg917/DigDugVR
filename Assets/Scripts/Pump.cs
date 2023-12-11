using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Pump : MonoBehaviour
{
    [SerializeField, Tooltip("The nozzle of the pump that is shot towards enemies")]
    private PumpNozzle pumpNozzle;
    [SerializeField, Tooltip("the pump handle")]private PumpHandle pumpHandle;

    [SerializeField, Tooltip("the attach point of the nozzle")]
    private Transform nozzleAttach;
    //if the nozzle is currently attached to the pump
    private bool isPumpAttached = false;

    [SerializeField, Tooltip("the speed at which the nozzle is launched")]
    private float nozzleLaunchSpeed = 10f;
    [SerializeField, Tooltip("the rigidbody for the pump")]private Rigidbody rb;
    [Header("debug")]
    [SerializeField, Tooltip("if the debug processes should run")]bool runDebug = false;
    [SerializeField]private ParticleSystem fireParticlesDebug;
    [SerializeField]private ParticleSystem activateParticlesDebug;
    [SerializeField]private ParticleSystem halfPumpParticlesDebug;
    [SerializeField]private ParticleSystem fullPumpParticlesDebug;
    //[SerializeField, Tooltip("the ")]
    // Start is called before the first frame update
    void Start()
    {
        pumpNozzle.SetPump(this, nozzleAttach);
        rb = GetComponent<Rigidbody>();
        pumpHandle.setGrabbable(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// gets the attach transform for the nozzle
    /// </summary>
    public Transform getNozzleAttach()
    {
        return nozzleAttach;
    }

    /// <summary>
    /// called when the nozzle reattaches to the pump
    /// </summary>
    public void OnNozzleReattach()
    {
        isPumpAttached = true;
    }

    /// <summary>
    /// fires the nozzle out from the pump if it is currently attached
    /// </summary>
    public void FireNozzle()
    {
        if(runDebug && activateParticlesDebug){
            activateParticlesDebug.Play();
        }
        if (isPumpAttached)
        {
            if(runDebug && fireParticlesDebug){
                fireParticlesDebug.Play();
            }

            pumpNozzle.Shoot(nozzleAttach.forward * -nozzleLaunchSpeed);
            isPumpAttached = false;
        }
    }

    /// <summary>
    /// force the nozzle to release from an enemy it is currently stuck in
    /// </summary>
    public void ForceReleaseNozzle()
    {
        pumpNozzle.ForceRelease();
    }

    /// <summary>
    /// called whenever the pump handle completes a pump action. used to deal damage to enemies
    /// </summary>
    public void CompletePump()
    {
        if(runDebug && fullPumpParticlesDebug){
            fullPumpParticlesDebug.Play();
        }
        pumpNozzle.Pump();
    }

    /// <summary>
    /// Called when the pump is picked up.
    /// </summary>
    /// <param name="args"></param>
    public void OnPickup(SelectEnterEventArgs args)
    {
        rb.constraints = RigidbodyConstraints.None;
        pumpHandle.setGrabbable(true);
    }
    /// <summary>
    /// Called when the pump is picked up.
    /// </summary>
    /// <param name="args"></param>
    public void OnRelease(SelectExitEventArgs args)
    {
        pumpHandle.setGrabbable(false);
    }
}
