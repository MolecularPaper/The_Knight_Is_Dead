using UnityEngine;
using TMPro;

public class StageUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highestStage;
    [SerializeField] TextMeshProUGUI stage;

    void Awake()
    {
        GameManager.gm.stageChanged += UpdateStageUI;
    }

    private void UpdateStageUI(GameInfo gameInfo)
    {
        highestStage.text = $"�ְ� ��������: {gameInfo.highestStageIndex + 1}";
        stage.text = $"�������� {gameInfo.stageIndex + 1}";
    }
}
