// Project:         BankingOverhaul mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2022 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    12/22/2021, 8:50 PM
// Last Edit:		12/23/2020, 11:50 PM
// Version:			1.00
// Special Thanks:  Lypyl, Hazelnut, TheLacus
// Modifier:

using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using System;
using Wenzil.Console;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop;

namespace BankingOverhaul
{
    [FullSerializer.fsObject("v1")]
    public class BOSaveData
    {
        public Dictionary<int, int[]> RegionRelationValues;
    }

    public class BankingOverhaulMain : MonoBehaviour, IHasModSaveData
    {
        static BankingOverhaulMain instance;

        public static BankingOverhaulMain Instance
        {
            get { return instance ?? (instance = FindObjectOfType<BankingOverhaulMain>()); }
        }

        static Mod mod;

        public static Dictionary<int, int[]> regionRelationValues = new Dictionary<int, int[]>()
        {
            { 0,  new int[] { 21, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 1,  new int[] { 17, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 5,  new int[] { 6, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 9,  new int[] { 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 11, new int[] { 19, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 16, new int[] { 11, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 17, new int[] { 2, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 18, new int[] { 3, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 19, new int[] { 1, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 20, new int[] { 21, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 21, new int[] { 4, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 22, new int[] { 15, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 23, new int[] { 12, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 26, new int[] { 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 32, new int[] { 4, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 33, new int[] { 10, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 34, new int[] { 9, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 35, new int[] { 8, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 36, new int[] { 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 37, new int[] { 7, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 38, new int[] { 5, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 39, new int[] { 5, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 40, new int[] { 4, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 41, new int[] { 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 42, new int[] { 3, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 43, new int[] { 24, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 44, new int[] { 23, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 45, new int[] { 23, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 46, new int[] { 22, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 47, new int[] { 20, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 48, new int[] { 19, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 49, new int[] { 16, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 50, new int[] { 14, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 51, new int[] { 15, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 52, new int[] { 14, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 53, new int[] { 17, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 54, new int[] { 18, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 55, new int[] { 21, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 56, new int[] { 21, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 57, new int[] { 13, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 58, new int[] { 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 59, new int[] { 4, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 60, new int[] { 3, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
        };

        public Type SaveDataType
        {
            get { return typeof(BOSaveData); }
        }

        public object NewSaveData()
        {
            return new BOSaveData
            {
                RegionRelationValues = regionRelationValues
            };
        }

        public object GetSaveData()
        {
            return new BOSaveData
            {
                RegionRelationValues = regionRelationValues
            };
        }

        public void RestoreSaveData(object saveData)
        {
            var BOSaveData = (BOSaveData)saveData;
            Debug.Log("Restoring save data");
            regionRelationValues = BOSaveData.RegionRelationValues;
            Debug.Log("Save data restored");
        }

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

            mod.SaveDataInterface = instance;
            if (regionRelationValues == null)
            {
                regionRelationValues = new Dictionary<int, int[]>();
            }

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

            StartGameBehaviour.OnStartGame += GenerateRegionRelationValues_OnStartGame;
            SaveLoadManager.OnLoad += GenerateRegionRelationValues_OnLoad;
            WorldTime.OnNewMonth += RegenRegionRelationValues_OnNewMonth;

            RegisterJACommands();

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

        void GenerateRegionRelationValues_OnStartGame(object Sender, EventArgs e)
        {
            for (int m = 0; m < 61; m++)
            {
                if (!regionRelationValues.ContainsKey(m))
                    continue;
                int[] currentRegionValues = regionRelationValues[m];
                for (int i = 0; i < 12; i++)
                {
                    if (i == 0 || i == 1) // Keep the "Distance" and "Primary Deity" values static
                        continue;
                    else // Randomize the other 10 "opinion" values
                    {
                        currentRegionValues[i] = UnityEngine.Random.Range(1, 11); // Generates number between 1 and 10... I think.
                    }
                }

                regionRelationValues[m] = currentRegionValues;
            }
            Debug.Log("Banking Overhaul just finished generating region relation values for a new game!");
        }

        void GenerateRegionRelationValues_OnLoad(SaveData_v1 saveData) // Do this upon loading a save without any data, so it's retroactive on existing saves.
        {
            int[] currentRegionValues = regionRelationValues[0];
            if (currentRegionValues[5] == 0)
            {
                for (int m = 0; m < 61; m++)
                {
                    if (!regionRelationValues.ContainsKey(m))
                        continue;
                    currentRegionValues = regionRelationValues[m];
                    for (int i = 0; i < 12; i++)
                    {
                        if (i == 0 || i == 1) // Keep the "Distance" and "Primary Deity" values static
                            continue;
                        else // Randomize the other 10 "opinion" values
                        {
                            currentRegionValues[i] = UnityEngine.Random.Range(1, 11); // Generates number between 1 and 10... I think.
                        }
                    }

                    regionRelationValues[m] = currentRegionValues;
                }
                Debug.Log("Banking Overhaul finished generating region relation values for an already in progress game with no previous mod save data.");
            }
            else
                Debug.Log("Banking Overhaul did not generate new region relation values, as this save already has mod save data.");
        }

        void RegenRegionRelationValues_OnNewMonth()
        {
            if (!(DaggerfallUnity.Instance.WorldTime.Now.Month % 2 == 0)) // Only regen values on even months of the year, so every 2-months.
            { Debug.Log("New Month is an odd value, so don't regen region relation values."); return; }

            for (int m = 0; m < 61; m++)
            {
                if (!regionRelationValues.ContainsKey(m))
                    continue;
                int[] currentRegionValues = regionRelationValues[m];
                for (int i = 0; i < 12; i++)
                {
                    if (i == 0 || i == 1) // Keep the "Distance" and "Primary Deity" values static
                        continue;
                    else // Randomize the other 10 "opinion" values
                    {
                        currentRegionValues[i] += UnityEngine.Random.Range(-3, 4); // Generates number between -3 and 3
                        currentRegionValues[i] = Mathf.Clamp(currentRegionValues[i], 1, 10); // Prevents value from going below 1 or above 10
                    }
                }

                regionRelationValues[m] = currentRegionValues;
            }
            Debug.Log("Two Months have passed, Banking Overhaul region relation values have been scrambled slightly!");
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

        public static float DetermineLoCValueMod(int relationValue)
        {
            switch(relationValue)
            {
                case -2:
                    return 0.25f; // Hate
                case -1:
                    return 0.50f; // Dislike
                case 0:
                    return 0.75f; // Indifferent
                case 1:
                    return 0.90f; // Like
                case 2:
                    return 1f;    // Love
            }

            return 1f;
        }

        public static int CompareRegionValues(int[] regionIndex, int[] locIndex, int region, int region2)
        {
            if (regionIndex.Length < 4 || locIndex.Length < 4) // Precaution in-case dictionary key does not return any filled array.
                return 0;

            bool check = false;
            int holderValue = 0;
            int finalResult = 0;
            int differencePoints = 0;
            int distanceDifference = 0;
            int opinionDifference = 0;

            for (int i = 0; i < 12; i++)
            {
                if (i == 0) // Distance value
                {
                    distanceDifference = regionIndex[0] - locIndex[0];
                    if (distanceDifference < 0)
                        distanceDifference *= -1;
                }
                else if (i == 1) // Primary Deity value
                {
                    if (regionIndex[1] != locIndex[1])
                        differencePoints += 15;
                }
                else if (i > 1) // "Opinion" values from 1-10
                {
                    opinionDifference = regionIndex[i] - locIndex[i];
                    if (opinionDifference < 0)
                        opinionDifference *= -1;
                    differencePoints += opinionDifference;
                }
            }

            if (differencePoints <= 35)
                finalResult = 2;
            else if (differencePoints <= 50)
                finalResult = 1;
            else if (differencePoints <= 65)
                finalResult = -1;
            else if (differencePoints > 65)
                finalResult = -2;

            if (distanceDifference >= 9 && !(differencePoints <= 30) && !(differencePoints >= 70))
                finalResult = 0;

            holderValue = SpecificReigonExceptions(region, region2, distanceDifference, out check);

            if (check)
                finalResult = holderValue;

            return finalResult;
        }

        public static int SpecificReigonExceptions (int regionIndex, int locIndex, int distance, out bool check)
        {
            check = false;

            if (regionIndex == (int)DaggerfallRegions.IsleOfBalfiera | locIndex == (int)DaggerfallRegions.IsleOfBalfiera) { check = true; return 0; } // Isle of Balfiera

            if ((regionIndex == (int)DaggerfallRegions.Sentinel | locIndex == (int)DaggerfallRegions.Sentinel) && (regionIndex == (int)DaggerfallRegions.Totambu | locIndex == (int)DaggerfallRegions.Totambu)) { check = true; return -1; } // Sentinel & Totambu

            if (regionIndex == (int)DaggerfallRegions.OrsiniumArea | locIndex == (int)DaggerfallRegions.OrsiniumArea) // Orsinium Area
            {
                if (regionIndex == (int)DaggerfallRegions.WrothgarianMountains | locIndex == (int)DaggerfallRegions.WrothgarianMountains) { check = true; return 1; }
                if (regionIndex == (int)DaggerfallRegions.Daggerfall | locIndex == (int)DaggerfallRegions.Daggerfall) { check = true; return 1; }
                if (regionIndex == (int)DaggerfallRegions.Koegria | locIndex == (int)DaggerfallRegions.Koegria) { check = true; return -2; }
                if (regionIndex == (int)DaggerfallRegions.Phrygias | locIndex == (int)DaggerfallRegions.Phrygias) { check = true; return -2; }
                if (regionIndex == (int)DaggerfallRegions.Gavaudon | locIndex == (int)DaggerfallRegions.Gavaudon) { check = true; return -2; }
                if (distance >= 6) { check = true; return 0; }

                check = true; return -1;
            }

            if (regionIndex == (int)DaggerfallRegions.Daggerfall | locIndex == (int)DaggerfallRegions.Daggerfall) // Daggerfall
            {
                if (regionIndex == (int)DaggerfallRegions.Wayrest | locIndex == (int)DaggerfallRegions.Wayrest) { check = true; return -1; } // Wayrest
                if (regionIndex == (int)DaggerfallRegions.Betony | locIndex == (int)DaggerfallRegions.Betony) { check = true; return 2; }  // Betony
            }

            if (regionIndex == (int)DaggerfallRegions.Dwynnen | locIndex == (int)DaggerfallRegions.Dwynnen) // Dwynnen
            {
                if (regionIndex == (int)DaggerfallRegions.Kambria | locIndex == (int)DaggerfallRegions.Kambria) { check = true; return 2; }
                if (regionIndex == (int)DaggerfallRegions.Phrygias | locIndex == (int)DaggerfallRegions.Phrygias) { check = true; return 2; }
                if (regionIndex == (int)DaggerfallRegions.Ykalon | locIndex == (int)DaggerfallRegions.Ykalon) { check = true; return 2; }
            }
            if (regionIndex == (int)DaggerfallRegions.Kambria | locIndex == (int)DaggerfallRegions.Kambria) // Kambria
            {
                if (regionIndex == (int)DaggerfallRegions.Dwynnen | locIndex == (int)DaggerfallRegions.Dwynnen) { check = true; return 2; }
                if (regionIndex == (int)DaggerfallRegions.Phrygias | locIndex == (int)DaggerfallRegions.Phrygias) { check = true; return 2; }
                if (regionIndex == (int)DaggerfallRegions.Ykalon | locIndex == (int)DaggerfallRegions.Ykalon) { check = true; return 2; }
            }
            if (regionIndex == (int)DaggerfallRegions.Phrygias | locIndex == (int)DaggerfallRegions.Phrygias) // Phrygias
            {
                if (regionIndex == (int)DaggerfallRegions.Dwynnen | locIndex == (int)DaggerfallRegions.Dwynnen) { check = true; return 2; }
                if (regionIndex == (int)DaggerfallRegions.Kambria | locIndex == (int)DaggerfallRegions.Kambria) { check = true; return 2; }
                if (regionIndex == (int)DaggerfallRegions.Ykalon | locIndex == (int)DaggerfallRegions.Ykalon) { check = true; return 2; }
            }
            if (regionIndex == (int)DaggerfallRegions.Ykalon | locIndex == (int)DaggerfallRegions.Ykalon) // Ykalon
            {
                if (regionIndex == (int)DaggerfallRegions.Dwynnen | locIndex == (int)DaggerfallRegions.Dwynnen) { check = true; return 2; }
                if (regionIndex == (int)DaggerfallRegions.Kambria | locIndex == (int)DaggerfallRegions.Kambria) { check = true; return 2; }
                if (regionIndex == (int)DaggerfallRegions.Phrygias | locIndex == (int)DaggerfallRegions.Phrygias) { check = true; return 2; }
            }

            return 0;
        }

        public static void RegisterJACommands()
        {
            Debug.Log("[JewelryAdditions] Trying to register console commands.");
            try
            {
                ConsoleCommandsDatabase.RegisterCommand(ShowBORegionWindow.command, ShowBORegionWindow.description, ShowBORegionWindow.usage, ShowBORegionWindow.Execute);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Error Registering JewelryAdditions Console commands: {0}", e.Message));
            }
        }

        private static class ShowBORegionWindow
        {
            public static readonly string command = "showboregionwindow";
            public static readonly string description = "Shows the custom Banking Overhaul Region Map Window.)";
            public static readonly string usage = "showboregionwindow";

            public static string Execute(params string[] args)
            {
                BORegionsWindow boRegionMapWindow;

                boRegionMapWindow = new BORegionsWindow(DaggerfallUI.UIManager);
                DaggerfallUI.UIManager.PushWindow(boRegionMapWindow);
                return "Complete";
            }
        }
    }
}
