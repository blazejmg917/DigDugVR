using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Holster : MonoBehaviour
{
    public void SetToolOnSocket(HoverEnterEventArgs args){
        HolsterTool tool = args.interactableObject.transform.GetComponent<HolsterTool>();
        if(tool){
            tool.SetPrimed(true);
        }
    }

    public void SetToolNotOnSocket(HoverExitEventArgs args){
        HolsterTool tool = args.interactableObject.transform.GetComponent<HolsterTool>();
        if(tool){
            tool.SetPrimed(false);
        }
    }
}
