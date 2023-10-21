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

            //check weapon swap
            if (noctisCon.weaponState == NoctisController.weaponType.NONE || noctisCon.weaponState == NoctisController.weaponType.SWORD)
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
            noctisCon.weaponState = NoctisController.weaponType.SWORD;
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
                        //check distance to target if target exists
                        if (noctisCon.GetTrackingTarget())
                        {
                            Target = noctisCon.GetTrackingTarget();
                            float distance = Vector3.Distance(base.transform.position, Target.transform.position);
                            if (distance > Modules.StaticValues.swordDashDistance)
                            {
                                Chat.AddMessage("aerial attack");
                                this.outer.SetNextState(new SwordSwapAerial
                                {
                                    isTarget = true,
                                    Target = Target
                                });
                                return;

                            }
                            else
                            {
                                Chat.AddMessage("aerial attack");
                                this.outer.SetNextState(new SwordSwapAerial
                                {
                                    isTarget = false,
                                });
                                return;

                            }
                        }
                        else
                        {
                            this.outer.SetNextState(new SwordSwapAerial
                            {
                                isTarget = false
                            });
                            return;

                        }

                    }
                    else
                    {
                        if (base.inputBank.moveVector == Vector3.zero)
                        {
                            //neutral attack
                            Chat.AddMessage("neutral attack- swap");
                            this.outer.SetNextState(new SwordSwapNeutral
                            {
                                swingIndex = currentSwingIndex,
                            });
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
                                if (noctisCon.GetTrackingTarget())
                                {
                                    Target = noctisCon.GetTrackingTarget();
                                    float distance = Vector3.Distance(base.transform.position, Target.transform.position);
                                    if (distance > Modules.StaticValues.swordDashDistance)
                                    {
                                        Chat.AddMessage("forward attack- swap");
                                        this.outer.SetNextState(new SwordSwapForward
                                        {
                                            isTarget = true,
                                            Target = Target
                                        });
                                        return;

                                    }
                                    else
                                    {
                                        //neutral attack
                                        Chat.AddMessage("neutral attack- swap");
                                        this.outer.SetNextState(new SwordSwapNeutral
                                        {
                                            swingIndex = currentSwingIndex,
                                        });
                                        return;

                                    }
                                }
                                else
                                {
                                    Chat.AddMessage("forward attack- swap");
                                    this.outer.SetNextState(new SwordSwapForward
                                    {
                                        isTarget = false
                                    });
                                    return;

                                }
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
                                this.outer.SetNextState(new SwordSwapNeutral
                                {
                                    swingIndex = currentSwingIndex,
                                });
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
                                this.outer.SetNextState(new SwordAerial
                                {
                                    isTarget = true,
                                    Target = Target
                                });
                                return;

                            }
                            else
                            {
                                //neutral attack
                                Chat.AddMessage("neutral attack");
                                this.outer.SetNextState(new SwordAerial
                                {
                                    isTarget = false,
                                });
                                return;

                            }
                        }
                        else
                        {
                            this.outer.SetNextState(new SwordAerial
                            {
                                isTarget = false
                            });
                            return;

                        }

                    }
                    else
                    {
                        if (base.inputBank.moveVector == Vector3.zero)
                        {
                            //neutral attack
                            Chat.AddMessage("neutral attack");
                            this.outer.SetNextState(new SwordNeutral
                            {
                                swingIndex = currentSwingIndex,
                            });
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
                                        this.outer.SetNextState(new SwordForward
                                        {
                                            isTarget = true,
                                            Target = Target
                                        });
                                        return;

                                    }
                                    else
                                    {
                                        //neutral attack
                                        Chat.AddMessage("neutral attack");
                                        this.outer.SetNextState(new SwordNeutral
                                        {
                                            swingIndex = currentSwingIndex,
                                        });
                                        return;

                                    }
                                }
                                else
                                {
                                    this.outer.SetNextState(new SwordForward
                                    {
                                        isTarget = false
                                    });
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
                                this.outer.SetNextState(new SwordNeutral
                                {
                                    swingIndex = currentSwingIndex,
                                });
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

    }
}



