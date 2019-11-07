using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    public int scoreValue = 20;

    private void OnBecameInvisible()
    {
        Destroy(gameObject, 1f);
    }
}
