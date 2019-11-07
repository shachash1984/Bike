using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHandler : MonoBehaviour {

    [SerializeField] private GameObject[] _carPrefabs;
    private const float Z_SPAWN_DISTANCE = 140f;

    private void Update()
    {
        if (Time.frameCount % 300 == 0)
            SpawnCar();
    }

    public void SpawnCar()
    {
        int rand = Random.Range(0, _carPrefabs.Length);
        GameObject newCar = Instantiate(_carPrefabs[rand], GetRandomPosition(), Quaternion.identity);
        Car c = newCar.GetComponent<Car>();
        c.currentLane = (short)(newCar.transform.position.x / Car.LANE_POSITION);
        c.switchLaneTimer = Random.Range(300, 480);
    }

    public Vector3 GetRandomPosition()
    {
        int randX = Random.Range(-1, 2);
        return new Vector3(randX, 0.4f, PlayerMovement.S.transform.position.z + Z_SPAWN_DISTANCE);
    }
}
