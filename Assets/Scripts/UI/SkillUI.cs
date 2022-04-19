using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SkillUI : MonoBehaviour, ISkillObserver
{
    [SerializeField] private string skillName;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI requestSkillPoint;
    [SerializeField] private TextMeshProUGUI unlockLevel;
    [SerializeField] private GameObject lockPanel;
    [SerializeField] private Button levelUpButton;
    [SerializeField] private Image icon;

    private string titleText;
    private string descriptionText;

    void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerCTRL playerCTRL = player.GetComponent<PlayerCTRL>();

        Skill skill = (Skill)playerCTRL[skillName];
        skill.Subscribe(this);

        unlockLevel.text = $"{skill.unlockLevel} 레벨 도달시 잠금해제됨";
        lockPanel.SetActive(skill.isLock);

        titleText = title.text;
        descriptionText = description.text;

        icon.sprite = skill.icon;

        SkillUpdated(skill);
    }

    public void SkillUpdated(SkillExtension skillExtension)
    {
        title.text = titleText.Replace("{LV}", skillExtension.level.ToString());
        description.text = descriptionText.Replace("{point}", skillExtension.Point.ToString() + "%");
        requestSkillPoint.text = $"{skillExtension.RequestSkillPoint}포인트";
        lockPanel.SetActive(skillExtension.isLock);

        levelUpButton.interactable = !skillExtension.isLock && skillExtension.canLevelUp;
    }

}
