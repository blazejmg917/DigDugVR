using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HapticSource : MonoBehaviour
{
    [SerializeField, Range(0,1), Tooltip("the intensity of the haptic")]private float intensity;
    [SerializeField, Tooltip("the duration of the haptic")]private float duration;
        
    public void TriggerHaptic(XRBaseController controller){
        if(intensity > 0){
            controller.SendHapticImpulse(intensity, duration);
        }
    }

    public float GetIntensity(){
        return intensity;
    }

    public float GetDuration(){
        return duration;
    }

}
