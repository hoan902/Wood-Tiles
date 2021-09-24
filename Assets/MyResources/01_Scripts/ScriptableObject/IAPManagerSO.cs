using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class IAPItemData
{
    public IAP_TYPE type;
    public string ProductID;
    public float DefaultPrice;
}

public enum IAP_TYPE
{
    NONE,
    CONSUMABLE,
    NON_CONSUMABLE,
    SUBSCRIPTION
}



[CreateAssetMenu(fileName = "RewardResources", menuName = "ScriptableObjects/IAPManagerSO", order = 1)]
public class IAPManagerSO : ScriptableObject
{
    [Header("List IAP Products")]
    public List<IAPItemData> _listIAPProducts;

}
