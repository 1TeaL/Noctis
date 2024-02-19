using EntityStates;
using RoR2;

namespace NoctisMod.SkillStates
{
    internal class NoctisCharacterMain : GenericCharacterMain
    {
        private EntityStateMachine bodyStateMachine;

        //unused
        //Ripped from mage lmao
        public override void OnEnter()
        {
            base.OnEnter();
            this.bodyStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
        }

        public override void ProcessJump()
        {
            float jumpCountBefore = characterMotor.jumpCount;
            base.ProcessJump();
            float jumpCountAfter = characterMotor.jumpCount;
            if(jumpCountAfter == jumpCountBefore)
            {
                if (this.hasCharacterMotor && this.hasInputBank && base.isAuthority)
                {
                    if (base.inputBank.jump.justPressed)
                    {
                        this.bodyStateMachine.SetNextState(new Dodge());
                    }
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
