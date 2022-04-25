using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class WeaponDrawSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;

    [HideInInspector] public int count;
    [HideInInspector] public string weaponName;

    public void SetSlot(Weapon weapon)
    {
        gameObject.SetActive(false);

        this.icon.sprite = weapon.weaponIcon;
        weaponName = weapon.weaponTitle;

        count = 1;
        countText.text = count.ToString() + "°³"; ;
    }

    public void ResetSlot()
    {
        count = 0;
        countText.text = count.ToString() + "°³"; ;

        gameObject.SetActive(false);
    }

    public void CountUp()
    {
        if(count == 0) {
            gameObject.SetActive(true);
        }

        count++;
        countText.text = count.ToString() + "°³";
    }
}
