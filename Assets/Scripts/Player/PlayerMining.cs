using System.Collections;
using UnityEngine;

public class PlayerMining : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem miningLaserParticle;
    [SerializeField]
    private ParticleSystem shootingLaserParticle;
    [SerializeField]
    private AudioSource miningAudio;
    [SerializeField]
    private AudioSource shootingAudio;
    [SerializeField]
    private AudioSource pickupAudio;
    [SerializeField]
    private AudioSource music;

    private float miningHitTime = 0;
    private float miningHitCD = 0.33f;

    private float shootingHitTime = 0;
    private float shootingHitCD = 0.25f;

    private float pickCD = 0.33f;
    private float lastPickTime = 0;

    private bool isMiningAudio = false;
    private bool isShootingAudio = false;

    private bool isPlayingMusic = true;
    private bool musicDisabled = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            musicDisabled = !musicDisabled;

            if (!isPlayingMusic && !musicDisabled) {
                isPlayingMusic = true;
                music.Play();
            }

            if (musicDisabled)
            {
                music.Stop();
            }
        }

        if (BuildingManager.main.IsFightMode)
        {
            if (Input.GetMouseButton(1))
            {
                miningLaserParticle.Play();

                if (!isMiningAudio)
                {
                    miningAudio.Play();
                    isMiningAudio = true;
                }
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
                miningAudio.Stop();
                isMiningAudio = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                shootingLaserParticle.Play();
                if (!isShootingAudio)
                {
                    StartCoroutine(ShootAudio());
                    isShootingAudio = true;
                }
                // if (Time.time - shootingHitTime > shootingHitCD)
                // {
                //     if (!isShootingAudio)
                //     {
                //         StartCoroutine(ShootAudio());
                //         isShootingAudio = true;
                //     }
                // }
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
                shootingAudio.Stop();
                isShootingAudio = false;
                shootingHitTime = Time.time;
            }
        }
        else
        {
            shootingLaserParticle.Stop();
            miningLaserParticle.Stop();
            shootingAudio.Stop();
            miningAudio.Stop();
            isShootingAudio = false;
            isMiningAudio = false;
        }
    }


    IEnumerator ShootAudio()
    {
        while(true)
        {
            yield return new WaitForSeconds(.25f);
            shootingAudio.Play();
            
            if (!isShootingAudio)
            {
                break;
            }
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
                pickupAudio.Play();
                Destroy(pickup.gameObject);
            }
        }
    }
}
