using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HolsterTool : MonoBehaviour
{
    [SerializeField, Tooltip("if this object is held")]private bool held;
    [SerializeField, Tooltip("if the object is primed")]private bool primed;
    [SerializeField, Tooltip("the XR Interactable")]private XRBaseInteractable interactable;

    void Start(){
        if(!interactable){
            interactable = GetComponent<XRBaseInteractable>();
        }
    }

    public void Grab(SelectEnterEventArgs args){
        held = true;
        interactable.interactionLayers = InteractionLayerMask.GetMask(new string[]{"Held Tool", "Default"});
    }
    public void Grab(){
        held = true;
        interactable.interactionLayers = InteractionLayerMask.GetMask(new string[]{"Held Tool", "Default"});
    }
    public void Release(SelectExitEventArgs args){
        held = false;
        if(!IsPrimed()){
            interactable.interactionLayers = InteractionLayerMask.GetMask(new string[]{"Default"});
        }
    }

    public bool IsHeld(){
        return held;
    }

    public void SetPrimed(bool isPrimed){
        primed = isPrimed;
        if(!isPrimed && !held){
            interactable.interactionLayers = InteractionLayerMask.GetMask(new string[]{"Default"});
        }
    }

    public bool IsPrimed(){
        return primed;
    }
}
