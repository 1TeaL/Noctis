using NoctisMod.Modules.Survivors;
using EntityStates;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using NoctisMod.Modules;
using UnityEngine.Networking;
using RoR2.ExpansionManagement;
using ExtraSkillSlots;
using R2API.Networking;
using System;
using System.ComponentModel;

namespace NoctisMod.SkillStates
{
    public class Freeze : BaseSkillState
    {
        Animator animator;
        private float duration = 1f;
        Vector3 startPos;

        public override void OnEnter()
        {
            base.OnEnter();

            animator = base.GetModelAnimator();
            if (animator)
            {
                animator.enabled= false;
            }
            attackSpeedStat = 0f;
            startPos = characterBody.corePosition;


            if (base.characterDirection)
            {
                base.characterDirection.moveVector = base.characterDirection.forward;
            }
            if (base.characterMotor)
            {
                base.characterMotor.velocity = Vector3.zero;
                base.characterMotor.rootMotion = Vector3.zero;
            }
            if (base.rigidbody != null)
            {
                //RigidbodyMotor rigidBodyMotor = base.gameObject.GetComponent<RigidbodyMotor>();
                //rigidBodyMotor.moveVector = Vector3.zero;
                //rigidBodyMotor.rootMotion = Vector3.zero;

                base.rigidbody.velocity = Vector3.zero;

            }
            SetPosition(startPos, characterBody);


        }

        private void SetPosition(Vector3 newPosition, CharacterBody charBody)
        {
            if (charBody.characterMotor)
            {
                charBody.characterMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity, true);
            }
            else if (charBody.rigidbody)
            {
                charBody.rigidbody.MovePosition(newPosition);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (animator)
            {
                animator.enabled = true;
            }
            attackSpeedStat = 1f;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            attackSpeedStat = 0f;


            if (base.characterDirection)
            {
                base.characterDirection.moveVector = Vector3.zero;
            }
            if (base.characterMotor)
            {
                base.characterMotor.velocity = Vector3.zero;
                base.characterMotor.rootMotion = Vector3.zero;
            }
            if (base.rigidbody != null)
            {
                //RigidbodyMotor rigidBodyMotor = base.gameObject.GetComponent<RigidbodyMotor>();
                //rigidBodyMotor.moveVector = Vector3.zero;
                //rigidBodyMotor.rootMotion = Vector3.zero;

                base.rigidbody.velocity = Vector3.zero;

            }
            SetPosition(startPos, characterBody);

            if (base.fixedAge > duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }

    }
}