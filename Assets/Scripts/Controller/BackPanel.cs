using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BackPanel : MonoBehaviour, IPlayerObserver
{
    /// <summary>
    /// 배경 애니메이터 컴포넌트
    /// </summary>
    [SerializeField] private List<Animator> backAnimators;
    /// <summary>
    /// 배경 애니메이션 속도 증감 속도
    /// </summary>
    [SerializeField] private float backRampRate;

    private void Awake()
    {
        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();
        playerCTRL.Subscribe(this);

        foreach (Animator backAnimator in backAnimators) {
            backAnimator.speed = 0;
        }
    }

    public async void PlayerUpdated(PlayerInfoExtension playerInfo)
    {
        if (!playerInfo.IsMove) {
            while (backAnimators[0].speed != 0) {
                foreach (Animator backAnimator in backAnimators) {
                    backAnimator.speed = Mathf.MoveTowards(backAnimator.speed, 0, backRampRate * Time.deltaTime);
                }
                try { await Task.Delay(1, GameManager.tokenSource.Token); }
                catch (TaskCanceledException) { return; }
            }
        }
        else if (playerInfo.IsMove) {
            while (backAnimators[0].speed != 1) {
                foreach (Animator backAnimator in backAnimators) {
                    backAnimator.speed = Mathf.MoveTowards(backAnimator.speed, 1, backRampRate * Time.deltaTime);
                }
                try { await Task.Delay(1, GameManager.tokenSource.Token); }
                catch (TaskCanceledException) { return; }
            }
        }
    }
}
