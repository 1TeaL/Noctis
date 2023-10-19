using System;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NoctisMod.Modules.Survivors
{

    public class EnergySystem : MonoBehaviour
    {
        public CharacterBody characterBody;

        //UI Energymeter
        public GameObject CustomUIObject;
        public RectTransform manaMeter;
        public RectTransform manaMeterGlowRect;
        public Image manaMeterGlowBackground;
        public HGTextMeshProUGUI plusChaosNumber;
        //public HGTextMeshProUGUI quirkGetUI;
        private bool informAFOToPlayers;
        public string quirkGetString;
        public float quirkGetStopwatch;


        //Energy system
        public float maxMana;
        public float currentMana;
        public float regenMana;
        public float costmultiplierplusChaos;
        public float costflatplusChaos;
        //public float plusChaosDecayTimer;
        public bool SetActiveTrue;
        //bools to stop energy regen after skill used
        private bool ifEnergyUsed;
        private float energyDecayTimer;
        private bool ifEnergyRegenAllowed;

        //Energy bar glow
        private enum GlowState
        {
            STOP,
            FLASH,
            DECAY
        }
        private float decayConst;
        private float flashConst;
        private float glowStopwatch;
        private Color targetColor;
        private Color originalColor;
        private Color currentColor;
        private GlowState state;

        public void Awake()
        {
            characterBody = gameObject.GetComponent<CharacterBody>();
        }

        public void Start()
        {
            //Energy
            maxMana = StaticValues.basePlusChaos + ((characterBody.level - 1) * StaticValues.levelPlusChaos);
            currentMana = maxMana;
            regenMana = maxMana * StaticValues.regenManaFraction;
            costmultiplierplusChaos = 1f;
            costflatplusChaos = 0f;
            ifEnergyRegenAllowed = true;
            ifEnergyUsed = false;

            //UI objects 
            CustomUIObject = UnityEngine.Object.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("noctisCustomUI"));
            CustomUIObject.SetActive(false);
            SetActiveTrue = false;

            manaMeter = CustomUIObject.transform.GetChild(0).GetComponent<RectTransform>();
            manaMeterGlowBackground = CustomUIObject.transform.GetChild(1).GetComponent<Image>();
            manaMeterGlowRect = CustomUIObject.transform.GetChild(1).GetComponent<RectTransform>();

            //setup the UI element for the min/max
            plusChaosNumber = this.CreateLabel(CustomUIObject.transform, "plusChaosNumber", $"{(int)currentMana} / {maxMana}", new Vector2(0, -110), 24f, new Color(0.92f, 0.12f, 0.8f));

            //ui element for information below the energy
            //quirkGetUI = this.CreateLabel(CustomUIObject.transform, "quirkGetString", quirkGetString, new Vector2(0, -220), 24f, Color.white);
            //quirkGetUI.SetText(quirkGetString);
            //quirkGetUI.enabled = true;


            // Start timer on 1f to turn off the timer.
            state = GlowState.STOP;
            decayConst = 1f;
            flashConst = 1f;
            glowStopwatch = 1f;
            originalColor = new Color(1f, 1f, 1f, 0f);
            targetColor = new Color(1f, 1f, 1f, 1f);
            currentColor = originalColor;

        }


        //Creates the label.
        private HGTextMeshProUGUI CreateLabel(Transform parent, string name, string text, Vector2 position, float textScale, Color color)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.parent = parent;
            gameObject.AddComponent<CanvasRenderer>();
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            HGTextMeshProUGUI hgtextMeshProUGUI = gameObject.AddComponent<HGTextMeshProUGUI>();
            hgtextMeshProUGUI.enabled = true;
            hgtextMeshProUGUI.text = text;
            hgtextMeshProUGUI.fontSize = textScale;
            hgtextMeshProUGUI.color = color;
            hgtextMeshProUGUI.alignment = TextAlignmentOptions.Center;
            hgtextMeshProUGUI.enableWordWrapping = false;
            rectTransform.localPosition = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = position;
            return hgtextMeshProUGUI;

        }

        private void CalculateEnergyStats()
        {
            //Energy updates
            if (characterBody)
            {
                maxMana = StaticValues.basePlusChaos + ((characterBody.level - 1) * StaticValues.levelPlusChaos)
                    + (StaticValues.backupGain * characterBody.master.inventory.GetItemCount(RoR2Content.Items.SecondarySkillMagazine))
                    + (StaticValues.afterburnerGain * characterBody.master.inventory.GetItemCount(RoR2Content.Items.UtilitySkillMagazine))
                    + (StaticValues.lysateGain * characterBody.master.inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid));

                regenMana = maxMana * StaticValues.regenManaFraction;

                costmultiplierplusChaos = (float)Math.Pow(0.75f, characterBody.master.inventory.GetItemCount(RoR2Content.Items.AlienHead));
                costflatplusChaos = (StaticValues.costFlatPlusChaosSpend * characterBody.master.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck));

                if (costmultiplierplusChaos > 1f)
                {
                    costmultiplierplusChaos = 1f;
                }
            }


            //plusChaos Currently have

            //allow regen
            if (ifEnergyUsed)
            {
                if (energyDecayTimer > 1f)
                {
                    energyDecayTimer = 0f;
                    ifEnergyRegenAllowed = true;
                    ifEnergyUsed = false;
                }
                else
                {
                    ifEnergyRegenAllowed = false;
                    energyDecayTimer += Time.fixedDeltaTime;
                }
            }
            if(ifEnergyRegenAllowed)
            {
                currentMana += regenMana * Time.fixedDeltaTime;
            }


            if (currentMana > maxMana)
            {
                currentMana = maxMana;
            }
            if (currentMana < 0f)
            {
                currentMana = 0f;
            }

            if (plusChaosNumber)
            {
                plusChaosNumber.SetText($"{(int)currentMana} / {maxMana}");
            }

            if (manaMeter)
            {
                // 2f because meter is too small probably.
                // Logarithmically scale.
                float logVal = Mathf.Log10(((maxMana / StaticValues.basePlusChaos) * 10f) + 1) * (currentMana / maxMana);
                manaMeter.localScale = new Vector3(2.0f * logVal, 0.05f, 1f);
                manaMeterGlowRect.localScale = new Vector3(2.3f * logVal, 0.1f, 1f);
            }

            //Chat.AddMessage($"{currentMana}/{maxMana}");
        }

        public void FixedUpdate()
        {
            if (characterBody.hasEffectiveAuthority)
            {
                CalculateEnergyStats();
            }

            if (characterBody.hasEffectiveAuthority && !SetActiveTrue)
            {
                CustomUIObject.SetActive(true);
                SetActiveTrue = true;
            }
        }

        public void Update()
        {
            //Debug.Log(quirkGetString+ "quirkgetstring");


            if (state != GlowState.STOP)
            {
                glowStopwatch += Time.deltaTime;
                float lerpFraction;
                switch (state)
                {
                    // Lerp to target color
                    case GlowState.FLASH:

                        lerpFraction = glowStopwatch / flashConst;
                        currentColor = Color.Lerp(originalColor, targetColor, lerpFraction);

                        if (glowStopwatch > flashConst)
                        {
                            state = GlowState.DECAY;
                            glowStopwatch = 0f;
                        }
                        break;

                    //Lerp back to original color;
                    case GlowState.DECAY:
                        //Linearlly lerp.
                        lerpFraction = glowStopwatch / decayConst;
                        currentColor = Color.Lerp(targetColor, originalColor, lerpFraction);

                        if (glowStopwatch > decayConst)
                        {
                            state = GlowState.STOP;
                            glowStopwatch = 0f;
                        }
                        break;
                    case GlowState.STOP:
                        //State does nothing.
                        break;
                }
            }

            manaMeterGlowBackground.color = currentColor;
        }


        public void SpendplusChaos(float plusChaos)
        {
            //float plusChaosflatCost = plusChaos - costflatplusChaos;
            //if (plusChaosflatCost < 0f) plusChaosflatCost = 0f;

            //float plusChaosCost = rageplusChaosCost * costmultiplierplusChaos * plusChaosflatCost;
            //if (plusChaosCost < 0f) plusChaosCost = 0f;

            currentMana -= plusChaos;
            TriggerGlow(0.3f, 0.3f, Color.magenta);
            ifEnergyUsed = true;

        }
        public void GainplusChaos(float plusChaos)
        {
            //float plusChaosflatCost = plusChaos - costflatplusChaos;
            //if (plusChaosflatCost < 0f) plusChaosflatCost = 0f;

            //float plusChaosCost = rageplusChaosCost * costmultiplierplusChaos * plusChaosflatCost;
            //if (plusChaosCost < 0f) plusChaosCost = 0f;

            currentMana += plusChaos;
            TriggerGlow(0.3f, 0.3f, Color.cyan);

        }

        public void TriggerGlow(float newDecayTimer, float newFlashTimer, Color newStartingColor)
        {
            decayConst = newDecayTimer;
            flashConst = newFlashTimer;
            originalColor = new Color(newStartingColor.r, newStartingColor.g, newStartingColor.b, 0f);
            targetColor = newStartingColor;
            glowStopwatch = 0f;
            state = GlowState.FLASH;
        }


        public void OnDestroy()
        {
            Destroy(CustomUIObject);
        }
    }
}

