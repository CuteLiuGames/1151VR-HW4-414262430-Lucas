using UnityEngine;

public class PassTrigger : MonoBehaviour
{
    [Header("Aircraft")]
    public PlaneController aircraft;

    [Header("Obstacle Dimensions")]
    public float obstacleDepth = 2f;

    [Header("Aircraft Dimensions")]
    public float aircraftLength = 3f;

    private bool passed = false;

    public void Initialize(PlaneController controller)
    {
        aircraft = controller;
        passed = false;
    }

    void Update()
    {
        if (passed || aircraft == null)
            return;

        if (aircraft.isCrashed)
            return;

        // Aircraft rear position
        float aircraftRearZ =
            aircraft.transform.position.z - aircraftLength / 2f;

        // Obstacle far edge
        float obstacleEndZ =
            transform.position.z + obstacleDepth / 2f;

        // Entire aircraft has cleared the obstacle
        if (aircraftRearZ > obstacleEndZ)
        {
            passed = true;

            aircraft.PassObstacle();

            Debug.Log("Obstacle fully passed!");
        }
    }
}