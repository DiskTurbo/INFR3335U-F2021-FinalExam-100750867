﻿using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;

    Player player;
    public void SetUp(Player _player)
    {
        player = _player;
        text.text = _player.NickName;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(player == otherPlayer)
        {
            Destroy(gameObject);
        }
        Debug.Log(otherPlayer.NickName + " has left the room.");
    }
    public override void OnLeftRoom()
    {
        Destroy(gameObject);
        Debug.Log("Left room.");
    }
}