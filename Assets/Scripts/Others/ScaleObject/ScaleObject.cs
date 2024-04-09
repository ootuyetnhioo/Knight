using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleObject : MonoBehaviour
{
    [SerializeField] float _timeEachScale;
    [SerializeField] GameObject _gameObject;
    [SerializeField] Vector3 _scaleOffset;
    [SerializeField] Vector3 _offsetEachScale;
    bool _isIncreasing = true;
    float _entryTime;

    void Start()
    {
        _entryTime = Time.time;
    }

    void Update()
    {
        if (Time.time - _entryTime >= _timeEachScale)
        {
            Vector3 scale = _gameObject.transform.localScale;

            scale += (_isIncreasing) ? _offsetEachScale : new Vector2(-1f, -1f) * _offsetEachScale;

            if (scale.magnitude >= _scaleOffset.magnitude && _isIncreasing)
                _isIncreasing = false;
            else if (scale.magnitude <= Vector3.one.magnitude && !_isIncreasing)
                _isIncreasing = true;

            _gameObject.transform.localScale = scale;
            _entryTime = Time.time;
        }
    }
}
