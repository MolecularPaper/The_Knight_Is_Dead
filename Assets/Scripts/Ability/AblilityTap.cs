using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AblilityTap : MonoBehaviour
{
    [SerializeField] private Transform abilityTap;
    [SerializeField] private GameObject abilityUI;

    void Start()
    {
        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();

        foreach (var item in playerCTRL.abilities) {
            AbilityUI abilityUI = Instantiate(this.abilityUI, abilityTap).GetComponent<AbilityUI>();
            abilityUI.SetAbility(playerCTRL, item);
        }
    }
}
