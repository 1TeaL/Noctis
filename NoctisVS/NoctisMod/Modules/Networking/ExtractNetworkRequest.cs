using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.UI.Image;


namespace NoctisMod.Modules.Networking
{
    internal class ExtractNetworkRequest : INetMessage
    {
        //Network these ones.
        NetworkInstanceId charnetID;
        Vector3 position;
        private float range;
        private float damage;

        //Don't network these.
        GameObject bodyObj;
        GameObject charbodyObj;
        private BullseyeSearch search;
        private List<HurtBox> trackingTargets;

        public ExtractNetworkRequest()
        {

        }

        public ExtractNetworkRequest(NetworkInstanceId charnetID, Vector3 position, float range, float damage)
        {
            this.position = position;
            this.range = range;
            this.damage = damage;
            this.charnetID = charnetID;
        }

        public void Deserialize(NetworkReader reader)
        {
            charnetID = reader.ReadNetworkId();
            position = reader.ReadVector3();
            range = reader.ReadSingle();
            damage = reader.ReadSingle();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(charnetID);
            writer.Write(position);
            writer.Write(range);
            writer.Write(damage);
        }

        public void OnReceived()
        {

            if (NetworkServer.active)
            {
                search = new BullseyeSearch();

                GameObject charmasterobject = Util.FindNetworkObject(charnetID);
                CharacterMaster charcharMaster = charmasterobject.GetComponent<CharacterMaster>();
                CharacterBody charcharBody = charcharMaster.GetBody();
                charbodyObj = charcharBody.gameObject;

                //Damage target and stun
                SearchForTarget(charcharBody);
                DamageTargets(charcharBody);
            }
        }

        private void SearchForTarget(CharacterBody charBody)
        {
            this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(charBody.teamComponent.teamIndex);
            this.search.filterByLoS = true;
            this.search.searchOrigin = position;
            this.search.searchDirection = charBody.characterDirection.forward;
            this.search.sortMode = BullseyeSearch.SortMode.Distance;
            this.search.maxDistanceFilter = range;
            this.search.maxAngleFilter = 360f;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(charBody.gameObject);
            this.trackingTargets = this.search.GetResults().ToList<HurtBox>();
        }

        private void DamageTargets(CharacterBody charcharBody)
        {
            if (trackingTargets.Count > 0)
            {
                foreach (HurtBox singularTarget in trackingTargets)
                {           
                    DamageInfo damageInfo = new DamageInfo
                    {
                        attacker = charbodyObj,
                        inflictor = charbodyObj,
                        damage = damage,
                        position = singularTarget.transform.position,
                        procCoefficient = 1f,
                        damageType = DamageType.BypassArmor | DamageType.BonusToLowHealth,
                        crit = charcharBody.RollCrit(),

                    };

                    singularTarget.healthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, singularTarget.healthComponent.gameObject);



                }
            }

            
        }      

    }
}