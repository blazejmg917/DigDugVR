using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField, Tooltip("the gem prefab")]private GameObject GemPrefab;
    [SerializeField, Tooltip("A reference to the sensor")]private Sensor sensor;
    [SerializeField, Tooltip("gem block for easy reference")]private Block gemBlock;
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
        if (!sensor)
        {
            sensor = FindObjectOfType<Sensor>();
        }
        if (sensor) {
            GridSpawner.Instance.SetSensor(sensor);
        }
        
        gemBlock = GridSpawner.Instance.AssignGem(); 
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
}
