﻿using Game.GameLogic;
using Mirror;
using UnityEngine;

namespace Game.Player
{
    public class HealthHandler : NetworkBehaviour, IDamageable
    {
        [SerializeField]
        [SyncVar]
        public int curHealth = 100;

        [SerializeField]
        private AudioSource getShotClip;

        [SerializeField]
        private UIManager _uIManager;

        [Command]
        private void CmdDamage(int damage)
        {
            if (!GameManager.instance.hasGameStarted) return;
            curHealth -= damage;
            if (curHealth > 0)
            {
                RpcDamagePlayer();
                TargetDamagePlayer(netIdentity.connectionToClient, curHealth);
                return;
            }
            GameManager.instance.ServerKillPlayer(GetComponent<NetworkGamePlayer>());
        }

        [ClientRpc]
        private void RpcDamagePlayer()
        {
            getShotClip?.Play();
        }

        [TargetRpc]
        public void TargetDamagePlayer(NetworkConnection conn, int newHealth)
        {
            _uIManager.UpdateHealth(newHealth);
        }

        [TargetRpc]
        public void Damage(int damage) => CmdDamage(damage);
    }
}