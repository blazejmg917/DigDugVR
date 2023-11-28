using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Transform getNozzleAttach()
    {
        return nozzleAttach;
    }

    public void OnNozzleReattach()
    {
        isPumpAttached = true;
    }

    public void FireNozzle()
    {
        if (isPumpAttached)
        {
            pumpNozzle.Shoot(nozzleAttach.forward * nozzleLaunchSpeed);
            isPumpAttached = false;
        }
    }

    public void ForceReleaseNozzle()
    {
        pumpNozzle.ForceRelease();
    }

    public void CompletePump()
    {
        pumpNozzle.Pump();
    }
}
