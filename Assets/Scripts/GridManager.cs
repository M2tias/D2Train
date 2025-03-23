using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gridPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in transform)
        {
            GameObject grid = Instantiate(gridPrefab, child.position, child.rotation, transform);
            grid.transform.localScale = child.localScale;

            grid.GetComponent<MeshRenderer>().material.SetVector("_gridScale", new Vector2(child.localScale.x * 10f, child.localScale.z * 10f));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
