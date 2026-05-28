using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    public float acceleration = 20f;
    public float maxSpeed = 25f;
    public float turnSpeed = 80f;
    public float drag = 1f;

    public bool canMove = false;
    public bool stunned = false;

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
        if (!canMove) return;

        if (!stunned)
        {
            moveInput = Input.GetAxis("Vertical");
            turnInput = Input.GetAxis("Horizontal");
        }
        else
        {
            // perde controle durante stun
            moveInput = 0;
            turnInput = 0;
        }
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

    public IEnumerator Spin()
    {
        stunned = true;

        // força lateral pra deslizar
        rb.AddForce(transform.right * 15f, ForceMode.Impulse);

        float timer = 0f;

        while (timer < 1.5f)
        {
            transform.Rotate(Vector3.up * 900 * Time.deltaTime);

            timer += Time.deltaTime;

            stunned = false;
            yield return null;
        }

    }
}