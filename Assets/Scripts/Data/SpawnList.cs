using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawn List", menuName = "Scriptable Object/Spawn List", order = 0)]
public class SpawnList : ScriptableObject
{
    public float hpIncrease;
    public float atkIncrease;
    public float soulIncrease;
    public float expIncrease;
    public List<GameObject> enemyPrefabs;
}
