using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Prefab Storage

public enum EFFECT_DESPAWN
{
    NONE,
    FADE_SCALE
}


public class ContentMgr : SingletonMonoBehaviour<ContentMgr>
{

    GameObject zObj;

    // functions to instantiate prefabs

    public T GetItem<T>(string key) where T : Component
    {
        GameObject obj = GetItem(key);
        if (obj)
            return obj.GetComponent<T>();
        return null;
    }


    public T GetItem<T>(string key, Vector3 position) where T : Component
    {
        zObj = GetItem(key);
        zObj.transform.position = position;
        return zObj.GetComponent<T>();
    }

    public T GetItem<T>(string key, Transform parent, Vector3 position) where T : Component
    {
        zObj = GetItem(key);
        zObj.transform.SetParent(parent);
        zObj.transform.position = position;
        return zObj.GetComponent<T>();
    }

    public GameObject GetItem(string key, Vector3 position)
    {
        zObj = GetItem(key);
        zObj.transform.position = position;
        return zObj;
    }

    public GameObject GetItem(string key, Vector3 position, Quaternion rotation)
    {
        zObj = GetItem(key, position);
        zObj.transform.rotation = rotation;
        return zObj;
    }


    [System.Serializable]
    public class ContentType
    {
        public string itemType;
        public GameObject go;
    }

    public List<ContentType> contents = new List<ContentType>();

    public GameObject GetItem(string itemType)
    {
        if (contents.Find(x => x.itemType == itemType) != null)
            return Lean.LeanPool.Spawn(contents.Find(x => x.itemType == itemType).go) as GameObject;
        return null;
    }

    public void Despaw(GameObject go, float delay = 0, EFFECT_DESPAWN _fx = EFFECT_DESPAWN.NONE)
    {
        if (_fx == EFFECT_DESPAWN.NONE)
            Lean.LeanPool.Despawn(go, delay);
        else
        {
            switch (_fx)
            {
                case EFFECT_DESPAWN.FADE_SCALE:

                    break;
                default:
                    break;
            }
        }
    }
}