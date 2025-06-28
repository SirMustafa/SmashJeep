using Unity.Netcode;
using UnityEngine;

public class PlayerInteractionContoller : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!IsOwner) return;

        if (other.gameObject.TryGetComponent<ICollectable>(out ICollectable collectable))
        {
            collectable.Collect();
        }
    }
}