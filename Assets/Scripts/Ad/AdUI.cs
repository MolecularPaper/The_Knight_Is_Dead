using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

[System.Serializable]
public class AdUI : MonoBehaviour, IAdObserver
{
    [Space(10)]
    public Image icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI desciption;
    public Button button;
    public TextMeshProUGUI buttonText;

    public void SetAdUI(AdMethodExtension ad)
    {
        gameObject.name = ad.adName;
        icon.sprite = ad.icon;

        title.text = ad.adTitle;
        desciption.text = ad.desciption;

        button.interactable = ad.canShowAd;
        button.onClick.AddListener(() => {
            if (ad.canShowAd) {
                ad.ShowAd();
            }
        });

        ad.Subscribe(this);
        AdUpdated(ad);
    }

    public void AdUpdated(AdExtension adExtension)
    {
        button.interactable = adExtension.canShowAd && adExtension.buttonEnbled;

        if (adExtension.currentSecond > 0) {
            buttonText.text = $"{string.Format("{0:D2}", (adExtension.currentSecond % 3600) / 60)}:{string.Format("{0:D2}", (adExtension.currentSecond % 3600) % 60)}";
        }
        else {
            buttonText.text = "±§∞ÌΩ√√ª";
        }
    }
}