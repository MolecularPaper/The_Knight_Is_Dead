using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager ui;

    [SerializeField] private PlayerCTRL player;
    [SerializeField] private CanvasGroup[] taps;
    private int activeTapIndex = 0;

    [Header("Item")]
    [SerializeField] List<ItemInfo> itemInfos;


    void Awake()
    {
        ui = this;
        UpdateItemUI();
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
}
