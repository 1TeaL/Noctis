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
    public class SwordCombo : BaseSkillState
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
            if (noctisCon.weaponStateR == NoctisController.WeaponTypeR.NONE || noctisCon.weaponStateR == NoctisController.WeaponTypeR.SWORD)
            {
                weaponSwap = false;
            }
            else
            {
                weaponSwap = true;
            }

            noctisCon.WeaponAppearR(5f, WeaponTypeR.SWORD);

            if (base.isAuthority)
            {
                if (Modules.Config.allowVoice.Value) { AkSoundEngine.PostEvent("NoctisVoice", base.gameObject); }
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
                        SwordSwapAerial SwordSwapAerial = new SwordSwapAerial();
                        this.outer.SetNextState(SwordSwapAerial);
                        return;

                    }
                    else
                    {
                        if (base.inputBank.moveVector == Vector3.zero)
                        {
                            //neutral attack
                            Chat.AddMessage("neutral attack- swap");
                            SwordSwapNeutral SwordSwapNeutral = new SwordSwapNeutral();
                            SwordSwapNeutral.swingIndex = currentSwingIndex;
                            this.outer.SetNextState(SwordSwapNeutral);
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
                                Chat.AddMessage("forward attack- swap");
                                SwordSwapForward SwordSwapForward = new SwordSwapForward();
                                this.outer.SetNextState(SwordSwapForward);
                                return;
                            }
                            else if (Vector3.Dot(base.inputBank.moveVector, normalized) <= -0.8f)
                            {
                                //backward attack
                                Chat.AddMessage("backward attack- swap");
                                this.outer.SetNextState(new SwordSwapBackward());
                                return;
                            }
                            else
                            {
                                //neutral attack
                                Chat.AddMessage("neutral attack- swap");
                                SwordSwapNeutral SwordSwapNeutral = new SwordSwapNeutral();
                                SwordSwapNeutral.swingIndex = currentSwingIndex;
                                this.outer.SetNextState(SwordSwapNeutral);
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
                        //check distance to target if target exists
                        if (noctisCon.GetTrackingTarget())
                        {
                            Target = noctisCon.GetTrackingTarget();
                            float distance = Vector3.Distance(base.transform.position, Target.transform.position);
                            if (distance > Modules.StaticValues.swordDashDistance)
                            {
                                Chat.AddMessage("aerial attack");
                                SwordAerial SwordAerial = new SwordAerial();
                                SwordAerial.isTarget = true;
                                SwordAerial.Target = Target;
                                this.outer.SetNextState(SwordAerial);
                                return;

                            }
                            else
                            {
                                //aerial attack
                                Chat.AddMessage("aerial attack");
                                SwordAerial SwordAerial = new SwordAerial();
                                SwordAerial.isTarget = false;
                                this.outer.SetNextState(SwordAerial);
                                return;

                            }
                        }
                        else
                        {
                            Chat.AddMessage("aerial attack");
                            SwordAerial SwordAerial = new SwordAerial();
                            SwordAerial.isTarget = false;
                            this.outer.SetNextState(SwordAerial);
                            return;

                        }

                    }
                    else
                    {
                        if (base.inputBank.moveVector == Vector3.zero)
                        {
                            //neutral attack
                            Chat.AddMessage("neutral attack");
                            SwordNeutral SwordNeutral = new SwordNeutral();
                            SwordNeutral.swingIndex = currentSwingIndex;
                            this.outer.SetNextState(SwordNeutral);
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
                                    if (distance > Modules.StaticValues.swordDashDistance)
                                    {
                                        Chat.AddMessage("forward attack");
                                        SwordForward SwordForward = new SwordForward();
                                        this.outer.SetNextState(SwordForward);
                                        return;

                                    }
                                    else
                                    {
                                        //neutral attack
                                        Chat.AddMessage("neutral attack");
                                        SwordNeutral SwordNeutral = new SwordNeutral();
                                        SwordNeutral.swingIndex = currentSwingIndex;
                                        this.outer.SetNextState(SwordNeutral);
                                        return;

                                    }
                                }
                                else
                                {
                                    Chat.AddMessage("forward attack");
                                    SwordForward SwordForward = new SwordForward();
                                    this.outer.SetNextState(SwordForward);
                                    return;

                                }
                            }
                            else if (Vector3.Dot(base.inputBank.moveVector, normalized) <= -0.8f)
                            {
                                //backward attack
                                Chat.AddMessage("backward attack");
                                this.outer.SetNextState(new SwordBackward());
                                return;
                            }
                            else
                            {
                                //neutral attack
                                Chat.AddMessage("neutral attack");
                                SwordNeutral SwordNeutral = new SwordNeutral();
                                SwordNeutral.swingIndex = currentSwingIndex;
                                this.outer.SetNextState(SwordNeutral);
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



