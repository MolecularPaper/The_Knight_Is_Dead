using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
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
    [SerializeField] private TextMeshProUGUI weaponInfoMountText;
    [SerializeField] private TextMeshProUGUI weaponInfoCount;
    [SerializeField] private TextMeshProUGUI weaponReinforceCount;
    [SerializeField] private TextMeshProUGUI weaponRequestSoul;

    [Space(10)]
    [SerializeField] private Button weaponInfoReinforceButton;
    [SerializeField] private EventTrigger weaponInfoReinforceTrigger;
    [SerializeField] private Button weaponInfoMountButton;

    private void Start()
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

        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();
        SetWeaponInfo(weapon);

        weaponInfoMountButton.onClick.AddListener(() => {
            if(!weapon.isUnlock) {
                return;
            }

            if(playerCTRL.currentWeapon != null) {
                playerCTRL.currentWeapon.isHold = false;
            }

            playerCTRL.currentWeapon = weapon;
            weapon.isHold = true;

            SetWeaponInfo(weapon);
        });

        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => {
            LevelUpWeapon(weapon);
        });

        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => {
            isHoldButton = false;
        });

        weaponInfoReinforceTrigger.triggers.Add(pointerDown);
        weaponInfoReinforceTrigger.triggers.Add(pointerUp);
    }

    public bool isHoldButton { get; set; }
    public async void LevelUpWeapon(Weapon weapon)
    {
        isHoldButton = true;
        while (weapon.canLevelUp && isHoldButton) {
            weapon.LevelUp();
            SetWeaponInfo(weapon);
            await GameManager.gm.Delay(100);
        }
    }

    public void SetWeaponInfo(Weapon weapon)
    {
        weaponInfoTitle.text = $"{weapon.weaponTitle} +{weapon.level}";
        weaponInfoDesciption.text = weapon.weaponDescription.Replace("{point_persent}", (weapon.Point / 100f).ToString());
        weaponInfoCount.text = $"현재 {weapon.count}개 보유함";
        weaponReinforceCount.text = $"{weapon.count} / {weapon.RequestCount}";
        weaponRequestSoul.text = $"{weapon.RequestSoul}소울";

        weaponInfoReinforceButton.interactable = weapon.canLevelUp;
        weaponInfoMountButton.interactable = !weapon.isHold;

        if (weapon.isHold) {
            weaponInfoMountText.text = "장착중";
        }
        else {
            weaponInfoMountText.text = "장착하기";
        }
    }
}
