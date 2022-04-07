using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapUI : MonoBehaviour
{
    [SerializeField] CanvasGroup[] taps;
    private int activeTapIndex = 0;

    public void TapControl(int _activeTapIndex)
    {
        taps[activeTapIndex].gameObject.SetActive(false);
        taps[_activeTapIndex].gameObject.SetActive(true);
        activeTapIndex = _activeTapIndex;
    }
}
