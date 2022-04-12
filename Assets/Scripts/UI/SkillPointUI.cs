using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillPointUI : MonoBehaviour, IPlayerObserver
{
    private TextMeshProUGUI description;
    private string descriptionText;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerCTRL playerCTRL = player.GetComponent<PlayerCTRL>();
        playerCTRL.Subscribe(this);

        description = GetComponent<TextMeshProUGUI>();
        descriptionText = description.text;
    }

    public void PlayerUpdated(PlayerInfoExtension playerInfo)
    {
        description.text = descriptionText.Replace("{point}", playerInfo.skillPoint.ToString());
    }
}