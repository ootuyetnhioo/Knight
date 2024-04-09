using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    private void ChangeTransitionCanvasOrder()
    {
        UIManager.Instance.DecreaseTransitionCanvasOrder();
        UIManager.Instance.PopUpHPCanvas();
    }

    private void PopUpHPCanvas()
    {
        UIManager.Instance.PopUpHPCanvas();
    }

    private void DisableMaincanvas()
    {
        UIManager.Instance.StartMenuCanvas.SetActive(false);
    }

    private IEnumerator DelayPlayLevelTheme()
    {
        yield return new WaitForSeconds(1f);
        SoundsManager.Instance.PlayMusic(GameEnums.ESoundName.Level1Theme);
    }
}
