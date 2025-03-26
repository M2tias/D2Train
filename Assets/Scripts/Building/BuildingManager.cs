using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField]
    List<Pair<BuildingType>> constructionPrefabs = new();
    [SerializeField]
    List<Pair<BuildingType>> buildingPrefabs = new();
    [SerializeField]
    List<Pair<BuildingType, List<Pair<List<Neighbour>, GameObject>>>> buildingModels = new();
    [SerializeField]
    private List<Pair<BuildingType, GameObject>> defaultBuildingModels = new();
    [SerializeField]
    private int resourcePerCar = 5;
    [SerializeField]
    private GameObject gridContainer;

    private int resources = 5;

    private Queue<Construction> buildQueue = new();
    private List<Building> buildings = new();
    private List<Building> neighbourlessBuildings = new();
    private List<Building> tracksToTraverse = new();

    private List<Vector2> reservedSpaces = new(); // use CellPos

    public List<Building> NeighbourlessBuildings { get { return neighbourlessBuildings; } }
    public List<Vector2> Reserved { get { return reservedSpaces; } }
    public List<Building> TracksToTraverse { get { return tracksToTraverse; } }

    private Construction currentConstruction;
    private Building previousBuilding;
    private bool isBuildingMode = false;
    private bool isHealingMode = false;
    private bool isFightMode = true;

    public bool IsBuildingMode { get { return isBuildingMode; } }
    public bool IsHealingMode { get { return isHealingMode; } }
    public bool IsFightMode { get { return isFightMode; } }
    public int Resources { get { return resources; } }

    public static BuildingManager main;

    private void Awake()
    {
        main = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector2 pos = Vec.CellPos(Vector3.zero + Vector3.forward * i);
            reservedSpaces.Add(pos);
            SpawnBuilding(BuildingType.Rail, Vec.V2to3(pos), "initialTrack" + i);
        }

        TrainManager.main.SetFirstTrack();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentConstruction == null && buildQueue.Count > 0)
        {
            currentConstruction = buildQueue.Dequeue();
            currentConstruction.StartBuilding();
        }
        else if (currentConstruction != null && currentConstruction.IsFinished())
        {
            SpawnBuilding(currentConstruction);
            currentConstruction.Destroy();
            currentConstruction = null;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            isBuildingMode = true;
            isHealingMode = false;
            isFightMode = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            isBuildingMode = false;
            isHealingMode = true;
            isFightMode = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            isBuildingMode = false;
            isHealingMode = false;
            isFightMode = true;
        }

        gridContainer.SetActive(isBuildingMode);
    }

    public void AddBuildingQueue(List<GridCursor> list)
    {
        resources -= list.Count;
        list.ForEach(x => buildQueue.Enqueue(MakeConstruction(x)));
    }

    public bool IsReserved(Vector2 pos)
    {
        Vector2 reservedSpace = reservedSpaces.FirstOrDefault(space => Vec.isSameCell(pos, space));

        if (reservedSpace != Vector2.zero)
        {
            return true;
        }

        return false;
    }

    public GameObject GetBuildingModel(BuildingType type, IEnumerable<Neighbour> neighbours)
    {
        List<Pair<List<Neighbour>, GameObject>> list = buildingModels.FirstOrDefault(x => x.Key == type)?.Value;

        foreach (var pair in list)
        {
            bool match = pair.Key.All(neighbours.Contains) && pair.Key.Count == neighbours.Count();

            if (match)
            {
                return pair.Value;
            }
        }

        Debug.LogError($"Couldn't find building model for {type} with neighbours {string.Join(", ", neighbours)}");
        return null;
    }

    public void AddResources()
    {
        resources += resourcePerCar;
    }

    public bool RemoveResources()
    {
        if (resources > 0)
        {
            resources -= 1;
            return true;
        }

        return false;
    }

    private Construction MakeConstruction(GridCursor cursor)
    {
        GameObject construction = constructionPrefabs.FirstOrDefault(x => x.Key == cursor.Type)?.Value;

        if (construction != null)
        {
            GameObject constrObj = Instantiate(construction, cursor.transform.position, Quaternion.identity);
            Construction constr = constrObj.GetComponent<Construction>();
            constr.Initialize(cursor.Type);
            reservedSpaces.Add(Vec.CellPos(cursor.transform.position));

            cursor.Destroy();

            return constr;
        }

        cursor.Destroy();
        return null;
    }

    private void SpawnBuilding(Construction construction)
    {
        SpawnBuilding(construction.Type, construction.transform.position);
    }

    private void SpawnBuilding(BuildingType type, Vector3 pos, string name = null)
    {
        GameObject buildingPrefab = buildingPrefabs.FirstOrDefault(x => x.Key == type)?.Value;

        if (buildingPrefab != null)
        {
            GameObject buildingObj = Instantiate(buildingPrefab, pos, Quaternion.identity);
            Building building = buildingObj.GetComponent<Building>();
            GameObject model = defaultBuildingModels.FirstOrDefault(x => x.Key == type)?.Value;
            buildingObj.name = $"Building {buildings.Count}";

            if (name != null)
            {
                buildingObj.name = name;
            }

            building.Initialize(type, model);
            buildings.Add(building);
            neighbourlessBuildings.Add(building);
            // Not needed, was added as construction: reservedSpaces.Add(Vec.CellPos(building.transform.position));

            if (type == BuildingType.Rail)
            {
                tracksToTraverse.Add(building);
            }
        }

        foreach (var building in neighbourlessBuildings)
        {
            if (!building.HasFreeNeighbours)
            {
                continue;
            }

            building.CheckNeighbours();
        }

        neighbourlessBuildings.RemoveAll(x => !x.HasFreeNeighbours);
    }
}

public enum BuildingType
{
    Rail
}