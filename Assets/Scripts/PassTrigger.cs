using UnityEngine;

public class PassTrigger : MonoBehaviour
{
    private bool passed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (passed) return;

        PlaneController aircraft =
            other.GetComponentInParent<PlaneController>();

        if (aircraft != null)
        {
            passed = true;
            aircraft.PassObstacle();
        }
    }
}