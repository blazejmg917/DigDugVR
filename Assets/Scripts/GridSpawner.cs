using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class GridSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ListWrapper<T> {
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
        public int Count {
            get
            {
                return list.Count;
            }
        }
        public void Add(T val) {
            list.Add(val);
        }

        public void RemoveAt(int i) {
            list.RemoveAt(i);
        }
    }

    [SerializeField, Tooltip("the prefab of the basic block to spawn")]
    private GameObject baseBlockPrefab;
    [SerializeField, Tooltip("the prefab for the outer walls blocks to spawn")]
    private GameObject wallsPrefab;
    [SerializeField, Tooltip("a list of the materials to assign to the blocks, should be listed in order from entry to deepest")]
    private List<Material> blockMaterials;
    [SerializeField, Tooltip("the grid")] 
    private List<ListWrapper<Block>> grid = new List<ListWrapper<Block>>();
    [SerializeField, Tooltip("the controller for the navmesh surface")]
    private NavMeshSurfaceController surfaceController;
    //[SerializeField, Tooltip("the parent transform to spawn objects under")]private Transform gridParent
    [Header("Spawning settings")]
    [SerializeField, Tooltip("the starting point for blocks to spawn. should be placed right at the center tunnel entrance")]
    private Vector3 spawnStart;
    [SerializeField, Tooltip("the width of block you want to spawn")]
    private int gridWidth = 13;
    [SerializeField, Tooltip("the depth of the blocks to spawn")]
    private int gridDepth = 16;
    [Header("Gem settings")]
    [SerializeField, Tooltip("the minimum depth that a gem can spawn at")]private int minGemDepth = 9;
    [SerializeField, Tooltip("if the gem can spawn in a space adjacent to a tunnel")]private bool allowGemNextToTunnel = true;

    private Sensor sensor;

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

    public Block AssignGem(){
        List<Block> randOptions = new List<Block>();
        for(int i = minGemDepth; i < grid.Count - 1; i++){
            for(int j = 1; j < grid[i].Count - 1;j++){
                if(grid[i][j]){
                    if(allowGemNextToTunnel || grid[i][j].AdjacentToTunnel()){
                        randOptions.Add(grid[i][j]);
                    }
                }
            }
        }
        int randSelection = Random.Range(0, randOptions.Count);
        randOptions[randSelection].AssignGem();
        sensor.SetTarget(randOptions[randSelection].transform);
        return randOptions[randSelection];
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
                if (j == gridWidth / 2) // cam code here, may be awful / not centered correctly
                {
                    StudioEventEmitter emitter = thisBlock.AddComponent<FMODUnity.StudioEventEmitter>();
                    emitter.EventReference = EventReference.Find("event:/Ambience/Cavenoise");
                    emitter.Play();
                    EventInstance masterReverbSnapshot = FMODUnity.RuntimeManager.CreateInstance("snapshot:/MasterReverbControl");
                    //masterReverbSnapshot.set3DAttributes(attributes);
                    masterReverbSnapshot.start();
                }
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
        return new Vector3((width - (gridWidth / 2)) * offset.x, spawnStart.y, depth * offset.z);
    }

    public void SetMaterial(Block block, int depth){
        if(blockMaterials == null || blockMaterials.Count < 1){
            Debug.LogError("TRYING TO SET BLOCK MATERIALS WITH NO MATERIALS ASSIGNED IN SPAWNER \nASSIGN MATERIALS TO GRID SPAWNER");
            return;
        }
        int depthSize = gridDepth / blockMaterials.Count;
        int depthLevel = depth / depthSize;
        block.SetMaterial(blockMaterials[depthLevel]);

        // then also set the block dig sounds correctly
        block.GetComponent<FMODUnity.StudioEventEmitter>().EventInstance.setParameterByName("DirtType", depthLevel); // this is not working yet
        
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

    public void OnBlockBroken(Block block)
    {
        Debug.Log("on block broken");
        if (surfaceController)
        {
            surfaceController.RegenerateSurface();
        }
        if(block.HasGem()){
            GameManager.Instance.SpawnGem(block.transform);
        }
    }

    public int GetDefaultSurfaceID()
    {
        return surfaceController.GetDefaultID();
    }

    public int GetInvisibleSurfaceID()
    {
        return surfaceController.GetInvisID();
    }

    public void SetSensor(Sensor s)
    {
        sensor = s;
    }
}
