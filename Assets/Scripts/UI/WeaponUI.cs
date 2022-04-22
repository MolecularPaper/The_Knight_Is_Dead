using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class WeaponUI : MonoBehaviour, IWeaponObserver
{
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponTitle;
    [SerializeField] private Button weaponInfoButton;
    [SerializeField] private CanvasGroup block;

    public void SetWeaponUI()
    {

    }

    public void WeaponUpdate(WeaponExtension weaponExtension)
    {
        throw new System.NotImplementedException();
    }
}
