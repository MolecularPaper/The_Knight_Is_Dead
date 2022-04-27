using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SkillUI : MonoBehaviour, ISkillObserver
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI requestSkillPoint;
    [SerializeField] private TextMeshProUGUI unlockLevel;
    [SerializeField] private GameObject lockPanel;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button levelUpButton;
    [SerializeField] private EventTrigger levelUpTrigger;
    [SerializeField] private Image icon;

    private string titleText;
    private string descriptionText;

    public void SetSkillUI(PlayerCTRL playerCTRL, SkillSelectUI skillSelectUI, Skill skill)
    {
        gameObject.name = skill.skillName;

        unlockLevel.text = $"{skill.unlockLevel} 레벨 도달시 잠금해제됨";
        lockPanel.SetActive(skill.isLock);

        titleText = title.text;
        descriptionText = description.text;

        icon.sprite = skill.skillicon;
        titleText = skill.skillTitle;
        descriptionText = skill.skillDescription;

        selectButton.onClick.AddListener(() => {
            skillSelectUI.SelectSkill(skill.skillName);
        });

        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => {
            playerCTRL.LevelUpSkill(skill.skillName);
        });

        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => {
            playerCTRL.isHoldButton = false;
        });

        levelUpTrigger.triggers.Add(pointerDown);
        levelUpTrigger.triggers.Add(pointerUp);

        SkillUpdated(skill);
        skill.Subscribe(this);
    }

    public void SkillUpdated(SkillExtension skillExtension)
    {
        title.text = titleText.Replace("{level}", skillExtension.level.ToString());
        description.text = descriptionText.Replace("{point_persent}", (skillExtension.Point / 100f).ToString());
        requestSkillPoint.text = $"{skillExtension.RequestSkillPoint}포인트";
        lockPanel.SetActive(skillExtension.isLock);

        levelUpButton.interactable = !skillExtension.isLock && skillExtension.canLevelUp;
    }
}
