using Photon.Pun;
using UnityEngine;
using System.Collections;

public class PlayerRoleManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public PlayerController playerController;
    public bool isSeeker;
    public bool canTag = true;
    public GameObject model;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (photonView.IsMine)
        {
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
            //StartCoroutine(playerController.Stun(3.0f));
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
        if (playerController.IsLocalPlayer())
        {
            playerController.StopAllCoroutines();
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isSeeker);
        }
        else
        {
            this.isSeeker = (bool)stream.ReceiveNext();
        }
    }
}
