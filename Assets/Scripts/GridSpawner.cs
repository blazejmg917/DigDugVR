using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FMODUnity;

struct tunnelLocation
{
    public string Type;
    public int PosX;
    public int PosY;
    public int Width;

    public tunnelLocation(string type, int posx, int posy, int width)
    {
        Type = type;
        PosX = posx;
        PosY = posy;
        Width = width;
    }
}

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

    [SerializeField, Tooltip("a list of materials for the particle systems")]
    private List<Material> particleMats1;
    [SerializeField, Tooltip("a second list of materials for the particle systems")]
    private List<Material> particleMats2;

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

    public bool bruh = false;

    private List<tunnelLocation> tunnelLocations = new List<tunnelLocation>();

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
        // bruh is just the ping button
        if (bruh)
        {
            GenerateTunnels();
            bruh = !bruh;
        }
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
            GameObject wall = Instantiate(wallsPrefab, GetBlockSpawnPos(-1, i, offset), wallsPrefab.transform.rotation, transform);
            thisList.Add(wall.GetComponent<Block>());
            //grid[i].Add(wall.GetComponent<Block>());
            for(int j = 0; j < gridWidth; j++){
                
                GameObject thisBlock = Instantiate(baseBlockPrefab, GetBlockSpawnPos(j, i, offset), baseBlockPrefab.transform.rotation, transform);
                Block block = thisBlock.GetComponent<Block>();
                SetMaterial(block, i);
                thisList.Add(block);
                if(i == 0){
                    block.SetSurface(true);
                }
            }
            //the ending side wall
            wall = Instantiate(wallsPrefab, GetBlockSpawnPos(gridWidth, i, offset), wallsPrefab.transform.rotation, transform);
            thisList.Add(wall.GetComponent<Block>());
            grid.Add(thisList);
        }
        //back wall
        {
            ListWrapper<Block> thisList = new ListWrapper<Block>();
            for(int j = -1; j <= gridWidth; j++){
                GameObject thisBlock = Instantiate(wallsPrefab, GetBlockSpawnPos(j, gridDepth, offset), wallsPrefab.transform.rotation, transform);
                thisList.Add(thisBlock.GetComponent<Block>());
            }
            grid.Add(thisList);
        }
        ConnectGrid();
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
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
        DestructibleObject destruct = block.gameObject.GetComponent<DestructibleObject>();
        destruct.setParticleMaterial(particleMats1[depthLevel], particleMats2[depthLevel]);

        // then also set the block dig sounds correctly
        block.GetComponent<FMODUnity.StudioEventEmitter>().EventReference = EventReference.Find("event:/Shovel/DigDirt " + (depthLevel+1));
        
    }

    public void GenerateTunnels() {

        // First Populate the tunnelLocations
        tunnelLocations.Clear();
        tunnelLocations.Add(new tunnelLocation("H", -1, -1, 5));
        tunnelLocations.Add(new tunnelLocation("V", -1, -1, 5));
        tunnelLocations.Add(new tunnelLocation("H", -1, -1, 3));
        tunnelLocations.Add(new tunnelLocation("H", -1, -1, 3));
        tunnelLocations.Add(new tunnelLocation("V", -1, -1, 3));
        tunnelLocations.Add(new tunnelLocation("V", -1, -1, 3));

        // Now for each entry generate a random location for these tunnels to be located. We will be selecting the
        // center positions for these tunnels
        for(int i = 0; i < tunnelLocations.Count; i++) { 
            // Get random x and y positions. These positions are based on the indices of the grid array
            tunnelLocations[i] = new tunnelLocation(tunnelLocations[i].Type, Random.Range(1, gridDepth - 1), Random.Range(1, gridWidth - 1), tunnelLocations[i].Width);
        }

        // Now create the tunnels in the grid
        foreach(tunnelLocation location in tunnelLocations)
        {
            int centerDepth = location.PosX;
            int centerHorizontal = location.PosY;
            int tunnelSize = (location.Width / 2) + 1;
            Block blockReference = grid[centerDepth][centerHorizontal];

            switch (location.Type)
            {
                case "H":
                    if (!blockReference.IsBroken())
                    {
                        grid[centerDepth][centerHorizontal].GetComponent<DestructibleObject>().Break();
                    }
                    // Delete the Left Blocks
                    for(int left = 1; left < tunnelSize; left++) {
                        if ((blockReference = grid[centerDepth][centerHorizontal - left]).GetComponent<DestructibleObject>() == null)
                            break;
                        else if(!blockReference.IsBroken())
                        {
                            blockReference.GetComponent<DestructibleObject>().Break();
                        }
                    }
                    // Delete the Right Blocks
                    for (int right = 1; right < 3; right++) {
                        if ((blockReference = grid[centerDepth][centerHorizontal + right]).GetComponent<DestructibleObject>() == null)
                            break;
                        else if (!blockReference.IsBroken())
                        {
                            blockReference.GetComponent<DestructibleObject>().Break();
                        }
                    }
                    break;
                case "V":
                    if (!blockReference.IsBroken())
                    {
                        grid[centerDepth][centerHorizontal].GetComponent<DestructibleObject>().Break();
                    }
                    // Delete the Up Blocks
                    for (int up = 1; up < tunnelSize; up++)
                    {
                        // Since there are no walls at entrance, make sure we can't go out of bounds there
                        if (centerDepth - up < 0 || (blockReference = grid[centerDepth - up][centerHorizontal]).GetComponent<DestructibleObject>() == null)
                            break;
                        else if (!blockReference.IsBroken())
                        {
                            blockReference.GetComponent<DestructibleObject>().Break();
                        }
                    }
                    // Delete the Down Blocks
                    for (int down = 1; down < 3; down++)
                    {
                        if ((blockReference = grid[centerDepth + down][centerHorizontal]).GetComponent<DestructibleObject>() == null)
                            break;
                        else if (!blockReference.IsBroken())
                        {
                            blockReference.GetComponent<DestructibleObject>().Break();
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        

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
