using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour
{
    private BuildingType type;
    public BuildingType Type { get { return type; } }

    private Dictionary<Neighbour, Building> neighbours = new();
    public Dictionary<Neighbour, Building> Neighbours { get { return neighbours; } }


    private GameObject buildingModel;

    private bool hasFreeNeighbours = true;
    public bool HasFreeNeighbours { get { return hasFreeNeighbours; } }
    private int previousNeighbourCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize(BuildingType buildingType, GameObject model)
    {
        type = buildingType;
        buildingModel = Instantiate(model, transform.position, Quaternion.identity, transform);
    }

    public void CheckNeighbours()
    {
        if (neighbours.Count >= 2)
        {
            return;
        }

        List<Building> possibleNeighbours = BuildingManager.main.NeighbourlessBuildings;

        foreach (KeyValuePair<Neighbour, Vector2> kvp in NeighbourHelper.NeighbourPositions)
        {
            if (neighbours.ContainsKey(kvp.Key)) { continue; }

            foreach (Building building in possibleNeighbours)
            {
                if (this == building) { continue; }
                if (!building.HasFreeNeighbours) { continue; }

                if (Vec.isSameCell(Vec.CellPos(transform.position) + kvp.Value, Vec.CellPos(building.transform.position)))
                {
                    neighbours.Add(kvp.Key, building);
                    building.AddNeighbour(NeighbourHelper.ReverseNeighbour[kvp.Key], this);
                }

                if (neighbours.Count >= 2)
                {
                    hasFreeNeighbours = false;
                    break;
                }
            }
        }

        UpdateModel();
    }

    private void UpdateModel()
    {
        if (neighbours.Count != previousNeighbourCount)
        {
            Destroy(buildingModel);
            GameObject prefab = BuildingManager.main.GetBuildingModel(type, neighbours.Keys);
            buildingModel = Instantiate(prefab, transform.position, prefab.transform.rotation, transform);
            previousNeighbourCount = neighbours.Count;
        }
    }

    public void AddNeighbour(Neighbour neighbour, Building building)
    {
        neighbours.Add(neighbour, building);

        if (neighbours.Count >= 2)
        {
            hasFreeNeighbours = false;
        }

        UpdateModel();
    }
}
