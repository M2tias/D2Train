using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float HP;
    [SerializeField]
    private MeshRenderer mesh;

    private EnemyNavigation nav;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nav = GetComponent<EnemyNavigation>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void DoDamage()
    {
        nav.SwitchToPlayer();
        HP -= 1;

        mesh.material.color = mesh.material.color * 0.8f;

        if (HP == 0)
        {
            mesh.enabled = false;
            Invoke("Kill", 2f);
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
