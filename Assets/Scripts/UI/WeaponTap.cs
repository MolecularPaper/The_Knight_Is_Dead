using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponTap : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private Sprite frame;
    [SerializeField] private TMP_FontAsset font;

    [Space(10)]
    [SerializeField] private Canvas weaponInfoCanvas;
    [SerializeField] private Image weaponInfoIcon;
    [SerializeField] private TextMeshProUGUI weaponInfoTitle;
    [SerializeField] private TextMeshProUGUI weaponInfoDesciption;
    [SerializeField] private Button weaponInfoReinforceButton;
    [SerializeField] private Button weaponInfoMountButton;
    [SerializeField] private TextMeshProUGUI weaponInfoMountText;
    [SerializeField] private TextMeshProUGUI weaponInfoCount;
    [SerializeField] private TextMeshProUGUI weaponReinforceCount;
    [SerializeField] private TextMeshProUGUI weaponRequestSoul;
 
    private void Awake()
    {
        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();

        foreach (var item in playerCTRL.weapons) {
            GameObject weaponSlot = new GameObject("WeaponSlot");
            weaponSlot.transform.parent = content;

            Image weaponSlotImage = weaponSlot.AddComponent<Image>();
            weaponSlotImage.sprite = frame;

            Button weaponSlotButton = weaponSlot.AddComponent<Button>();
            weaponSlotButton.image = weaponSlotImage;
            weaponSlotButton.onClick.AddListener(() => { 
                WeaponReinforce(item);
            });

            GameObject weaponIcon = new GameObject("WeaponIcon");
            weaponIcon.transform.parent = weaponSlot.transform;
            Image weaponIconImage = weaponIcon.AddComponent<Image>();
            weaponIconImage.sprite = item.weaponIcon;

            RectTransform weaponIconRect = weaponIcon.GetComponent<RectTransform>();
            weaponIconRect.anchorMin = new Vector2(0, 0);
            weaponIconRect.anchorMax = new Vector2(1, 1);
            weaponIconRect.offsetMin = new Vector2(20, 70);
            weaponIconRect.offsetMax = new Vector2(-20, -20);

            GameObject weaponTitle = new GameObject("WeaponTitle");
            weaponTitle.transform.parent = weaponSlot.transform;

            TextMeshProUGUI weaponTitleTMP = weaponTitle.AddComponent<TextMeshProUGUI>();
            weaponTitleTMP.font = font;
            weaponTitleTMP.text = item.weaponTitle;
            weaponTitleTMP.enableAutoSizing = true;
            weaponTitleTMP.enableWordWrapping = false;
            weaponTitleTMP.alignment = TextAlignmentOptions.Center;

            RectTransform weaponTitleRect = weaponTitle.GetComponent<RectTransform>();
            weaponTitleRect.anchorMin = new Vector2(0, 0);
            weaponTitleRect.anchorMax = new Vector2(1, 1);
            weaponTitleRect.offsetMin = new Vector2(20, 20);
            weaponTitleRect.offsetMax = new Vector2(-20, -130);
        }
    }

    private void WeaponReinforce(Weapon weapon)
    {
        weaponInfoCanvas.enabled = true;
        weaponInfoIcon.sprite = weapon.weaponIcon;
        weaponInfoTitle.text = $"{weapon.weaponTitle} +{weapon.level}";
        weaponInfoDesciption.text = weapon.weaponDescription.Replace("{point_persent}", (weapon.Point / 100f).ToString());
        weaponInfoCount.text = $"ÇöÀç {weapon.count}°³ º¸À¯ÇÔ";
        weaponReinforceCount.text = $"{weapon.count} / {weapon.RequestCount}";
        weaponRequestSoul.text = $"{weapon.RequestSoul}¼Ò¿ï";

        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();

        weaponInfoMountButton.interactable = playerCTRL.currentWeapon != weapon;
        weaponInfoMountButton.onClick.AddListener(() => {
            if(!weapon.isUnlock) {
                return;
            }

            if(playerCTRL.currentWeapon != null) {
                playerCTRL.currentWeapon.isHold = false;
            }

            playerCTRL.currentWeapon = weapon;
            weapon.isHold = true;

            if (playerCTRL.currentWeapon == weapon) {
                weaponInfoMountText.text = "ÀåÂøÁß";
            }
            else {
                weaponInfoMountText.text = "ÀåÂøÇÏ±â";
            }

            weaponInfoMountButton.interactable = playerCTRL.currentWeapon != weapon;
        });

        if(playerCTRL.currentWeapon == weapon) {
            weaponInfoMountText.text = "ÀåÂøÁß";
        }
        else {
            weaponInfoMountText.text = "ÀåÂøÇÏ±â";
        }

        weaponInfoReinforceButton.interactable = weapon.canLevelUp;
        weaponInfoReinforceButton.onClick.AddListener(() => {
            weapon.LevelUp();
            weaponInfoReinforceButton.interactable = weapon.canLevelUp;
        });
    }
}
