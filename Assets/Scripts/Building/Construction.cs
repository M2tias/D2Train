using UnityEngine;

public class Construction : MonoBehaviour
{
    [SerializeField]
    private float buildTime;

    private BuildingType type;
    private float progress = 0f;
    private float buildStartTime = 0f;
    private float buildSpeedMultiplier = 1f;
    private bool buildStarted = false;

    public BuildingType Type { get { return type; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (buildStarted)
        {
            float sinceStart = Time.time - buildStartTime;

            progress = Mathf.Lerp(0f, 1f, sinceStart / buildTime * buildSpeedMultiplier);
        }
    }

    public void StartBuilding()
    {
        buildStarted = true;
        buildStartTime = Time.time;
    }

    public void Initialize(BuildingType buildingType)
    {
        type = buildingType;
    }

    public bool IsFinished()
    {
        return progress >= 1f;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
