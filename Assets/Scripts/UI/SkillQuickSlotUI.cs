using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SkillQuickSlotUI : MonoBehaviour, IPlayerObserver
{
    [SerializeField] private Image enbledSkillIcon;

    public void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerCTRL playerCTRL = player.GetComponent<PlayerCTRL>();
        playerCTRL.Subscribe(this);
    }

    public void PlayerUpdated(PlayerInfoExtension playerInfo)
    {
        print(playerInfo.enbledSkill != null);

        if(playerInfo.enbledSkill != null) {
            enbledSkillIcon.gameObject.SetActive(true);
            enbledSkillIcon.sprite = playerInfo.enbledSkill.skillIcon;
        }
        else {
            enbledSkillIcon.gameObject.SetActive(false);
            enbledSkillIcon.sprite = null;
        }
        
    }
}
