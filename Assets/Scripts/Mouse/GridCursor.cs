using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GridCursor : MonoBehaviour
{
    [SerializeField]
    private Color defaultColor;
    [SerializeField]
    private Color holdColor;
    [SerializeField]
    private Color failColor;
    [SerializeField]
    private bool isDragPlacement;

    [SerializeField]
    private GameObject placementPrefab;

    private int resources = 6;

    private MeshRenderer mesh;
    private Camera cam;
    private bool isPlacementValid = true;
    private bool isDragValid = true;
    private Vector2 prevDragPos = Vector2.right * 1000f;
    private Vector2 dragStartPos = Vector2.right * 1000f;
    private bool dragStarted = false;
    private bool disableDragging = false;
    private BuildingType type;

    private List<GridCursor> placementObjects = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        mesh = GetComponent<MeshRenderer>();
    }

    public void Initialize(BuildingType t)
    {
        //transform.position = new Vector3(pos.x, 0.025f, pos.z);
        type = t;
    }

    public void SetValid(bool isValid)
    {
        isPlacementValid = isValid;
    }

    public BuildingType Type { get { return type; } }

    public void Destroy()
    {
        if (isDragPlacement)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragPlacement)
        {
            if (isPlacementValid) {
                mesh.material.color = defaultColor;
            }
            else
            {
                mesh.material.color = failColor;
            }

            return;
        }

        placementObjects.RemoveAll(item => item == null);

        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, 100, 1 << LayerMask.NameToLayer("MouseRay")))
        {
            float xHit = Mathf.Floor(hitInfo.point.x) + 0.5f;
            float zHit = Mathf.Floor(hitInfo.point.z) + 0.5f;

            transform.position = new Vector3(xHit, 0.025f, zHit);
        }

        float x = Mathf.Floor(transform.position.x) + 0.5f;
        float z = Mathf.Floor(transform.position.z) + 0.5f;
        Vector2 cellPos = Vec.CellPos(transform.position); // new Vector2(x, z);

        if (Input.GetMouseButton(0) && !disableDragging)
        {

            // Debug.Log($"Dragging, {dragStartPos}, {transform.position}");

            if (IsEnoughResources() && isDragValid)
            {
                Debug.Log($"lol fug {IsEnoughResources()}, {isDragValid}");
                mesh.material.color = holdColor;
                placementObjects.ForEach(x => x.SetValid(true));
            }
            else
            {
                mesh.material.color = failColor;

                placementObjects.ForEach(x => x.SetValid(false));
            }

            if (!dragStarted) {
                dragStartPos = cellPos;
                dragStarted = true;
                prevDragPos = dragStartPos;
                isDragValid = true;
            }
            else
            {

                bool isNotPrevPos = Vec.isNotSameCell(prevDragPos, cellPos);
                int existingPlacementIndex = placementObjects.FindIndex(0, placementObjects.Count, p => !Vec.isNotSameCell(cellPos, p.transform.position));

                bool isReserved = BuildingManager.main.IsReserved(cellPos);

                if (isReserved && placementObjects.Count == 0)
                {
                    mesh.material.color = failColor;

                    placementObjects.ForEach(x => x.SetValid(false));
                    isDragValid = false;
                }

                // Debug.Log($"{isNotPrevPos}, {existingPlacementIndex > -1}, {placementObjects.Count}");

                if (isNotPrevPos && existingPlacementIndex < 0)
                {
                    AddGridPlacement();

                    bool tmpIsReserved = isReserved;

                    if (!isReserved)
                    {
                        isReserved = placementObjects.Any(x => BuildingManager.main.IsReserved(x.transform.position));
                        Debug.Log($"placements {string.Join(",", placementObjects.Select(x => Vec.CellPos(x.transform.position).ToString()))}");
                        Debug.Log($"reserveds  {string.Join(",", BuildingManager.main.Reserved.Select(x => x.ToString()))}");
                    }

                    if (isReserved)
                    {
                        mesh.material.color = failColor;

                        placementObjects.ForEach(x => x.SetValid(false));
                        isDragValid = false;
                    }

                    Debug.Log($"{isDragValid} ! {tmpIsReserved} -> {isReserved}, {cellPos}, {placementObjects.Count}");

                    prevDragPos = cellPos;
                }
                else if (existingPlacementIndex >= 0)
                {
                    for (int i = existingPlacementIndex; i < placementObjects.Count; i++)
                    {
                        GridCursor placement = placementObjects[i];
                        placement.Destroy();
                    }

                    placementObjects.RemoveRange(existingPlacementIndex, placementObjects.Count- existingPlacementIndex);
                    prevDragPos = cellPos;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && !disableDragging)
        {
            if (IsEnoughResources() && isDragValid)
            {
                prevDragPos = cellPos;
                AddGridPlacement();
                BuildingManager.main.AddBuildingQueue(placementObjects);
            }
            else
            {
                DeletePlacements();
            }

            dragStarted = false;
        }
        else
        {
            mesh.material.color = defaultColor;
        }

        if (dragStarted && Input.GetMouseButtonDown(1))
        {
            dragStarted = false;
            DeletePlacements();
            disableDragging = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            Invoke("EnableDragging", 0.35f);
        }
    }

    private void EnableDragging()
    {
        disableDragging = false;
    }

    private void DeletePlacements()
    {
        placementObjects.ForEach(x => x.Destroy());
        placementObjects = new();
    }

    private bool IsEnoughResources()
    {
        return resources >= placementObjects.Count + 1;
    }

    private void AddGridPlacement()
    {
        GameObject obj = Instantiate(placementPrefab, new Vector3(prevDragPos.x, 0.022f, prevDragPos.y), Quaternion.identity);
        GridCursor placement = obj.GetComponent<GridCursor>();
        placement.Initialize(type);
        placementObjects.Add(placement);
    }

    private Vector2 V2to3(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
}
