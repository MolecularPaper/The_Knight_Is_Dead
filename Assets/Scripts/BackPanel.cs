using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BackPanel : MonoBehaviour, IPlayerObserver
{
    /// <summary>
    /// ��� �ִϸ����� ������Ʈ
    /// </summary>
    [SerializeField] private List<Animator> backAnimators;
    /// <summary>
    /// ��� �ִϸ��̼� �ӵ� ���� �ӵ�
    /// </summary>
    [SerializeField] private float backRampRate;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerCTRL playerCTRL = player.GetComponent<PlayerCTRL>();
        playerCTRL.Subscribe(this);

        foreach (Animator backAnimator in backAnimators) {
            backAnimator.speed = 0;
        }
    }

    public async void PlayerUpdated(PlayerInfo playerInfo)
    {
        if (!playerInfo.IsMove) {
            while (backAnimators[0].speed != 0) {
                foreach (Animator backAnimator in backAnimators) {
                    backAnimator.speed = Mathf.MoveTowards(backAnimator.speed, 0, backRampRate * Time.deltaTime);
                }
                await Task.Delay(1);
            }
        }
        else if (playerInfo.IsMove) {
            while (backAnimators[0].speed != 1) {
                foreach (Animator backAnimator in backAnimators) {
                    backAnimator.speed = Mathf.MoveTowards(backAnimator.speed, 1, backRampRate * Time.deltaTime);
                }
                await Task.Delay(1);
            }
        }
    }
}