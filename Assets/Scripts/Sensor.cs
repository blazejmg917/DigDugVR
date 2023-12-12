using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using FMODUnity;

public class Sensor : MonoBehaviour
{
    [SerializeField]
    private ExitZone exit;

    [SerializeField]
    private Transform currentTarget;

    private MeshRenderer meshRenderer;
    [SerializeField]
    private FMODUnity.StudioEventEmitter pingSfx;
    [SerializeField]
    private FMODUnity.StudioEventEmitter buttonSfx;
    [SerializeField]
    private FMODUnity.StudioEventEmitter rechargeSfx;

    [SerializeField]
    private UnityEngine.UI.Image readyGraphic;
    [SerializeField]
    private UnityEngine.UI.Image arrow;
    [SerializeField]
    private UnityEngine.UI.Image chargeGraphic;

    [SerializeField]
    private Material bulbOn;
    [SerializeField]
    private Material bulbOff;

    enum Status { READY, IN_USE, CHARGING };
    [SerializeField]
    private Status status = Status.READY;

    [SerializeField]
    private float arrowDisplayTime = 3.0f;
    [SerializeField]
    private float chargeTime; // 12.0f

    private XRBaseController holdingController;
    //if this shovel is currently being held by the player
    private bool held;

    public bool bruh = false;

    // Start is called before the first frame update
    void Start()
    {
        GridSpawner.Instance.SetSensor(this);

        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // bruh is just the ping button
        if (bruh)
        {
            Ping();
            bruh = !bruh;
        }
    }

    /// <summary>
    /// Called when the player presses the button on the sensor.
    /// </summary>
    /// <param name="args"></param>
    public void Ping()
    {
        buttonSfx.Play();
        switch (status)
        {
            case Status.READY:
                status = Status.IN_USE; // prevent other calls from doing anything

                // use the gemstone's location to rotate the arrow...
                Vector3 dist = Vector3.ProjectOnPlane(currentTarget.transform.position - transform.position, Vector3.up);
                float degreesToRotate = Vector3.SignedAngle(Vector3.ProjectOnPlane(-transform.forward, Vector3.up), dist, Vector3.up);
                arrow.rectTransform.localEulerAngles = new Vector3(0f, 0f, degreesToRotate);
                
                // swap the images
                arrow.enabled = true;
                readyGraphic.enabled = false;

                // play the haptic
                if (held && holdingController)
                {
                    holdingController.SendHapticImpulse(0.5f, 0.3f);
                }

                // play the sound
                pingSfx.Play();

                // wait to turn the arrow off and set the state to charging
                Invoke(nameof(BeginCharge), arrowDisplayTime);

                // wait longer to reset to ready
                Invoke(nameof(EndCharge), arrowDisplayTime + chargeTime);
                break;

            case Status.IN_USE:
                // ignore input entirely, maybe that means this case can be deleted

                break;

            case Status.CHARGING:
                // error noise, maybe display "low power" icon on screen, etc

                break;
        }
        
    }

    private void BeginCharge()
    {
        status = Status.CHARGING;
        arrow.enabled = false; // hide the arrow
        chargeGraphic.enabled = true;

        // TODO set material texture of top bulb to red
        meshRenderer.material = bulbOff;
    }

    private void EndCharge()
    {
        status = Status.READY;
        chargeGraphic.enabled = false;
        readyGraphic.enabled = true;

        rechargeSfx.Play();

        // TODO set material texture of top bulb to green
        meshRenderer.material = bulbOn;
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }


    /// <summary>
    /// Called when the shovel is picked up. Used to know if the shovel can dig blocks or not
    /// </summary>
    /// <param name="args"></param>
    public void OnPickup(SelectEnterEventArgs args)
    {
        held = true;
        if (args.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            holdingController = controllerInteractor.xrController;
        }
    }

    /// <summary>
    /// Called when the shovel is released. Used to know if the shovel can dig blocks or not
    /// </summary>
    /// <param name="_"></param>
    public void OnRelease(SelectExitEventArgs _)
    {
        held = false;
        holdingController = null;
    }

}
