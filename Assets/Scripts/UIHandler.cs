using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {

    static public UIHandler S;
    private Text _scoreText;


    private void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
    }
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _scoreText = transform.GetChild(0).GetComponent<Text>();
        SetScoreText(0);
    }

    public void SetScoreText(int newScore)
    {
        _scoreText.text = newScore.ToString();
    }

    
}
