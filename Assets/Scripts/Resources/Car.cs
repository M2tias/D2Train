using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField]
    private float HP;
    [SerializeField]
    private MeshRenderer mesh;
    [SerializeField]
    private GameObject resourcePickupPrefab;

    private BoxCollider col;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void DoDamage()
    {
        HP -= 1;

        mesh.material.color = mesh.material.color * 0.8f;

        if (HP == 0)
        {
            Instantiate(resourcePickupPrefab, transform.position, Quaternion.identity);
            // BuildingManager.main.AddResources();
            col.enabled = false;
            mesh.enabled = false;
            Invoke("Kill", 2f);
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
