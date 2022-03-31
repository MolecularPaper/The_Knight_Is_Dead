using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Increase Data", menuName = "Scriptable Object/Increase Data", order = int.MaxValue)]
public class IncreaseData : ScriptableObject
{
    [Space(10)]
    public float hpIncreseWidth;
    public float hpSoulIncreseWidth;

    [Space(10)]
    public float atkIncreseWidth;
    public float atkSoulIncreseWidth;

    [Space(10)]
    public float defIncreseWidth;
    public float defSoulIncreseWidth;
    public float defConst;

    [Space(10)]
    public float lukIncreseWidth;
    public float lukSoulIncreseWidth;

    [Space(10)]
    public float cridIncreseWidth;
    public float cridSoulIncreseWidth;

    [Space(10)]
    public float cripIncreseWidth;
    public float cripSoulIncreseWidth;

    [Space(10)]
    public float enemyHPIncrese;
    public float enemyAtkIncrese;
    public float enemySoulIncrese;
}
