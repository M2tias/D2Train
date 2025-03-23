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
    private Color activeModeColor;
    [SerializeField]
    private Color inactiveModeColor;

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
        }
        else
        {
            buildMode.color = inactiveModeColor;
            fightMode.color = activeModeColor;
        }

        resourceCount.text = BuildingManager.main.Resources.ToString();
    }

    public void UpdateTrainDistance(float distance)
    {
        trainDistance.text = ((int)distance).ToString();
    }
}
