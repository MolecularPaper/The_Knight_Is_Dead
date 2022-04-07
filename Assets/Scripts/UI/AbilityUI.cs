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

    public void AbilityUpdated(AbilityInfo abilityInfo)
    {
        this.title.text = $"{titleName} {abilityInfo.level}LV";
        this.description.text = $"{abilityInfo.point} -> {abilityInfo.NextPoint}";
        this.requestSoul.text = $"{abilityInfo.RequestSoul}¼Ò¿ï";
        this.levelUpButton.interactable = abilityInfo.canLevelUp;
    }
}