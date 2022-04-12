using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapUI : MonoBehaviour
{
    [SerializeField] CanvasGroup[] taps;
    private int activeTapIndex = 0;

    public void TapControl(int _activeTapIndex)
    {
        taps[activeTapIndex].alpha = 0.0f;
        taps[activeTapIndex].blocksRaycasts = false;

        taps[_activeTapIndex].alpha = 1.0f;
        taps[_activeTapIndex].blocksRaycasts = true;

        activeTapIndex = _activeTapIndex;
    }
}
