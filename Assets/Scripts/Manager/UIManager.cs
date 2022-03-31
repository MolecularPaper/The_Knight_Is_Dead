using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager ui;
    private AudioSource audioSource;

    [SerializeField] private TextMeshProUGUI stage;
    [SerializeField] PlayerCTRL player;
    [SerializeField] CanvasGroup[] taps;
    private int activeTapIndex = 0;

    [Header("Abillity")]
    public InceaseAbillity[] abilityUI;

    [Header("Item")]
    [SerializeField] ItemInfo[] itemInfos;

    [Header("Sound")]
    [SerializeField] AudioClip buttonClickSound;

    void Awake()
    {
        ui = this;
        audioSource = GetComponent<AudioSource>();

        UpdateItemUI();
        AddButtonSoundEffect();
    }

    private void Start()
    {
        UdateAllAbilityUI();
    }

    public void UpdateStage(int stageNum) => stage.text = $"스테이지 {stageNum}";

    public void TapControl(int _activeTapIndex)
    {
        taps[activeTapIndex].gameObject.SetActive(false);
        taps[_activeTapIndex].gameObject.SetActive(true);
        activeTapIndex = _activeTapIndex;
    }

    public void UpdateItemUI()
    {
        foreach (ItemInfo itemInfo in itemInfos) {
            if (itemInfo.isActive) {
                switch (itemInfo.itemType) {
                    case ItemType.Soul:
                        itemInfo.count.text = player.playerData.soul.ToString();
                        break;
                    case ItemType.Diamond:
                        itemInfo.count.text = player.playerData.diamond.ToString();
                        break;
                    case ItemType.Crystal:
                        itemInfo.count.text = player.playerData.crystal.ToString();
                        break;
                }
            }
        }
    }

    private void AddButtonSoundEffect()
    {
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button button in allButtons) {
            button.onClick.AddListener(() => { audioSource.PlayOneShot(buttonClickSound); });
        }
    }

    public void UdateAllAbilityUI()
    {
        for (int i = 0; i < abilityUI.Length; i++) {
            UdateAbilityUI((AbilityType)i);
        }
    }

    public void UdateAbilityUI(AbilityType type)
    {
        Ability ability = player.playerData.abilities[type];

        if (ability.requestSoul > player.playerData.soul || ability.level >= ability.maxLevel) {
            abilityUI[(int)type].levelUpButton.interactable = false;
        }
        else {
            abilityUI[(int)type].levelUpButton.interactable = true;
        }

        string point = ability.point.ToString();
        string nextPoint = ability.nextPoint.ToString();
        if(!string.IsNullOrEmpty(ability.sign)) {
            point = string.Format("{0:F2}", ability.point / 100f) + ability.sign;
            nextPoint = string.Format("{0:F2}", ability.nextPoint / 100f) + ability.sign;
        }

        abilityUI[(int)type].level.text = $"{abilityUI[(int)type].title} {ability.level}LV";
        abilityUI[(int)type].description.text = $"{point} -> {nextPoint}";
        abilityUI[(int)type].soul.text = $"{string.Format("{0:#,#}", ability.requestSoul)} 소울";
    }
}
