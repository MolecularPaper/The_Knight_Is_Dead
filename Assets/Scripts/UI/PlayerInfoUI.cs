using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PlayerInfoUI : MonoBehaviour, IPlayerObserver
{
    [SerializeField] private TextMeshProUGUI infoText;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerCTRL playerCTRL = player.GetComponent<PlayerCTRL>();
        playerCTRL.Subscribe(this);
    }

    public void PlayerUpdated(PlayerInfoExtension playerInfo)
    {
        infoText.text = $"{playerInfo.nickName} {playerInfo.level}LV";
    }
}
