using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Gemstone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Called when the gemstone is picked up. Used to know if the shovel can dig blocks or not
    /// </summary>
    /// <param name="args"></param>
    public void OnPickup(SelectEnterEventArgs args)
    {
        FindObjectOfType<Sensor>().SetTarget(FindObjectOfType<ExitZone>().transform);
    }
}
