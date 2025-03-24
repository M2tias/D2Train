using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigation : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem laserParticle;

    private float shootDist = 6f;
    private float moveDist = 7.5f;
    private EnemyState state = EnemyState.Idle;

    private float idleTime = 1f;
    private float idleStarted = 0f;

    private float movementSpeed = 3f;

    private bool targetIsTrain = true;

    NavMeshAgent agent;

    private float shootTime = 0f;
    private float shootCD = 3f;
    private float shootEffectTime = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        idleStarted = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.enabled) { return; }

        Transform train = TrainManager.main.Train.transform;
        Vector3 trainPos = train.position;
        Vector3 pos = transform.position;

        if (!targetIsTrain)
        {
            trainPos = TrainManager.main.Player.transform.position;
        }

        // TODO: Raycast to check if shooting is possible?
        // TODO: SphereCast to check if player is closer
        // TODO: target player if player inflicts damage

        if (state == EnemyState.Idle)
        {
            laserParticle.Stop();
            agent.isStopped = true;

            if (Time.time - idleStarted > idleTime)
            {
                state = EnemyState.Move;
            }
        }
        else if (state == EnemyState.Move)
        {
            laserParticle.Stop();
            agent.speed = movementSpeed;
            agent.SetDestination(trainPos);
            agent.isStopped = false;

            if (Vector3.Distance(Xz(trainPos), Xz(pos)) < shootDist)
            {
                state = EnemyState.Shoot;
            }
        }
        else if (state ==  EnemyState.Shoot)
        {
            // TODO: shooting
            agent.isStopped = true;
            agent.speed = 0.05f;

            Vector3 direction = (trainPos - pos).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);

            if (Time.time - shootTime > shootCD)
            {
                shootTime = Time.time;
                Debug.Log("shoot");
                laserParticle.Play();
                Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 8f, 1 << LayerMask.NameToLayer("Player"));

                if (hitInfo.collider != null)
                {
                    Debug.Log("hit");
                    bool isPlayer = hitInfo.collider.name.Contains("Player");

                    TrainManager.main.DoDamage(!isPlayer);
                }
            }

            if (Time.time - shootTime > shootEffectTime)
            {
                laserParticle.Stop();
            }

            if (Vector3.Distance(Xz(trainPos), Xz(pos)) > moveDist)
            {
                laserParticle.Stop();
                state = EnemyState.Move;
            }
        }

    }

    public void SwitchToPlayer()
    {
        targetIsTrain = false;
    }

    private Vector3 Xz(Vector3 pos)
    {
        return new Vector3(pos.x, 0, pos.z);
    }
}

public enum EnemyState
{
    Shoot,
    Idle,
    Move
}
