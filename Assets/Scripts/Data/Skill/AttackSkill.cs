using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSkill : SkillObject
{
    public void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        this.playerInfo = player.GetComponent<PlayerCTRL>();
    }

    public override void Result()
    {
        Ability atk = (Ability)this.playerInfo["ATK"];
        GameManager.gm.AttackEnemy((ulong)(atk.point * (skillInfo.point / 10000f)));

        this.skillInfo = null;
        gameObject.SetActive(false);
    }
}