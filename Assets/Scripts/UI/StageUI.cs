using UnityEngine;
using TMPro;

public class StageUI : MonoBehaviour, IEnemyObserver
{
    [SerializeField] TextMeshProUGUI highestStage;
    [SerializeField] TextMeshProUGUI stage;

    [SerializeField] GameObject hpBar;
    [SerializeField] RectTransform hpBarFill;
    [SerializeField] TextMeshProUGUI hpBarText;

    private void Start()
    {
        highestStage.text = $"최고 스테이지: {GameManager.gm.highestStageIndex + 1}";
        stage.text = $"스테이지 {GameManager.gm.stageIndex + 1}";
    }

    public void EnemyUpdated(EnemyObservable enemyCTRL)
    {
        hpBar.SetActive(!enemyCTRL.IsDead);

        Ability hp = (Ability)enemyCTRL["HP"];
        long currentHp = (long)hp.point - enemyCTRL.totalDamage;
        currentHp = currentHp < 0 ? 0 : currentHp;

        hpBarFill.localScale = new Vector3(currentHp / (float)hp.point, 1, 1);
        hpBarText.text = $"{currentHp}/{hp.point}";

        if (!enemyCTRL.IsDead && enemyCTRL.IsStop) {
            highestStage.text = $"최고 스테이지: {GameManager.gm.highestStageIndex + 1}";
            stage.text = $"스테이지 {GameManager.gm.stageIndex + 1}";
        }
    }
}
