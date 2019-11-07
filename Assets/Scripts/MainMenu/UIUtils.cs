using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public static class UIUtils  {

    #region Static Methods
    public static void ToggleUIElement(CanvasGroup cv, bool on, bool immediate = false)
    {
        if (immediate)
        {
            if (on)
            {
                cv.gameObject.SetActive(true);
                cv.alpha = 1f;
            }
            else
            {
                cv.alpha = 0f;
                cv.gameObject.SetActive(false);
            }
            cv.interactable = on;
            cv.blocksRaycasts = on;
        }
        else
        {
            if (on)
            {
                cv.gameObject.SetActive(true);
                cv.DOFade(1, 0.25f);
            }
            else
            {
                cv.DOFade(0, 0.25f).OnComplete(() => cv.gameObject.SetActive(false));
            }
            cv.interactable = on;
            cv.blocksRaycasts = on;
        }
    }

    public static void ChangeTextColor(Text t, Color c)
    {
        t.DOColor(c, 0.25f);
    }

    public static void SetTextSize(Text t, float size)
    {
        t.transform.DOScale(size, 0.25f);
    }
    #endregion
}
