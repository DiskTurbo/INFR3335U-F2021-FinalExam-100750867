using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{

    PhotonView PV;
    public Scene scene;


    [SerializeField] GameObject death;

    GameObject controller;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        scene = SceneManager.GetActiveScene();

        if (PV.IsMine)
        {
            CreateController();
        }
    }

    public void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();

        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });

        Debug.Log("Player controller instantiated.");
    }
    public void Death()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }


    public void switchPlayer(bool _isPlayer, float currentHealth)
    {
        if(PV.IsMine)
        {
            Vector3 currentPos = controller.transform.position;//+ new Vector3(0f, 1f, 0f);
            Quaternion currentRot = controller.transform.rotation;
            PhotonNetwork.Destroy(controller);
            if (_isPlayer == true)
            {
                controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "CarPlayerController"), currentPos, currentRot, 0, new object[] { PV.ViewID });
            }
            if (_isPlayer == false)
            {
                controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerControllerRed"), currentPos, new Quaternion(0f, currentRot.y, 0f, 1f), 0, new object[] { PV.ViewID });
            }
            Debug.Log("Player switched.");
        }
    }
    IEnumerator destroyObjectAfterTime(GameObject obj)
    {
        yield return new WaitForSeconds(5.0f);
        PhotonNetwork.Destroy(obj);
    }
}
