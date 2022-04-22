using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTap : MonoBehaviour
{
    [SerializeField] private Transform skillTap;
    [SerializeField] private GameObject skillUI;

    void Start()
    {
        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();
        SkillSelectUI skillSelectUI = FindObjectOfType<SkillSelectUI>();

        foreach (var item in playerCTRL.skills) {
            SkillUI skillUI = Instantiate(this.skillUI, skillTap).GetComponent<SkillUI>();
            skillUI.SetSkillUI(playerCTRL, skillSelectUI, item);
        }
    }
}
