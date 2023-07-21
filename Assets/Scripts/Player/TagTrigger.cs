using UnityEngine;
using Photon.Pun;

public class TagTrigger : MonoBehaviour
{
    public PlayerRoleManager roleManager;

    private void OnTriggerEnter(Collider other)
    {
        if (roleManager == null || !roleManager.isSeeker || !roleManager.canTag)
            return;

        PlayerRoleManager otherPlayerRoleManager = other.gameObject.GetComponentInParent<PlayerRoleManager>();

        if (otherPlayerRoleManager == null || otherPlayerRoleManager == roleManager || otherPlayerRoleManager.isSeeker)
        {
            return;
        }

        Debug.Log(gameObject.name + " is tagging " + other.gameObject.name);

        otherPlayerRoleManager.photonView.RPC("SetSeeker", RpcTarget.AllBuffered, true);
        roleManager.photonView.RPC("SetSeeker", RpcTarget.AllBuffered, false);
    }
}
