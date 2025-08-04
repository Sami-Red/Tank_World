using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 10, -8);
    [SerializeField] private float smoothSpeed = 0.125f;

    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.rotation = Quaternion.Euler(45f, 0f, 0f);
    }


    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.Euler(45f, 0f, 0f);
        }
    }
}
