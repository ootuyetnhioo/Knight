using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBuffs : MonoBehaviour
{
    [SerializeField] protected float _buffDuration;
    protected float _entryTime;
    protected bool _isAllowToUpdate;

    public bool IsAllowToUpdate { get => _isAllowToUpdate; set => _isAllowToUpdate = value; }

    public virtual void Awake() { }

    public virtual void Start() { }

    public virtual void Update() { }

    public virtual void ApplyBuff()
    {
        _entryTime = Time.time;
        _isAllowToUpdate = true;
    }
}
