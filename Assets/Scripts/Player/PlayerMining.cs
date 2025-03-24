using UnityEngine;

public class PlayerMining : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem miningLaserParticle;
    [SerializeField]
    private ParticleSystem shootingLaserParticle;

    private float miningHitTime = 0;
    private float miningHitCD = 0.33f;

    private float shootingHitTime = 0;
    private float shootingHitCD = 0.25f;

    private float pickCD = 0.33f;
    private float lastPickTime = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (BuildingManager.main.IsFightMode)
        {
            if (Input.GetMouseButton(1))
            {
                miningLaserParticle.Play();
                Physics.Raycast(miningLaserParticle.transform.position, miningLaserParticle.transform.forward, out RaycastHit hitInfo, 10f, 1 << LayerMask.NameToLayer("Car"));

                if (hitInfo.collider != null)
                {
                    if (Time.time - miningHitTime > miningHitCD)
                    {
                        Debug.Log($"CAR BEING DEMOLISHED {hitInfo.collider.gameObject.name}");
                        hitInfo.collider.gameObject.GetComponent<Car>().DoDamage();
                        miningHitTime = Time.time;
                    }
                }
            }
            else
            {
                miningLaserParticle.Stop();
            }

            if (Input.GetMouseButtonDown(0))
            {
                shootingLaserParticle.Play();
            }
            
            if (Input.GetMouseButton(0))
            {
                Physics.Raycast(shootingLaserParticle.transform.position, shootingLaserParticle.transform.forward, out RaycastHit hitInfo, 6f, 1 << LayerMask.NameToLayer("Enemy"));

                if (hitInfo.collider != null)
                {
                    if (Time.time - shootingHitTime > shootingHitCD)
                    {
                        Debug.Log($"ENEMY BEING DEMOLISHED {hitInfo.collider.gameObject.name}");
                        hitInfo.collider.gameObject.GetComponent<Enemy>().DoDamage();
                        shootingHitTime = Time.time;
                    }
                }
            }
            
            // if (Input.GetMouseButtonUp(0))
            else
            {
                shootingLaserParticle.Stop();
                shootingHitTime = Time.time;
            }
        }
        else
        {
            shootingLaserParticle.Stop();
            miningLaserParticle.Stop();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Resource")
        {
            bool isNotCd = Time.time - lastPickTime > pickCD;

            if (isNotCd && other.gameObject.TryGetComponent(out ResourcePickup pickup))
            {
                lastPickTime = Time.time;
                BuildingManager.main.AddResources();
                Destroy(pickup.gameObject);
            }
        }
    }
}
