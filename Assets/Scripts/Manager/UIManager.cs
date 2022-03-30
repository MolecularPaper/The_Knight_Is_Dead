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

    [SerializeField] PlayerCTRL player;
    [SerializeField] CanvasGroup[] taps;
    private int activeTapIndex = 0;

    [Header("Abillity")]
    public InceaseAbillity[] abillities;

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
        UpdateAbillityIncreaseUI();
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
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button button in allButtons) {
            button.onClick.AddListener(() => { audioSource.PlayOneShot(buttonClickSound); });
        }
    }

    public void HoldTest()
    {
        print("HoldCheck");
    }

    public void UpdateAbillityIncreaseUI()
    {
        PlayerData playerData = player.playerData;

        //공격력
        abillities[0].description.text = $"{playerData.atkPoint} -> {playerData.atkPoint + playerData.IncreasePoint(GameManager.gm.increaseData.atkIncreseWidth, playerData.atkLevel)} ";
        abillities[0].soul.text = $"{playerData.RequestSoul(GameManager.gm.increaseData.atkSoulIncreseWidth, playerData.atkLevel)} 소울";

        //방어력
        abillities[1].description.text = $"{playerData.defPoint} -> {playerData.defPoint + playerData.IncreasePoint(GameManager.gm.increaseData.defIncreseWidth, playerData.defLevel)} ";
        abillities[1].soul.text = $"{playerData.RequestSoul(GameManager.gm.increaseData.defSoulIncreseWidth, playerData.defLevel)} 소울";

        //운
        abillities[2].description.text = $"{string.Format("{0:F2}", playerData.lukPoint)} -> {string.Format("{0:F2}", playerData.lukPoint + playerData.IncreasePoint(GameManager.gm.increaseData.lukIncreseWidth, playerData.lukLevel)/100f)}%";
        abillities[2].soul.text = $"{playerData.RequestSoul(GameManager.gm.increaseData.lukSoulIncreseWidth, playerData.lukLevel)} 소울";

        //치명 데미지
        abillities[3].description.text = $"{string.Format("{0:F2}", playerData.cridPoint)}% -> {string.Format("{0:F2}", playerData.cridPoint + playerData.IncreasePoint(GameManager.gm.increaseData.cridIncreseWidth, playerData.cridLevel)/100f)}% ";
        abillities[3].soul.text = $"{playerData.RequestSoul(GameManager.gm.increaseData.cripSoulIncreseWidth, playerData.cridLevel)} 소울";
        
        //치명 확률
        if (player.playerData.cripPoint <= 10000) {
            abillities[4].description.text = $"{string.Format("{0:F2}", playerData.cripPoint)}% -> {string.Format("{0:F2}", playerData.cripPoint + 0.01f)}%";
            abillities[4].soul.text = $"{playerData.RequestSoul(GameManager.gm.increaseData.cripSoulIncreseWidth, playerData.cripLevel)} 소울";
        }
        else {
            player.playerData.cripPoint = 10000;
            abillities[4].levelUpButton.interactable = false;
        }
    }
}
