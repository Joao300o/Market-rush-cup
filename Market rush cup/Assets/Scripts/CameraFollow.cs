using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public PlayerMove playerMove;

    [Header("Movimento")]
    public float distanciaMax = 10f;
    public float distanciaMin = 5f;

    private Vector3 posicaoInicial;


    void Start()
    {
        posicaoInicial = transform.localPosition;

    }

    void LateUpdate()
    {
        float porcentageVelocidade =
            playerMove.velocidadeAtual / playerMove.velocidadeMaxima;

        float distanciaAtual = Mathf.Lerp(
            distanciaMin,
            distanciaMax,
            porcentageVelocidade
        );

        transform.localPosition = new Vector3(
            posicaoInicial.x,
            posicaoInicial.y,
            - distanciaAtual
        );
    }

    void Update()
    {

    }
}
