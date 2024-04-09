using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    protected Animator _anim;
    protected string _ID;

    [Tooltip("Với vài GObj cần Tutor thì để ý 2 phần này, còn kh thì bỏ qua. Nó sẽ " +
        "Link GObj này với Tutor để tắt Tutor khi Player đã tác động lên GObj này")]
    [SerializeField] protected bool _needTutor;
    [SerializeField] protected GameObject _tutorRef;

    [Header("Special Obj?"), Tooltip("Tick vào và chọn skill nếu đây là obj đặc biệt, " +
    "cung cấp skill cho Player")]
    [SerializeField] protected bool _isApplySkillToPlayer;
    [SerializeField] protected GameEnums.EPlayerState _skillUnlocked;
    [SerializeField] protected float _skillUnlockDelay;

    public Animator Animator { get { return _anim; } }

    public string ID { get { return _ID; } }

    protected virtual void Awake()
    {
        GetReferenceComponents();
        HandleObjectState();
    }

    protected virtual void Start()
    {
        SetUpProperties();
    }

    protected virtual void GetReferenceComponents()
    {
        _anim = GetComponent<Animator>();
    }

    protected virtual void HandleObjectState()
    {
        _ID = gameObject.name;

        if (PlayerPrefs.HasKey(GameEnums.ESpecialStates.Deleted + _ID))
            Destroy(gameObject);
    }

    protected virtual void SetUpProperties() { }

    protected IEnumerator NotifyUnlockSkill()
    {
        yield return new WaitForSeconds(_skillUnlockDelay);

        EventsManager.Instance.NotifyObservers(GameEnums.EEvents.PlayerOnUnlockSkills, _skillUnlocked);
        PlayerPrefs.SetString(GameEnums.ESpecialStates.SkillUnlocked + _skillUnlocked.ToString(), "Unlocked");
    }
}
