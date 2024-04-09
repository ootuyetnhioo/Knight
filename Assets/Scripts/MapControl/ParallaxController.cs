using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [SerializeField] private Transform _camera;
    [SerializeField] private float _relativeMoveRateX;
    [SerializeField] private float _relativeMoveRateY;
    [SerializeField] private bool _lockYMove;

    void LateUpdate()
    {
        if (SoundsManager.Instance.IsPlayingBossTheme) 
            return;

        if (_lockYMove)
            transform.position = new Vector2(_camera.position.x * _relativeMoveRateX, transform.position.y);
        else
            transform.position = new Vector2(_camera.position.x * _relativeMoveRateX, _camera.position.y * _relativeMoveRateY);
    }
}
