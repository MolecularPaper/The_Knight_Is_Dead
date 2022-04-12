using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SkillUI : MonoBehaviour, ISkillObserver
{
    [SerializeField] private string skillName;

    public TextMeshProUGUI title;
    public TextMeshProUGUI requestSkillPoint;
    public TextMeshProUGUI description;
    public Button levelUpButton;

    private string titleText;
    private string descriptionText;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerCTRL playerCTRL = player.GetComponent<PlayerCTRL>();
        ((Skill)playerCTRL[skillName]).Subscribe(this);

        titleText = title.text;
        descriptionText = description.text;
    }

    public void SkillUpdated(SkillExtension skillExtension)
    {
        title.text = titleText.Replace("{LV}", skillExtension.level.ToString());
        requestSkillPoint.text = $"{skillExtension.RequestSkillPoint}Æ÷ÀÎÆ®";
        description.text = descriptionText.Replace("{point}", (skillExtension.point / 100f).ToString());
        levelUpButton.interactable = skillExtension.canLevelUp;
    }
}