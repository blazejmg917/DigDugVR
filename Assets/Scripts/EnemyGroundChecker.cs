using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundChecker : MonoBehaviour
{
    [SerializeField, Tooltip("the main enemy script")]private Enemy enemy;
    [SerializeField, Tooltip("the number of blocks this enemy is colliding with")]private int numCollidedBlocks = 0;
    // Start is called before the first frame update
    public void Reset(){
        numCollidedBlocks = 0;
    }

    void Start(){
        if(!enemy){
            enemy = transform.parent.parent.GetComponent<Enemy>();
        }
        Reset();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.GetComponent<Block>()){
            numCollidedBlocks++;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.GetComponent<Block>()){
            numCollidedBlocks--;
            if(numCollidedBlocks <= 0){
                if(enemy){
                    enemy.ExitGround();
                }
            }
        }
    }
}
