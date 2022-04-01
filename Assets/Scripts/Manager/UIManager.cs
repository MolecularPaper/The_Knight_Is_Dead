using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager ui;
    private AudioSource audioSource;

    [SerializeField] TextMeshProUGUI highestStage;
    [SerializeField] TextMeshProUGUI stage;
    [SerializeField] PlayerCTRL player;
    [SerializeField] CanvasGroup[] taps;
    
    [Space(10)]
    [SerializeField] CanvasGroup fade;
    [SerializeField] float fadeSpeed;

    private int activeTapIndex = 0;

    [Header("Abillity")]
    public AbillityUI[] abilityUI;

    [Header("Item")]
    [SerializeField] ItemInfo[] itemInfos;

    [Header("Sound")]
    [SerializeField] AudioClip buttonClickSound;

    void Awake()
    {
        ui = this;
        audioSource = GetComponent<AudioSource>();
        AddButtonSoundEffect();
    }

    private void Start()
    {
        UpdateItemUI();
        UdateAllAbilityUI();
    }

    public void UpdateStage(int stageIndex, int highestIndex)
    {
        highestStage.text = $"최고 스테이지: {highestIndex + 1}";
        stage.text = $"스테이지 {stageIndex + 1}";
    }

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
        EventTrigger[] allButtons = FindObjectsOfType<EventTrigger>();
        foreach (EventTrigger eventTrigger in allButtons) {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.callback.AddListener((data) => { audioSource.PlayOneShot(buttonClickSound); });
            eventTrigger.triggers.Add(entry);
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

        string point = ability.point.ToString();
        string nextPoint = ability.nextPoint.ToString();
        if (!string.IsNullOrEmpty(ability.sign)) {
            point = string.Format("{0:F2}", ability.point / 100f) + ability.sign;
            nextPoint = string.Format("{0:F2}", ability.nextPoint / 100f) + ability.sign;
        }

        if (ability.requestSoul > player.playerData.soul) {
            abilityUI[(int)type].levelUpButton.interactable = false;
        }
        else if(ability.point >= ability.maxPoint) {
            abilityUI[(int)type].levelUpButton.interactable = false;
            abilityUI[(int)type].description.text = point;
            abilityUI[(int)type].soul.text = "MAX";
            return;
        }
        else {
            abilityUI[(int)type].levelUpButton.interactable = true;
        }

        abilityUI[(int)type].level.text = $"{abilityUI[(int)type].title} {ability.level}LV";
        abilityUI[(int)type].description.text = $"{point} -> {nextPoint}";
        abilityUI[(int)type].soul.text = $"{string.Format("{0:#,#}", ability.requestSoul)} 소울";
    }

    public async Task FadeOut(bool isFadeOut)
    {
        fade.gameObject.SetActive(true);

        while (true) {
            if (isFadeOut) {
                fade.alpha = Mathf.MoveTowards(fade.alpha, 1, fadeSpeed * Time.deltaTime);
                if (fade.alpha == 1) break;
            }
            else {
                fade.alpha = Mathf.MoveTowards(fade.alpha, 0, fadeSpeed * Time.deltaTime);
                if (fade.alpha == 0) {
                    fade.gameObject.SetActive(false);
                    break;
                }
            }

            await Task.Delay(1);
        }
    }
}
