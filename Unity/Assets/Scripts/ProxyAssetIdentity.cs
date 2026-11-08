using UnityEngine;
namespace Majoras.Foundation
{
    // Import metadata only. Gameplay state must not live on visualization proxies.
    public sealed class ProxyAssetIdentity : MonoBehaviour
    {
        public string assetId;
        public string provenance;
        public bool collisionIntent;
    }
}
