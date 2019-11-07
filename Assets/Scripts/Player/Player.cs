using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    #region Public Fields
    static public Player S;
    public int score = 0;
    #endregion

    #region Private Fields
    private int _currentDistance;
    private int _previousDistance;
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        if (S != null)
            Destroy(this);
        S = this;
    }

    public void Update()
    {
        _currentDistance = (int)transform.position.z;
        if (_currentDistance > _previousDistance)
            AddScore(1);
        _previousDistance = _currentDistance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float crashVelocity = CalculateCrashVelocity(collision);
        //if car or side walls
        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 9)
        {
            if (collision.relativeVelocity.magnitude > crashVelocity)
            {
                PlayerMovement.S.enabled = false;
                
            }
            StartCoroutine(ReloadScene());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if passed car
        if (other.gameObject.layer == 13)
            AddScore(10);

        //if collected coin
        if (other.gameObject.layer == 14)
        {
            AddScore(other.GetComponent<Coin>().scoreValue);
            Destroy(other.gameObject);
        }
    }
    #endregion

    #region Private Methods
    private float CalculateCrashVelocity(Collision collision)
    {
        float crashVelocity = float.MaxValue;
        if (collision.gameObject.layer == 9)
            crashVelocity = 0.5f;
        else if (collision.gameObject.layer == 12)
            crashVelocity = 0.01f;
        return crashVelocity;
    }

    private void SetScore(int newScore)
    {
        score = newScore;
        UIHandler.S.SetScoreText(score);
    }

    private void AddScore(int scoreToAdd)
    {
        SetScore(score + scoreToAdd);
    }
    #endregion

    #region Public Methods
    public IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(2f);
        //TEMP
        SceneManager.LoadScene(0);

        //Display end game panel
    }
    #endregion

}
