
using UnityEngine;
using UnityEngine.AI;

public class BuildNavMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

  
}
