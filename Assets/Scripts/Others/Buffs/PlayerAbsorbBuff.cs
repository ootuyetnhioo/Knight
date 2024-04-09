using UnityEngine;

public class PlayerAbsorbBuff : PlayerBuffs
{
    [SerializeField] private float _tempHPDuration;
    [SerializeField] private float _tempHPRunOutDuration;
    [SerializeField] private Transform _tempShieldIcon; 
    [SerializeField] private Transform _tempShieldIconPos;

    public float TempHPDuration { get { return _tempHPDuration; } }

    public float TempHPRunOutDuration { get { return _tempHPRunOutDuration; } }

    public override void Awake(){}

    public override void Start()
    {
        _tempShieldIcon.gameObject.SetActive(false);
    }

    public override void Update()
    {
        if (_isAllowToUpdate)
        {
            if (Time.time - _entryTime >= _buffDuration)
            {
                _isAllowToUpdate = false;
                _tempShieldIcon.gameObject.SetActive(false);
                SoundsManager.Instance.PlaySfx(GameEnums.ESoundName.SpecialBuffDebuffSfx, 1.0f);
            }
            else
                _tempShieldIcon.transform.position = _tempShieldIconPos.position;
        }
    }

    public override void ApplyBuff()
    {
        base.ApplyBuff();
        _tempShieldIcon.gameObject.SetActive(true);
        SoundsManager.Instance.PlaySfx(GameEnums.ESoundName.AbsorbBuffSfx, 1.0f);
    }
}
