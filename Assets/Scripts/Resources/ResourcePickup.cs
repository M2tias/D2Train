using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    [SerializeField]
    private GameObject resourcePickupModel;

    private Vector3 modelPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        modelPos = resourcePickupModel.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        resourcePickupModel.transform.position = new Vector3(modelPos.x, Mathf.Sin(Time.time * 2f) * 0.33f + modelPos.y, modelPos.z);
        resourcePickupModel.transform.rotation = Quaternion.Slerp(Quaternion.Euler(-90f, 0, -20f), Quaternion.Euler(-90f, 180f, 20f), (Mathf.Sin(Time.time) + 1f) / 2f);
    }
}
