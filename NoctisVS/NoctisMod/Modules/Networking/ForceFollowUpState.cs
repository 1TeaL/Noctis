using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using R2API.Networking.Interfaces;
using UnityEngine;
using UnityEngine.Networking;
using NoctisMod.SkillStates;

namespace NoctisMod.Modules.Networking
{
    public class ForceFollowUpState : INetMessage
    {
        NetworkInstanceId IDNet;
        NetworkInstanceId enemyIDNet;

        public ForceFollowUpState()
        {

        }

        public ForceFollowUpState(NetworkInstanceId IDNet, NetworkInstanceId enemyIDNet)
        {
            this.IDNet = IDNet;
            this.enemyIDNet = enemyIDNet;
        }

        public void Deserialize(NetworkReader reader)
        {
            IDNet = reader.ReadNetworkId();
            enemyIDNet = reader.ReadNetworkId();
        }

        public void OnReceived()
        {
            if (NetworkServer.active)
            {
                GameObject masterobj = Util.FindNetworkObject(IDNet);
                if (!masterobj)
                {
                    Debug.Log("masterobj not found");
                    return;
                }
                CharacterMaster charmast = masterobj.GetComponent<CharacterMaster>();
                if (!charmast)
                {
                    Debug.Log("charmast not found");
                    return;
                }
                GameObject charbodyobj = charmast.GetBodyObject();
                if (!charbodyobj)
                {
                    Debug.Log("charbodyobj not found");
                    return;
                }
                GameObject enemymasterobj = Util.FindNetworkObject(enemyIDNet);
                if (!enemymasterobj)
                {
                    Debug.Log("enemymasterobj not found");
                    return;
                }
                CharacterMaster enemycharmast = enemymasterobj.GetComponent<CharacterMaster>();
                if (!charmast)
                {
                    Debug.Log("enemycharmast not found");
                    return;
                }
                GameObject enemycharbodyobj = enemycharmast.GetBodyObject();
                if (!charbodyobj)
                {
                    Debug.Log("enemycharbodyobj not found");
                    return;
                }
                CharacterBody enemycharBody = enemycharmast.GetBody();
                EntityStateMachine[] statemachines = charbodyobj.GetComponents<EntityStateMachine>();
                foreach (EntityStateMachine statemachine in statemachines)
                {
                    if (statemachine.customName == "Body")
                    {
                        statemachine.SetState(new GreatswordFollowUpSlam
                        {
                            Target = enemycharBody,
                        });
                    }
                }

            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(IDNet);
        }
    }
}
