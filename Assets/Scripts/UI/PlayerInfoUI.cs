using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PlayerInfoUI : MonoBehaviour, IPlayerObserver
{
    [SerializeField] private TextMeshProUGUI info;
    [SerializeField] private TextMeshProUGUI skillPoint;

    [SerializeField] RectTransform hpBarFill;
    [SerializeField] TextMeshProUGUI hpBarText;

    private string skillPointText;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerCTRL playerCTRL = player.GetComponent<PlayerCTRL>();
        playerCTRL.Subscribe(this);

        skillPointText = skillPoint.text;

        PlayerUpdated(playerCTRL);
    }

    public void PlayerUpdated(PlayerInfoExtension playerInfo)
    {
        Ability hp = (Ability)playerInfo["HP"];
        long currentHp = (long)hp.point - playerInfo.totalDamage;
        currentHp = currentHp < 0 ? 0 : currentHp;
        
        hpBarFill.localScale = new Vector3(currentHp / (float)hp.point, 1, 1);
        hpBarText.text = $"{currentHp}/{hp.point}";

        info.text = $"{GameManager.gm.playerNickname} {playerInfo.level}LV";
        skillPoint.text = skillPointText.Replace("{point}", playerInfo.skillPoint.ToString());
    }
}
