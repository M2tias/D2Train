using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerHP;
    [SerializeField]
    private TextMeshProUGUI trainHP;
    [SerializeField]
    private TextMeshProUGUI resourceCount;
    [SerializeField]
    private TextMeshProUGUI trainDistance;
    [SerializeField]
    private TextMeshProUGUI buildMode;
    [SerializeField]
    private TextMeshProUGUI fightMode;
    [SerializeField]
    private TextMeshProUGUI healMode;

    [SerializeField]
    private Color activeModeColor;
    [SerializeField]
    private Color inactiveModeColor;

    [SerializeField]
    private GameObject fightControls;
    [SerializeField]
    private GameObject buildControls;
    [SerializeField]
    private GameObject healControls;

    public static UIManager main;

    void Awake()
    {
        main = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (BuildingManager.main.IsBuildingMode)
        {
            buildMode.color = activeModeColor;
            fightMode.color = inactiveModeColor;
            healMode.color = inactiveModeColor;
            buildControls.SetActive(true);
            fightControls.SetActive(false);
            healControls.SetActive(false);
        }
        else if (BuildingManager.main.IsHealingMode)
        {
            buildMode.color = inactiveModeColor;
            fightMode.color = inactiveModeColor;
            healMode.color = activeModeColor;
            buildControls.SetActive(false);
            fightControls.SetActive(false);
            healControls.SetActive(true);
        }
        else if (BuildingManager.main.IsFightMode)
        {
            buildMode.color = inactiveModeColor;
            fightMode.color = activeModeColor;
            healMode.color = inactiveModeColor;
            buildControls.SetActive(false);
            fightControls.SetActive(true);
            healControls.SetActive(false);
        }

        resourceCount.text = BuildingManager.main.Resources.ToString();

        playerHP.text = TrainManager.main.PlayerHP.ToString() + " %";
        trainHP.text = TrainManager.main.TrainHP.ToString() + " %";
    }

    public void UpdateTrainDistance(float distance)
    {
        trainDistance.text = ((int)distance * 10f).ToString() + " m";
    }
}
