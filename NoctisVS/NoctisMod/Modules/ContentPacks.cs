﻿using RoR2.ContentManagement;

namespace NoctisMod.Modules
{
    internal class ContentPacks : IContentPackProvider
    {
        internal ContentPack contentPack = new ContentPack();
        public string identifier => NoctisPlugin.MODUID + "alt_CONTENT_PACK";

        public void Initialize()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        public System.Collections.IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            this.contentPack.identifier = this.identifier;
            contentPack.bodyPrefabs.Add(Prefabs.bodyPrefabs.ToArray());
            contentPack.buffDefs.Add(Buffs.buffDefs.ToArray());
            contentPack.effectDefs.Add(NoctisAssets.effectDefs.ToArray());
            contentPack.entityStateTypes.Add(States.entityStates.ToArray());
            contentPack.masterPrefabs.Add(Prefabs.masterPrefabs.ToArray());
            contentPack.networkSoundEventDefs.Add(NoctisAssets.networkSoundEventDefs.ToArray());
            contentPack.projectilePrefabs.Add(Prefabs.projectilePrefabs.ToArray());
            contentPack.skillDefs.Add(Skills.skillDefs.ToArray());
            contentPack.skillFamilies.Add(Skills.skillFamilies.ToArray());
            contentPack.survivorDefs.Add(Prefabs.survivorDefinitions.ToArray());
            contentPack.unlockableDefs.Add(Unlockables.unlockableDefs.ToArray());

            args.ReportProgress(1f);
            yield break;
        }

        public System.Collections.IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(this.contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public System.Collections.IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}