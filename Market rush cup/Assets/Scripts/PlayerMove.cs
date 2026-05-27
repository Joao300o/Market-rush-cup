using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movimento")]
    public float acceleration = 25f;
    public float maxSpeed = 30f;
    public float turnSpeed = 100f;           // Virada

    [Header("Controle")]
    public float drag = 2f;
    public float turnSpeedAtLowSpeed = 1.4f; // Ajuda a virar parado

    private Rigidbody rb;

    private float moveInput;
    private float turnInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearDamping = drag;
        rb.angularDamping = 3f;
        rb.centerOfMass = new Vector3(0, -0.6f, 0);   // Importante para estabilidade
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
        if (rb.linearVelocity.magnitude < maxSpeed && moveInput > 0.1f)
        {
            rb.AddForce(transform.forward * moveInput * acceleration, ForceMode.Acceleration);
        }
    }

    void Turn()
    {
        if (Mathf.Abs(turnInput) < 0.05f) return;

        float currentSpeed = rb.linearVelocity.magnitude;
        float speedFactor = Mathf.Lerp(turnSpeedAtLowSpeed, 0.7f, currentSpeed / maxSpeed);

        // === CORREÇÃO PRINCIPAL ===
        float turnAmount = turnInput * turnSpeed * speedFactor * Time.fixedDeltaTime;

        // Rotação correta no eixo Y (virada)
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + turnAmount, 0);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 8f * Time.fixedDeltaTime));
    }

    public float CurrentSpeed()
    {
        return rb.linearVelocity.magnitude;
    }
}