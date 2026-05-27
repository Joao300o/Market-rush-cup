using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Referência do Player")]
    public PlayerMove playerMove;

    [Header("Configurações da Câmera")]
    public float distanciaMin = 5f;
    public float distanciaMax = 10f;
    public float altura = 4f;           // Opcional: altura da câmera

    private Vector3 posicaoInicial;

    void Start()
    {
        if (playerMove == null)
        {
            Debug.LogError("PlayerMove não foi atribuído na câmera!");
            return;
        }

        posicaoInicial = transform.localPosition;
    }

    void LateUpdate()
    {
        if (playerMove == null) return;

        // Pega a velocidade atual usando o método que criamos
        float velocidadeAtual = playerMove.CurrentSpeed();
        float velocidadeMaxima = playerMove.maxSpeed;

        // Calcula a porcentagem da velocidade (0 a 1)
        float porcentagemVelocidade = Mathf.Clamp01(velocidadeAtual / velocidadeMaxima);

        // Interpolação da distância da câmera
        float distanciaAtual = Mathf.Lerp(distanciaMin, distanciaMax, porcentagemVelocidade);

        // Atualiza posição da câmera
        transform.localPosition = new Vector3(
            posicaoInicial.x,
            altura,                    // Usa altura configurável
            -distanciaAtual
        );
    }
}