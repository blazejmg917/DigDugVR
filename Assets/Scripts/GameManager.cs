using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField, Tooltip("the gem prefab")]private GameObject GemPrefab;

    [SerializeField, Tooltip("A reference to the sensor")]private Sensor sensor;
    [SerializeField, Tooltip("gem block for easy reference")]private Block gemBlock;
    [SerializeField, Tooltip("if you should run the random generation")]private bool generateRandomLevel = true;
    [SerializeField, Tooltip("the fade in/out camera script")]private FadeCamera fadeCamera;
    [SerializeField, Tooltip("the script that handles player controllers on game end")]private PlayerDeath playerDeath;

    private bool LeavingLevel = false;
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<GameManager>();
                    Debug.Log("Generating new GameManager");
                }
            }
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // Generate Grid
        if(generateRandomLevel){
            GridSpawner.Instance.SpawnGrid();
        }

        if (!sensor)
        {
            sensor = FindObjectOfType<Sensor>();
        }
        if (sensor) {
            GridSpawner.Instance.SetSensor(sensor);
        }
        // Generate Tunnels
        if(generateRandomLevel){
            GridSpawner.Instance.GenerateTunnels();
            GridSpawner.Instance.GenerateEnemies();
        }
        gemBlock = GridSpawner.Instance.AssignGem(); 

        if(!playerDeath){
            playerDeath = FindObjectOfType<PlayerDeath>();
        }
        playerDeath.Deactivate();

        if(!fadeCamera){
            fadeCamera = FindObjectOfType<FadeCamera>();
        }
        StartCoroutine(fadeCamera.FadeFromBlack());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnGem(Transform gemSpawnPos){
        GameObject gemstone = Instantiate(GemPrefab,gemSpawnPos.position, Quaternion.identity);
        //gemstone.transform.position = new Vector3(0,2,0);
        Debug.Log("Gem spawned " + gemSpawnPos.position + gemstone);
    }

    public void WinAndReload()
    {
        // TODO: Jack's fade to black peekaboo ass transition

        // Clear existing grid and enemies

        // Generate Grid
        GridSpawner.Instance.SpawnGrid(); // need to have this populate enemies as well eventually

        if (!sensor)
        {
            sensor = FindObjectOfType<Sensor>(); // maybe auto make a new one if the sensor doesn't exist because they really need it honestly
        }
        if (sensor)
        {
            GridSpawner.Instance.SetSensor(sensor);
        }
        // Generate Tunnels
        if (generateRandomLevel)
        {
            GridSpawner.Instance.GenerateTunnels();
        }
        gemBlock = GridSpawner.Instance.AssignGem();
    }


    public void ReloadLevel(){
        if(LeavingLevel){
            return;
        }
        StartCoroutine(Reset());
    }

    public IEnumerator Reset(){
        yield return StartCoroutine(fadeCamera.FadeToBlack());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
