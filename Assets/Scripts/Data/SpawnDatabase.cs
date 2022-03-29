using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Data", menuName = "Scriptable Object/Spawn Data", order = int.MaxValue)]
public class SpawnDatabase : ScriptableObject
{
    public List<SpawnData> spawnDatas;
}
