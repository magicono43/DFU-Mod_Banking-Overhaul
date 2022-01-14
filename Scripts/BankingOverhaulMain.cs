// Project:         BankingOverhaul mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2022 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    12/22/2021, 8:50 PM
// Last Edit:		12/23/2020, 11:50 PM
// Version:			1.00
// Special Thanks:  Lypyl, Hazelnut
// Modifier:

using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using System;

namespace BankingOverhaul
{
    public class BankingOverhaulMain : MonoBehaviour
    {
        static BankingOverhaulMain instance;

        public static BankingOverhaulMain Instance
        {
            get { return instance ?? (instance = FindObjectOfType<BankingOverhaulMain>()); }
        }

        static Mod mod;

        public static int RequiredRecoveryHours { get; set; }
        public static int NonMemberCostMultiplier { get; set; }
        public static float FinalTrainingCostMultiplier { get; set; }
        public static int MaxTrainAwful { get; set; }
        public static int MaxTrainPoor { get; set; }
        public static int MaxTrainDecent { get; set; }
        public static int MaxTrainGood { get; set; }
        public static int MaxTrainGreat { get; set; }
        public static float FinalTrainedAmountMultiplier { get; set; }
        public static int HoursPassedDuringTraining { get; set; }
        public static bool AllowHealthMagicDamage { get; set; }
        public static int MaximumPossibleTraining { get; set; }

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            instance = new GameObject("BankingOverhaul").AddComponent<BankingOverhaulMain>(); // Add script to the scene.

            mod.LoadSettingsCallback = LoadSettings; // To enable use of the "live settings changes" feature in-game.

            mod.IsReady = true;
        }

        private void Start()
        {
            Debug.Log("Begin mod init: Banking Overhaul");

            mod.LoadSettings();

            DaggerfallWorkshop.Game.Formulas.FormulaHelper.RegisterOverride(mod, "CalculateMaxBankLoan", (Func<int>)CalculateMaxBankLoan);
            DaggerfallWorkshop.Game.Formulas.FormulaHelper.RegisterOverride(mod, "CalculateBankLoanRepayment", (Func<int, int, int>)CalculateBankLoanRepayment);

            UIWindowFactory.RegisterCustomUIWindow(UIWindowType.Banking, typeof(BankingOverhaulWindow));

            Debug.Log("Finished mod init: Banking Overhaul");
        }

        #region Settings

        static void LoadSettings(ModSettings modSettings, ModSettingsChange change)
        {
            RequiredRecoveryHours = mod.GetSettings().GetValue<int>("TimeRelated", "HoursNeededBetweenSessions");
            NonMemberCostMultiplier = mod.GetSettings().GetValue<int>("GoldCost", "Non-MemberCostMultiplier");
            FinalTrainingCostMultiplier = mod.GetSettings().GetValue<float>("GoldCost", "FinalCostMultiplier");
            MaxTrainAwful = mod.GetSettings().GetValue<int>("MaxSkillsBasedOnQuality", "MaxAwfulHallsCanTrain");
            MaxTrainPoor = mod.GetSettings().GetValue<int>("MaxSkillsBasedOnQuality", "MaxPoorHallsCanTrain");
            MaxTrainDecent = mod.GetSettings().GetValue<int>("MaxSkillsBasedOnQuality", "MaxDecentHallsCanTrain");
            MaxTrainGood = mod.GetSettings().GetValue<int>("MaxSkillsBasedOnQuality", "MaxGoodHallsCanTrain");
            MaxTrainGreat = mod.GetSettings().GetValue<int>("MaxSkillsBasedOnQuality", "MaxGreatHallsCanTrain");
            FinalTrainedAmountMultiplier = mod.GetSettings().GetValue<float>("TrainingExperience", "TrainedXPMultiplier");
            HoursPassedDuringTraining = mod.GetSettings().GetValue<int>("TimeRelated", "HoursPassedDuringSessions");
            AllowHealthMagicDamage = mod.GetSettings().GetValue<bool>("VitalsRelated", "AllowHealthMagicDamage");
            MaximumPossibleTraining = mod.GetSettings().GetValue<int>("MaxSkillsCanBeTrain", "MaxPossibleTraining");
        }

