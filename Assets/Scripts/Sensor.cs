using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    [SerializeField]
    private Gemstone gemstone;

    [SerializeField]
    private ExitZone exit;

    private MonoBehaviour currentTarget;

    [SerializeField]
    private UnityEngine.UI.Image readyGraphic;
    [SerializeField]
    private UnityEngine.UI.Image arrow;
    [SerializeField]
    private UnityEngine.UI.Image chargeGraphic;

    enum Status { READY, IN_USE, CHARGING };

    [SerializeField]
    private Status status = Status.READY;

    [SerializeField]
    private float arrowDisplayTime = 3.0f;

    [SerializeField]
    private float chargeTime; // 12.0f

    public bool bruh = false;

    // Start is called before the first frame update
    void Start()
    {
        currentTarget = gemstone;
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
    void Ping()
    {
        // TODO: make the button press noise
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
    }

    private void EndCharge()
    {
        status = Status.READY;
        chargeGraphic.enabled = false;
        readyGraphic.enabled = true;

        // TODO set material texture of top bulb to green
    }
}
