using UnityEngine;

public class PlayerBoost : MonoBehaviour
{
    [Header("Configurações do Boost")]
    public float boostMultiplier = 1.85f;
    public float maxBoostTime = 5f;
    public float boostDuration = 0f;

    [Header("Efeitos")]
    public ParticleSystem boostParticles;
    public AudioSource boostSound;

    private PlayerMove playerMove;
    private Rigidbody rb;

    private float originalMaxSpeed;
    private bool isBoosting = false;

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        rb = GetComponent<Rigidbody>();

        if (playerMove != null)
        {
            originalMaxSpeed = playerMove.maxSpeed;
        }

        if (boostParticles != null)
        {
            boostParticles.Stop();
        }
    }

    void Update()
    {
        // Conta o tempo do boost
        if (isBoosting)
        {
            boostDuration -= Time.deltaTime;

            if (boostDuration <= 0)
            {
                StopBoost();
            }
        }
    }

    // Chamado quando pega item
    public void AddBoost(float amount)
    {
        boostDuration = amount;

        StartBoost();
    }

    void StartBoost()
    {
        if (playerMove == null) return;

        // Se já estiver boostando, reseta antes
        if (isBoosting)
        {
            StopBoost();
        }

        isBoosting = true;

        // Aumenta velocidade
        playerMove.maxSpeed = originalMaxSpeed * boostMultiplier;

        // Dá impulso imediato
        if (rb != null)
        {
            rb.linearVelocity *= boostMultiplier;
        }

        // Efeitos
        if (boostParticles != null)
        {
            boostParticles.Play();
        }

        if (boostSound != null)
        {
            boostSound.Play();
        }
    }

    void StopBoost()
    {
        isBoosting = false;

        // Volta velocidade normal
        if (playerMove != null)
        {
            playerMove.maxSpeed = originalMaxSpeed;
        }

        // Para efeitos
        if (boostParticles != null)
        {
            boostParticles.Stop();
        }

        if (boostSound != null)
        {
            boostSound.Stop();
        }
    }
}