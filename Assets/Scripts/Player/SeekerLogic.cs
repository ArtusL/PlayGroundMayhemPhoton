//using System.Collections;
//using UnityEngine;
//using Photon.Pun;

//public class SeekerLogic : MonoBehaviourPunCallbacks, IPunObservable
//{
//    public bool isSeeker;
//    public bool isStunned;

//    void OnSeekerStatusChanged(bool newSeekerStatus)
//    {
//        GetComponent<Renderer>().material.color = isSeeker ? Color.red : Color.blue;
//        float stamina = isSeeker ? GetComponent<PlayerController>().maxStaminaSeeker : GetComponent<PlayerController>().maxStaminaHider;
//        GetComponent<PlayerController>().SetStamina(stamina);
//    }

//    IEnumerator StunCoroutine()
//    {
//        isStunned = true;

//        yield return new WaitForSeconds(3);

//        isStunned = false;
//    }

//    void TagHider(GameObject other)
//    {
//        isSeeker = false;
//        SeekerLogic otherSeekerLogic = other.GetComponent<SeekerLogic>();
//        otherSeekerLogic.isSeeker = true;
//        otherSeekerLogic.StunPlayer();

//        PlayerController pc = GetComponent<PlayerController>();
//        pc.SetStamina(pc.maxStaminaHider);

//        PlayerController otherPc = other.GetComponent<PlayerController>();
//        otherPc.SetStamina(otherPc.maxStaminaSeeker);
//    }

//    public void StunPlayer()
//    {
//        if (photonView.IsMine)
//        {
//            StartCoroutine(StunCoroutine());
//        }
//    }

//    void OnCollisionEnter(Collision other)
//    {
//        if (photonView.IsMine && isSeeker && !isStunned && other.gameObject.GetComponent<SeekerLogic>() != null)
//        {
//            TagHider(other.gameObject);
//            Debug.Log("tagged hider has been stunned");
//        }
//    }

//    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//    {
//        if (stream.IsWriting)
//        {
//            stream.SendNext(isSeeker);
//            stream.SendNext(isStunned);
//        }
//        else
//        {
//            isSeeker = (bool)stream.ReceiveNext();
//            isStunned = (bool)stream.ReceiveNext();

//            OnSeekerStatusChanged(isSeeker);
//        }
//    }
//}
