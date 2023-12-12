using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class ExitZone : MonoBehaviour
{
    private bool playerEnter = false;

    private bool gemstoneEnter = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerEnter && gemstoneEnter)
        {
            // call the game manager here
            GameManager.Instance.WinAndReload();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.GetComponent("Gemstone") as Gemstone) != null) // Check player collision
        {
            playerEnter = true;
        }

        if ((other.GetComponent("XROrigin") as XROrigin) != null) // Check gemstone collision
        {
            gemstoneEnter = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if ((other.GetComponent("Gemstone") as Gemstone) != null) // Check player collision
        {
            playerEnter = false;
        }

        if ((other.GetComponent("XROrigin") as XROrigin) != null) // Check gemstone collision
        {
            gemstoneEnter = false;
        }
    }
}
