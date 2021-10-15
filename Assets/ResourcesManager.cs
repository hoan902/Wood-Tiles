using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EffectDefine
{
    public FX_ENUM _fxEnum;
    public AutoDespawnParticles _fxPrefab;
}


[Serializable]
public class SpriteDefine
{
    public string _name;
    public Sprite _sprite;
}



public class ResourcesManager : SingletonMonoBehaviour<ResourcesManager>
{
    public MissionDataSO _missionData;
    public static string PathLevelDesign = "LevelDesigns";

    [Header("Effect Defines")]
    public List<EffectDefine> _ListEffects;

    [Header("Rewards Sprites")]
    public List<SpriteDefine> _listSprites;

    [Header("IAP Items")]
    public IAPManagerSO _iAPManagerSO;

    [Header("Offer Items")]
    public List<IAPOfferData> _listOfferItems;

    public AutoDespawnParticles GetEffect(FX_ENUM _fxEnum)
    {
        AutoDespawnParticles result = null;

        var finder = _ListEffects.FirstOrDefault(x => x._fxEnum == _fxEnum);
        if (finder != null)
            result = finder._fxPrefab;
        return result;
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public MissionData GetMissionByID(MISSION_ID id)
    {
        MissionData result = null;
        result = _missionData._listMissions.FirstOrDefault(x => x.ID == id);

        return result;
    }

    public static string GetLevelFileName(MISSION_ID mission, int levelID)
    {
        return $"w_{(int)mission + 1}_lvl_{levelID}";
    }

    public MoveMode[,] ParseLevelDesign(TextAsset textAsset, ref int w, ref int h, ref int t, ref bool isLoop, ref bool isMagnet)
    {
        MoveMode[,] result = null;
        var lines = textAsset.text.Split('\n');
        if (lines.Length > 3)
        {
            int.TryParse(lines[0], out w);
            int.TryParse(lines[1], out h);
            isLoop = lines[2].Trim().ToLower().Contains("true");
            isMagnet = lines[3].Trim().ToLower().Contains("true");
            //add theme HoanDN
            int.TryParse(lines[4], out t);
            //if (w > 0 && h > 0 && w * h % 2 == 0)
            if (w > 0 && h > 0)
            {
                try
                {
                    result = new MoveMode[w, h];
                    int rowIndex = 5;
                    for (int j = h - 1; j >= 0; j--)
                    {
                        var row = lines[rowIndex];
                        var cols = row.Split('\t');
                        for (int i = 0; i < w; i++)
                        {
                            int move = 0;
                            int.TryParse(cols[i], out move);
                            result[i, j] = (MoveMode)move;
                        }

                        rowIndex++;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Sth went wrong with design file {textAsset.name} ex{ex}");
                    result = null;
                }

            }
            else
            {
                Debug.LogError($"Sth went wrong with design file {textAsset.name}");
            }
        }

        return result;
    }

    public static string GetBoosterName(BOOSTER_ID id)
    {
        string result = "";
        switch (id)
        {
            case BOOSTER_ID.NONE:
                break;
            case BOOSTER_ID.SEARCH:
                result = "Search";
                break;
            case BOOSTER_ID.SHUFFLE:
                result = "Shuffle";
                break;
            case BOOSTER_ID.ROCKET:
                result = "Rocket";
                break;
            case BOOSTER_ID.THEME:
                result = "Theme";
                break;
            default:
                break;
        }
        return result;
    }

    public Sprite GetSpriteResource(string name)
    {
        var finder = _listSprites.FirstOrDefault(x => x._name.Equals(name));
        if (finder != null)
            return finder._sprite;

        return null;
    }
}
