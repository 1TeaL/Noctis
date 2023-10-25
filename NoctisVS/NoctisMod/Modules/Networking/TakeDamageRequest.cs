using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


namespace NoctisMod.Modules.Networking
{
    internal class TakeDamageRequest : INetMessage
    {
        //Network these ones.
        NetworkInstanceId enemyNetID;
        NetworkInstanceId netID;
        Vector3 direction;
        private float force;
        private float damage;

        //Don't network these.
        GameObject bodyObj;
        GameObject enemybodyObj;
        private BullseyeSearch search;
        private List<HurtBox> trackingTargets;
        private GameObject blastEffectPrefab;

        public TakeDamageRequest()
        {

        }

        public TakeDamageRequest(NetworkInstanceId netID, NetworkInstanceId enemyNetID, float damage)
        {
            this.netID = netID;
            this.enemyNetID = enemyNetID;
            this.damage = damage;
        }

        public void Deserialize(NetworkReader reader)
        {
            netID = reader.ReadNetworkId();
            enemyNetID = reader.ReadNetworkId();
            damage = reader.ReadSingle();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netID);
            writer.Write(enemyNetID);
            writer.Write(damage);
        }

        public void OnReceived()
        {

            if (NetworkServer.active)
            {
                GameObject masterobject = Util.FindNetworkObject(netID);
                CharacterMaster charMaster = masterobject.GetComponent<CharacterMaster>();
                CharacterBody charBody = charMaster.GetBody();
                bodyObj = charBody.gameObject;

                GameObject enemymasterobject = Util.FindNetworkObject(enemyNetID);
                CharacterMaster enemycharMaster = enemymasterobject.GetComponent<CharacterMaster>();
                CharacterBody enemycharBody = enemycharMaster.GetBody();
                enemybodyObj = enemycharBody.gameObject;

                //Smash targets and stun
                DamageTarget(charBody, enemycharBody);
            }
        }




        private void DamageTarget(CharacterBody charBody, CharacterBody enemycharBody)
        {

            DamageInfo damageInfo = new DamageInfo
            {
                attacker = bodyObj,
                damage = damage,
                position = enemycharBody.transform.position,
                procCoefficient = StaticValues.swordProc,
                damageType = DamageType.Generic,
                crit = charBody.RollCrit(),

            };

            Vector3 direction = charBody.characterDirection.forward;

            EffectManager.SpawnEffect(Modules.Assets.noctisHitEffect, new EffectData
            {
                origin = enemycharBody.transform.position,
                scale = 1f,
                rotation = Quaternion.LookRotation(direction).normalized,

            }, true);
            EffectManager.SpawnEffect(blastEffectPrefab, new EffectData
            {
                origin = enemycharBody.transform.position,
                scale = 1f,
                rotation = Quaternion.LookRotation(direction).normalized,

            }, true);


            enemycharBody.healthComponent.TakeDamage(damageInfo);
            GlobalEventManager.instance.OnHitEnemy(damageInfo, enemycharBody.healthComponent.gameObject);

        }


    }
}