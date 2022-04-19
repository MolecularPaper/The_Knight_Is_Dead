using UnityEngine;
using TMPro;

public class StageUI : MonoBehaviour, IGameObserver, IEnemyObserver
{
    [SerializeField] TextMeshProUGUI highestStage;
    [SerializeField] TextMeshProUGUI stage;

    [SerializeField] GameObject hpBar;
    [SerializeField] RectTransform hpBarFill;
    [SerializeField] TextMeshProUGUI hpBarText;
    
    void Start()
    {
        GameManager.gm.Subscribe(this);
    }

    public void EnemyUpdated(EnemyObservable enemyCTRL)
    {
        hpBar.SetActive(!enemyCTRL.IsDead);

        Ability hp = (Ability)enemyCTRL["HP"];
        long currentHp = (long)hp.point - enemyCTRL.totalDamage;
        currentHp = currentHp < 0 ? 0 : currentHp;

        hpBarFill.localScale = new Vector3(currentHp / (float)hp.point, 1, 1);
        hpBarText.text = $"{currentHp}/{hp.point}";
    }

    public void GameUpdated(GameInfoExtension gameInfo)
    {
        highestStage.text = $"�ְ� ��������: {gameInfo.highestStageIndex + 1}";
        stage.text = $"�������� {gameInfo.stageIndex + 1}";
    }
}
