using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject playerModel;

    private float rotateSpeed = 15f * 60f;
    private float speed = 4f;
    private Camera cam;
    private Vector3 mouseDir = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
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

    private void LateUpdate()
    {
        if (!BuildingManager.main.IsBuildingMode)
        {
            Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, 100, 1 << LayerMask.NameToLayer("MouseRay")))
            {
                Vector3 mousePoint = new Vector3(hitInfo.point.x, 0f, hitInfo.point.z);
                mouseDir = (mousePoint - playerModel.transform.position).normalized;
                mouseDir = new Vector3(mouseDir.x, 0f, mouseDir.z);
            }
        }

        float step = rotateSpeed * Time.deltaTime;
        playerModel.transform.rotation = Quaternion.RotateTowards(playerModel.transform.rotation, Quaternion.LookRotation(mouseDir), step);
    }
}
