using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Increase Data", menuName = "Scriptable Object/Increase Data", order = int.MaxValue)]
public class IncreaseData : ScriptableObject
{
    [Space(10)]
    public float atkIncreseWidth;
    public float atkSoulIncreseWidth;

    [Space(10)]
    public float defIncreseWidth;
    public float defSoulIncreseWidth;

    [Space(10)]
    public float lukIncreseWidth;
    public float lukSoulIncreseWidth;

    [Space(10)]
    public float cridIncreseWidth;
    public float cridSoulIncreseWidth;

    [Space(10)]
    public float cripIncreseWidth;
    public float cripSoulIncreseWidth;
}
