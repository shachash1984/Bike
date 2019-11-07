using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScoreboardHandler : MonoBehaviour {

    #region Private Fields
    [SerializeField] private Button _localButton;
    [SerializeField] private Button _friendsButton;
    [SerializeField] private Button _globalButton;
    [SerializeField] private Text _selectedText;
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        Init();
    }
    #endregion

    #region Public Methods
    public void Init()
    {
        //Assiging scoreboard buttons **Hard coded**
        _localButton = transform.GetChild(0).GetComponent<Button>();
        _friendsButton = transform.GetChild(1).GetComponent<Button>();
        _globalButton = transform.GetChild(2).GetComponent<Button>();
        ClearPanel();
        DisplayLocalScores();

        //Add functionality to buttons
        _localButton.onClick.RemoveAllListeners();
        _localButton.onClick.AddListener(() =>
        {
            DisplayLocalScores();
        });
        _friendsButton.onClick.RemoveAllListeners();
        _friendsButton.onClick.AddListener(() =>
        {
            DisplayFriendsScores();
        });
        _globalButton.onClick.RemoveAllListeners();
        _globalButton.onClick.AddListener(() =>
        {
            DisplayGlobalScores();
        });
    }
    #endregion

    #region Private Methods
    private void SetSelectedText(Text t)
    {
        if (_selectedText)
            ToggleHighlightSelectedText(false);

        _selectedText = t;
        ToggleHighlightSelectedText(true);
    }

    private void ToggleHighlightSelectedText(bool on)
    {
        if (on)
        {
            UIUtils.SetTextSize(_selectedText, 1.25f);
            UIUtils.ChangeTextColor(_selectedText, Color.yellow);
        }
        else
        {
            UIUtils.SetTextSize(_selectedText, 1f);
            UIUtils.ChangeTextColor(_selectedText, Color.white);
        }
        
    }

    private void DisplayLocalScores()
    {
        Text t = _localButton.transform.GetChild(0).GetComponent<Text>();
        SetSelectedText(t);
        // load scores from player prefs / DB
    }

    private void DisplayFriendsScores()
    {
        Text t = _friendsButton.transform.GetChild(0).GetComponent<Text>();
        SetSelectedText(t);
        // load scores from  DB
    }

    private void DisplayGlobalScores()
    {
        Text t = _globalButton.transform.GetChild(0).GetComponent<Text>();
        SetSelectedText(t);
        // load scores from  DB
    }

    private void ClearPanel()
    {
        //Clear current panel results
    }
    #endregion
}
