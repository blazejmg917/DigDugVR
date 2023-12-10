using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshSurfaceController : MonoBehaviour
{
    [SerializeField, Tooltip("the default navmeshsurface that allows blocks to stop enemies")]
    private NavMeshSurface defaultSurface;
    [SerializeField, Tooltip("the secondary navmeshsurface that doesn't allow blocks to stop enemies")]
    private NavMeshSurface invisibleSurface;
    // Start is called before the first frame update
    void Start()
    {
        RegenerateSurface(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegenerateSurface(bool generateSecondary = false)
    {
        Debug.Log("attempt to rebuild surface");
        
        if(generateSecondary){
            defaultSurface.BuildNavMesh();
            invisibleSurface.BuildNavMesh();

        }
        else{
            Invoke("BuildNavMesh", .5f);
        }
    }

    public void BuildNavMesh()
    {
        if (defaultSurface != null)
        {
            Debug.Log("pt 2");
            //Debug.Log(defaultSurface.UpdateNavMesh(defaultSurface.navMeshData).progress);
            defaultSurface.UpdateNavMesh(defaultSurface.navMeshData);
        }
    }
}