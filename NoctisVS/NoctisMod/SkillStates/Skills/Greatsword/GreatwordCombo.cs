using EntityStates;
using RoR2;
using UnityEngine;
using NoctisMod.Modules.Survivors;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using NoctisMod.SkillStates.BaseStates;
using R2API;
using System.Reflection;

namespace NoctisMod.SkillStates
{
    public class GreatswordCombo : BaseSkillState
    {
        public NoctisController noctisCon;
        private bool weaponSwap;
        public int currentSwingIndex;
        public HurtBox Target;


        public override void OnEnter()
        {

            base.OnEnter();
            noctisCon = gameObject.GetComponent<NoctisController>();
            Ray aimRay = base.GetAimRay();

            weaponSwap = false;
            //check weapon swap
            if (noctisCon.weaponState == NoctisController.WeaponType.NONE || noctisCon.weaponState == NoctisController.WeaponType.GREATSWORD)
            {
                weaponSwap = false;
            }
            else
            {
                weaponSwap = true;
            }


        }

        public void Exit()
        {
            base.OnExit();
        }

        public override void Update()
        {
            base.Update();

            if (base.isAuthority)
            {
                //check input
                if (weaponSwap)
                {
                    //weapon swap combos
                    if (!base.isGrounded)
                    {
                        //aerial attack
                        Chat.AddMessage("aerial attack");
                        GreatswordSwapAerial GreatswordSwapAerial = new GreatswordSwapAerial();
                        this.outer.SetNextState(GreatswordSwapAerial);
                        return;

                    }
                    else
                    {
                        if (base.inputBank.moveVector == Vector3.zero)
                        {
                            //neutral attack
                            Chat.AddMessage("neutral attack- swap");
                            GreatswordSwapNeutral GreatswordSwapNeutral = new GreatswordSwapNeutral();
                            GreatswordSwapNeutral.swingIndex = currentSwingIndex;
                            this.outer.SetNextState(GreatswordSwapNeutral);
                            return;
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
                                Chat.AddMessage("forward attack- swap");
                                GreatswordSwapForward GreatswordSwapForward = new GreatswordSwapForward();
                                this.outer.SetNextState(GreatswordSwapForward);
                                return;
                            }
                            else if (Vector3.Dot(base.inputBank.moveVector, normalized) <= -0.8f)
                            {
                                //backward attack
                                Chat.AddMessage("backward attack- swap");
                                this.outer.SetNextState(new GreatswordSwapBackward());
                                return;
                            }
                            else
                            {
                                //neutral attack
                                Chat.AddMessage("neutral attack- swap");
                                GreatswordSwapNeutral GreatswordSwapNeutral = new GreatswordSwapNeutral();
                                GreatswordSwapNeutral.swingIndex = currentSwingIndex;
                                this.outer.SetNextState(GreatswordSwapNeutral);
                                return;
                            }

                        }

                    }


                }
                else if (!weaponSwap)
                {
                    //normal combos

                    if (!base.isGrounded)
                    {
                        //aerial attack
                        Chat.AddMessage("aerial attack");
                        GreatswordAerial GreatswordAerial = new GreatswordAerial();
                        this.outer.SetNextState(GreatswordAerial);
                        return;

                    }
                    else
                    {
                        if (base.inputBank.moveVector == Vector3.zero)
                        {
                            //neutral attack
                            Chat.AddMessage("neutral attack");
                            GreatswordNeutral GreatswordNeutral = new GreatswordNeutral();
                            GreatswordNeutral.swingIndex = currentSwingIndex;
                            this.outer.SetNextState(GreatswordNeutral);
                            return;
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
                                Chat.AddMessage("forward attack");
                                GreatswordForward GreatswordForward = new GreatswordForward();
                                this.outer.SetNextState(GreatswordForward);
                                return;
                            }
                            else if (Vector3.Dot(base.inputBank.moveVector, normalized) <= -0.8f)
                            {
                                //backward attack
                                Chat.AddMessage("backward attack");
                                this.outer.SetNextState(new GreatswordBackward());
                                return;
                            }
                            else
                            {
                                //neutral attack
                                Chat.AddMessage("neutral attack");
                                GreatswordNeutral GreatswordNeutral = new GreatswordNeutral();
                                GreatswordNeutral.swingIndex = currentSwingIndex;
                                this.outer.SetNextState(GreatswordNeutral);
                                return;
                            }

                        }

                    }
                }

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

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.currentSwingIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.currentSwingIndex = reader.ReadInt32();            
        }
    }
}



