using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponTap : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject weaponUI;

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
            WeaponUI weaponSlot = Instantiate(this.weaponUI, content).GetComponent<WeaponUI>();
            weaponSlot.SetWeaponUI(item, () => {
                WeaponReinforce(item);
            });
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
