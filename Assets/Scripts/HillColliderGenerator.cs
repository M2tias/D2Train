using UnityEngine;

public class HillColliderGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform t in transform)
        {
            if (t.name.Contains("Hill"))
            {
                MeshCollider col = t.gameObject.AddComponent<MeshCollider>();
                col.convex = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
