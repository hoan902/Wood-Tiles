using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI.Extensions;

[RequireComponent(typeof(UILineRenderer))]
public class LineConnect : MonoBehaviour
{

    [SerializeField]
    UILineRenderer line;

    [SerializeField]
    LineRenderer line3D;

    public void DrawLine(List<CellData> data)
    {

        //line.Points = new Vector2[data.Count];
        line3D.positionCount = data.Count;

        for (int i = 0; i < data.Count; i++)
        {
            //line.Points[i] = data[i].posWord;

            line3D.SetPosition(i, data[i].posWord);
            // Debug.LogError(data[i].posWord);
        }
        gameObject.SetActive(true);
        StartCoroutine(DeActive());
    }

    IEnumerator DeActive()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }




}
