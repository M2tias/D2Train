using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float speed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 p = transform.position;
        Vector3 dir = Quaternion.AngleAxis(45, Vector3.up) * new Vector3(horizontal, 0, vertical).normalized;
        Vector3 velocity = dir * speed * Time.deltaTime;


        transform.position = p + velocity;

    }
}
