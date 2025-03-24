using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealing : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem healingLaserParticle;
    [SerializeField]
    private ParticleSystem healingParticle;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioSource audioSource2;
    [SerializeField]
    private AudioSource hitSound;

    private float healingHitTime = 0;
    private float healingCD = 0.33f;

    private bool audioPlaying = false;

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
                if (!audioPlaying)
                {
                    audioSource.Play();
                    audioPlaying = true;
                }

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
                audioSource.Stop();
                audioPlaying = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                healingParticle.Play();
                audioSource2.Play();
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
                audioSource2.Stop();
            }
        }
        else
        {
            healingParticle.Stop();
            healingLaserParticle.Stop();
            audioSource.Stop();
            audioSource2.Stop();
            audioPlaying = false;
        }
    }

    public void HitSound()
    {
        hitSound.Play();
    }
}
