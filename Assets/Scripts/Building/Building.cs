using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour
{
    private BuildingType type;
    public BuildingType Type { get { return type; } }

    private Dictionary<Neighbour, Building> Neighbours = new();
    private Dictionary<Neighbour, Vector2> NeighbourPositions = new() {
        { Neighbour.Left, Vector2.left },
        { Neighbour.Right, Vector2.right },
        { Neighbour.Top, Vector2.up },
        { Neighbour.Bottom, Vector2.down }
    };

    private Dictionary<Neighbour, Neighbour> reverseNeighbour = new()
    {
        {Neighbour.Left, Neighbour.Right },
        {Neighbour.Right, Neighbour.Left },
        {Neighbour.Top, Neighbour.Bottom },
        {Neighbour.Bottom, Neighbour.Top },
    };

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
        if (Neighbours.Count >= 2)
        {
            return;
        }

        List<Building> possibleNeighbours = BuildingManager.main.NeighbourlessBuildings;

        foreach (KeyValuePair<Neighbour, Vector2> kvp in NeighbourPositions)
        {
            if (Neighbours.ContainsKey(kvp.Key)) { continue; }

            foreach (Building building in possibleNeighbours)
            {
                if (this == building) { continue; }
                if (!building.HasFreeNeighbours) { continue; }

                if (Vec.isSameCell(Vec.CellPos(transform.position) + kvp.Value, Vec.CellPos(building.transform.position)))
                {
                    Neighbours.Add(kvp.Key, building);
                    building.AddNeighbour(reverseNeighbour[kvp.Key], this);
                }

                if (Neighbours.Count >= 2)
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
        if (Neighbours.Count != previousNeighbourCount)
        {
            Destroy(buildingModel);
            GameObject prefab = BuildingManager.main.GetBuildingModel(type, Neighbours.Keys);
            buildingModel = Instantiate(prefab, transform.position, prefab.transform.rotation, transform);
            previousNeighbourCount = Neighbours.Count;
        }
    }

    public void AddNeighbour(Neighbour neighbour, Building building)
    {
        Neighbours.Add(neighbour, building);

        if (Neighbours.Count >= 2)
        {
            hasFreeNeighbours = false;
        }

        UpdateModel();
    }
}
