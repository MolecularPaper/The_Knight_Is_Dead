using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class WeaponUI : MonoBehaviour, IWeaponObserver
{
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponTitle;
    [SerializeField] private Button weaponInfoButton;
    [SerializeField] private CanvasGroup block;

    public void SetWeaponUI(Weapon weapon, UnityAction action)
    {
        weaponIcon.sprite = weapon.weaponIcon;
        weaponTitle.text = weapon.weaponTitle;
        weaponTitle.text = weapon.weaponTitle;
        weaponInfoButton.onClick.AddListener(action);

        WeaponUpdate(weapon);
        weapon.Subscribe(this);
    }

    public void WeaponUpdate(WeaponObservable weaponObservable)
    {
        if (weaponObservable.isUnlock) {
            block.alpha = 0;
            block.blocksRaycasts = false;
            weaponObservable.Unsubscribe(this);
        }
    }
}
