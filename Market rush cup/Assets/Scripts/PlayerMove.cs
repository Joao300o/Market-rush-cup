using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    public float acceleration = 20f;
    public float maxSpeed = 25f;
    public float turnSpeed = 80f;
    public float drag = 1f;

    private Rigidbody rb;

    private float moveInput;
    private float turnInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearDamping = drag;
        rb.angularDamping = 5f;
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        Move();
        Turn();
    }

    void Move()
    {
        // Só acelera se não passou da velocidade máxima
        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(transform.forward * moveInput * acceleration);
        }
    }

    void Turn()
    {
        // Quanto mais rápido, mais consegue virar
        float speedFactor = rb.linearVelocity.magnitude / maxSpeed;

        float turn = turnInput * turnSpeed * speedFactor * Time.fixedDeltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        rb.MoveRotation(rb.rotation * turnRotation);
    }

    public float CurrentSpeed()
    {
        return rb.linearVelocity.magnitude;
    }
}