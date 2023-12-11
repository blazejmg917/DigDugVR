using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum Direction{
        RIGHT,
        LEFT,
        FRONT,
        BACK
    }
    [SerializeField, Tooltip("a reference to the block to the left of this one (when facing directly deeper)")]private Block leftBlock;
    [SerializeField, Tooltip("a reference to the block to the right of this one (when facing directly deeper)")]private Block rightBlock;
    [SerializeField, Tooltip("a reference to the block to the back of this one (when facing directly deeper)")]private Block backBlock;
    [SerializeField, Tooltip("a reference to the block to the front of this one (when facing directly deeper)")]private Block frontBlock;
    [SerializeField, Tooltip("whether or not the enemies can move through this block")]private bool enemyWalkable = true;
    private Vector3 size;

    [SerializeField, Tooltip("if this block has the goal gem. will always be set to false on game start before random assignment")]private bool hasGem = false;
    [SerializeField, Tooltip("if this block is at the surface. If so, enemies will not be able to burrow through it while wandering")]private bool isSurface = false;
    
    public void Awake(){
        hasGem = false;
    }

    public bool CanMoveThrough(){
        return enemyWalkable;
    }

    public void AssignBlocks(Block left, Block right, Block back, Block front){
        leftBlock = left;
        rightBlock = right;
        backBlock = back;
        frontBlock = front;
    }

    public bool AdjacentToTunnel(){
        return leftBlock && rightBlock && backBlock && frontBlock;
    }

    public void AssignGem(){
        hasGem = true;
    }

    public void SetMaterial(Material material){
        GetComponent<Renderer>().material = material;
    }

    public void setLeft(Block left){
        leftBlock = left;
    }
    public void setRight(Block right){
        rightBlock = right;
    }
    public void setBack(Block back){
        backBlock = back;
    }
    public void setFront(Block front){
        frontBlock = front;
    }

    public void OnBreak(){
        Debug.Log("on block break");
        if(leftBlock)
            leftBlock.setRight(null); 
        if (rightBlock)
            rightBlock.setLeft(null);
        if (backBlock)
            backBlock.setFront(null);
        if (frontBlock)
            frontBlock.setBack(null);
        GridSpawner.Instance.OnBlockBroken(this);
    }

    public Vector3 GetSize(){
        if(size == Vector3.zero){
            size = GetComponent<Collider>().bounds.size;
        }
        return size;
    }

    public Vector3 GetSize2D(){
        if(size == Vector3.zero){
            size = GetComponent<Collider>().bounds.size;
        }
        return new Vector3(size.x, 0, size.z);
    }
    public Vector3 GetSize2DFlipX(){
        if(size == Vector3.zero){
            size = GetComponent<Collider>().bounds.size;
        }
        return new Vector3(-size.x, 0, size.z);
    }
    public Vector3 GetSize2DFlipZ(){
        if(size == Vector3.zero){
            size = GetComponent<Collider>().bounds.size;
        }
        return new Vector3(size.x, 0, -size.z);
    }
    

    public bool CanWalkThrough(int maxBlocksToMoveThrough, Direction direction, out Vector3 othersidePos){
        othersidePos = Vector3.zero;
        if(!enemyWalkable || maxBlocksToMoveThrough == 0){
            return false;
        }
        
        switch(direction){
            case Direction.RIGHT:
                if(leftBlock == null){
                    othersidePos = transform.position + new Vector3(-size.x,0,0);
                    return true;
                }
                return leftBlock.CanWalkThrough(maxBlocksToMoveThrough - 1, direction, out othersidePos);
            case Direction.LEFT:
                if(rightBlock == null){
                    othersidePos = transform.position + new Vector3(size.x,0,0);
                    return true;
                }
                return rightBlock.CanWalkThrough(maxBlocksToMoveThrough - 1, direction, out othersidePos);
            case Direction.FRONT:
                if(backBlock == null){
                    othersidePos = transform.position + new Vector3(0,0,size.z);
                    return true;
                }
                return backBlock.CanWalkThrough(maxBlocksToMoveThrough - 1, direction, out othersidePos);
            case Direction.BACK:
                if (isSurface)
                {
                    return false;
                }
                if(frontBlock == null){
                    othersidePos = transform.position + new Vector3(0,0,-size.z);
                    return true;
                }
                return frontBlock.CanWalkThrough(maxBlocksToMoveThrough - 1, direction, out othersidePos);
            default:
                return false;
                
        }
    }

    public bool HasGem(){
        return hasGem;
    }
}
