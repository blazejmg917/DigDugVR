using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBelt : MonoBehaviour
{
    [SerializeField, Tooltip("the character controller to follow (within xr origin)")]
    private CharacterController followController;
    private Transform followPosition;
    [Header("height")]
    [SerializeField, Tooltip("if this should adjust to height as well")]
    private bool followHeight = true;
    [SerializeField, Tooltip("if following height, what percentage of the controller's height this should follow at (.5 is halfway up, .25 is close to the ground, .75 is close to the head, etc.)")]
    private float followHeightPercent = .6f;
    [Header("rotation")]
    [SerializeField, Tooltip("if this should try to follow camera rotation as well so turning left and right doesn't mess up toolbelt positions")]
    private bool followRotation = true;
    [SerializeField, Tooltip("the camera offset to be used to track rotation")]
    private GameObject mainCamera;
    [SerializeField, Tooltip("the maximum angle allowed before this should start to rotate towards the camera")]
    private float turnAngle = 30;


    // Start is called before the first frame update
    void Start()
    {
        followPosition = followController.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float yVal;
        if(followHeight){
            yVal = followController.center.y + ((followController.height * followHeightPercent) - (followController.height / 2));
        }
        else{
            yVal = transform.localPosition.y;
        }
        transform.localPosition = new Vector3(followController.center.x, yVal, followController.center.z);

        if(followRotation){
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraHeading = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            float angleVar = Vector3.SignedAngle(transform.forward, cameraHeading, transform.up);
            if(Mathf.Abs(angleVar) > turnAngle){
                RotateSelf(angleVar, turnAngle);
            }
        }
    }

    private void RotateSelf(float angleVar, float threshold){
        float turnAngle = Mathf.MoveTowards(angleVar, 0, threshold);
        transform.RotateAround(transform.position,transform.up,turnAngle);
    }
}
