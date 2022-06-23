using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpUI : MonoBehaviour, IPlayerObserver
{
    [SerializeField] private RectTransform expFill;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerCTRL playerCTRL = player.GetComponent<PlayerCTRL>();
        playerCTRL.Subscribe(this);
    }

    public void PlayerUpdated(PlayerInfoExtension playerInfo)
    {
        expFill.anchorMax = new Vector2(playerInfo.exp / playerInfo.RequestExp, 1);
    }
}