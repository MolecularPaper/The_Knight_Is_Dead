using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillEffect : MonoBehaviour
{
    public delegate void SkillDamageDel();
    public SkillDamageDel skillDamageDel;

    public void Damage() => skillDamageDel.Invoke();

    public void Destroy() => Destroy(this.gameObject);
}