        #endregion

        public static int GetReqRecovHours()
        {
            return RequiredRecoveryHours;
        }

        public static int GetNonMembMulti()
        {
            return NonMemberCostMultiplier;
        }

        public static float GetFinalTrainCostMulti()
        {
            return FinalTrainingCostMultiplier;
        }

        public static int GetMaxTrainAwful()
        {
            return MaxTrainAwful;
        }

        public static int GetMaxTrainPoor()
        {
            return MaxTrainPoor;
        }

        public static int GetMaxTrainDecent()
        {
            return MaxTrainDecent;
        }

        public static int GetMaxTrainGood()
        {
            return MaxTrainGood;
        }

        public static int GetMaxTrainGreat()
        {
            return MaxTrainGreat;
        }

        public static float GetFinalTrainedAmountMulti()
        {
            return FinalTrainedAmountMultiplier;
        }

        public static int GetHoursPassedTraining()
        {
            return HoursPassedDuringTraining;
        }

        public static bool GetAllowHPMPDamage()
        {
            return AllowHealthMagicDamage;
        }

        public static int GetMaxPossibleTrain()
        {
            return MaximumPossibleTraining;
        }

        public static int CalculateMaxBankLoan()
        {
            int regionIndex = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
            DaggerfallConnect.Arena2.FactionFile.FactionData factionData;
            GameManager.Instance.PlayerEntity.FactionData.GetRegionFaction(regionIndex, out factionData);
            int regionRep = factionData.rep;
            int legalRep = GameManager.Instance.PlayerEntity.RegionData[regionIndex].LegalRep;
            int personality = GameManager.Instance.PlayerEntity.Stats.LivePersonality - 50;
            int baseLoan = 40000 + (personality * 500);
            int regionLoan = 0;
            int legalLoan = 0;

            if (regionRep > 0)
                regionLoan = regionRep * (3000 + (personality * 30));
            else if (regionRep < 0)
                regionLoan = regionRep * (1000 + (personality * -10));

            if (legalRep > 0)
                legalLoan = legalRep * (1000 + (personality * 5));
            else if (legalRep < 0)
                legalLoan = legalRep * (2000 + (personality * -30));

            return baseLoan + regionLoan + legalLoan;
        }

        public static int CalculateBankLoanRepayment(int amount, int regionIndex) // 5, 11, 18, 26
        {
            DaggerfallConnect.Arena2.FactionFile.FactionData factionData;
            GameManager.Instance.PlayerEntity.FactionData.GetRegionFaction(regionIndex, out factionData);
            int regionRep = factionData.rep;
            int legalRep = GameManager.Instance.PlayerEntity.RegionData[regionIndex].LegalRep;
            int personality = GameManager.Instance.PlayerEntity.Stats.LivePersonality - 50;
            float regionMod = 0f;
            float legalMod = 0f;
            float combinedMod = 1f;

            if (regionRep > 0)
                regionMod = regionRep * (-0.01f * ((personality * 0.005f) + 1));
            else if (regionRep < 0)
                regionMod = regionRep * (-0.005f * ((personality * -0.005f) + 1));

            if (legalRep > 0)
                legalMod = legalRep * (-0.0025f * ((personality * 0.005f) + 1));
            else if (legalRep < 0)
                legalMod = legalRep * (-0.01f * ((personality * -0.005f) + 1));

            if (regionMod + legalMod < 0)
                combinedMod = (regionMod + legalMod + 1) * -1;
            else if (regionMod + legalMod > 0)
                combinedMod = (regionMod + legalMod + 1);

            return (int)(amount + amount * (0.1f * combinedMod));
        }
    }
}
