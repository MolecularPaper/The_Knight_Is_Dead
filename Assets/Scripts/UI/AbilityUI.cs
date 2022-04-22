using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class AbilityUI : MonoBehaviour, IAbilityObserver
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI requestSoul;
    [SerializeField] private Button levelUpButton;
    [SerializeField] private EventTrigger levelUpTrigger;

    private string titleText;
    private string descriptionText;

    public void SetAbility(PlayerCTRL playerCTRL, Ability ability)
    {
        this.gameObject.name = ability.abilityName;

        this.icon.sprite = ability.ablilityIcon;
        this.titleText = ability.ablilityTitle;
        this.descriptionText = ability.ablilityDescription;

        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => {
            playerCTRL.LevelUpAbility(ability.abilityName);
        });

        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => {
            playerCTRL.isHoldButton = false;
        });

        levelUpTrigger.triggers.Add(pointerDown);
        levelUpTrigger.triggers.Add(pointerUp);

        ability.Subscribe(this);
        AbilityUpdated(ability);
    }

    public void AbilityUpdated(AbilityExtension abilityInfo)
    {
        title.text = titleText.Replace("{level}", abilityInfo.level.ToString());

        description.text = descriptionText.Replace("{point}", abilityInfo.point.ToString());
        description.text = description.text.Replace("{next_point}", abilityInfo.NextPoint.ToString());
        description.text = description.text.Replace("{point_persent}", string.Format("{0:0.00}", abilityInfo.point / 100f));
        description.text = description.text.Replace("{next_point_persent}", string.Format("{0:0.00}", abilityInfo.NextPoint / 100f));

        requestSoul.text = $"{abilityInfo.RequestSoul}¼Ò¿ï";
        levelUpButton.interactable = abilityInfo.canLevelUp;
    }
}