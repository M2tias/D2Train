using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float HP;
    [SerializeField]
    private MeshRenderer mesh;

    private SphereCollider col;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<SphereCollider>();
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
