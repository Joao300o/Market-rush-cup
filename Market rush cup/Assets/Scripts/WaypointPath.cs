using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;

    // Gizmos para visualizar no Scene
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawSphere(waypoints[i].position, 0.3f);
                
                // Desenha linha entre os pontos
                if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }
        }
    }

    // Método útil para pegar o próximo waypoint
    public Transform GetNextWaypoint(int currentIndex)
    {
        if (waypoints == null || waypoints.Length == 0) return null;
        return waypoints[(currentIndex + 1) % waypoints.Length];
    }
}