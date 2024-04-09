using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : NMEnemiesManager
{
    [Header("Bullet & Shoot Pos")]
    [SerializeField] private Transform _shootPosition;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void SpawnBullet()
    {
        if (BuffsManager.Instance.GetTypeOfBuff(GameEnums.EBuffs.Invisible).IsAllowToUpdate)
        {
            ChangeState(_nmEnemiesIdleState);
            return;
        }

        GameObject bullet = Pool.Instance.GetObjectInPool(GameEnums.EPoolable.PlantBullet);
        string bulletID = "";
        bullet.SetActive(true);
        bulletID = bullet.GetComponent<BulletController>().BulletID;

        BulletInfor info = new BulletInfor(GameEnums.EPoolable.PlantBullet, bulletID, _isFacingRight, _shootPosition.position);
        EventsManager.Instance.NotifyObservers(GameEnums.EEvents.BulletOnReceiveInfo, info);
        SoundsManager.Instance.PlaySfx(GameEnums.ESoundName.PlantShootSfx, 1.0f);
    }
}
