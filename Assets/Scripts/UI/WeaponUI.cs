using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private Sprite frame;
 
    private void Awake()
    {
        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();

        foreach (var item in playerCTRL.weapons) {
            GameObject weaponSlot = Instantiate(new GameObject(), content);
            weaponSlot.name = "WeaponSlot";
            Image weaponSlotImage = weaponSlot.AddComponent<Image>();
            weaponSlotImage.sprite = frame;

            GameObject weaponIcon = Instantiate(new GameObject(), weaponSlot.transform);
            weaponIcon.name = "WeaponIcon";
            Image weaponIconImage = weaponIcon.AddComponent<Image>();
            weaponIconImage.sprite = item.weaponIcon;

            RectTransform weaponIconRect = weaponIcon.GetComponent<RectTransform>();
            weaponIconRect.anchorMin = new Vector2(0, 0);
            weaponIconRect.anchorMax = new Vector2(1, 1);
            weaponIconRect.offsetMin = new Vector2(20, 20);
            weaponIconRect.offsetMax = new Vector2(-20, -20);
        }
    }
}
