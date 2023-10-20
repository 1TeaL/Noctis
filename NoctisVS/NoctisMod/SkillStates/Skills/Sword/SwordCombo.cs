using EntityStates;
using RoR2;
using UnityEngine;
using NoctisMod.Modules.Survivors;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using NoctisMod.SkillStates.BaseStates;
using R2API;

namespace NoctisMod.SkillStates
{
    public class SwordCombo : BaseSkillState
    {
        public NoctisController noctisCon;
        private bool weaponSwap;

        protected Vector2 inputVector;

        public override void OnEnter()
        {

            base.OnEnter();
            noctisCon = gameObject.GetComponent<NoctisController>();
            Ray aimRay = base.GetAimRay();

            //check weapon swap
            if (noctisCon.weaponState != NoctisController.weaponType.NONE || noctisCon.weaponState != NoctisController.weaponType.SWORD)
            {
                weaponSwap = true;
            }
            else
            {
                weaponSwap = false;
            }


        }

        public override void Update()
        {
            base.Update();

            //check input
            if (weaponSwap)
            {
                //weapon swap combos
                if (!base.isGrounded)
                {
                    //aerial attack

                }
                else
                {
                    if (base.inputBank.moveVector == Vector3.zero)
                    {
                        //neutral attack
                        Chat.AddMessage("neutral attack- swap");
                    }
                    else
                    {
                        Vector3 moveVector = base.inputBank.moveVector;
                        Vector3 aimDirection = base.inputBank.aimDirection;
                        Vector3 normalized = new Vector3(aimDirection.x, 0f, aimDirection.z).normalized;
                        Vector3 up = base.transform.up;
                        Vector3 normalized2 = Vector3.Cross(up, normalized).normalized;

                        if (Vector3.Dot(base.inputBank.moveVector, normalized) >= 0.8f)
                        {
                            //forward attack
                            Chat.AddMessage("forward attack- swap");
                        }
                        else if (Vector3.Dot(base.inputBank.moveVector, normalized) <= -0.8f)
                        {
                            //backward attack
                            Chat.AddMessage("backward attack- swap");
                        }
                        else
                        {
                            //neutral attack
                            Chat.AddMessage("neutral attack- swap");
                        }

                    }

                }


            }
            else if (!weaponSwap)
            {
                //normal combos
            }
        }


        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

    }
}



