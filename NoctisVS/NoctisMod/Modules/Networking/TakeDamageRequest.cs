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
        private bool causeForce;
        private bool vulnerability;

        //Don't network these.
        GameObject bodyObj;
        GameObject enemybodyObj;
        private BullseyeSearch search;
        private List<HurtBox> trackingTargets;
        private GameObject blastEffectPrefab;

        public TakeDamageRequest()
        {

        }

        public TakeDamageRequest(NetworkInstanceId netID, NetworkInstanceId enemyNetID, float damage, Vector3 direction, bool causeForce, bool vulnerability)
        {
            this.netID = netID;
            this.enemyNetID = enemyNetID;
            this.damage = damage;
            this.direction = direction;
            this.causeForce = causeForce;
            this.vulnerability = vulnerability;
        }

        public void Deserialize(NetworkReader reader)
        {
            netID = reader.ReadNetworkId();
            enemyNetID = reader.ReadNetworkId();
            damage = reader.ReadSingle();
            direction = reader.ReadVector3();
            causeForce = reader.ReadBoolean();
            vulnerability = reader.ReadBoolean();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netID);
            writer.Write(enemyNetID);
            writer.Write(damage);
            writer.Write(direction);
            writer.Write(causeForce);
            writer.Write(vulnerability);
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
            if (vulnerability)
            {
                DamageAPI.AddModdedDamageType(damageInfo, Damage.noctisVulnerability);
            }

            EffectManager.SpawnEffect(Modules.NoctisAssets.noctisHitEffect, new EffectData
            {
                origin = enemycharBody.transform.position,
                scale = 1f,
                rotation = Quaternion.LookRotation(direction).normalized,

            }, true);
            AkSoundEngine.PostEvent("NoctisHitSFX", enemycharBody.gameObject);


            float Weight = 1f;
            if (enemycharBody.characterMotor)
            {
                Weight = enemycharBody.characterMotor.mass;
            }
            else if (enemycharBody.rigidbody)
            {
                Weight = enemycharBody.rigidbody.mass;
            }

            if (causeForce)
            {
                enemycharBody.healthComponent.TakeDamageForce(direction * 40f * (Weight), true, true);
            }

            enemycharBody.healthComponent.TakeDamage(damageInfo);
            GlobalEventManager.instance.OnHitEnemy(damageInfo, enemycharBody.healthComponent.gameObject);

        }


    }
}