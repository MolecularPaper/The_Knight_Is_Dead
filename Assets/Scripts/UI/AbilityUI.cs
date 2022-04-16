using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class AbilityUI : MonoBehaviour, IAbilityObserver
{
    [SerializeField] private string abilityName;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI requestSoul;
    [SerializeField] private Button levelUpButton;

    private string titleText;
    private string descriptionText;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerCTRL playerCTRL = player.GetComponent<PlayerCTRL>();
        ((Ability)playerCTRL[abilityName]).Subscribe(this);

        titleText = title.text;
        descriptionText = description.text;
    }

    public void AbilityUpdated(AbilityExtension abilityInfo)
    {
        title.text = titleText.Replace("{LV}", abilityInfo.level.ToString());

        description.text = descriptionText.Replace("{point}", abilityInfo.point.ToString());
        description.text = description.text.Replace("{next_point}", abilityInfo.NextPoint.ToString());
        description.text = description.text.Replace("{point_persent}", string.Format("{0:0.00}", abilityInfo.point / 100f));
        description.text = description.text.Replace("{next_point_persent}", string.Format("{0:0.00}", abilityInfo.NextPoint / 100f));

        requestSoul.text = $"{abilityInfo.RequestSoul}�ҿ�";
        levelUpButton.interactable = abilityInfo.canLevelUp;
    }
}