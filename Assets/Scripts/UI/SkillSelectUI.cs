using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelectUI : MonoBehaviour, IPlayerObserver
{
    public string skillName;

    public void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerCTRL playerCTRL = player.GetComponent<PlayerCTRL>();
        playerCTRL.Subscribe(this);
    }

    public void PlayerUpdated(PlayerInfoExtension playerInfo) => gameObject.SetActive(((Skill)playerInfo[skillName]).level > 0);
}
