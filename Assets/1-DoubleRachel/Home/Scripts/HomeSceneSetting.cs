using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HomeInitSetting", menuName = "TronTron/HomeInitSetting", order = 1)]
[Serializable]
public class HomeSceneSetting : ScriptableObject
{
    public int InitHomeIndex = 0;
}
