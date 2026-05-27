using UnityEngine;

public class WallImpact : MonoBehaviour
{
    [Header("Configurações da Parede")]
    [Tooltip("Quanto % da velocidade atual o jogador perde")]
    public float velocidadeLossPercent = 45f;

    [Tooltip("Força do impacto")]
    public float impactForce = 12f;

    [Header("Efeitos")]
    public AudioSource hitSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMove playerMove = collision.gameObject.GetComponent<PlayerMove>();
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerMove != null && rb != null)
            {
                AplicarPerdaDeVelocidade(rb);
                AplicarImpactoFisico(rb, collision);
                
                if (hitSound != null)
                    hitSound.Play();
            }
        }
    }

    private void AplicarPerdaDeVelocidade(Rigidbody rb)
    {
        float currentSpeed = rb.linearVelocity.magnitude;
        float newSpeed = currentSpeed * (1 - velocidadeLossPercent / 100f);
        
        // Mantém a direção, mas reduz a velocidade
        if (currentSpeed > 0.1f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * newSpeed;
        }
    }

    private void AplicarImpactoFisico(Rigidbody rb, Collision collision)
    {
        Vector3 impactDirection = collision.contacts[0].normal;
        rb.AddForce(impactDirection * impactForce, ForceMode.Impulse);
    }
}