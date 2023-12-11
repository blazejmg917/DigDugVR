using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOn : MonoBehaviour
{
    public GameObject leftController;
    public GameObject rightController;
    // Start is called before the first frame update
    void Start()
    {
        leftController.SetActive(true);
        rightController.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
