using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerCTRL _player;
    [SerializeField] private float canActionDistance;

    public static GameManager gm { get; set; }
    public EnemyCTRL currentEnemy { get; private set; }
    public PlayerCTRL player { get => _player; }

    public IncreaseData increaseData;

    public bool canAction {
        get => currentEnemy && player && Vector3.Distance(player.transform.position, currentEnemy.transform.position) <= canActionDistance;
    }

    void Awake()
    {
        gm = this;
    }

    void Update()
    {

    }

    public void SetEnemy(EnemyCTRL enemy)
    {
        currentEnemy = enemy;
        player.Stop();
    }

    public async void EnemyDead()
    {
        currentEnemy = null;

        await Task.Delay(1500);
        player.Move();

        await Task.Delay(2000);
        SpawnManager.sm.canSpawn = true;
    }

    public async void PlayerDead()
    {
        currentEnemy = null;
        await Task.Delay(1);
    }

    public void AddSoul(long count)
    {
        player.playerData.soul += count + (long)(count * player.playerData.lukPoint);
        UIManager.ui.UpdateItemUI();
    }

    public bool doIncreaseAbillity { get; set; }
    public async void IncreaseAbility(int index)
    {
        doIncreaseAbillity = true;
        bool canIncreaseAbillity = true;
        long requestSoul = 0;

        while (doIncreaseAbillity) {
            if (!canIncreaseAbillity) return;

            PlayerData playerData = player.playerData;
            canIncreaseAbillity = false;
            switch (index) {
                case 0:
                    requestSoul = playerData.RequestSoul(increaseData.atkSoulIncreseWidth, playerData.atkLevel);
                    if (!player.playerData.CanLevelUp(requestSoul)) return;

                    playerData.soul -= requestSoul;
                    playerData.atkPoint += (long)playerData.IncreasePoint(increaseData.atkIncreseWidth, playerData.atkLevel);
                    playerData.atkLevel++;
                    break;
                case 1:
                    requestSoul = playerData.RequestSoul(increaseData.defSoulIncreseWidth, playerData.defLevel);
                    if (!player.playerData.CanLevelUp(requestSoul)) return;

                    playerData.soul -= requestSoul;
                    playerData.defPoint += (long)playerData.IncreasePoint(increaseData.defIncreseWidth, playerData.defLevel);
                    playerData.defLevel++;
                    break;
                case 2:
                    requestSoul = playerData.RequestSoul(increaseData.lukSoulIncreseWidth, playerData.lukLevel);
                    if (!player.playerData.CanLevelUp(requestSoul)) return;

                    playerData.soul -= requestSoul;
                    playerData.lukPoint += playerData.IncreasePoint(increaseData.lukIncreseWidth, playerData.lukLevel)/100;
                    playerData.lukLevel++;
                    break;
                case 3:
                    requestSoul = playerData.RequestSoul(increaseData.cridSoulIncreseWidth, playerData.cridLevel);
                    if (!player.playerData.CanLevelUp(requestSoul)) return;

                    playerData.soul -= requestSoul;
                    playerData.cridPoint += playerData.IncreasePoint(increaseData.cridIncreseWidth, playerData.cridLevel) / 100f;
                    playerData.cridLevel++;
                    break;
                case 4:
                    requestSoul = playerData.RequestSoul(increaseData.cripSoulIncreseWidth, playerData.cripLevel);
                    if (!player.playerData.CanLevelUp(requestSoul)) return;

                    playerData.soul -= requestSoul;
                    playerData.cripPoint += 0.01f;
                    playerData.cripLevel++;
                    break;
                default:
                    throw new System.FormatException();
            }

            UIManager.ui.UpdateAbillityIncreaseUI();
            UIManager.ui.UpdateItemUI();

            await Task.Delay(100);
            canIncreaseAbillity = true;
        }
    }
}
