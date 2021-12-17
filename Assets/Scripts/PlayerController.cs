using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.Audio;
using JSAM;

public class PlayerController : MonoBehaviourPunCallbacks
{

    [SerializeField] Camera tpCamera;

    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    [SerializeField] AudioMixerGroup Master;

    [SerializeField] GameObject UI;

    [SerializeField] GameObject Joystick;
    [SerializeField] GameObject camJoystick;

    float verticalLookRotation;

    bool grounded;
    bool isPlayer = true;

    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;

    PhotonView PV;

    PlayerManager playerManager;

    [SerializeField] Animator anim;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        Master = GetComponent<AudioMixerGroup>();
        
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start()
    {
        if(PV.IsMine)
        {
            // NULL.
        }
        else
        {
            Destroy(tpCamera);
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(UI);
        }
    }

    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }
        Look();
        Move();
        Jump();
        if (transform.position.y < -50.0f)
        {
            Death();
        }
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * mouseSensitivity);
        verticalLookRotation += Input.GetAxis("Vertical") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
    }


    void Move()
    {
        float horMove = Joystick.GetComponent<Joystick>().Horizontal;
        float verMove = Joystick.GetComponent<Joystick>().Vertical;
        Vector3 moveDir = new Vector3(horMove, 0, verMove).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
        if (!grounded)
        {
            anim.SetBool("isJumping", true);
        }
        else
        {
            anim.SetBool("isJumping", false);
        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    IEnumerator CameraZoomIn(Quaternion _CameraRot)
    {
        var t = 0.0f;
        while (t <= 1.0f)
        {
            t += 7f * Time.fixedUnscaledDeltaTime;
            tpCamera.transform.localPosition = Vector3.Lerp(new Vector3(0, 1.5f, -5), new Vector3(1, 1, -2), t);
            yield return null;
        }
    }
    void Death()
    {
        playerManager.Death();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
