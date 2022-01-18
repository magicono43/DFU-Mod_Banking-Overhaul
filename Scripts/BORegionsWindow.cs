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
using System;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using BankingOverhaul;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements Banking Overhaul's Region Map.
    /// </summary>
    public class BORegionsWindow : DaggerfallPopupWindow
    {
        #region Fields

        protected const int betonyIndex = 19;

        protected const string overworldImgName                       = "TRAV0I00.IMG";
        protected const string regionPickerImgName                    = "TRAV0I01.IMG";
        protected const string findAtButtonImgName                    = "TRAV0I03.IMG";
        protected const string locationFilterButtonEnabledImgName     = "TRAV01I0.IMG";
        protected const string locationFilterButtonDisabledImgName    = "TRAV01I1.IMG";
        protected const string downArrowImgName                       = "TRAVAI05.IMG";
        protected const string upArrowImgName                         = "TRAVBI05.IMG";
        protected const string rightArrowImgName                      = "TRAVCI05.IMG";
        protected const string leftArrowImgName                       = "TRAVDI05.IMG";
        protected const string regionBorderImgName                    = "MBRD00I0.IMG";
        protected const string colorPaletteColName                    = "FMAP_PAL.COL";
        protected const int regionPanelOffset                         = 12;
        protected const int identifyFlashCount                        = 4;
        protected const int identifyFlashCountSelected                = 2;
        protected const float identifyFlashInterval                   = 0.5f;
        protected const int dotsOutlineThickness                      = 1;
        protected Color32 dotOutlineColor                             = new Color32(0, 0, 0, 128);
        protected Vector2[] outlineDisplacements =
        {
            new Vector2(-0.5f, -0f),
            new Vector2(0f, -0.5f),
            new Vector2(0f, 0.5f),
            new Vector2(0.5f, 0f)
        };

        protected DaggerfallTravelPopUp popUp;

        protected Dictionary<string, Vector2> offsetLookup = new Dictionary<string, Vector2>();
        protected string[] selectedRegionMapNames;

        protected DFBitmap regionPickerBitmap;
        protected DFRegion currentDFRegion;
        protected int currentDFRegionIndex = -1;
        protected int lastQueryLocationIndex = -1;
        protected string lastQueryLocationName;
        protected ContentReader.MapSummary locationSummary;

        protected KeyCode toggleClosedBinding;

        protected Panel borderPanel;
        protected Panel regionTextureOverlayPanel;
        protected Panel[] regionLocationDotsOutlinesOverlayPanel;
        protected Panel regionLocationDotsOverlayPanel;
        protected Panel playerRegionOverlayPanel;
        protected Panel identifyOverlayPanel;

        protected TextLabel regionLabel;

        protected Texture2D overworldTexture;
        protected Texture2D identifyTexture;
        protected Texture2D locationDotsTexture;
        protected Texture2D locationDotsOutlineTexture;
        protected Texture2D findButtonTexture;
        protected Texture2D atButtonTexture;
        protected Texture2D dungeonFilterButtonEnabled;
        protected Texture2D dungeonFilterButtonDisabled;
        protected Texture2D templesFilterButtonEnabled;
        protected Texture2D templesFilterButtonDisabled;
        protected Texture2D homesFilterButtonEnabled;
        protected Texture2D homesFilterButtonDisabled;
        protected Texture2D townsFilterButtonEnabled;
        protected Texture2D townsFilterButtonDisabled;
        protected Texture2D upArrowTexture;
        protected Texture2D downArrowTexture;
        protected Texture2D leftArrowTexture;
        protected Texture2D rightArrowTexture;
        protected Texture2D borderTexture;

        protected Button exitButton;

        protected Rect playerRegionOverlayPanelRect   = new Rect(0, 0, 320, 200);
        protected Rect regionTextureOverlayPanelRect  = new Rect(0, regionPanelOffset, 320, 160);
        protected Rect dungeonsFilterButtonSrcRect    = new Rect(0, 0, 99, 11);
        protected Rect templesFilterButtonSrcRect     = new Rect(0, 11, 99, 11);
        protected Rect homesFilterButtonSrcRect       = new Rect(99, 0, 80, 11);
        protected Rect townsFilterButtonSrcRect       = new Rect(99, 11, 80, 11);
        protected Rect findButtonRect                 = new Rect(0, 0, 45, 11);
        protected Rect atButtonRect                   = new Rect(0, 11, 45, 11);

        protected Color32[] identifyPixelBuffer;
        protected Color32[] locationDotsPixelBuffer;
        protected Color32[] locationDotsOutlinePixelBuffer;
        protected Color32[] locationPixelColors;              // Pixel colors for different location types
        protected Color currentRegionColor;
        protected Color loveColor;
        protected Color likeColor;
        protected Color indifferentColor;
        protected Color dislikeColor;
        protected Color hateColor;

        protected int zoomfactor                  = 2;
        protected int mouseOverRegion             = -1;
        protected int selectedRegion              = -1;
        protected int mapIndex                    = 0;        // Current index of loaded map from selectedRegionMapNames
        protected float scale                     = 1.0f;
        protected float identifyLastChangeTime    = 0;
        protected float identifyChanges           = 0;

        protected bool identifyState          = false;
        protected bool identifying            = false;

        protected bool filterDungeons = false;
        protected bool filterTemples = false;
        protected bool filterHomes = false;
        protected bool filterTowns = false;

        protected Vector2 lastMousePos = Vector2.zero;
        protected Vector2 zoomOffset = Vector2.zero;
        protected Vector2 zoomPosition = Vector2.zero;

        protected readonly Dictionary<string, Texture2D> regionTextures = new Dictionary<string, Texture2D>();
        protected readonly Dictionary<int, Texture2D> importedOverlays = new Dictionary<int, Texture2D>();

        protected readonly int maxMatchingResults = 1000;
        protected string distanceRegionName = null;
        protected IDistance distance;

        #endregion

        #region Properties

        protected string RegionImgName { get; set; }

        protected bool RegionSelected
        {
            get { return selectedRegion != -1; }
        }

        protected bool MouseOverRegion
        {
            get { return mouseOverRegion != -1; }
        }

        public ContentReader.MapSummary LocationSummary { get => locationSummary; }

        #endregion

        #region Constructors

        public BORegionsWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        #endregion

        #region User Interface

        protected override void Setup()
        {
            ParentPanel.BackgroundColor = Color.black;

            // Set location pixel colors and identify flash color from palette file
            DFPalette colors = new DFPalette();
            if (!colors.Load(Path.Combine(DaggerfallUnity.Instance.Arena2Path, colorPaletteColName)))
                throw new Exception("DaggerfallTravelMap: Could not load color palette.");

            locationPixelColors = new Color32[]
            {
                new Color32(colors.GetRed(237), colors.GetGreen(237), colors.GetBlue(237), 255),  //dunglab (R215, G119, B39)
                new Color32(colors.GetRed(35), colors.GetGreen(35), colors.GetBlue(35), 255),     //hamlet (R188, G138, B138)
                new Color32(colors.GetRed(37), colors.GetGreen(37), colors.GetBlue(37), 255),     //village (R155, G105, B106)
            };

            currentRegionColor = new Color32(0xF0, 0xF1, 0x2F, 160);
            loveColor = new Color32(0x13, 0xFD, 0x42, 160);
            likeColor = new Color32(0x2B, 0x8B, 0x3E, 160);
            indifferentColor = new Color32(0x92, 0x95, 0x93, 160);
            dislikeColor = new Color32(0xCC, 0x87, 0x37, 160);
            hateColor = new Color32(0xFF, 0x00, 0x00, 160);

            // Load picker colours
            regionPickerBitmap = DaggerfallUI.GetImgBitmap(regionPickerImgName);

            // Add region label
            regionLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 2), string.Empty, NativePanel);
            regionLabel.HorizontalAlignment = HorizontalAlignment.Center;

            // Handle clicks
            NativePanel.OnMouseClick += ClickHandler;

            // Setup buttons for first time
            LoadButtonTextures();
            SetupButtons();

            // Region overlay panel
            regionTextureOverlayPanel = DaggerfallUI.AddPanel(regionTextureOverlayPanelRect, NativePanel);
            regionTextureOverlayPanel.Enabled = false;

            // Current region overlay panel
            playerRegionOverlayPanel = DaggerfallUI.AddPanel(playerRegionOverlayPanelRect, NativePanel);
            playerRegionOverlayPanel.Enabled = false;

            // Overlay for the region panel
            identifyOverlayPanel = DaggerfallUI.AddPanel(regionTextureOverlayPanelRect, NativePanel);
            identifyOverlayPanel.Enabled = false;

            // Borders around the region maps
            borderTexture = DaggerfallUI.GetTextureFromImg(regionBorderImgName);
            borderPanel = DaggerfallUI.AddPanel(new Rect(new Vector2(0, regionTextureOverlayPanelRect.position.y), regionTextureOverlayPanelRect.size), NativePanel);
            borderPanel.BackgroundTexture = borderTexture;
            borderPanel.Enabled = false;

            // Load native overworld texture
            overworldTexture = ImageReader.GetTexture(overworldImgName);
            NativePanel.BackgroundTexture = overworldTexture;

            // Setup pixel buffer and texture for region/location identify
            identifyPixelBuffer = new Color32[(int)regionTextureOverlayPanelRect.width * (int)regionTextureOverlayPanelRect.height];
            identifyTexture = new Texture2D((int)regionTextureOverlayPanelRect.width, (int)regionTextureOverlayPanelRect.height, TextureFormat.ARGB32, false);
            identifyTexture.filterMode = FilterMode.Point;

            // Identify current region
            UpdateIdentifyTextureForPlayerRegion();
        }

        public override void OnPush()
        {
            base.OnPush();

            toggleClosedBinding = InputManager.Instance.GetBinding(InputManager.Actions.TravelMap);

            if (IsSetup)
            {
                UpdateIdentifyTextureForPlayerRegion();
                CloseRegionPanel();
            }

        }

        public override void OnPop()
        {
            base.OnPop();
            distanceRegionName = null;
            distance = null;
        }

        public override void Update()
        {
            base.Update();

            // Toggle window closed with same hotkey used to open it
            if (InputManager.Instance.GetKeyUp(toggleClosedBinding))
            {
                if (RegionSelected)
                    CloseRegionPanel();
                else
                    CloseWindow();
            }

            // Input handling
            HotkeySequence.KeyModifiers keyModifiers = HotkeySequence.GetKeyboardKeyModifiers();
            Vector2 currentMousePos = new Vector2((NativePanel.ScaledMousePosition.x), (NativePanel.ScaledMousePosition.y));

            if (currentMousePos != lastMousePos)
            {
                lastMousePos = currentMousePos;
                UpdateMouseOverRegion();
            }

            UpdateRegionLabel();

            // Show/hide identify panel when identify is running
            identifyOverlayPanel.Enabled = true;
        }

        #endregion

        #region Setup

        // Initial button setup
        void SetupButtons()
        {
            // Exit button
            exitButton = DaggerfallUI.AddButton(new Rect(278, 175, 39, 22), NativePanel);
            exitButton.OnMouseClick += ExitButtonClickHandler;

            // Store toggle closed binding for this window
            toggleClosedBinding = InputManager.Instance.GetBinding(InputManager.Actions.TravelMap);

        }

        // Loads textures for buttons
        void LoadButtonTextures()
        {
            Texture2D baselocationFilterButtonEnabledText = ImageReader.GetTexture(locationFilterButtonEnabledImgName);
            Texture2D baselocationFilterButtonDisabledText = ImageReader.GetTexture(locationFilterButtonDisabledImgName);
            DFSize baseSize = new DFSize(179, 22);

            // Dungeons toggle button
            dungeonFilterButtonEnabled = ImageReader.GetSubTexture(baselocationFilterButtonEnabledText, dungeonsFilterButtonSrcRect, baseSize);
            dungeonFilterButtonDisabled = ImageReader.GetSubTexture(baselocationFilterButtonDisabledText, dungeonsFilterButtonSrcRect, baseSize);

            // Dungeons toggle button
            templesFilterButtonEnabled = ImageReader.GetSubTexture(baselocationFilterButtonEnabledText, templesFilterButtonSrcRect, baseSize);
            templesFilterButtonDisabled = ImageReader.GetSubTexture(baselocationFilterButtonDisabledText, templesFilterButtonSrcRect, baseSize);

            // Homes toggle button
            homesFilterButtonEnabled = ImageReader.GetSubTexture(baselocationFilterButtonEnabledText, homesFilterButtonSrcRect, baseSize);
            homesFilterButtonDisabled = ImageReader.GetSubTexture(baselocationFilterButtonDisabledText, homesFilterButtonSrcRect, baseSize);

            // Towns toggle button
            townsFilterButtonEnabled = ImageReader.GetSubTexture(baselocationFilterButtonEnabledText, townsFilterButtonSrcRect, baseSize);
            townsFilterButtonDisabled = ImageReader.GetSubTexture(baselocationFilterButtonDisabledText, townsFilterButtonSrcRect, baseSize);

            DFSize buttonsFullSize = new DFSize(45, 22);

            findButtonTexture = ImageReader.GetTexture(findAtButtonImgName);
            findButtonTexture = ImageReader.GetSubTexture(findButtonTexture, findButtonRect, buttonsFullSize);

            atButtonTexture = ImageReader.GetTexture(findAtButtonImgName);
            atButtonTexture = ImageReader.GetSubTexture(atButtonTexture, atButtonRect, buttonsFullSize);

            // Arrows
            upArrowTexture = ImageReader.GetTexture(upArrowImgName);
            downArrowTexture = ImageReader.GetTexture(downArrowImgName);
            leftArrowTexture = ImageReader.GetTexture(leftArrowImgName);
            rightArrowTexture = ImageReader.GetTexture(rightArrowImgName);
        }

        #endregion

        #region Map Texture Management

        // Called when a region is selected
        protected virtual void UpdateMapTextures()
        {
            // Region must be selected
            if (!RegionSelected)
                return;

            // Cached region texture if not available
            string mapName = selectedRegionMapNames[mapIndex];
            if (!regionTextures.ContainsKey(mapName))
            {
                Texture2D regionTextureOut;
                if (!TextureReplacement.TryImportImage(selectedRegionMapNames[mapIndex], false, out regionTextureOut))
                    regionTextureOut = ImageReader.GetTexture(mapName);
                regionTextures.Add(mapName, regionTextureOut);
            }

            // Present region and locations
            regionTextureOverlayPanel.BackgroundTexture = regionTextures[mapName];
        }

        // Set region block for identify overlay
        protected virtual void UpdateIdentifyTextureForPlayerRegion()
        {
            // Only for overworld map
            if (RegionSelected)
                return;

            // Player must be inside a valid region
            int playerRegion = GetPlayerRegion();
            if (playerRegion == -1)
                return;

            // Clear existing pixel buffer
            Array.Clear(identifyPixelBuffer, 0, identifyPixelBuffer.Length);

            // Import custom map overlays named TRAV0I00.IMG-RegionName (e.g. TRAV0I00.IMG-Ilessan Hills) if available
            // Custom image must be based on 320x160 interior snip of TRAV0I00.IMG (so exclude top and bottom bars) but can be a higher resolution like 1600x800
            Texture2D customRegionOverlayTexture;
            if (importedOverlays.TryGetValue(playerRegion, out customRegionOverlayTexture) ||
                TextureReplacement.TryImportImage(string.Format("{0}-{1}", overworldImgName, GetRegionName(playerRegion)), false, out customRegionOverlayTexture))
            {
                identifyOverlayPanel.BackgroundTexture = importedOverlays[playerRegion] = customRegionOverlayTexture;
                return;
            }

            // Region shape is filled from picker bitmap, so this has to be open
            if (regionPickerBitmap == null)
                regionPickerBitmap = DaggerfallUI.GetImgBitmap(regionPickerImgName);

            // Create a texture overlay for the region area
            int width = regionPickerBitmap.Width;
            int height = regionPickerBitmap.Height;
            int pickerOverlayPanelHeightDifference = height - (int)regionTextureOverlayPanelRect.height - regionPanelOffset + 1;
            for (int y = 0; y < height; y++)
            {
                for (int i = 0; i < DaggerfallUnity.Instance.ContentReader.MapFileReader.RegionCount; i++)
                {
                    if (!BankingOverhaulMain.regionRelationValues.ContainsKey(playerRegion) || !BankingOverhaulMain.regionRelationValues.ContainsKey(i))
                        continue;

                    int regionRelation = BankingOverhaulMain.CompareRegionValues(BankingOverhaulMain.regionRelationValues[playerRegion], BankingOverhaulMain.regionRelationValues[i], playerRegion, i);

                    for (int x = 0; x < width; x++)
                    {
                        int srcOffset = y * width + x;
                        int dstOffset = ((height - y - pickerOverlayPanelHeightDifference) * width) + x;
                        int sampleRegion = regionPickerBitmap.Data[srcOffset] - 128;
                        if (sampleRegion != playerRegion && sampleRegion == i)
                        {
                            if (regionRelation == 0)
                                identifyPixelBuffer[dstOffset] = indifferentColor;
                            else if (regionRelation == 1)
                                identifyPixelBuffer[dstOffset] = likeColor;
                            else if (regionRelation == -1)
                                identifyPixelBuffer[dstOffset] = dislikeColor;
                            else if (regionRelation == 2)
                                identifyPixelBuffer[dstOffset] = loveColor;
                            else if (regionRelation == -2)
                                identifyPixelBuffer[dstOffset] = hateColor;
                        }
                        else if (sampleRegion == playerRegion)
                            identifyPixelBuffer[dstOffset] = currentRegionColor;
                    }
                }
            }
            /*for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcOffset = y * width + x;
                    int dstOffset = ((height - y - pickerOverlayPanelHeightDifference) * width) + x;
                    int sampleRegion = regionPickerBitmap.Data[srcOffset] - 128;
                    if (sampleRegion == playerRegion)
                        identifyPixelBuffer[dstOffset] = identifyFlashColor;
                }
            }*/
            identifyTexture.SetPixels32(identifyPixelBuffer);
            identifyTexture.Apply();
            identifyOverlayPanel.BackgroundTexture = identifyTexture;
        }

        protected virtual void UpdateCrosshair()
        {
            UpdateIdentifyTextureForPosition(TravelTimeCalculator.GetPlayerTravelPosition(), selectedRegion);
        }

        protected virtual void UpdateIdentifyTextureForPosition(DFPosition pos, int regionIndex = -1)
        {
            if (regionIndex == -1)
                regionIndex = GetPlayerRegion();
            UpdateIdentifyTextureForPosition(pos.X, pos.Y, regionIndex);
        }

        // Set location crosshair for identify overlay
        protected virtual void UpdateIdentifyTextureForPosition(int mapPixelX, int mapPixelY, int regionIndex)
        {
            // Only for regions
            if (!RegionSelected)
                return;

            // Clear existing pixel buffer
            Array.Clear(identifyPixelBuffer, 0, identifyPixelBuffer.Length);

            string mapName = selectedRegionMapNames[mapIndex];
            Vector2 origin = offsetLookup[mapName];
            float scale = GetRegionMapScale(regionIndex);

            // Manually adjust Betony vertical offset
            int yAdjust = 0;
            if (regionIndex == betonyIndex)
                yAdjust = -477;

            int scaledX = (int)((mapPixelX - origin.x) * scale);
            int scaledY = (int)((mapPixelY - origin.y) * scale) + regionPanelOffset + yAdjust;

            int width = (int)regionTextureOverlayPanelRect.width;
            int height = (int)regionTextureOverlayPanelRect.height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == scaledX || y + regionPanelOffset == scaledY)
                    {
                        identifyPixelBuffer[(height - y - 1) * width + x] = currentRegionColor;
                    }
                }
            }
            identifyTexture.SetPixels32(identifyPixelBuffer);
            identifyTexture.Apply();
            identifyOverlayPanel.BackgroundTexture = identifyTexture;
        }

        #endregion

        #region Event Handlers

        // Handle clicks on the main panel
        protected virtual void ClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            position.y -= regionPanelOffset;

            // Ensure clicks are inside region texture
            if (position.x < 0 || position.x > regionTextureOverlayPanelRect.width || position.y < 0 || position.y > regionTextureOverlayPanelRect.height)
                return;
        }

        protected virtual void ExitButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseTravelWindows();
        }

        #endregion

        #region Private Methods

        // Close region panel and reset values
        protected virtual void CloseRegionPanel()
        {
            selectedRegion = -1;
            mouseOverRegion = -1;
            mapIndex = 0;
            regionTextureOverlayPanel.Enabled = false;
            if (DaggerfallUnity.Settings.TravelMapLocationsOutline)
                for (int i = 0; i < outlineDisplacements.Length; i++)
                    regionLocationDotsOutlinesOverlayPanel[i].Enabled = false;
            regionLocationDotsOverlayPanel.Enabled = false;
            UpdateIdentifyTextureForPlayerRegion();
        }

        protected Vector2 GetCoordinates()
        {
            string mapName = selectedRegionMapNames[mapIndex];
            Vector2 origin = offsetLookup[mapName];
            int height = (int)regionTextureOverlayPanelRect.height;

            Vector2 results = Vector2.zero;
            Vector2 pos = regionTextureOverlayPanel.ScaledMousePosition;

            results.x = (int)Math.Floor(origin.x + pos.x);
            results.y = (int)Math.Floor(origin.y + pos.y);

            return results;
        }

        // Check if mouse over a region
        protected virtual void UpdateMouseOverRegion()
        {
            mouseOverRegion = -1;

            int x = 0;
            int y = 0;

            x = (int)lastMousePos.x;
            y = (int)lastMousePos.y;

            // Get offset into region picker bitmap
            int offset = y * regionPickerBitmap.Width + x;
            if (offset < 0 || offset >= regionPickerBitmap.Data.Length)
                return;

            // Get region from bitmap, if any
            int region = regionPickerBitmap.Data[offset] - 128;
            if (region < 0 || region >= DaggerfallUnity.Instance.ContentReader.MapFileReader.RegionCount)
                return;

            // Store valid region
            mouseOverRegion = region;
        }

        // Updates the text label at top of screen
        protected virtual void UpdateRegionLabel()
        {
            int regionRelation = 0;
            string relationText = "";

            if (mouseOverRegion == -1 || GetPlayerRegion() == mouseOverRegion || mouseOverRegion >= 61 || GetPlayerRegion() >= 61)
                regionLabel.Text = "You are in " + GetRegionName(GetPlayerRegion());
            else
            {
                regionRelation = BankingOverhaulMain.CompareRegionValues(BankingOverhaulMain.regionRelationValues[GetPlayerRegion()], BankingOverhaulMain.regionRelationValues[mouseOverRegion], GetPlayerRegion(), mouseOverRegion);

                switch (regionRelation)
                {
                    case 2:
                        relationText = "  Loves  "; break;
                    case 1:
                        relationText = "  Trusts  "; break;
                    case 0:
                        relationText = "  Is Indifferent Toward  "; break;
                    case -1:
                        relationText = "  Distrusts  "; break;
                    case -2:
                        relationText = "  Hates  "; break;
                }

                regionLabel.Text = GetRegionName(mouseOverRegion) + relationText + GetRegionName(GetPlayerRegion());
            }
        }

        // Closes windows based on context
        public void CloseTravelWindows(bool forceClose = false)
        {
            if (RegionSelected == false || forceClose)
                CloseWindow();
            else
                CloseRegionPanel();
        }

        #endregion

        #region Helper Methods

        private class MatchesCutOff
        {
            private readonly float threshold;

            public MatchesCutOff(float bestRelevance)
            {
                // If perfect match exists, return all perfect matches only
                // Normally there should be only one perfect match, but if string canonization generates collisions that's no longer guaranteed
                threshold = bestRelevance == 1f ? 1f : bestRelevance * 0.5f;
            }

            public bool Keep(float relevance)
            {
                return relevance >= threshold;
            }
        }

        // Gets current player region or -1 if player not in any region (e.g. in ocean)
        protected int GetPlayerRegion()
        {
            DFPosition position = TravelTimeCalculator.GetPlayerTravelPosition();
            int region = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetPoliticIndex(position.X, position.Y) - 128;
            if (region < 0 || region >= DaggerfallUnity.Instance.ContentReader.MapFileReader.RegionCount)
                return -1;

            return region;
        }

        // Gets name of region
        protected string GetRegionName(int region)
        {
            return DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegionName(region);
        }

        // Gets scale of region map
        protected virtual float GetRegionMapScale(int region)
        {
            if (region == betonyIndex)
                return 4f;
            else
                return 1;
        }

        protected virtual void CreatePopUpWindow()
        {
            DFPosition pos = MapsFile.GetPixelFromPixelID(locationSummary.ID);

            if (popUp == null)
            {
                popUp = (DaggerfallTravelPopUp)UIWindowFactory.GetInstanceWithArgs(UIWindowType.TravelPopUp, new object[] { uiManager, uiManager.TopWindow, this });
            }
            popUp.EndPos = pos;
            uiManager.PushWindow(popUp);
        }

        #endregion
    }
}
