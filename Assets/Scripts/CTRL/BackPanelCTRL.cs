using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPanelCTRL : MonoBehaviour
{
    /// <summary>
    /// ��� �ִϸ����� ������Ʈ
    /// </summary>
    [SerializeField] private List<Animator> backAnimators;
    /// <summary>
    /// ��� �ִϸ��̼� �ӵ� ���� �ӵ�
    /// </summary>
    [SerializeField] private float backRampRate;
    
    private PlayerCTRL player;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerCTRL>();

        foreach (Animator backAnimator in backAnimators) {
            backAnimator.speed = 0;
        }
    }

    void Update()
    {
        if (backAnimators.Count > 0) {
            UpdateBackgroundAnimation();
        }
    }

    public void UpdateBackgroundAnimation()
    {
        foreach (Animator backAnimator in backAnimators) {
            if (player && !player.isMove && backAnimator.speed != 0) backAnimator.speed = Mathf.MoveTowards(backAnimator.speed, 0, backRampRate);
            else if (player && player.isMove && backAnimator.speed != 1) backAnimator.speed = Mathf.MoveTowards(backAnimator.speed, 1, backRampRate);
        }
    }
}
