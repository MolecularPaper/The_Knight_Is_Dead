using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class AbilityUI : MonoBehaviour, IAbilityObserver
{
    [SerializeField] private string titleName;
    [SerializeField] private string abilityName;

    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public TextMeshProUGUI requestSoul;
    public Button levelUpButton;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerCTRL playerCTRL = player.GetComponent<PlayerCTRL>();
        playerCTRL[abilityName].Subscribe(this);
    }

    public void AbilityUpdated(AbilityExtension abilityInfo)
    {
        this.title.text = $"{titleName} {abilityInfo.level}LV";
        if (string.IsNullOrEmpty(abilityInfo.sign)) {
            this.description.text = $"{abilityInfo.point} -> {abilityInfo.NextPoint}";
        }
        else {
            this.description.text = $"{string.Format("{0:0.00}",abilityInfo.point / 100f)}{abilityInfo.sign} -> {string.Format("{0:0.00}", abilityInfo.NextPoint / 100f)}{abilityInfo.sign}";
        }
        this.requestSoul.text = $"{abilityInfo.RequestSoul}¼Ò¿ï";
        this.levelUpButton.interactable = abilityInfo.canLevelUp;
    }
}