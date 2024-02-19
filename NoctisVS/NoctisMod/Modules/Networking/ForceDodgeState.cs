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
    public class ForceDodgeState : INetMessage
    {
        NetworkInstanceId IDNet;

        public ForceDodgeState()
        {

        }

        public ForceDodgeState(NetworkInstanceId IDNet)
        {
            this.IDNet = IDNet;
        }

        public void Deserialize(NetworkReader reader)
        {
            IDNet = reader.ReadNetworkId();
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
               
                EntityStateMachine[] statemachines = charbodyobj.GetComponents<EntityStateMachine>();
                foreach (EntityStateMachine statemachine in statemachines)
                {
                    if (statemachine.customName == "Body")
                    {
                        statemachine.SetState(new Dodge
                        {                            
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
