using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreUI : MonoBehaviour
{
    public GameObject adUI;

    void Start() => UpdateUI();

    public void UpdateUI()
    {
        adUI.SetActive(!GameManager.gm.adDeleted);
    }
}
