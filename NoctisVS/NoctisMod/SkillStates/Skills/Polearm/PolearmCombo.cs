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
using static NoctisMod.Modules.Survivors.NoctisController;

namespace NoctisMod.SkillStates
{
    public class PolearmCombo : BaseSkillState
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
            if (noctisCon.weaponStateR == NoctisController.WeaponTypeR.NONE || noctisCon.weaponStateR >= NoctisController.WeaponTypeR.POLEARM)
            {
                weaponSwap = false;
            }
            else
            {
                weaponSwap = true;
            }


            noctisCon.WeaponAppearR(5f, WeaponTypeR.POLEARM);
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

                        Vector3 moveVector = base.inputBank.moveVector;
                        Vector3 aimDirection = base.inputBank.aimDirection;
                        Vector3 normalized = new Vector3(aimDirection.x, 0f, aimDirection.z).normalized;
                        Vector3 up = base.transform.up;
                        Vector3 normalized2 = Vector3.Cross(up, normalized).normalized;

                        if (Vector3.Dot(base.inputBank.moveVector, normalized) <= -0.8f)
                        {
                            //backward attack
                            Chat.AddMessage("backward aerial attack");
                            this.outer.SetNextState(new PolearmDoubleDragoonThrust());
                            return;
                        }
                        else
                        {
                            //check distance to target if target exists
                            if (noctisCon.GetTrackingTarget())
                            {
                                Target = noctisCon.GetTrackingTarget();
                                float distance = Vector3.Distance(base.transform.position, Target.transform.position);
                                if (distance > Modules.StaticValues.polearmDashSpeed)
                                {
                                    Chat.AddMessage("aerial attack");
                                    PolearmSwapAerial PolearmSwapAerial = new PolearmSwapAerial();
                                    PolearmSwapAerial.isTarget = true;
                                    PolearmSwapAerial.Target = Target;
                                    this.outer.SetNextState(PolearmSwapAerial);
                                    return;

                                }
                                else
                                {
                                    Chat.AddMessage("aerial attack");
                                    PolearmSwapAerial PolearmSwapAerial = new PolearmSwapAerial();
                                    PolearmSwapAerial.isTarget = false;
                                    this.outer.SetNextState(PolearmSwapAerial);
                                    return;

                                }
                            }
                            else
                            {
                                PolearmSwapAerial PolearmSwapAerial = new PolearmSwapAerial();
                                PolearmSwapAerial.isTarget = false;
                                this.outer.SetNextState(PolearmSwapAerial);
                                return;

                            }
                        }

                    }
                    else
                    {
                        if (base.inputBank.moveVector == Vector3.zero)
                        {
                            //neutral attack
                            Chat.AddMessage("neutral attack- swap");
                            PolearmSwapNeutral PolearmSwapNeutral = new PolearmSwapNeutral();
                            PolearmSwapNeutral.swingIndex = currentSwingIndex;
                            this.outer.SetNextState(PolearmSwapNeutral);
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
                                PolearmSwapForward PolearmSwapForward = new PolearmSwapForward();
                                this.outer.SetNextState(PolearmSwapForward);
                                return;
                            }
                            else if (Vector3.Dot(base.inputBank.moveVector, normalized) <= -0.8f)
                            {
                                //backward attack
                                Chat.AddMessage("backward attack- swap");
                                this.outer.SetNextState(new PolearmSwapBackward());
                                return;
                            }
                            else
                            {
                                //neutral attack
                                Chat.AddMessage("neutral attack- swap");
                                PolearmSwapNeutral PolearmSwapNeutral = new PolearmSwapNeutral();
                                PolearmSwapNeutral.swingIndex = currentSwingIndex;
                                this.outer.SetNextState(PolearmSwapNeutral);
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
                        Vector3 moveVector = base.inputBank.moveVector;
                        Vector3 aimDirection = base.inputBank.aimDirection;
                        Vector3 normalized = new Vector3(aimDirection.x, 0f, aimDirection.z).normalized;
                        Vector3 up = base.transform.up;
                        Vector3 normalized2 = Vector3.Cross(up, normalized).normalized;

                        if (Vector3.Dot(base.inputBank.moveVector, normalized) <= -0.8f)
                        {
                            //backward attack
                            Chat.AddMessage("backward aerial attack");
                            this.outer.SetNextState(new PolearmDragoonThrust());
                            return;
                        }
                        else
                        {
                            //neutral attack
                            Chat.AddMessage("aerial attack");
                            PolearmAerial PolearmAerial = new PolearmAerial();
                            this.outer.SetNextState(PolearmAerial);
                            return;
                        }

                    }
                    else
                    {
                        if (base.inputBank.moveVector == Vector3.zero)
                        {
                            //neutral attack
                            Chat.AddMessage("neutral attack");
                            PolearmNeutral PolearmNeutral = new PolearmNeutral();
                            PolearmNeutral.swingIndex = currentSwingIndex;
                            this.outer.SetNextState(PolearmNeutral);
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
                                //check distance to target if target exists
                                if (noctisCon.GetTrackingTarget())
                                {
                                    Target = noctisCon.GetTrackingTarget();
                                    float distance = Vector3.Distance(base.transform.position, Target.transform.position);
                                    if (distance > Modules.StaticValues.polearmDashSpeed)
                                    {
                                        Chat.AddMessage("forward attack");
                                        PolearmForward PolearmForward = new PolearmForward();
                                        this.outer.SetNextState(PolearmForward);
                                        return;

                                    }
                                    else
                                    {
                                        //neutral attack
                                        Chat.AddMessage("neutral attack");
                                        PolearmNeutral PolearmNeutral = new PolearmNeutral();
                                        PolearmNeutral.swingIndex = currentSwingIndex;
                                        this.outer.SetNextState(PolearmNeutral);
                                        return;

                                    }
                                }
                                else
                                {
                                    Chat.AddMessage("forward attack");
                                    PolearmForward PolearmForward = new PolearmForward();
                                    this.outer.SetNextState(PolearmForward);
                                    return;

                                }
                            }
                            else if (Vector3.Dot(base.inputBank.moveVector, normalized) <= -0.8f)
                            {
                                //backward attack
                                Chat.AddMessage("backward attack");
                                this.outer.SetNextState(new PolearmBackward());
                                return;
                            }
                            else
                            {
                                //neutral attack
                                Chat.AddMessage("neutral attack");
                                PolearmNeutral PolearmNeutral = new PolearmNeutral();
                                PolearmNeutral.swingIndex = currentSwingIndex;
                                this.outer.SetNextState(PolearmNeutral);
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



