using UnityEngine;

public class PlayerBoost : MonoBehaviour
{
    [Header("Configurações do Boost")]
    public float boostMultiplier = 1.85f;      // 85% mais rápido
    public float maxBoostTime = 5f;
    public float boostDuration = 0f;

    [Header("Efeitos")]
    public ParticleSystem boostParticles;
    public AudioSource boostSound;

    private PlayerMove playerMove;
    private float originalMaxSpeed;
    private bool isBoosting = false;

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();

        if (playerMove != null)
            originalMaxSpeed = playerMove.maxSpeed;

        if (boostParticles != null)
            boostParticles.Stop();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && boostDuration > 0.1f)
        {
            StartBoost();
        }

        if (isBoosting)
        {
            boostDuration -= Time.deltaTime;

            if (boostDuration <= 0)
            {
                StopBoost();
            }
        }
    }

    public void AddBoost(float amount)
    {
        boostDuration = Mathf.Min(boostDuration + amount, maxBoostTime);
    }

    void StartBoost()
    {
        if (isBoosting || playerMove == null) return;

        isBoosting = true;

        originalMaxSpeed = playerMove.maxSpeed;
        playerMove.maxSpeed = originalMaxSpeed * boostMultiplier;

        // Força o carro a ir mais rápido imediatamente
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity *= boostMultiplier;
        }

        if (boostParticles != null) boostParticles.Play();
        if (boostSound != null) boostSound.Play();
    }

    void StopBoost()
    {
        isBoosting = false;
        boostDuration = 0f;

        if (playerMove != null)
        {
            playerMove.maxSpeed = originalMaxSpeed;
        }

        if (boostParticles != null) boostParticles.Stop();
        if (boostSound != null) boostSound.Stop();
    }
}