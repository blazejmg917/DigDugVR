using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField, Tooltip("teleport interactor")]private GameObject teleportMovement;
    [SerializeField, Tooltip("smooth movement")]private ActionBasedControllerManager smoothMovement;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("player collision with: " + other); 
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if(enemy && enemy.IsVisible()){
            Debug.Log("player touched enemy");
            Die();
        }
    }

    private void Die(){
        GameManager.Instance.ReloadLevel();
    }

    public void Deactivate(){
        if(teleportMovement){
            teleportMovement.SetActive(false);
        }
        if(smoothMovement){
            smoothMovement.smoothMotionEnabled = false;
        }
        
    }

    public void ActivatePlayer(){
        if(teleportMovement){
            teleportMovement.SetActive(true);
        }
        if(smoothMovement){
            smoothMovement.smoothMotionEnabled = true;
        }
    }
}
