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
    [Header("debug")]
    [SerializeField, Tooltip("if the debug processes should run")]bool runDebug = false;
    [SerializeField]private ParticleSystem fireParticlesDebug;
    [SerializeField]private ParticleSystem activateParticlesDebug;
    //[SerializeField, Tooltip("the ")]
    // Start is called before the first frame update
    void Start()
    {
        pumpNozzle.SetPump(this, nozzleAttach);
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

            pumpNozzle.Shoot(nozzleAttach.forward * nozzleLaunchSpeed);
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
        pumpNozzle.Pump();
    }

    /// <summary>
    /// Called when the pump is picked up.
    /// </summary>
    /// <param name="args"></param>
    public void OnPickup(SelectEnterEventArgs args)
    {
        
    }
    /// <summary>
    /// Called when the pump is picked up.
    /// </summary>
    /// <param name="args"></param>
    public void OnRelease(SelectExitEventArgs args)
    {
        
    }
}
