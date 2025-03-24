using UnityEngine;

public class PlayerHealing : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem healingLaserParticle;
    [SerializeField]
    private ParticleSystem healingParticle;

    private float healingHitTime = 0;
    private float healingCD = 0.33f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (BuildingManager.main.IsHealingMode)
        {
            if (Input.GetMouseButton(1))
            {
                healingLaserParticle.Play();
                Physics.Raycast(healingLaserParticle.transform.position, healingLaserParticle.transform.forward, out RaycastHit hitInfo, 10f, 1 << LayerMask.NameToLayer("Player"));

                if (hitInfo.collider != null)
                {
                    if (Time.time - healingHitTime > healingCD)
                    {
                        Debug.Log($"TRAIN BEING HEALED {hitInfo.collider.gameObject.name}");
                        TrainManager.main.HealTrain();
                        healingHitTime = Time.time;
                    }
                }
            }
            else
            {
                healingLaserParticle.Stop();
            }

            if (Input.GetMouseButtonDown(0))
            {
                healingParticle.Play();
            }

            if (Input.GetMouseButton(0))
            {
                if (Time.time - healingHitTime > healingCD)
                {
                    Debug.Log($"PLAYER BEING HEALED");
                    TrainManager.main.HealPlayer();
                    healingHitTime = Time.time;
                }
            }

            // if (Input.GetMouseButtonUp(0))
            else
            {
                healingParticle.Stop();
            }
        }
        else
        {
            healingParticle.Stop();
            healingLaserParticle.Stop();
        }
    }
}
