using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseSingleton<GameManager>
{
    [SerializeField] float _delayTrans;
    [SerializeField] float _delayPlayThemeMusic;
    bool _isReplay;
    bool _fullUnlock;
    bool _deleteScene1Data;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void SwitchNextScene()
    {
        UIManager.Instance.IncreaseTransitionCanvasOrder();
        StartCoroutine(SwitchScene(SceneManager.GetActiveScene().buildIndex + 1));
        SoundsManager.Instance.PlaySfx(GameEnums.ESoundName.ButtonSelectedSfx, 1.0f);
        StartCoroutine(PlayNextSceneSong(SceneManager.GetActiveScene().buildIndex + 1, true));
        ResetGameData();
    }

    private IEnumerator PlayNextSceneSong(int index, bool needWait)
    {
        if (needWait)
            yield return new WaitForSeconds(_delayPlayThemeMusic);

        switch (index)
        {
            case 0:
                SoundsManager.Instance.PlayMusic(GameEnums.ESoundName.StartMenuTheme);
                break;

            case 1:
                SoundsManager.Instance.PlayMusic(GameEnums.ESoundName.Level1Theme);
                break;

            case 2:
                SoundsManager.Instance.PlayMusic(GameEnums.ESoundName.Level2Theme);
                Debug.Log("here");
                break;
        }
    }

    private void ResetGameData()
    {
        PlayerHealthManager.Instance.DecreaseHP();
        PlayerHealthManager.Instance.RestartHP();
        PlayerPrefs.DeleteAll();
        _deleteScene1Data = false;
        Debug.Log("reset GData");
    }

    public void SwitchToScene(int sceneIndex)
    {
        StartCoroutine(SwitchScene(sceneIndex));
    }

    public IEnumerator SwitchScene(int sceneIndex)
    {
        UIManager.Instance.IncreaseTransitionCanvasOrder();
        UIManager.Instance.PopDownAllPanels();
        UIManager.Instance.TriggerAnimation(GameConstants.SCENE_TRANS_END);

        yield return new WaitForSeconds(_delayTrans);

        if (SceneManager.GetActiveScene().buildIndex == 1 && sceneIndex == 2)
            _deleteScene1Data = true;

        SceneManager.LoadSceneAsync(sceneIndex);
        UIManager.Instance.TriggerAnimation(GameConstants.SCENE_TRANS_START);
        SoundsManager.Instance.PlaySfx(GameEnums.ESoundName.SceneEntrySfx, 1.0f);
        if (_isReplay)
        {
            _isReplay = false;
            PlayerHealthManager.Instance.RestartHP();
            Debug.Log("Replay");
            if (SoundsManager.Instance.IsPlayingBossTheme)
                SoundsManager.Instance.PlayMusic(GameEnums.ESoundName.Level2Theme);
        }
        else if (sceneIndex == GameConstants.GAME_LEVEL_2)
        {
            if (PlayerPrefs.HasKey(GameEnums.ESpecialStates.PlayerSkillUnlockedLV2.ToString()))
                _fullUnlock = true;
            else
                _fullUnlock = false;
            if (_deleteScene1Data)
            {
                _deleteScene1Data = false;
                PlayerPrefs.DeleteAll();
                StartCoroutine(PlayNextSceneSong(sceneIndex, false));
                Debug.Log("PlaySong: " + sceneIndex);
            }
            PlayerPrefs.SetString((_fullUnlock) ? GameEnums.ESpecialStates.PlayerSkillUnlockedLV2.ToString() : GameEnums.ESpecialStates.PlayerSkillUnlockedLV1.ToString(), "Unlocked");
            PlayerPrefs.Save();

            PlayerHealthManager.Instance.IncreaseHP();
        }
    }

    public void ReloadScene()
    {
        Time.timeScale = 1.0f;
        UIManager.Instance.PopDownAllPanels();
        _isReplay = true;
        SwitchToScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackHome()
    {
        UIManager.Instance.PopDownAllPanels();
        UIManager.Instance.StartMenuCanvas.SetActive(true);
        SceneManager.LoadSceneAsync(GameConstants.GAME_MENU);
        StartCoroutine(PlayNextSceneSong(GameConstants.GAME_MENU, false));
    }

    public void Restart()
    {
        UIManager.Instance.PopDownAllPanels();
        EventsManager.Instance.NotifyObservers(GameEnums.EEvents.ObjectOnRestart, null);
        ResetGameData();
        StartCoroutine(PlayNextSceneSong(GameConstants.GAME_LEVEL_1, true));
        SwitchToScene(GameConstants.GAME_LEVEL_1);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
        PlayerHealthManager.Instance.DecreaseHP();
        Debug.Log("Quit");
    }
}
