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
    internal class TakeMeleeDamageForceRequest : INetMessage
    {
        //Network these ones.
        NetworkInstanceId enemynetID;
        NetworkInstanceId charnetID;
        Vector3 direction;
        private float force;
        private float damage;

        //Don't network these.
        GameObject bodyObj;
        GameObject charbodyObj;
        private BullseyeSearch search;
        private List<HurtBox> trackingTargets;
        private GameObject blastEffectPrefab;

        public TakeMeleeDamageForceRequest()
        {

        }

        public TakeMeleeDamageForceRequest(NetworkInstanceId enemynetID, Vector3 direction, float force, float damage, NetworkInstanceId charnetID)
        {
            this.enemynetID = enemynetID;
            this.direction = direction;
            this.force = force;
            this.damage = damage;
            this.charnetID = charnetID;
        }

        public void Deserialize(NetworkReader reader)
        {
            enemynetID = reader.ReadNetworkId();
            direction = reader.ReadVector3();
            force = reader.ReadSingle();
            damage = reader.ReadSingle();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(enemynetID);
            writer.Write(direction);
            writer.Write(force);
            writer.Write(damage);
        }

        public void OnReceived()
        {

            if (NetworkServer.active)
            {
                search = new BullseyeSearch();
                //Spawn the effect around this object.
                GameObject enemymasterobject = Util.FindNetworkObject(enemynetID);
                CharacterMaster enemycharMaster = enemymasterobject.GetComponent<CharacterMaster>();
                CharacterBody enemycharBody = enemycharMaster.GetBody();
                bodyObj = enemycharBody.gameObject;

                GameObject charmasterobject = Util.FindNetworkObject(charnetID);
                CharacterMaster charcharMaster = charmasterobject.GetComponent<CharacterMaster>();
                CharacterBody charcharBody = charcharMaster.GetBody();
                charbodyObj = charcharBody.gameObject;

                //Damage target and stun
            }
        }
     

    }
}