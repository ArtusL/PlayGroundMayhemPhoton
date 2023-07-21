using Photon.Pun;
using UnityEngine;
using System.Collections;

public class PlayerRoleManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool isSeeker;
    public bool canTag = true;
    public GameObject model;  // Assign the player model in Unity Inspector

    private void Start()
    {
        if (photonView.IsMine)
        {
            // Decide who is the seeker
            isSeeker = PhotonNetwork.IsMasterClient;
            photonView.RPC("SetSeeker", RpcTarget.AllBuffered, isSeeker);
        }
    }

    [PunRPC]
    void SetSeeker(bool _isSeeker)
    {
        isSeeker = _isSeeker;
        model.GetComponentInChildren<Renderer>().material.color = isSeeker ? Color.red : Color.blue;
        if (isSeeker)
        {
            StartCoroutine(TagCooldown());
        }
    }

    IEnumerator TagCooldown()
    {
        canTag = false;
        yield return new WaitForSeconds(3f);
        canTag = true;
    }

    [PunRPC]
    public void ResetRoleAndPosition(Vector3 newPosition)
    {
        isSeeker = false;
        transform.position = newPosition;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(isSeeker);
        }
        else
        {
            // Network player, receive data
            this.isSeeker = (bool)stream.ReceiveNext();
        }
    }
}
