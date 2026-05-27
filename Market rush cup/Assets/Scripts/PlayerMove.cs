using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidadeMaxima = 20f;
    public float aceleracaoPorSegundo = 1f; // substitui o AumentoGradual

    [Header("Câmera")]
    public Transform cameraTransform;

    private Rigidbody rb;
    public float velocidadeAtual = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Evita que o Rigidbody rotacione pelo physics (você controla a rotação)
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Aumenta a velocidade gradualmente ao longo do tempo
        if (h != 0 || v != 0)
        {
            velocidadeAtual = Mathf.MoveTowards(
                velocidadeAtual,
                velocidadeMaxima,
                aceleracaoPorSegundo * Time.fixedDeltaTime
            );
        }
        else
        {
            velocidadeAtual = 0f; // para ao soltar as teclas
        }

        // Direção baseada na câmera
        Vector3 forward = cameraTransform.forward;
        Vector3 right   = cameraTransform.right;

        forward.y = 0f;
        right.y   = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 direcao = (forward * v + right * h).normalized;
        Vector3 movimento = direcao * velocidadeAtual;

        // Mantém a velocidade vertical (gravidade do Rigidbody é automática)
        movimento.y = rb.linearVelocity.y;

        rb.linearVelocity = movimento;
    }
}