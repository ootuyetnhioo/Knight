using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

[System.Serializable]
public struct PoolableObject
{
    public EPoolable _ePoolable;
    public GameObject _GObjPoolable;
    public int _ammount;
}

[System.Serializable]
public struct BulletPiecePair
{
    public EPoolable _bulletType;
    public GameObject _pair1;
    public GameObject _pair2;
    public int _ammount;

    public BulletPiecePair(EPoolable bulletType, GameObject pair1, GameObject pair2, int ammount)
    {
        _bulletType = bulletType;
        _pair1 = pair1;
        _pair2 = pair2;
        _ammount = ammount;
    }

    public GameObject Pair1 { get { return _pair1; } }

    public GameObject Pair2 { get { return _pair2; } }
}

public class Pool : BaseSingleton<Pool>
{
    [SerializeField] List<PoolableObject> _listPoolableObj = new();
    [SerializeField] List<BulletPiecePair> _listBPiecePair = new();
    private Dictionary<EPoolable, List<GameObject>> _dictPool = new();
    private Dictionary<EPoolable, List<BulletPiecePair>> _dictBPPPool = new();

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        FillInFirstDictionary();
        FillInSecondDictionary();

        InstantiateGameObjects();
    }

    private void FillInFirstDictionary()
    {
        for (int i = 0; i < _listPoolableObj.Count; i++)
            if (!_dictPool.ContainsKey(_listPoolableObj[i]._ePoolable))
                _dictPool.Add(_listPoolableObj[i]._ePoolable, new());
    }

    private void FillInSecondDictionary()
    {
        for (int i = 0; i < _listBPiecePair.Count; i++)
            if (!_dictBPPPool.ContainsKey(_listBPiecePair[i]._bulletType))
                _dictBPPPool.Add(_listBPiecePair[i]._bulletType, new());
    }

    private void InstantiateGameObjects()
    {
        for (int i = 0; i < _listPoolableObj.Count; i++)
        {
            for(int j = 0; j < _listPoolableObj[i]._ammount; j++)
            {
                GameObject gObj = Instantiate(_listPoolableObj[i]._GObjPoolable);
                gObj.SetActive(false);
                _dictPool[_listPoolableObj[i]._ePoolable].Add(gObj);
            }
        }

        for (int i = 0; i < _listBPiecePair.Count; i++)
        {
            for (int j = 0; j < _listBPiecePair[i]._ammount; j++)
            {
                GameObject gObj1 = Instantiate(_listBPiecePair[i]._pair1);
                gObj1.SetActive(false);
                GameObject gObj2 = Instantiate(_listBPiecePair[i]._pair2);
                gObj2.SetActive(false);
                BulletPiecePair bPP = new(_listBPiecePair[i]._bulletType, gObj1, gObj2, _listBPiecePair[i]._ammount);
                _dictBPPPool[_listBPiecePair[i]._bulletType].Add(bPP);
            }
        }
    }

    public GameObject GetObjectInPool(EPoolable objType)
    {
        for (int i = 0; i < _dictPool[objType].Count; i++)
        {
            if (!_dictPool[objType][i].activeInHierarchy)
            {
                return _dictPool[objType][i];
            }
        }

        Debug.Log("out of " + objType);
        return null;
    }

    public BulletPiecePair GetPiecePairInPool(EPoolable bulletType)
    {
        BulletPiecePair bulletPiecePair = new();

        for (int i = 0; i < _dictBPPPool[bulletType].Count; i++)
        {
            if (_dictBPPPool[bulletType][i].Pair1 && _dictBPPPool[bulletType][i].Pair2)
            {
                if (!_dictBPPPool[bulletType][i].Pair1.activeInHierarchy && !_dictBPPPool[bulletType][i].Pair2.activeInHierarchy)
                {
                    return _dictBPPPool[bulletType][i];
                }
            }
        }

        Debug.Log("out of piece " + bulletType);
        return bulletPiecePair;
    }
}