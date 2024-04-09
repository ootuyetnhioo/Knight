using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossParticleController : MonoBehaviour
{
    [SerializeField] Transform _bossRef;
    [SerializeField] float _rotateSpeed;
    [SerializeField] float _radius;
    [SerializeField] float _angleIndex;

    void Update()
    {
        _angleIndex += Time.deltaTime * _rotateSpeed;
        float xOffset = Mathf.Cos(_angleIndex) * _radius;
        float yOffset = Mathf.Sin(_angleIndex) * _radius;
        transform.position = new Vector3(_bossRef.position.x + xOffset, _bossRef.position.y + yOffset, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.PLAYER_TAG))
        {
            EventsManager.Instance.NotifyObservers(GameEnums.EEvents.PlayerOnTakeDamage, _bossRef.GetComponent<BossStateManager>().GetIsFacingRight());
        }
    }
}
