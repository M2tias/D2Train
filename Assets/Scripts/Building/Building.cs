using UnityEngine;

public class Building : MonoBehaviour
{
    private BuildingType type;
    public BuildingType Type { get { return type; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize(BuildingType buildingType)
    {
        type = buildingType;
    }
}
