using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PositionPath : MonoBehaviour {
    public float distanceBetweenPoints = 0.5f;
    public SpriteRenderer pointPrefab;
    private List<SpriteRenderer> _points = new List<SpriteRenderer>();
    
    public bool visible;
    public float fadeSpeed = 4f;
    
    private float _alpha;
    public float Alpha {
        get => _alpha;
        set => _alpha = value;
    }

    // Update is called once per frame
    void Update()
    {
        if (visible) {
            if (_alpha < 1) {
                _alpha += fadeSpeed * Time.deltaTime;
                if (_alpha > 1)
                    _alpha = 1;
                SetPointsAlpha(_alpha);
            }
        } else {
            if (_alpha > 0) {
                _alpha -= fadeSpeed * Time.deltaTime;
                if (_alpha < 0) {
                    _alpha = 0;
                }
                SetPointsAlpha(_alpha);
            }
        }
    }

    public void SetPointsAlpha(float alpha) {
        foreach (var point in _points) {
            var pointColor = point.color;
            pointColor.a = alpha;
            point.color = pointColor;
        }
    }

    public void MakePath(Vector2[] positions) {
        // Delete old points
        foreach (var point in _points) {
            Destroy(point.gameObject);
        }
        _points.Clear();


        Vector2 lastPos = transform.position;
        // Create new points
        for (int i = 0; i < positions.Length; i++) {
            Vector2 pos = positions [i];
            if(pos.sqrMagnitude == 0)
                continue;
            float dist = (pos - lastPos).magnitude;
            if (dist >= distanceBetweenPoints) {
                var spriteRenderer = Instantiate(pointPrefab, pos, Quaternion.identity, transform);
                _points.Add(spriteRenderer);
                lastPos = pos;
            } else {
                // skip this point
                continue;
            }
        }
        SetPointsAlpha(0);
    }
}
