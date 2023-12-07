using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ListWrapper<T>{
        public List<T> list = new List<T>();
        public T this[int key]
        {
            get
            {
                return list[key];
            }
            set
            {
                list[key] = value;
            }
        }
        public int Count{
            get
            {
                return list.Count;
            }
        }
        public void Add(T val){
            list.Add(val);
        }

        public void RemoveAt(int i){
            list.RemoveAt(i);
        }
    }

    [SerializeField, Tooltip("the prefab of the basic block to spawn")]private GameObject baseBlockPrefab;
    [SerializeField, Tooltip("the prefab for the outer walls blocks to spawn")]private GameObject wallsPrefab;
    [SerializeField, Tooltip("a list of the materials to assign to the blocks, should be listed in order from entry to deepest")]private List<Material> blockMaterials;
    [SerializeField, Tooltip("the grid")]private List<ListWrapper<Block>> grid = new List<ListWrapper<Block>>();
    [Header("Spawning settings")]
    [SerializeField, Tooltip("the starting point for blocks to spawn. should be placed right at the center tunnel entrance")]private Vector3 spawnStart;
    [SerializeField, Tooltip("the width of block you want to spawn")]private int gridWidth = 13;
    [SerializeField, Tooltip("the depth of the blocks to spawn")]private int gridDepth = 16;

    private static GridSpawner _instance;
    public static GridSpawner Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GridSpawner>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<GridSpawner>();
                    Debug.Log("Generating new GridSpawner");
                }
            }
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnGrid(){
        ClearGrid();
        Vector3 offset = baseBlockPrefab.GetComponent<Block>().GetSize2D();
        //int center = gridWidth / 2;
        //each row of the grid
        for(int i = 0; i < gridDepth; i++){
            ListWrapper<Block> thisList = new ListWrapper<Block>();
            //the starting side wall
            GameObject wall = Instantiate(wallsPrefab, GetBlockSpawnPos(-1, i, offset), Quaternion.identity, transform);
            thisList.Add(wall.GetComponent<Block>());
            //grid[i].Add(wall.GetComponent<Block>());
            for(int j = 0; j < gridWidth; j++){
                
                GameObject thisBlock = Instantiate(baseBlockPrefab, GetBlockSpawnPos(j, i, offset), Quaternion.identity, transform);
                Block block = thisBlock.GetComponent<Block>();
                SetMaterial(block, i);
                thisList.Add(block);

            }
            //the ending side wall
            wall = Instantiate(wallsPrefab, GetBlockSpawnPos(gridWidth, i, offset), Quaternion.identity, transform);
            thisList.Add(wall.GetComponent<Block>());
            grid.Add(thisList);
        }
        //back wall
        {
            ListWrapper<Block> thisList = new ListWrapper<Block>();
            for(int j = -1; j <= gridWidth; j++){
                GameObject thisBlock = Instantiate(wallsPrefab, GetBlockSpawnPos(j, gridDepth, offset), Quaternion.identity, transform);
                thisList.Add(thisBlock.GetComponent<Block>());
            }
            grid.Add(thisList);
        }
        ConnectGrid();
    }

    public void ConnectGrid(){
        for(int i = 0; i < grid.Count; i++){
            for(int j = 0; j < grid[i].Count; j++){
                if(grid[i][j] == null){
                    return;
                }
                if(i > 0){
                    grid[i][j].setFront(grid[i-1][j]);
                }
                if(j > 0){
                    grid[i][j].setLeft(grid[i][j-1]);
                }
                if(i < grid.Count - 1){
                    grid[i][j].setBack(grid[i+1][j]);
                }
                if(j < grid[i].Count - 1){
                    grid[i][j].setRight(grid[i][j+1]);
                }
            }
            
        }
    }

    public Vector3 GetBlockSpawnPos(int width, int depth, Vector3 offset){
        return new Vector3((width - (gridWidth / 2)) * offset.x, 0, depth * offset.z);
    }

    public void SetMaterial(Block block, int depth){
        if(blockMaterials == null || blockMaterials.Count < 1){
            Debug.LogError("TRYING TO SET BLOCK MATERIALS WITH NO MATERIALS ASSIGNED IN SPAWNER \nASSIGN MATERIALS TO GRID SPAWNER");
            return;
        }
        int depthSize = gridDepth / blockMaterials.Count;
        int depthLevel = depth / depthSize;
        block.SetMaterial(blockMaterials[depthLevel]);
    }


    public void ClearGrid(bool destroyBlocks = true){
        for(int i = grid.Count - 1; i >= 0; i--){
            for(int j = grid[i].Count - 1; j >= 0; j--){
                if(grid[i][j] == null){
                    continue;
                }
                if(destroyBlocks){
                    DestroyImmediate(grid[i][j].gameObject);
                }
                grid[i].RemoveAt(j);
                
            }
            grid.RemoveAt(i);
            
        }
    }
}