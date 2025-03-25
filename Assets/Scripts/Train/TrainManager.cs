using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainManager : MonoBehaviour
{
    [SerializeField]
    private GameObject TrainEngine;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private AudioSource trainHitSound;

    private Building targetTrack;
    private Building currentTrack;
    private Building previousTrack;
    private Neighbour comingToDir = Neighbour.Top;
    private Neighbour targetDir = Neighbour.Top;
    private List<Building> previousTracks;

    private Quaternion prevRot = Quaternion.identity;
    private Quaternion curRot = Quaternion.identity;
    private Quaternion nextRot = Quaternion.identity;

    private Vector2 startPos = Vec.CellPos(Vector2.zero);

    private float lerpSpeed = 1.85f;
    private float lerpT = 0f;
    private Vector3 lerpStart = Vector3.zero;
    private Vector3 lerpEnd = Vector3.zero;

    public static TrainManager main;

    private float distance = 0f;

    private int playerHP = 100;
    private int trainHP = 100;
    private int maxHP = 100;

    public GameObject Train { get { return TrainEngine; } }
    public GameObject Player { get { return player; } }
    public int TrainHP { get { return trainHP; } }
    public int PlayerHP { get { return playerHP; } }
    public float Distance { get { return distance; } }
    public float Goal { get { return 293f; } }

    private void Awake()
    {
        main = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TrainEngine.transform.position = Vec.V2to3(startPos - DirToVector(comingToDir) * 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 engineCellPos = Vec.CellPos(TrainEngine.transform.position + Vec.V2to3(NeighbourHelper.NeighbourPositions[comingToDir] * 0.5f));

        if (currentTrack == null)
        {
            Building track = BuildingManager.main.TracksToTraverse
                .FirstOrDefault(x => Vec.isSameCell(engineCellPos, x.transform.position));

            if (track.Neighbours.Count >= 2)
            {
                Neighbour? nextDir = track.Neighbours.Keys.FirstOrDefault(x => x != comingToDir);

                if (nextDir == null)
                {
                    Debug.LogError("targetDir for next step is unexpected!");
                }

                targetDir = nextDir.Value;

                if (track != null)
                {
                    currentTrack = track;
                }
            }
        }

        if (targetTrack == null)
        {
            Building track = currentTrack.Neighbours.Values.FirstOrDefault(x => x != previousTrack);

            if (track != null && track.Neighbours.Count >= 2)
            {
                Neighbour? nextDir = track.Neighbours.Keys.FirstOrDefault(x => x != NeighbourHelper.ReverseNeighbour[comingToDir]);

                if (nextDir == null)
                {
                    Debug.LogError("targetDir for next step is unexpected!");
                }

                targetDir = nextDir.Value;

                if (track != null)
                {
                    targetTrack = track;
                    // Debug.Log($"Setting next track to {targetTrack.name}");
                }

                lerpEnd = Vec.V2to3(Vec.CellPos(targetTrack.transform.position));
            }
        }

        if (currentTrack != null && targetTrack != null)
        {
            if (lerpT >= 1f)
            {
                prevRot = curRot;
                var curRotTarget = GetNextBuilding(targetTrack, currentTrack);
                curRot = Quaternion.LookRotation(curRotTarget.transform.position - targetTrack.transform.position);
                var nextRotTarget = GetNextBuilding(curRotTarget, targetTrack);
                if (nextRotTarget != null)
                {
                    nextRot = Quaternion.LookRotation(nextRotTarget.transform.position - curRotTarget.transform.position);
                }

                previousTrack = currentTrack;
                currentTrack = targetTrack;
                targetTrack = null;
                lerpT = 0f;
                lerpStart = lerpEnd;
            }

            lerpT = lerpT + Time.deltaTime * lerpSpeed;
            TrainEngine.transform.position = Vector3.Lerp(lerpStart, lerpEnd, lerpT);
            TrainEngine.transform.rotation = Quaternion.Slerp(curRot, nextRot, lerpT);

            if (lerpT < 0.5f)
            {
                TrainEngine.transform.rotation = Quaternion.Slerp(prevRot, curRot, lerpT + 0.5f);
            }
            else
            {
                TrainEngine.transform.rotation = Quaternion.Slerp(curRot, nextRot, lerpT - 0.5f);
            }
        }

        distance = Goal - TrainEngine.transform.position.z; // (new Vector3(0, 0, Goal) - TrainEngine.transform.position).magnitude;
        UIManager.main.UpdateTrainDistance(distance);

        if (distance <= 0f)
        {
            Invoke("YouWin", 3f);
        }
    }

    private Building GetNextBuilding(Building b, Building prev)
    {
        return b.Neighbours.Values.FirstOrDefault(x => x != prev);
    }

    public void SetFirstTrack()
    {
        Building track = BuildingManager.main.TracksToTraverse[0];
        currentTrack = track;
        targetTrack = BuildingManager.main.TracksToTraverse[1];
        targetDir = Neighbour.Top;
        lerpStart = Vec.V2to3(Vec.CellPos(currentTrack.transform.position) - NeighbourHelper.NeighbourPositions[targetDir]);
        lerpEnd = Vec.V2to3(Vec.CellPos(targetTrack.transform.position) - NeighbourHelper.NeighbourPositions[targetDir]);
    }

    public void DoDamage(bool isTrain)
    {
        if (isTrain)
        {
            trainHitSound.Play();
            trainHP -= 2;
        }
        else
        {
            player.GetComponent<PlayerHealing>().HitSound();
            playerHP -= 10;
        }

        if (trainHP <= 0 || playerHP <= 0)
        {
            Invoke("GameOver", 2f);
        }
    }

    public void HealPlayer()
    {
        if (playerHP < maxHP && BuildingManager.main.RemoveResources())
        {
            playerHP = Mathf.Min(maxHP, playerHP + 10);
        }
    }

    public void HealTrain()
    {
        if (trainHP < maxHP && BuildingManager.main.RemoveResources())
        {
            trainHP = Mathf.Min(maxHP, trainHP + 10);
        }
    }

    public void GameOver()
    {
        Debug.Log("Game over!");
        SceneManager.LoadScene("gameover");
    }

    public void YouWin()
    {
        Debug.Log("You win!");
        SceneManager.LoadScene("win");
    }

    private Vector2 DirToVector(Neighbour tileDir)
    {
        return tileDir switch
        {
            Neighbour.Left => Vector2.left,
            Neighbour.Right => Vector2.right,
            Neighbour.Top => Vector2.up,
            Neighbour.Bottom => Vector2.down,
            _ => Vector2.zero
        };
    }

    private Vector2 LerpPoint(Vector2 cellPos, Neighbour tileDir)
    {
        return cellPos + DirToVector(tileDir);
    }
}
