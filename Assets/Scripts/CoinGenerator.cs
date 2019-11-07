using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoinGroupLayout { StraightLine = 0, DiagonalLine, RandomSpread}
public class CoinGenerator : MonoBehaviour {

    static public CoinGenerator S;
    [SerializeField] private GameObject _coinPrefab;

    private void Awake()
    {
        if (S != null)
            Destroy(this);
        S = this;
    }

    private void Update()
    {
        if (Time.frameCount % Random.Range(240, 360) == 0)
            GenerateCoins(CoinGroupLayout.StraightLine, 6, 20);
    }

    public void GenerateCoins(CoinGroupLayout layout, int amount, int scoreValue)
    {
        float zPosition = Player.S.transform.position.z + 140;
        float xPosition = Random.Range(-6.5f, 6.5f);
        int coinSpace = Random.Range(3, 6);
        for (int i = 0; i < amount; i++)
        {
            switch (layout)
            {
                case CoinGroupLayout.StraightLine:
                    GameObject coin1 = Instantiate(_coinPrefab, new Vector3(xPosition, 1.5f, zPosition + i * coinSpace), Quaternion.identity);
                    coin1.GetComponent<Coin>().scoreValue = scoreValue;
                    break;
                case CoinGroupLayout.DiagonalLine:
                    GameObject coin2 = Instantiate(_coinPrefab, new Vector3(xPosition, 1.5f, zPosition + i * coinSpace), Quaternion.identity);
                    coin2.GetComponent<Coin>().scoreValue = scoreValue;
                    break;
                case CoinGroupLayout.RandomSpread:
                    break;
                default:
                    break;
            }
        }
    }

    

}
