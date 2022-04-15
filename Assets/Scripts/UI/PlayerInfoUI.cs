using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PlayerInfoUI : MonoBehaviour, IPlayerObserver
{
    [SerializeField] private TextMeshProUGUI infoText;

    [SerializeField] RectTransform hpBarFill;
    [SerializeField] TextMeshProUGUI hpBarText;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerCTRL playerCTRL = player.GetComponent<PlayerCTRL>();
        playerCTRL.Subscribe(this);

        infoText.text = $"{playerCTRL.nickName} {playerCTRL.level}LV";
    }

    public void PlayerUpdated(PlayerInfoExtension playerInfo)
    {
        Ability hp = (Ability)playerInfo["HP"];
        long currentHp = (long)hp.point - playerInfo.totalDamage;
        currentHp = currentHp < 0 ? 0 : currentHp;

        hpBarFill.localScale = new Vector3(currentHp / (float)hp.point, 1, 1);
        hpBarText.text = $"{currentHp}/{hp.point}";
    }
}
