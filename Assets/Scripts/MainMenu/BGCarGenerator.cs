using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGCarGenerator : MonoBehaviour {

    [SerializeField] private GameObject[] _bgCarPrefabs;
    [SerializeField] float[] _lanes;
    private int _spawnFrame = 120;

    private void Update()
    {
        if (Time.frameCount % _spawnFrame == 0)
            SpawnCar();
    }

    public void SpawnCar()
    {
        int laneIndex = Random.Range(0, _lanes.Length);
        int carIndex = Random.Range(0, _bgCarPrefabs.Length);
        Vector3 spawnPos = new Vector3(_lanes[laneIndex], -2.5f, -7f);
        GameObject car = Instantiate(_bgCarPrefabs[carIndex], spawnPos, Quaternion.identity, this.transform);
        _spawnFrame = Random.Range(240, 480);
    }
}
