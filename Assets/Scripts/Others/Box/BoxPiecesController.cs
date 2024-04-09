using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPiecesController : MonoBehaviour
{
    [SerializeField] private Vector2 _bounceForce;
    [SerializeField] private float _existTime;

    private Rigidbody2D _rb;
    private float _entryTime;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.AddForce(_bounceForce);
        _entryTime = Time.time;
    }

    void Update()
    {
        if (Time.time - _entryTime >= _existTime)
            Destroy(gameObject);
    }
}
