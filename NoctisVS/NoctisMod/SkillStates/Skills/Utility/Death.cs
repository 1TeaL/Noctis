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
using R2API.Networking;
using NoctisMod.Modules;
using HG;
using NoctisMod.Modules.Networking;
using R2API.Networking.Interfaces;

namespace NoctisMod.SkillStates
{

    internal class Death : GenericCharacterDeath
    {
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("FullBody, Override", "Death", "Attack.playbackRate", 1000f);
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }

    
}



