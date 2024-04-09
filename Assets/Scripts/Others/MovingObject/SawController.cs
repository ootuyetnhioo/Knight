using UnityEngine;

public class SawController : MovingObjectController
{
    [SerializeField] float _rotateSpeed;
    [SerializeField] bool _isBossGate;

    protected override void Start()
    {
        if (_isBossGate)
        {
            gameObject.SetActive(false);
            EventsManager.Instance.SubcribeToAnEvent(GameEnums.EEvents.BossGateOnClose, ActiveGate);
        }
    }
    private void OnDestroy()
    {
        if (_isBossGate)
        {
            EventsManager.Instance.UnSubcribeToAnEvent(GameEnums.EEvents.BossGateOnClose, ActiveGate);
        }
    }

    protected override void Update()
    {
        if (!_isBossGate)
            base.Update();
        transform.Rotate(0f, 0f, 360f * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.SHIELD_TAG))
            gameObject.SetActive(false);
    }

    private void ActiveGate(object obj)
    {
        Debug.Log("activated");
        GameObject activeVfx = Pool.Instance.GetObjectInPool(GameEnums.EPoolable.RedExplode);
        activeVfx.SetActive(true);
        activeVfx.transform.position = transform.position;
        gameObject.SetActive(true);
    }
}
