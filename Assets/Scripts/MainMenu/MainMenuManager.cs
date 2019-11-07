using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour {

    #region Public Fields
    static public MainMenuManager S;
    #endregion

    #region Private Fields
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _scoresButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private CanvasGroup _backButtonImage;
    [SerializeField] private CanvasGroup _mainpanel;
    [SerializeField] private CanvasGroup _scoreBoardPanel;
    [SerializeField] private int _gameSceneIndex = 1;
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        if (S != null)
            Destroy(this);
        S = this;
        Init();
    }
    #endregion

    #region Public Methods
    public void Init()
    {
        //Assiging main buttons **Hard coded**
        _playButton = transform.GetChild(2).GetChild(0).GetComponent<Button>();
        _scoresButton = transform.GetChild(2).GetChild(1).GetComponent<Button>();
        _optionsButton = transform.GetChild(2).GetChild(2).GetComponent<Button>();
        _shopButton = transform.GetChild(2).GetChild(3).GetComponent<Button>();
        _backButton = transform.GetChild(0).GetComponent<Button>();
        _backButtonImage = _backButton.GetComponent<CanvasGroup>();

        //Assigning panels
        _mainpanel = transform.GetChild(2).GetComponent<CanvasGroup>();
        _scoreBoardPanel = transform.GetChild(3).GetComponent<CanvasGroup>();

        //Closing irrelevant panels
        UIUtils.ToggleUIElement(_scoreBoardPanel, false, true);
        UIUtils.ToggleUIElement(_backButtonImage, false, true);

        //Adding main buttons functionality
        _playButton.onClick.RemoveAllListeners();
        _playButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(_gameSceneIndex);
        });

        _scoresButton.onClick.RemoveAllListeners();
        _scoresButton.onClick.AddListener(() =>
        {
            ToggleMainPanel(false);
            ToggleScoreBoard(true);
            ToggleBackButton(true);
            AssignBackButton(new CanvasGroup[] { _mainpanel }, _scoreBoardPanel, _backButtonImage);

        });
    }
    #endregion

    #region Private Methods
    private void ToggleMainPanel(bool on)
    {
        if (on)
            UIUtils.ToggleUIElement(_mainpanel, true);
        else
            UIUtils.ToggleUIElement(_mainpanel, false);
    }

    private void ToggleScoreBoard(bool on)
    {
        if (on)
            UIUtils.ToggleUIElement(_scoreBoardPanel, true);
        else
            UIUtils.ToggleUIElement(_scoreBoardPanel, false);
        //ScoreBoardHandler takes care of displaying the information
    }

    private void ToggleBackButton(bool on)
    {
        if (on)
            UIUtils.ToggleUIElement(_backButtonImage, true);
        else
            UIUtils.ToggleUIElement(_backButtonImage, false);
    }

    private void AssignBackButton(CanvasGroup[] itemsToDisplay, params CanvasGroup[] itemsToHide)
    {
        _backButton.onClick.RemoveAllListeners();
        _backButton.onClick.AddListener(() =>
        {
            foreach (var item in itemsToHide)
            {
                UIUtils.ToggleUIElement(item, false);
            }
            foreach (var item in itemsToDisplay)
            {
                UIUtils.ToggleUIElement(item, true);
            }
        });
    }
    #endregion





}
