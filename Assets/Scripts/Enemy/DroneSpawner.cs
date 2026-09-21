using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] AnimationCurve timeBetweenDrones;
    [SerializeField] RockstarDrone drone;
    float droneSpawnTime = 10f;

    float progressThrough => 0f;
    private void Update()
    {
        transform.position = player.position;
        droneSpawnTime -= Time.deltaTime;
        if (droneSpawnTime <= 0f)
        {
            droneSpawnTime = timeBetweenDrones.Evaluate(progressThrough);
            drone.StartStrafe();
        }
    }
}
