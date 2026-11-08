using UnityEngine;

namespace Majoras.City
{
    // Bounded preview stops built from the original exit-floor polygon bounds.
    public sealed class CityThreshold : MonoBehaviour
    {
        public string destination;
        public Collider stop;
        public static string Nearby(CityExploration player)
        {
            if (player == null) return null;
            Vector3 eye = player.transform.position + Vector3.up * .65f;
            foreach (var threshold in Object.FindObjectsByType<CityThreshold>())
                if (threshold.stop != null && Vector3.Distance(eye, threshold.stop.ClosestPoint(eye)) < .85f)
                    return threshold.destination;
            return null;
        }
    }
}
