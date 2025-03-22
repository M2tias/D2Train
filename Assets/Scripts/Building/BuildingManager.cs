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


    public static BuildingManager main;

    private Queue<Construction> buildQueue = new();
    private List<Building> buildings = new();
    private List<Vector2> reservedSpaces = new(); // use CellPos
    public List<Vector2> Reserved {  get { return reservedSpaces; } }

    private Construction currentConstruction;

    private void Awake()
    {
        main = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
    }

    public void AddBuildingQueue(List<GridCursor> list)
    {
        list.ForEach(x => buildQueue.Enqueue(MakeConstruction(x)));
    }

    public bool IsReserved(Vector3 pos)
    {
        return reservedSpaces.Any(space => !Vec.isNotSameCell(pos, space));
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
        GameObject buildingPrefab = buildingPrefabs.FirstOrDefault(x => x.Key == construction.Type)?.Value;

        if (buildingPrefab != null)
        {
            GameObject buildingObj = Instantiate(buildingPrefab, construction.transform.position, Quaternion.identity);
            Building building = buildingObj.GetComponent<Building>();
            building.Initialize(construction.Type);
            buildings.Add(building);
            // Not needed, was added as construction: reservedSpaces.Add(Vec.CellPos(building.transform.position));
        }
    }
}

public enum BuildingType
{
    Rail
}
