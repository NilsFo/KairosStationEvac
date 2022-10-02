using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UITilemapSaved : MonoBehaviour
{

    public Tilemap myMap;
    public Vector2Int tilePositionCurrent;
    public Vector2Int tilePositionTarget;

    public UITilemapDictionary dictionary;
    
    public int savedCurrent = 0;
    public int savedTarget = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        Vector3Int c = new Vector3Int(tilePositionCurrent.x,tilePositionCurrent.y,0);
        Vector3Int t = new Vector3Int(tilePositionTarget.x,tilePositionTarget.y,0);
        
        myMap.SetTile(c,dictionary.Get(savedCurrent));
        myMap.SetTile(t,dictionary.Get(savedTarget));
    }
    
}
