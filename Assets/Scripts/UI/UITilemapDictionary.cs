using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UITilemapDictionary : MonoBehaviour
{

    public List<TileBase> digitsMap;

    public TileBase Get(int index)
    {
        return digitsMap[index];
    }

}
