using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConnectionUI : MonoBehaviour {
    private Tilemap _tilemap;
    public bool visible;
    public float fadeSpeed = 4f;
    
    private float _alpha;
    // Start is called before the first frame update
    void Start() {
        _tilemap = GetComponent<Tilemap>();
        
        var tilemapColor = _tilemap.color;
        tilemapColor.a = _alpha;
        _tilemap.color = tilemapColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (visible) {
            if (_alpha < 1) {
                _alpha += fadeSpeed * Time.deltaTime;
                if (_alpha > 1)
                    _alpha = 1;
                
                var tilemapColor = _tilemap.color;
                tilemapColor.a = _alpha;
                _tilemap.color = tilemapColor;
            }
        } else {
            if (_alpha > 0) {
                _alpha -= fadeSpeed * Time.deltaTime;
                if (_alpha < 0) {
                    _alpha = 0;
                }
                var tilemapColor = _tilemap.color;
                tilemapColor.a = _alpha;
                _tilemap.color = tilemapColor;
            }
            
        }
    }

    private void OnMouseEnter() {
        visible = true;
    }

    private void OnMouseExit() {
        visible = false;
    }
}
