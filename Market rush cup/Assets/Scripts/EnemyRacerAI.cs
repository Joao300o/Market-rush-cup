using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IA de Inimigo para Jogo de Corrida Caótico
/// 
/// SETUP RÁPIDO:
/// 1. Adicione este script ao GameObject do inimigo
/// 2. Adicione um Rigidbody e WheelColliders (FL, FR, RL, RR)
/// 3. Crie waypoints e atribua-os no Inspector
/// 4. Atribua o Transform do jogador em "playerTransform"
/// 5. Configure as rodas nos campos de WheelCollider
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class EnemyRacerAI : MonoBehaviour
{
    // ─────────────────────────────────────────
    //  CONFIGURAÇÃO DAS RODAS
    // ─────────────────────────────────────────
    [Header("Wheel Colliders")]
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;

    [Header("Wheel Meshes (Visual)")]
    public Transform meshFL;
    public Transform meshFR;
    public Transform meshRL;
    public Transform meshRR;

    // ─────────────────────────────────────────
    //  NAVEGAÇÃO / WAYPOINTS
    // ─────────────────────────────────────────
    [Header("Navegação")]
    [Tooltip("Lista de waypoints que o inimigo vai seguir na pista")]
    public Transform[] waypoints;

    [Tooltip("Distância para considerar que chegou ao waypoint")]
    public float waypointReachDistance = 8f;

    private int currentWaypointIndex = 0;

    // ─────────────────────────────────────────
    //  FÍSICA DO CARRO
    // ─────────────────────────────────────────
    [Header("Física do Veículo")]
    public float maxMotorTorque   = 3000f;
    public float maxBrakeTorque   = 4000f;
    public float maxSteerAngle    = 35f;
    public float topSpeed         = 80f;   // km/h
    public float downforce        = 100f;

    // ─────────────────────────────────────────
    //  COMPORTAMENTO CAÓTICO
    // ─────────────────────────────────────────
    [Header("Comportamento Caótico")]
    [Range(0f, 1f)]
    [Tooltip("0 = comportado, 1 = completamente louco")]
    public float chaosLevel = 0.6f;

    [Tooltip("Intervalo entre mudanças de comportamento aleatório")]
    public float chaosChangeInterval = 2.5f;

    [Tooltip("Força do empurrão lateral ao colidir com o jogador")]
    public float ramForce = 8000f;

    [Tooltip("Distância para tentar colidir com o jogador")]
    public float ramDistance = 12f;

    // ─────────────────────────────────────────
    //  REFERÊNCIAS
    // ─────────────────────────────────────────
    [Header("Referências")]
    public Transform playerTransform;

    // ─────────────────────────────────────────
    //  ESTADO INTERNO
    // ─────────────────────────────────────────
    private Rigidbody rb;
    private float currentSteer;
    private float currentMotor;
    private float currentBrake;

    private float chaosTimer;
    private float chaosSteerOffset;     // zig-zag aleatório
    private bool  isRamming;            // modo agressivo
    private bool  isDrifting;           // deriva intencional
    private float driftTimer;

    private enum AIState { Racing, Ramming, Drifting, Recovering }
    private AIState state = AIState.Racing;

    // ─────────────────────────────────────────
    //  UNITY MESSAGES
    // ─────────────────────────────────────────
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f); // baixa CG para estabilidade

        // Começa num waypoint aleatório para não empilhar inimigos
        if (waypoints != null && waypoints.Length > 0)
            currentWaypointIndex = Random.Range(0, waypoints.Length);

        StartCoroutine(ChaosRoutine());
    }

    void FixedUpdate()
    {
        ApplyDownforce();
        UpdateAIState();
        DriveToTarget();
        ApplyWheelPhysics();
        UpdateWheelMeshes();
    }

    // ─────────────────────────────────────────
    //  MÁQUINA DE ESTADOS
    // ─────────────────────────────────────────
    void UpdateAIState()
    {
        if (IsFlipped())
        {
            state = AIState.Recovering;
            return;
        }

        float distToPlayer = playerTransform != null
            ? Vector3.Distance(transform.position, playerTransform.position)
            : float.MaxValue;

        // Decisão caótica baseada no nível de caos
        if (chaosLevel > 0.4f && distToPlayer < ramDistance && Random.value < chaosLevel * 0.02f)
            state = AIState.Ramming;
        else if (isDrifting)
            state = AIState.Drifting;
        else
            state = AIState.Racing;
    }

    // ─────────────────────────────────────────
    //  LÓGICA DE CONDUÇÃO
    // ─────────────────────────────────────────
    void DriveToTarget()
    {
        switch (state)
        {
            case AIState.Racing:    HandleRacing();    break;
            case AIState.Ramming:   HandleRamming();   break;
            case AIState.Drifting:  HandleDrifting();  break;
            case AIState.Recovering:HandleRecovering();break;
        }
    }

    void HandleRacing()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 toTarget = target.position - transform.position;

        // Avança para o próximo waypoint quando próximo
        if (toTarget.magnitude < waypointReachDistance)
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

        SteerTowards(target.position, chaosSteerOffset);

        float speed = GetSpeedKmh();
        float speedRatio = speed / topSpeed;

        currentMotor = maxMotorTorque * (1f - speedRatio * 0.8f);
        currentBrake = 0f;

        // Freia antes de curvas fechadas
        float angle = Vector3.Angle(transform.forward, toTarget.normalized);
        if (angle > 45f)
        {
            currentMotor *= 0.3f;
            currentBrake  = maxBrakeTorque * 0.4f;
        }
    }

    void HandleRamming()
    {
        if (playerTransform == null) { state = AIState.Racing; return; }

        SteerTowards(playerTransform.position, 0f);

        // Aceleração total no modo agressivo
        currentMotor = maxMotorTorque * 1.2f;
        currentBrake = 0f;

        // Aplica força de empurrão ao colidir
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist < 4f)
        {
            Rigidbody playerRb = playerTransform.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 pushDir = (playerTransform.position - transform.position).normalized;
                pushDir.y = 0f;
                playerRb.AddForce(pushDir * ramForce, ForceMode.Impulse);
            }
            state = AIState.Racing; // volta à corrida após colisão
        }
    }

    void HandleDrifting()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        SteerTowards(target.position, chaosSteerOffset * 2f);

        // Gás + freio traseiro = deriva
        currentMotor = maxMotorTorque * 0.8f;
        currentBrake = maxBrakeTorque * 0.3f;

        // Reduz fricção traseira para deslizar
        SetRearWheelFriction(0.4f);

        driftTimer -= Time.fixedDeltaTime;
        if (driftTimer <= 0f)
        {
            isDrifting = false;
            SetRearWheelFriction(1f); // restaura fricção
            state = AIState.Racing;
        }
    }

    void HandleRecovering()
    {
        // Tenta virar o carro de volta às rodas
        currentMotor = -maxMotorTorque * 0.5f;
        currentBrake = 0f;
        currentSteer = 0f;

        // Força de recuperação
        if (Vector3.Dot(transform.up, Vector3.down) > 0.5f)
        {
            rb.AddTorque(transform.right * 5000f, ForceMode.Force);
        }

        if (!IsFlipped()) state = AIState.Racing;
    }

    // ─────────────────────────────────────────
    //  STEERING
    // ─────────────────────────────────────────
    void SteerTowards(Vector3 targetPos, float offset)
    {
        Vector3 localTarget = transform.InverseTransformPoint(targetPos);
        float targetAngle   = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float steer         = Mathf.Clamp(targetAngle + offset, -maxSteerAngle, maxSteerAngle);

        // Suavização do steering
        currentSteer = Mathf.Lerp(currentSteer, steer, Time.fixedDeltaTime * 5f);
    }

    // ─────────────────────────────────────────
    //  FÍSICA DAS RODAS
    // ─────────────────────────────────────────
    void ApplyWheelPhysics()
    {
        // Direção nas rodas dianteiras
        wheelFL.steerAngle = currentSteer;
        wheelFR.steerAngle = currentSteer;

        // Tração nas rodas traseiras (RWD)
        wheelRL.motorTorque = currentMotor;
        wheelRR.motorTorque = currentMotor;

        // Freio em todas as rodas
        wheelFL.brakeTorque = currentBrake;
        wheelFR.brakeTorque = currentBrake;
        wheelRL.brakeTorque = currentBrake;
        wheelRR.brakeTorque = currentBrake;
    }

    void UpdateWheelMeshes()
    {
        UpdateSingleWheel(wheelFL, meshFL);
        UpdateSingleWheel(wheelFR, meshFR);
        UpdateSingleWheel(wheelRL, meshRL);
        UpdateSingleWheel(wheelRR, meshRR);
    }

    void UpdateSingleWheel(WheelCollider col, Transform mesh)
    {
        if (col == null || mesh == null) return;
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }

    void SetRearWheelFriction(float multiplier)
    {
        SetWheelFriction(wheelRL, multiplier);
        SetWheelFriction(wheelRR, multiplier);
    }

    void SetWheelFriction(WheelCollider wheel, float multiplier)
    {
        if (wheel == null) return;
        WheelFrictionCurve sideways = wheel.sidewaysFriction;
        sideways.stiffness = multiplier;
        wheel.sidewaysFriction = sideways;
    }

    // ─────────────────────────────────────────
    //  DOWNFORCE
    // ─────────────────────────────────────────
    void ApplyDownforce()
    {
        float speed = rb.linearVelocity.magnitude;
        rb.AddForce(-transform.up * downforce * speed);
    }

    // ─────────────────────────────────────────
    //  COROUTINE DE CAOS
    // ─────────────────────────────────────────
    IEnumerator ChaosRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(chaosChangeInterval);

            if (state == AIState.Recovering) continue;

            float roll = Random.value;

            // Zig-zag aleatório
            if (roll < chaosLevel * 0.5f)
            {
                chaosSteerOffset = Random.Range(-maxSteerAngle * 0.4f, maxSteerAngle * 0.4f);
            }
            else
            {
                chaosSteerOffset = 0f;
            }

            // Deriva intencional
            if (roll < chaosLevel * 0.25f && !isDrifting)
            {
                isDrifting  = true;
                driftTimer  = Random.Range(1f, 3f);
                state       = AIState.Drifting;
            }

            // Frenagem brusca caótica
            if (roll > 1f - chaosLevel * 0.15f)
            {
                StartCoroutine(ChaosBrake());
            }
        }
    }

    IEnumerator ChaosBrake()
    {
        float savedMotor = currentMotor;
        currentBrake = maxBrakeTorque;
        currentMotor = 0f;
        yield return new WaitForSeconds(Random.Range(0.3f, 0.8f));
        currentBrake = 0f;
    }

    // ─────────────────────────────────────────
    //  UTILIDADES
    // ─────────────────────────────────────────
    float GetSpeedKmh()
    {
        return rb.linearVelocity.magnitude * 3.6f;
    }

    bool IsFlipped()
    {
        return Vector3.Dot(transform.up, Vector3.up) < 0.1f;
    }

    // ─────────────────────────────────────────
    //  COLISÕES
    // ─────────────────────────────────────────
    void OnCollisionEnter(Collision col)
    {
        // Reação ao bater em obstáculos — vira na direção oposta
        if (col.gameObject.CompareTag("Obstacle") || col.gameObject.CompareTag("Wall"))
        {
            Vector3 bounce = Vector3.Reflect(transform.forward, col.contacts[0].normal);
            float   angle  = Vector3.SignedAngle(transform.forward, bounce, Vector3.up);
            chaosSteerOffset = Mathf.Clamp(angle * 0.5f, -maxSteerAngle, maxSteerAngle);
        }
    }

    // ─────────────────────────────────────────
    //  GIZMOS (visível no Editor)
    // ─────────────────────────────────────────
    void OnDrawGizmos()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue;
                Gizmos.DrawSphere(waypoints[i].position, 0.5f);
                if (i + 1 < waypoints.Length && waypoints[i + 1] != null)
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }

        // Raio de agressão
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, ramDistance);

        // Estado atual
        Gizmos.color = state == AIState.Ramming ? Color.red :
                       state == AIState.Drifting ? Color.yellow :
                       state == AIState.Recovering ? Color.magenta : Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.8f);
    }
}