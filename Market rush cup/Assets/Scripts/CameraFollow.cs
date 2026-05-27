using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Rigidbody targetRb;

    public Vector3 closeOffset = new Vector3(0, 3, -5);
    public Vector3 farOffset = new Vector3(0, 4, -9);

    public float smoothSpeed = 5f;
    public float speedForMaxDistance = 25f;

    void LateUpdate()
    {
        float speed = targetRb.linearVelocity.magnitude;

        // Quanto mais rápido, mais longe
        float t = Mathf.InverseLerp(0, speedForMaxDistance, speed);

        Vector3 currentOffset = Vector3.Lerp(closeOffset, farOffset, t);

        Vector3 desiredPosition = target.position + target.TransformDirection(currentOffset);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}