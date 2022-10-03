using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class UITilemapSaved : MonoBehaviour
{

    private GameState Game;
    public TilemapRenderer myRenderer;
    public Tilemap myMap;
    public Vector2Int tilePositionCurrent;
    public Vector2Int tilePositionTarget;

    public UITilemapDictionary dictionary;
    
    public int savedCurrent = 0;
    public int savedTarget = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        Game = FindObjectOfType<GameState>();
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
        myRenderer.enabled = !Game.limitedUI;
    }

    private void UpdateText()
    {
        Vector3Int c = new Vector3Int(tilePositionCurrent.x,tilePositionCurrent.y,0);
        Vector3Int t = new Vector3Int(tilePositionTarget.x,tilePositionTarget.y,0);
        
        myMap.SetTile(c,dictionary.Get(savedCurrent));
        myMap.SetTile(t,dictionary.Get(savedTarget));
    }
    
}
