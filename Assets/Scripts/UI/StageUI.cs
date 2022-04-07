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
        highestStage.text = $"최고 스테이지: {gameInfo.highestStageIndex + 1}";
        stage.text = $"스테이지 {gameInfo.stageIndex + 1}";
    }
}
