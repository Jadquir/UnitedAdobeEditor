using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.Helpers;

namespace UnitedAdobeEditor.Components.ColorChanger
{
    public class PhotoshopTheme
    {
        // Complete Black Theme - Pure darkness with subtle accents
        // Complete Black Theme - Ultra-comprehensive pure darkness theme
        public static Dictionary<string, ColorMetadata> CompleteBlackTheme = new Dictionary<string, ColorMetadata>
{
    // Basic Colors
    { "White", new ColorMetadata(40, 40, 40, 1.0) }, // Dark gray instead of white
    { "Black", new ColorMetadata(0, 0, 0, 1.0) }, // Pure black
    { "Red", new ColorMetadata(120, 30, 30, 1.0) }, // Muted dark red
    { "Green", new ColorMetadata(30, 120, 30, 1.0) }, // Muted dark green
    { "Blue", new ColorMetadata(30, 30, 120, 1.0) }, // Muted dark blue
    { "Yellow", new ColorMetadata(120, 120, 30, 1.0) }, // Muted dark yellow
    { "Cyan", new ColorMetadata(30, 120, 120, 1.0) }, // Muted dark cyan
    { "Magenta", new ColorMetadata(120, 30, 120, 1.0) }, // Muted dark magenta
    
    // Main UI Elements
    { "CanvasColor", new ColorMetadata(0, 0, 0, 1.0) }, // Pure black canvas
    { "PanelMain", new ColorMetadata(12, 12, 12, 1.0) }, // Very dark panels
    { "DefaultText", new ColorMetadata(180, 180, 180, 1.0) }, // Light gray text
    { "WidgetText", new ColorMetadata(160, 160, 160, 1.0) }, // Slightly dimmer widget text
    { "WidgetTextDisabled", new ColorMetadata(80, 80, 80, 1.0) }, // Disabled text
    { "WidgetLabelText", new ColorMetadata(140, 140, 140, 1.0) }, // Label text
    { "WidgetLabelTextDisabled", new ColorMetadata(60, 60, 60, 1.0) }, // Disabled labels
    
    // Buttons and Controls
    { "ButtonUpFill", new ColorMetadata(25, 25, 25, 1.0) }, // Dark button fill
    { "ButtonDownFill", new ColorMetadata(15, 15, 15, 1.0) }, // Pressed button fill
    { "WidgetButtonFill", new ColorMetadata(30, 30, 30, 1.0) }, // Widget button fill
    { "WidgetButtonFillPressed", new ColorMetadata(20, 20, 20, 1.0) }, // Pressed widget button
    { "WidgetButtonFillDisabled", new ColorMetadata(18, 18, 18, 1.0) }, // Disabled button
    { "WidgetButtonStroke", new ColorMetadata(60, 60, 60, 1.0) }, // Button borders
    { "WidgetButtonStrokePressed", new ColorMetadata(80, 80, 80, 1.0) }, // Pressed button border
    { "WidgetButtonStrokeDisabled", new ColorMetadata(40, 40, 40, 1.0) }, // Disabled button border
    
    // Widget States
    { "WidgetFill", new ColorMetadata(35, 35, 35, 1.0) }, // General widget fill
    { "WidgetFillPressed", new ColorMetadata(25, 25, 25, 1.0) }, // Pressed widget fill
    { "WidgetFillOn", new ColorMetadata(45, 45, 45, 1.0) }, // Active widget fill
    { "WidgetFillOnPressed", new ColorMetadata(35, 35, 35, 1.0) }, // Active pressed widget
    { "WidgetFillDisabled", new ColorMetadata(22, 22, 22, 1.0) }, // Disabled widget fill
    { "WidgetFillOnDisabled", new ColorMetadata(28, 28, 28, 1.0) }, // Disabled active widget
    { "WidgetStroke", new ColorMetadata(55, 55, 55, 1.0) }, // Widget borders
    { "WidgetStrokePressed", new ColorMetadata(75, 75, 75, 1.0) }, // Pressed widget border
    { "WidgetStrokeOn", new ColorMetadata(85, 85, 85, 1.0) }, // Active widget border
    { "WidgetStrokeOnPressed", new ColorMetadata(95, 95, 95, 1.0) }, // Active pressed border
    { "WidgetStrokeDisabled", new ColorMetadata(35, 35, 35, 1.0) }, // Disabled widget border
    
    // Primary Elements
    { "WidgetPrimaryFill", new ColorMetadata(50, 50, 50, 1.0) }, // Primary widget fill
    { "Alert", new ColorMetadata(140, 40, 40, 1.0) }, // Alert color
    
    // HUD Elements
    { "HUDBackground", new ColorMetadata(5, 5, 5, 1.0) }, // Almost black HUD
    { "HUDText", new ColorMetadata(200, 200, 200, 1.0) }, // Light HUD text
    { "HUDShadow", new ColorMetadata(0, 0, 0, 1.0) }, // Pure black shadow
    { "HUDGlow", new ColorMetadata(60, 60, 60, 1.0) }, // Subtle glow
    
    // Scrolling Lists
    { "ScrollingListFill", new ColorMetadata(18, 18, 18, 1.0) }, // List background
    { "ScrollingListAlternate", new ColorMetadata(22, 22, 22, 1.0) }, // Alternate rows
    { "ScrollingListSelected", new ColorMetadata(40, 40, 40, 1.0) }, // Selected items
    { "ScrollingListSelectedDefault", new ColorMetadata(45, 45, 45, 1.0) }, // Default selection
    { "ScrollingListSelectedOne", new ColorMetadata(50, 50, 50, 1.0) }, // Single selection
    { "ScrollingListSelectedFrame", new ColorMetadata(70, 70, 70, 1.0) }, // Selection frame
    { "ScrollingListSelectedFrameDefault", new ColorMetadata(75, 75, 75, 1.0) }, // Default selection frame
    { "ScrollingListSelectedFrameOne", new ColorMetadata(80, 80, 80, 1.0) }, // Single selection frame
    { "ScrollingListSeparator", new ColorMetadata(40, 40, 40, 1.0) }, // List separators
    { "ScrollingListDarkBackground", new ColorMetadata(8, 8, 8, 1.0) }, // Dark list background
    { "ScrollingListSelectedOverlay", new ColorMetadata(60, 60, 60, 0.5) }, // Selection overlay
    
    // Icons
    { "IconFill", new ColorMetadata(140, 140, 140, 1.0) }, // Icon color
    { "IconFillDimmed", new ColorMetadata(100, 100, 100, 1.0) }, // Dimmed icons
    { "IconFillPale", new ColorMetadata(80, 80, 80, 1.0) }, // Pale icons
    { "IconFillMedium", new ColorMetadata(120, 120, 120, 1.0) }, // Medium icons
    { "IconFillActive", new ColorMetadata(180, 180, 180, 1.0) }, // Active icons
    { "IconBackground", new ColorMetadata(25, 25, 25, 1.0) }, // Icon backgrounds
    { "IconFrame", new ColorMetadata(50, 50, 50, 1.0) }, // Icon frames
    { "IconFrameActive", new ColorMetadata(70, 70, 70, 1.0) }, // Active icon frames
    
    // Tooltips
    { "ToolTipDefaultBackground", new ColorMetadata(28, 28, 28, 1.0) }, // Tooltip background
    { "ToolTipDefaultText", new ColorMetadata(190, 190, 190, 1.0) }, // Tooltip text
    
    // Rulers
    { "RulerBackground", new ColorMetadata(10, 10, 10, 1.0) }, // Ruler background
    { "RulerText", new ColorMetadata(150, 150, 150, 1.0) }, // Ruler text
    { "RulerMarkers", new ColorMetadata(100, 100, 100, 1.0) }, // Ruler markers
    { "RulerLocation", new ColorMetadata(200, 200, 200, 1.0) }, // Ruler location indicator
    { "RulerDisabledBackground", new ColorMetadata(8, 8, 8, 1.0) }, // Disabled ruler background
    { "RulerDisabledText", new ColorMetadata(60, 60, 60, 1.0) }, // Disabled ruler text
    { "RulerDisabledMarkers", new ColorMetadata(40, 40, 40, 1.0) }, // Disabled ruler markers
    
    // Frames and Borders
    { "WindowsMicroFrame", new ColorMetadata(35, 35, 35, 1.0) }, // Window frames
    { "GroupFrame", new ColorMetadata(45, 45, 45, 1.0) }, // Group frames
    { "GroupFrameDisabled", new ColorMetadata(25, 25, 25, 1.0) }, // Disabled group frames
    { "WidgetGeneralFrame", new ColorMetadata(55, 55, 55, 1.0) }, // General widget frames
    { "PaletteFrame", new ColorMetadata(30, 30, 30, 1.0) }, // Palette frames
    
    // Scrollbars
    { "WidgetScrollbarFill", new ColorMetadata(40, 40, 40, 1.0) }, // Scrollbar track
    { "WidgetScrollbarElevatorFill", new ColorMetadata(60, 60, 60, 1.0) }, // Scrollbar thumb
    { "WidgetScrollbarFillPressed", new ColorMetadata(80, 80, 80, 1.0) }, // Pressed scrollbar
    { "WidgetScrollbarArrows", new ColorMetadata(120, 120, 120, 1.0) }, // Scrollbar arrows
    
    // Sliders
    { "WidgetSliderGreySlide", new ColorMetadata(35, 35, 35, 1.0) }, // Slider track
    { "WidgetSliderThumbGreyStroke", new ColorMetadata(80, 80, 80, 1.0) }, // Slider thumb border
    { "WidgetSliderThumbGreyFill", new ColorMetadata(50, 50, 50, 1.0) }, // Slider thumb fill
    { "WidgetSliderThumbGreySplitter", new ColorMetadata(90, 90, 90, 1.0) }, // Slider thumb splitter
    
    // Bevels and Shadows
    { "BevelShadow", new ColorMetadata(0, 0, 0, 1.0) }, // Bevel shadow
    { "BevelHighlight", new ColorMetadata(60, 60, 60, 1.0) }, // Bevel highlight
    { "WidgetAlphaBevel", new ColorMetadata(40, 40, 40, 0.7) }, // Alpha bevel
    
    // Tabs
    { "UnselectedTab", new ColorMetadata(20, 20, 20, 1.0) }, // Unselected tabs
    { "TabSlot", new ColorMetadata(15, 15, 15, 1.0) }, // Tab slots
    
    // Overlays
    { "ViewDisabledOverlay", new ColorMetadata(0, 0, 0, 0.6) }, // Disabled view overlay
    { "CropOverlay", new ColorMetadata(0, 0, 0, 0.8) }, // Crop overlay
    
    // Edit Elements
    { "EditHighlight", new ColorMetadata(80, 80, 80, 1.0) }, // Edit highlight
    { "EditHighlightDefault", new ColorMetadata(85, 85, 85, 1.0) }, // Default edit highlight
    { "EditHighlightOne", new ColorMetadata(90, 90, 90, 1.0) }, // Single edit highlight
    { "TargetFocused", new ColorMetadata(100, 100, 100, 1.0) }, // Focused targets
    { "TargetFocusedDefault", new ColorMetadata(105, 105, 105, 1.0) }, // Default focused targets
    { "TargetFocusedOne", new ColorMetadata(110, 110, 110, 1.0) }, // Single focused target
    { "TargetDefault", new ColorMetadata(70, 70, 70, 1.0) }, // Default targets
    { "TargetDefaultDefault", new ColorMetadata(75, 75, 75, 1.0) }, // Default default targets
    { "TargetDefaultOne", new ColorMetadata(80, 80, 80, 1.0) }, // Single default target
    
    // Dividers
    { "WidgetDivider", new ColorMetadata(50, 50, 50, 1.0) }, // Widget dividers
    { "GripperStroke", new ColorMetadata(70, 70, 70, 1.0) }, // Gripper strokes
    
    // Links
    { "HyperLink", new ColorMetadata(120, 120, 180, 1.0) }, // Hyperlinks
    
    // Color Picker
    { "ColorPickerCircle", new ColorMetadata(100, 100, 100, 1.0) }, // Color picker circle
    { "ColorPickerCircleDark", new ColorMetadata(60, 60, 60, 1.0) }, // Dark color picker circle
    
    // Mini Elements
    { "MiniProgressFill", new ColorMetadata(60, 60, 60, 1.0) }, // Mini progress fill
    { "MiniProgressBackground", new ColorMetadata(25, 25, 25, 1.0) }, // Mini progress background
    { "MiniProgressText", new ColorMetadata(160, 160, 160, 1.0) } // Mini progress text
};

        // VALORANT Theme - Complete gaming-inspired red and blue theme
        public static Dictionary<string, ColorMetadata> VALORANTTheme = new Dictionary<string, ColorMetadata>
{
    // Basic Colors
    { "White", new ColorMetadata(255, 246, 228, 1.0) }, // Warm white
    { "Black", new ColorMetadata(8, 12, 18, 1.0) }, // Deep dark blue
    { "Red", new ColorMetadata(255, 70, 85, 1.0) }, // VALORANT red
    { "Green", new ColorMetadata(100, 255, 150, 1.0) }, // Bright green accent
    { "Blue", new ColorMetadata(83, 151, 255, 1.0) }, // VALORANT blue
    { "Yellow", new ColorMetadata(255, 200, 100, 1.0) }, // Warm yellow
    { "Cyan", new ColorMetadata(100, 200, 255, 1.0) }, // Bright cyan
    { "Magenta", new ColorMetadata(255, 100, 200, 1.0) }, // Bright magenta
    
    // Main UI Elements
    { "CanvasColor", new ColorMetadata(12, 16, 24, 1.0) }, // Dark blue canvas
    { "PanelMain", new ColorMetadata(20, 25, 35, 1.0) }, // Dark blue panels
    { "DefaultText", new ColorMetadata(255, 246, 228, 1.0) }, // Warm white text
    { "WidgetText", new ColorMetadata(200, 220, 255, 1.0) }, // Light blue widget text
    { "WidgetTextDisabled", new ColorMetadata(100, 120, 150, 1.0) }, // Disabled text
    { "WidgetLabelText", new ColorMetadata(180, 200, 240, 1.0) }, // Label text
    { "WidgetLabelTextDisabled", new ColorMetadata(80, 100, 130, 1.0) }, // Disabled labels
    
    // Buttons and Controls
    { "ButtonUpFill", new ColorMetadata(255, 70, 85, 1.0) }, // VALORANT red buttons
    { "ButtonDownFill", new ColorMetadata(200, 45, 60, 1.0) }, // Darker red pressed
    { "WidgetButtonFill", new ColorMetadata(83, 151, 255, 1.0) }, // Blue widget buttons
    { "WidgetButtonFillPressed", new ColorMetadata(60, 120, 200, 1.0) }, // Darker blue pressed
    { "WidgetButtonFillDisabled", new ColorMetadata(40, 60, 90, 1.0) }, // Disabled button
    { "WidgetButtonStroke", new ColorMetadata(120, 180, 255, 1.0) }, // Light blue borders
    { "WidgetButtonStrokePressed", new ColorMetadata(255, 100, 115, 1.0) }, // Red pressed border
    { "WidgetButtonStrokeDisabled", new ColorMetadata(60, 80, 110, 1.0) }, // Disabled border
    
    // Widget States
    { "WidgetFill", new ColorMetadata(30, 40, 60, 1.0) }, // General widget fill
    { "WidgetFillPressed", new ColorMetadata(25, 35, 55, 1.0) }, // Pressed widget fill
    { "WidgetFillOn", new ColorMetadata(83, 151, 255, 1.0) }, // Active widget fill (blue)
    { "WidgetFillOnPressed", new ColorMetadata(60, 120, 200, 1.0) }, // Active pressed widget
    { "WidgetFillDisabled", new ColorMetadata(22, 28, 40, 1.0) }, // Disabled widget fill
    { "WidgetFillOnDisabled", new ColorMetadata(40, 60, 90, 1.0) }, // Disabled active widget
    { "WidgetStroke", new ColorMetadata(100, 150, 255, 1.0) }, // Widget borders
    { "WidgetStrokePressed", new ColorMetadata(255, 70, 85, 1.0) }, // Red pressed border
    { "WidgetStrokeOn", new ColorMetadata(150, 200, 255, 1.0) }, // Active widget border
    { "WidgetStrokeOnPressed", new ColorMetadata(255, 100, 115, 1.0) }, // Active pressed border
    { "WidgetStrokeDisabled", new ColorMetadata(50, 70, 100, 1.0) }, // Disabled widget border
    
    // Primary Elements
    { "WidgetPrimaryFill", new ColorMetadata(83, 151, 255, 1.0) }, // Primary blue
    { "Alert", new ColorMetadata(255, 85, 85, 1.0) }, // Bright red alerts
    
    // HUD Elements
    { "HUDBackground", new ColorMetadata(15, 20, 30, 1.0) }, // Dark blue HUD
    { "HUDText", new ColorMetadata(255, 246, 228, 1.0) }, // Warm white HUD text
    { "HUDShadow", new ColorMetadata(5, 8, 12, 1.0) }, // Dark shadow
    { "HUDGlow", new ColorMetadata(83, 151, 255, 0.8) }, // Blue glow
    
    // Scrolling Lists
    { "ScrollingListFill", new ColorMetadata(25, 30, 40, 1.0) }, // List background
    { "ScrollingListAlternate", new ColorMetadata(28, 35, 45, 1.0) }, // Alternate rows
    { "ScrollingListSelected", new ColorMetadata(255, 70, 85, 0.3) }, // Red selection
    { "ScrollingListSelectedDefault", new ColorMetadata(83, 151, 255, 0.3) }, // Blue default selection
    { "ScrollingListSelectedOne", new ColorMetadata(255, 70, 85, 0.4) }, // Red single selection
    { "ScrollingListSelectedFrame", new ColorMetadata(255, 70, 85, 1.0) }, // Red selection frame
    { "ScrollingListSelectedFrameDefault", new ColorMetadata(83, 151, 255, 1.0) }, // Blue selection frame
    { "ScrollingListSelectedFrameOne", new ColorMetadata(255, 100, 115, 1.0) }, // Light red frame
    { "ScrollingListSeparator", new ColorMetadata(50, 70, 100, 1.0) }, // List separators
    { "ScrollingListDarkBackground", new ColorMetadata(18, 23, 33, 1.0) }, // Dark list background
    { "ScrollingListSelectedOverlay", new ColorMetadata(83, 151, 255, 0.2) }, // Blue selection overlay
    
    // Icons
    { "IconFill", new ColorMetadata(150, 180, 255, 1.0) }, // Light blue icons
    { "IconFillDimmed", new ColorMetadata(100, 130, 200, 1.0) }, // Dimmed blue icons
    { "IconFillPale", new ColorMetadata(80, 110, 180, 1.0) }, // Pale blue icons
    { "IconFillMedium", new ColorMetadata(120, 160, 230, 1.0) }, // Medium blue icons
    { "IconFillActive", new ColorMetadata(255, 70, 85, 1.0) }, // Red active icons
    { "IconBackground", new ColorMetadata(35, 45, 65, 1.0) }, // Icon backgrounds
    { "IconFrame", new ColorMetadata(70, 100, 150, 1.0) }, // Icon frames
    { "IconFrameActive", new ColorMetadata(255, 100, 115, 1.0) }, // Red active icon frames
    
    // Tooltips
    { "ToolTipDefaultBackground", new ColorMetadata(35, 45, 60, 1.0) }, // Blue tooltip background
    { "ToolTipDefaultText", new ColorMetadata(255, 246, 228, 1.0) }, // Warm white tooltip text
    
    // Rulers
    { "RulerBackground", new ColorMetadata(18, 23, 33, 1.0) }, // Dark blue ruler background
    { "RulerText", new ColorMetadata(150, 180, 255, 1.0) }, // Light blue ruler text
    { "RulerMarkers", new ColorMetadata(100, 140, 220, 1.0) }, // Blue ruler markers
    { "RulerLocation", new ColorMetadata(255, 70, 85, 1.0) }, // Red ruler location
    { "RulerDisabledBackground", new ColorMetadata(15, 18, 25, 1.0) }, // Disabled ruler background
    { "RulerDisabledText", new ColorMetadata(60, 80, 120, 1.0) }, // Disabled ruler text
    { "RulerDisabledMarkers", new ColorMetadata(40, 60, 90, 1.0) }, // Disabled ruler markers
    
    // Frames and Borders
    { "WindowsMicroFrame", new ColorMetadata(40, 50, 70, 1.0) }, // Blue window frames
    { "GroupFrame", new ColorMetadata(60, 80, 120, 1.0) }, // Light blue group frames
    { "GroupFrameDisabled", new ColorMetadata(30, 40, 60, 1.0) }, // Disabled group frames
    { "WidgetGeneralFrame", new ColorMetadata(80, 110, 160, 1.0) }, // General widget frames
    { "PaletteFrame", new ColorMetadata(45, 60, 85, 1.0) }, // Palette frames
    
    // Scrollbars
    { "WidgetScrollbarFill", new ColorMetadata(40, 55, 80, 1.0) }, // Scrollbar track
    { "WidgetScrollbarElevatorFill", new ColorMetadata(83, 151, 255, 1.0) }, // Blue scrollbar thumb
    { "WidgetScrollbarFillPressed", new ColorMetadata(255, 70, 85, 1.0) }, // Red pressed scrollbar
    { "WidgetScrollbarArrows", new ColorMetadata(200, 220, 255, 1.0) }, // Light scrollbar arrows
    
    // Sliders
    { "WidgetSliderGreySlide", new ColorMetadata(40, 55, 80, 1.0) }, // Slider track
    { "WidgetSliderThumbGreyStroke", new ColorMetadata(150, 180, 255, 1.0) }, // Blue slider border
    { "WidgetSliderThumbGreyFill", new ColorMetadata(83, 151, 255, 1.0) }, // Blue slider fill
    { "WidgetSliderThumbGreySplitter", new ColorMetadata(255, 70, 85, 1.0) }, // Red slider splitter
    
    // Bevels and Shadows
    { "BevelShadow", new ColorMetadata(5, 8, 12, 1.0) }, // Dark bevel shadow
    { "BevelHighlight", new ColorMetadata(100, 140, 200, 1.0) }, // Blue bevel highlight
    { "WidgetAlphaBevel", new ColorMetadata(83, 151, 255, 0.3) }, // Blue alpha bevel
    
    // Tabs
    { "UnselectedTab", new ColorMetadata(25, 35, 50, 1.0) }, // Unselected tabs
    { "TabSlot", new ColorMetadata(20, 28, 40, 1.0) }, // Tab slots
    
    // Overlays
    { "ViewDisabledOverlay", new ColorMetadata(8, 12, 18, 0.7) }, // Disabled view overlay
    { "CropOverlay", new ColorMetadata(8, 12, 18, 0.8) }, // Crop overlay
    
    // Edit Elements
    { "EditHighlight", new ColorMetadata(255, 70, 85, 0.4) }, // Red edit highlight
    { "EditHighlightDefault", new ColorMetadata(83, 151, 255, 0.4) }, // Blue default highlight
    { "EditHighlightOne", new ColorMetadata(255, 100, 115, 0.4) }, // Light red highlight
    { "TargetFocused", new ColorMetadata(83, 151, 255, 1.0) }, // Blue focused targets
    { "TargetFocusedDefault", new ColorMetadata(100, 160, 255, 1.0) }, // Light blue focused
    { "TargetFocusedOne", new ColorMetadata(255, 70, 85, 1.0) }, // Red single focused
    { "TargetDefault", new ColorMetadata(70, 100, 150, 1.0) }, // Default targets
    { "TargetDefaultDefault", new ColorMetadata(80, 120, 180, 1.0) }, // Default default targets
    { "TargetDefaultOne", new ColorMetadata(90, 140, 210, 1.0) }, // Single default target
    
    // Dividers
    { "WidgetDivider", new ColorMetadata(60, 80, 120, 1.0) }, // Blue widget dividers
    { "GripperStroke", new ColorMetadata(100, 140, 200, 1.0) }, // Blue gripper strokes
    
    // Links
    { "HyperLink", new ColorMetadata(150, 200, 255, 1.0) }, // Light blue hyperlinks
    
    // Color Picker
    { "ColorPickerCircle", new ColorMetadata(200, 220, 255, 1.0) }, // Light blue color picker
    { "ColorPickerCircleDark", new ColorMetadata(60, 100, 160, 1.0) }, // Dark blue color picker
    
    // Mini Elements
    { "MiniProgressFill", new ColorMetadata(83, 151, 255, 1.0) }, // Blue progress fill
    { "MiniProgressBackground", new ColorMetadata(30, 40, 60, 1.0) }, // Dark progress background
    { "MiniProgressText", new ColorMetadata(200, 220, 255, 1.0) } // Light blue progress text
};

        // Claude AI Theme - Complete sophisticated orange and dark professional theme
        public static Dictionary<string, ColorMetadata> ClaudeAITheme = new Dictionary<string, ColorMetadata>()
{
    { "Cyan", new ColorMetadata(64, 128, 128, 1.0) },
    { "Magenta", new ColorMetadata(128, 64, 128, 1.0) },
    { "Yellow", new ColorMetadata(192, 160, 64, 1.0) },
    { "Green", new ColorMetadata(64, 128, 64, 1.0) },
    { "Blue", new ColorMetadata(64, 96, 160, 1.0) },
    { "Red", new ColorMetadata(160, 64, 64, 1.0) },
    { "White", new ColorMetadata(224, 224, 208, 1.0) },
    { "Black", new ColorMetadata(32, 32, 32, 1.0) },
    { "BetaSplashText", new ColorMetadata(200, 192, 176, 1.0) },
    { "BetaSplashFill", new ColorMetadata(128, 96, 80, 1.0) },
    { "ButtonUpFill", new ColorMetadata(176, 144, 112, 1.0) },
    { "ButtonDownFill", new ColorMetadata(144, 112, 80, 1.0) },
    { "UnselectedTab", new ColorMetadata(160, 144, 128, 1.0) },
    { "IconFillDimmed", new ColorMetadata(128, 128, 128, 1.0) },
    { "ScrollingListSubSelected", new ColorMetadata(160, 144, 112, 1.0) },
    { "ScrollingListSubSelectedDefault", new ColorMetadata(176, 160, 128, 1.0) },
    { "ScrollingListSubSelectedOne", new ColorMetadata(192, 176, 144, 1.0) },
    { "TabSlot", new ColorMetadata(112, 96, 80, 1.0) },
    { "PaletteFill", new ColorMetadata(192, 176, 160, 1.0) },
    { "OWLPaletteFill", new ColorMetadata(160, 144, 128, 1.0) },
    { "BevelShadow", new ColorMetadata(64, 48, 32, 1.0) },
    { "BevelHighlight", new ColorMetadata(224, 208, 192, 1.0) },
    { "IconFill", new ColorMetadata(128, 112, 96, 1.0) },
    { "IconFillPale", new ColorMetadata(160, 144, 128, 1.0) },
    { "IconFillMedium", new ColorMetadata(112, 96, 80, 1.0) },
    { "IconBackground", new ColorMetadata(192, 176, 160, 1.0) },
    { "IconBlue", new ColorMetadata(64, 96, 160, 1.0) },
    { "IconRed", new ColorMetadata(160, 64, 64, 1.0) },
    { "IconGreen", new ColorMetadata(64, 128, 64, 1.0) },
    { "IconYellow", new ColorMetadata(192, 160, 64, 1.0) },
    { "IconOrange", new ColorMetadata(192, 128, 64, 1.0) },
    { "IconFillActive", new ColorMetadata(224, 192, 160, 1.0) },
    { "IconFrameActive", new ColorMetadata(128, 112, 96, 1.0) },
    { "ToolTipDefaultBackground", new ColorMetadata(224, 208, 192, 1.0) },
    { "ToolTipDefaultText", new ColorMetadata(64, 48, 32, 1.0) },
    { "CanvasColor", new ColorMetadata(240, 232, 224, 1.0) },
    { "ScrollingListDimSelected", new ColorMetadata(160, 144, 128, 1.0) },
    { "ScrollingListFill", new ColorMetadata(192, 176, 160, 1.0) },
    { "ScrollingListAlternate", new ColorMetadata(176, 160, 144, 1.0) },
    { "ScrollingListSelected", new ColorMetadata(224, 208, 192, 1.0) },
    { "ScrollingListSelectedDefault", new ColorMetadata(208, 192, 176, 1.0) },
    { "ScrollingListSelectedOne", new ColorMetadata(192, 176, 160, 1.0) },
    { "ScrollingListSeparator", new ColorMetadata(128, 112, 96, 1.0) },
    { "SlotlistNoSlotArea", new ColorMetadata(160, 144, 128, 1.0) },
    { "ScrollingListSelectedFrame", new ColorMetadata(224, 208, 192, 1.0) },
    { "ScrollingListSelectedFrameDefault", new ColorMetadata(208, 192, 176, 1.0) },
    { "ScrollingListSelectedFrameOne", new ColorMetadata(192, 176, 160, 1.0) },
    { "ScrollingListNeutralSelected", new ColorMetadata(176, 160, 144, 1.0) },
    { "ScrollingListDarkBackground", new ColorMetadata(64, 48, 32, 1.0) },
    { "ScrollingListSelectedOverlay", new ColorMetadata(224, 208, 192, 1.0) },
    { "BrushShapeEditorBackground", new ColorMetadata(192, 176, 160, 1.0) },
    { "BrushShapeEditorBrushProfile", new ColorMetadata(160, 144, 128, 1.0) },
    { "BrushShapeEditorHandleEdge", new ColorMetadata(128, 112, 96, 1.0) },
    { "BrushShapeEditorHandleFill", new ColorMetadata(176, 160, 144, 1.0) },
    { "BrushShapeEditorAxis", new ColorMetadata(112, 96, 80, 1.0) },
    { "BrushPreview", new ColorMetadata(192, 176, 160, 1.0) },
    { "BrushPreviewBackground", new ColorMetadata(224, 208, 192, 1.0) },
    { "BrushPreviewBackgroundDisabled", new ColorMetadata(176, 160, 144, 1.0) },
    { "ModifiedBrushPresetSelected", new ColorMetadata(192, 176, 160, 1.0) },
    { "FBColorPickerFocusRing", new ColorMetadata(160, 128, 96, 1.0) },
    { "FBColorPickerOuterBorder", new ColorMetadata(128, 112, 96, 1.0) },
    { "FBColorPickerInnerBorder", new ColorMetadata(176, 160, 144, 1.0) },
    { "FBColorPickerSelectedOuterBorder", new ColorMetadata(192, 176, 160, 1.0) },
    { "RulerMarkers", new ColorMetadata(64, 48, 32, 1.0) },
    { "RulerLocation", new ColorMetadata(128, 112, 96, 1.0) },
    { "RulerBackground", new ColorMetadata(224, 208, 192, 1.0) },
    { "RulerText", new ColorMetadata(64, 48, 32, 1.0) },
    { "RulerDisabledMarkers", new ColorMetadata(96, 80, 64, 1.0) },
    { "RulerDisabledBackground", new ColorMetadata(192, 176, 160, 1.0) },
    { "RulerDisabledText", new ColorMetadata(128, 112, 96, 1.0) },
    { "CropOverlay", new ColorMetadata(192, 128, 96, 1.0) },
    { "ColorPickerCircle", new ColorMetadata(160, 128, 112, 1.0) },
    { "ColorPickerCircleDark", new ColorMetadata(128, 96, 80, 1.0) },
    { "PlotFrame", new ColorMetadata(192, 176, 160, 1.0) },
    { "PlotIntersection", new ColorMetadata(128, 112, 96, 1.0) },
    { "PlotBackground", new ColorMetadata(224, 208, 192, 1.0) },
    { "PlotGridLine", new ColorMetadata(160, 144, 128, 1.0) },
    { "PlotGridMidLine", new ColorMetadata(192, 176, 160, 1.0) },
    { "PlotCompositeCurveLine", new ColorMetadata(128, 112, 96, 1.0) },
    { "PlotCompositeCurveLineDim", new ColorMetadata(96, 80, 64, 1.0) },
    { "IconFrame", new ColorMetadata(128, 112, 96, 1.0) },
    { "RangeSetSideFill", new ColorMetadata(192, 176, 160, 1.0) },
    { "RangeSetMiddleFill", new ColorMetadata(176, 160, 144, 1.0) },
    { "RangeSetWellFill", new ColorMetadata(160, 144, 128, 1.0) },
    { "QuadStateFrame", new ColorMetadata(128, 112, 96, 1.0) },
    { "QuadStateFrameDisabled", new ColorMetadata(96, 80, 64, 1.0) },
    { "QuadStateFillSelected", new ColorMetadata(192, 176, 160, 1.0) },
    { "ViewDisabledOverlay", new ColorMetadata(128, 112, 96, 1.0) },
    { "MiniCancelCheck", new ColorMetadata(64, 48, 32, 1.0) },
    { "MiniCancelBackground", new ColorMetadata(192, 176, 160, 1.0) },
    { "MiniCancelBackgroundRollOver", new ColorMetadata(208, 192, 176, 1.0) },
    { "MiniCancelBackgroundDown", new ColorMetadata(176, 160, 144, 1.0) },
    { "MiniProgressFill", new ColorMetadata(160, 128, 96, 1.0) },
    { "MiniProgressBackground", new ColorMetadata(128, 112, 96, 1.0) },
    { "MiniProgressText", new ColorMetadata(64, 48, 32, 1.0) },
    { "AnimationTimelineVideoClipFill", new ColorMetadata(192, 176, 160, 1.0) },
    { "AnimationTimelineLayerClipFill", new ColorMetadata(160, 144, 128, 1.0) },
    { "AnimationTimelineAudioClipFill", new ColorMetadata(128, 112, 96, 1.0) },
    { "AnimationTimelineSectionTopFill", new ColorMetadata(176, 160, 144, 1.0) },
    { "AnimationTimelineClipBorder", new ColorMetadata(64, 48, 32, 1.0) },
    { "AnimationTimelineClipInnerShadow", new ColorMetadata(96, 80, 64, 1.0) },
    { "AnimationTimelineClipFocusInner", new ColorMetadata(128, 112, 96, 1.0) },
    { "AnimationTimelineClipFocusOuter", new ColorMetadata(160, 144, 128, 1.0) },
    { "AnimationTimelineTransitionForeground", new ColorMetadata(176, 160, 144, 1.0) },
    { "AnimationTimelineTransitionBackground", new ColorMetadata(192, 176, 160, 1.0) },
    { "AnimationTimelineEffectTrackBackground", new ColorMetadata(208, 192, 176, 1.0) },
    { "SlotColorNone", new ColorMetadata(224, 208, 192, 1.0) },
    { "SlotColorNoneDown", new ColorMetadata(192, 176, 160, 1.0) },
    { "SlotColorNoneDisabled", new ColorMetadata(160, 144, 128, 1.0) },
    { "SlotColorRed", new ColorMetadata(160, 64, 64, 1.0) },
    { "SlotColorOrange", new ColorMetadata(192, 128, 64, 1.0) },
    { "SlotColorYellow", new ColorMetadata(192, 160, 64, 1.0) },
    { "SlotColorGreen", new ColorMetadata(64, 128, 64, 1.0) },
    { "SlotColorSeafoam", new ColorMetadata(128, 192, 160, 1.0) },
    { "SlotColorBlue", new ColorMetadata(64, 96, 160, 1.0) },
    { "SlotColorIndigo", new ColorMetadata(64, 64, 128, 1.0) },
    { "SlotColorMagenta", new ColorMetadata(128, 64, 128, 1.0) },
    { "SlotColorFuchsia", new ColorMetadata(192, 128, 192, 1.0) },
    { "SlotColorViolet", new ColorMetadata(128, 96, 160, 1.0) },
    { "SlotColorGray", new ColorMetadata(128, 128, 128, 1.0) },
    { "SlotColorRedDown", new ColorMetadata(128, 48, 48, 1.0) },
    { "SlotColorOrangeDown", new ColorMetadata(160, 96, 48, 1.0) },
    { "SlotColorYellowDown", new ColorMetadata(160, 128, 48, 1.0) },
    { "SlotColorGreenDown", new ColorMetadata(48, 96, 48, 1.0) },
    { "SlotColorSeafoamDown", new ColorMetadata(96, 160, 128, 1.0) },
    { "SlotColorBlueDown", new ColorMetadata(48, 64, 128, 1.0) },
    { "SlotColorIndigoDown", new ColorMetadata(48, 48, 96, 1.0) },
    { "SlotColorMagentaDown", new ColorMetadata(96, 48, 96, 1.0) },
    { "SlotColorFuchsiaDown", new ColorMetadata(160, 96, 160, 1.0) },
    { "SlotColorVioletDown", new ColorMetadata(96, 64, 128, 1.0) },
    { "SlotColorGrayDown", new ColorMetadata(96, 96, 96, 1.0) },
    { "SlotColorRedDisabled", new ColorMetadata(128, 64, 64, 1.0) },
    { "SlotColorOrangeDisabled", new ColorMetadata(160, 112, 64, 1.0) },
    { "SlotColorYellowDisabled", new ColorMetadata(160, 144, 64, 1.0) },
    { "SlotColorGreenDisabled", new ColorMetadata(64, 112, 64, 1.0) },
    { "SlotColorSeafoamDisabled", new ColorMetadata(112, 160, 128, 1.0) },
    { "SlotColorBlueDisabled", new ColorMetadata(64, 96, 128, 1.0) },
    { "SlotColorIndigoDisabled", new ColorMetadata(64, 64, 112, 1.0) },
    { "SlotColorMagentaDisabled", new ColorMetadata(112, 64, 112, 1.0) },
    { "SlotColorFuchsiaDisabled", new ColorMetadata(160, 112, 160, 1.0) },
    { "SlotColorVioletDisabled", new ColorMetadata(112, 80, 144, 1.0) },
    { "SlotColorGrayDisabled", new ColorMetadata(96, 96, 96, 1.0) },
    { "HUDBackground", new ColorMetadata(160, 144, 128, 1.0) },
    { "HUDText", new ColorMetadata(64, 48, 32, 1.0) },
    { "HUDShadow", new ColorMetadata(32, 32, 32, 1.0) },
    { "HUDGlow", new ColorMetadata(224, 208, 192, 1.0) },
    { "Alert", new ColorMetadata(160, 64, 64, 1.0) },
    { "PanelMain", new ColorMetadata(192, 176, 160, 1.0) },
    { "WidgetPrimaryFill", new ColorMetadata(160, 144, 128, 1.0) },
    { "TargetFocused", new ColorMetadata(128, 112, 96, 1.0) },
    { "TargetFocusedDefault", new ColorMetadata(176, 160, 144, 1.0) },
    { "TargetFocusedOne", new ColorMetadata(208, 192, 176, 1.0) },
    { "EditHighlight", new ColorMetadata(192, 160, 128, 1.0) },
    { "EditHighlightDefault", new ColorMetadata(224, 192, 160, 1.0) },
    { "EditHighlightOne", new ColorMetadata(208, 176, 144, 1.0) },
    { "TargetDefault", new ColorMetadata(128, 112, 96, 1.0) },
    { "TargetDefaultDefault", new ColorMetadata(160, 144, 128, 1.0) },
    { "TargetDefaultOne", new ColorMetadata(176, 160, 144, 1.0) },
    { "WidgetPillStrokeCTA", new ColorMetadata(128, 112, 96, 1.0) },
    { "WidgetPillStroke", new ColorMetadata(96, 80, 64, 1.0) },
    { "WidgetPillFillPressedCTA", new ColorMetadata(160, 144, 128, 1.0) },
    { "WidgetPillFillPressed", new ColorMetadata(144, 128, 112, 1.0) },
    { "WidgetPillTextPressed", new ColorMetadata(64, 48, 32, 1.0) },
    { "WidgetPillStrokeFocused", new ColorMetadata(128, 112, 96, 1.0) },
    { "WidgetPillStrokeDisabled", new ColorMetadata(96, 80, 64, 1.0) },
    { "WidgetButtonStroke", new ColorMetadata(128, 112, 96, 1.0) },
    { "WidgetButtonStrokePressed", new ColorMetadata(160, 144, 128, 1.0) },
    { "WidgetButtonStrokeDisabled", new ColorMetadata(96, 80, 64, 1.0) },
    { "WidgetButtonFill", new ColorMetadata(192, 176, 160, 1.0) },
    { "WidgetButtonFillPressed", new ColorMetadata(176, 160, 144, 1.0) },
    { "WidgetButtonFillDisabled", new ColorMetadata(160, 144, 128, 1.0) },
    { "WidgetStroke", new ColorMetadata(128, 112, 96, 1.0) },
    { "WidgetStrokePressed", new ColorMetadata(160, 144, 128, 1.0) },
    { "WidgetStrokeOn", new ColorMetadata(192, 176, 160, 1.0) },
    { "WidgetStrokeOnPressed", new ColorMetadata(208, 192, 176, 1.0) },
    { "WidgetStrokeDisabled", new ColorMetadata(96, 80, 64, 1.0) },
    { "WidgetCheck", new ColorMetadata(64, 48, 32, 1.0) },
    { "WidgetCheckDisabled", new ColorMetadata(96, 80, 64, 1.0) },
    { "WidgetFill", new ColorMetadata(192, 176, 160, 1.0) },
    { "WidgetFillPressed", new ColorMetadata(176, 160, 144, 1.0) },
    { "WidgetFillOn", new ColorMetadata(208, 192, 176, 1.0) },
    { "WidgetFillOnPressed", new ColorMetadata(224, 208, 192, 1.0) },
    { "WidgetFillDisabled", new ColorMetadata(160, 144, 128, 1.0) },
    { "WidgetFillOnDisabled", new ColorMetadata(176, 160, 144, 1.0) },
    { "MacWidgetMark", new ColorMetadata(128, 112, 96, 1.0) },
    { "MacWidgetMarkDisabled", new ColorMetadata(96, 80, 64, 1.0) },
    { "WinWidgetMark", new ColorMetadata(128, 112, 96, 1.0) },
    { "WinWidgetMarkDisabled", new ColorMetadata(96, 80, 64, 1.0) },
    { "WidgetLightButtonStroke", new ColorMetadata(160, 144, 128, 1.0) },
    { "WidgetLightButtonFill", new ColorMetadata(192, 176, 160, 1.0) },
    { "WidgetFlyoutStroke", new ColorMetadata(128, 112, 96, 1.0) },
    { "WidgetFlyoutStrokeFocused", new ColorMetadata(160, 144, 128, 1.0) },
    { "WidgetFlyoutFill", new ColorMetadata(192, 176, 160, 1.0) },
    { "WidgetFlyoutFillPressed", new ColorMetadata(176, 160, 144, 1.0) },
    { "WidgetScrollbarFill", new ColorMetadata(192, 176, 160, 1.0) },
    { "WidgetScrollbarArrows", new ColorMetadata(128, 112, 96, 1.0) },
    { "WidgetScrollbarElevatorFill", new ColorMetadata(160, 144, 128, 1.0) },
    { "WidgetScrollbarFillPressed", new ColorMetadata(176, 160, 144, 1.0) },
    { "WidgetAnglePicker", new ColorMetadata(160, 144, 128, 1.0) },
    { "WidgetText", new ColorMetadata(64, 48, 32, 1.0) },
    { "WidgetTextDisabled", new ColorMetadata(128, 112, 96, 1.0) },
    { "DefaultText", new ColorMetadata(64, 48, 32, 1.0) },
    { "WidgetLabelText", new ColorMetadata(128, 112, 96, 1.0) },
    { "WidgetLabelTextDisabled", new ColorMetadata(96, 80, 64, 1.0) },
    { "WidgetDivider", new ColorMetadata(160, 144, 128, 1.0) },
    { "WidgetIconButtonRolloverFill", new ColorMetadata(192, 176, 160, 1.0) },
    { "WidgetIconButtonRolloverStroke", new ColorMetadata(128, 112, 96, 1.0) },
    { "WidgetGeneralFrame", new ColorMetadata(192, 176, 160, 1.0) },
    { "WidgetSliderGreySlide", new ColorMetadata(128, 112, 96, 1.0) },
    { "WidgeSliderThumbGreyStroke", new ColorMetadata(96, 80, 64, 1.0) },
    { "WidgetSliderThumbGreySplitter", new ColorMetadata(112, 96, 80, 1.0) },
    { "WidgetSliderThumbGreyFill", new ColorMetadata(160, 144, 128, 1.0) },
    { "WidgetSliderThumbWhiteStroke", new ColorMetadata(224, 208, 192, 1.0) },
    { "WidgetSliderThumbWhiteSplitter", new ColorMetadata(192, 176, 160, 1.0) },
    { "WidgetSliderThumbWhiteFill", new ColorMetadata(208, 192, 176, 1.0) },
    { "WidgetSliderThumbBlackStroke", new ColorMetadata(64, 48, 32, 1.0) },
    { "WidgetSliderThumbBlackFill", new ColorMetadata(128, 112, 96, 1.0) },
    { "WidgetSliderThumbBlackSplitter", new ColorMetadata(96, 80, 64, 1.0) },
    { "SplashBoxText", new ColorMetadata(64, 48, 32, 1.0) },
    { "SplashBoxFill", new ColorMetadata(192, 176, 160, 1.0) },
    { "AboutBoxText", new ColorMetadata(64, 48, 32, 1.0) },
    { "AboutBoxFill", new ColorMetadata(192, 176, 160, 1.0) },
    { "BetaAboutBoxText", new ColorMetadata(64, 48, 32, 1.0) },
    { "BetaAboutBoxFill", new ColorMetadata(176, 160, 144, 1.0) },
    { "AboutBoxLinkText", new ColorMetadata(128, 112, 96, 1.0) },
    { "SlotArtboard", new ColorMetadata(192, 176, 160, 1.0) },
    { "PaletteTitleInActive", new ColorMetadata(160, 144, 128, 1.0) },
    { "PaletteFrame", new ColorMetadata(128, 112, 96, 1.0) },
    { "HyperLink", new ColorMetadata(64, 96, 160, 1.0) },
    { "WidgetSliderThumbStdStroke", new ColorMetadata(128, 112, 96, 1.0) },
    { "WidgetSliderThumbStdFill", new ColorMetadata(160, 144, 128, 1.0) },
    { "WidgetSliderThumbStdSplitter", new ColorMetadata(96, 80, 64, 1.0) },
    { "LineConnector", new ColorMetadata(128, 112, 96, 1.0) },
    { "WidgetEditLowerFrame", new ColorMetadata(192, 176, 160, 1.0) },
    { "WidgetEditLowerFrameDisabled", new ColorMetadata(160, 144, 128, 1.0) },
    { "StylePreviewBackground", new ColorMetadata(224, 208, 192, 1.0) },
    { "LightSwitchFrame", new ColorMetadata(128, 112, 96, 1.0) },
    { "LightSwitchFill", new ColorMetadata(192, 176, 160, 1.0) },
    { "LightSwitchMark", new ColorMetadata(128, 112, 96, 1.0) },
    { "LightSwitchMarkIntermediate", new ColorMetadata(160, 144, 128, 1.0) },
    { "ColorSliderFill", new ColorMetadata(160, 144, 128, 1.0) },
    { "WidgetAlphaBevel", new ColorMetadata(192, 176, 160, 1.0) },
    { "PlotBaseline", new ColorMetadata(128, 112, 96, 1.0) },
    { "WindowsMicroFrame", new ColorMetadata(128, 112, 96, 1.0) },
    { "GroupFrame", new ColorMetadata(160, 144, 128, 1.0) },
    { "GroupFrameDisabled", new ColorMetadata(128, 112, 96, 1.0) },
    { "ColorSliderFrame", new ColorMetadata(128, 112, 96, 1.0) },
    { "MenuColorRed", new ColorMetadata(160, 64, 64, 1.0) },
    { "MenuColorOrange", new ColorMetadata(192, 128, 64, 1.0) },
    { "MenuColorYellow", new ColorMetadata(192, 160, 64, 1.0) },
    { "MenuColorGreen", new ColorMetadata(64, 128, 64, 1.0) },
    { "MenuColorSeafoam", new ColorMetadata(128, 192, 160, 1.0) },
    { "MenuColorBlue", new ColorMetadata(64, 96, 160, 1.0) },
    { "MenuColorIndigo", new ColorMetadata(64, 64, 128, 1.0) },
    { "MenuColorMagenta", new ColorMetadata(128, 64, 128, 1.0) },
    { "MenuColorFuchsia", new ColorMetadata(192, 128, 192, 1.0) },
    { "MenuColorViolet", new ColorMetadata(128, 96, 160, 1.0) },
    { "MenuColorGray", new ColorMetadata(128, 128, 128, 1.0) },
    { "GripperStroke", new ColorMetadata(96, 80, 64, 1.0) },
    { "LayerQuickMaskSelection", new ColorMetadata(160, 144, 128, 1.0) },
    { "ShareButton", new ColorMetadata(192, 176, 160, 1.0) },
    { "ShareButtonRollOver", new ColorMetadata(208, 192, 176, 1.0) },
    { "ShareButtonSelectedRollOver", new ColorMetadata(224, 208, 192, 1.0) },
    { "ShareButtonDisabled", new ColorMetadata(160, 144, 128, 1.0) },
    { "ShareButtonDisabledText", new ColorMetadata(128, 112, 96, 1.0) },
    { "ShareButtonTextNeutralColor", new ColorMetadata(64, 48, 32, 1.0) },
    { "ShareButtonStrokeNeutralColor", new ColorMetadata(128, 112, 96, 1.0) },
    { "Insertion", new ColorMetadata(160, 144, 128, 1.0) },
    { "HistogramSelect", new ColorMetadata(128, 112, 96, 1.0) },
    { "HistogramSelectAlt", new ColorMetadata(96, 80, 64, 1.0) },
    { "HistogramSelectEndPoint", new ColorMetadata(160, 144, 128, 1.0) },
    { "MondoBackgroundColor", new ColorMetadata(192, 176, 160, 1.0) },
    { "DialogOptionsPane", new ColorMetadata(160, 144, 128, 1.0) },
    { "StaticHeadingDescText", new ColorMetadata(64, 48, 32, 1.0) },
    { "ButtonHoverRing", new ColorMetadata(128, 112, 96, 1.0) },
    { "ChooseButton", new ColorMetadata(192, 176, 160, 1.0) },
    { "ChooseButtonRollOver", new ColorMetadata(208, 192, 176, 1.0) },
    { "ModernButtonDimmed", new ColorMetadata(160, 144, 128, 1.0) }
};


        public DesiredTheme PhotoshopOverrideTheme = DesiredTheme.Dark;

        public Dictionary<string, ColorMetadata> Colors { get; private set; }

        public PhotoshopTheme()
        {
            Colors = new Dictionary<string, ColorMetadata>();
        }

        public static PhotoshopTheme LoadColorsFromJson(string jsonFilePath)
        {
            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException("JSON file not found.", jsonFilePath);
            }

            string json = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<PhotoshopTheme>(json);
        }

        public void ApplyTheme(DesiredTheme theme)
        {
            foreach (var color in Colors)
            {
                // Here you can apply the color based on the theme
                // For example, you might want to adjust the RGBA values based on the theme
                // This is a placeholder for your logic
                Console.WriteLine($"Applying {color.Key}: {color.Value} for theme {theme}");
            }
        }
    }

    public class ColorMetadata
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public double A { get; set; }

        public ColorMetadata(byte r, byte g, byte b, double a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public override string ToString()
        {
            return $"RGBA({R}, {G}, {B}, {A})";
        }

        internal Color ToColor()
        {
            return Color.FromArgb(R, G, B);
        }
    }
    public class ColorMetadataConverter : JsonConverter<ColorMetadata>
    {
        public override ColorMetadata ReadJson(
        JsonReader reader,
        Type objectType,
        ColorMetadata? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
        {
            // Read the JSON object
            JObject jsonObject = JObject.Load(reader);

            // Extract RGBA values
            byte r = jsonObject["R"].Value<byte>();
            byte g = jsonObject["G"].Value<byte>();
            byte b = jsonObject["B"].Value<byte>();
            double a = jsonObject["A"].Value<double>();

            return new ColorMetadata(r, g, b, a);
        }

        public override void WriteJson(JsonWriter writer, ColorMetadata value, JsonSerializer serializer)
        {
            // Create a JSON object
            JObject jsonObject = new JObject
        {
            { "R", value.R },
            { "G", value.G },
            { "B", value.B },
            { "A", value.A }
        };

            // Write the JSON object to the writer
            jsonObject.WriteTo(writer);
        }
    }
    //internal class ColorChangerNew
    //{
    //    private static string PhotoshopFolder { get { return CurrentOperation.SelectedPath.AdobeFolder; } }
    //    private static string BackupFolder { get { return Files.AdobeAppBackupFolder; } }
    //    private static string UIColorFilePath
    //    {
    //        get
    //        {
    //            return CurrentOperation.SelectedPath.UIColorsPath;
    //        }
    //    }
    //    private static bool IsCelfile
    //    {
    //        get
    //        {
    //            return new FileInfo(UIColorFilePath).Extension == "cel";
    //        }
    //    }
    //    private static string GetValue(ColorHolder colorHolder)
    //    {
    //        if (IsCelfile)
    //        {
    //            return colorHolder.GetCelValue();
    //        }
    //        return colorHolder.GetTxtValue();
    //    }
    //    private static string GetValue(string name)
    //    {
    //        ColorHolder value = UIColorsData.Instance.MainColor;
    //        if (UIColorsData.Instance.IsAdvancedActivated)
    //        {
    //            value = UIColorsData.GetColorValue(name);
    //        }
    //        return GetValue(value);
    //    }

    //    public static async Task Change()
    //    {
    //        await Misc.RunBackground((s, e) => { ChangeFunc(); });
    //    }
    //    public static async void ChangeFunc()
    //    {
    //        (bool, string) path = GetUIColorsFile.GetPath(PhotoshopFolder);
    //        if (!path.Item1)
    //        {
    //            MessageBoxJ.ShowOK(path.Item2);
    //            return;
    //        }
    //        try
    //        {
    //            if (!(await Backup()))
    //            {
    //                return;
    //            }
    //            string[] obj = (IsCelfile ? ColorValues.CelValues : ColorValues.TxtValues);
    //            string text = obj[^1];
    //            string[] array = obj;
    //            foreach (string text2 in array)
    //            {
    //                await ChangeInUIColors(text2, GetValue(text2), text == text2, false);
    //                if (IsCelfile && text2.Contains("Start"))
    //                {
    //                    string text3 = text2.Clone() as string;
    //                    text3 = text3.Replace("Start", "End");
    //                    await ChangeInUIColors(text3, GetValue(text3), text == text2, false);
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageBoxJ.ShowOK("Error while changing UIColors : " + ex.Message);
    //            return;
    //        }
    //        MessageBoxJ.ShowOK("UIColors successfully changed!");
    //    }
    //    private static async Task<bool> Backup()
    //    {
    //        if (!CreateBackup())
    //        {
    //            bool continueChanging = false;
    //            await MessageBoxJ.ShowYesNo
    //                ("Application couldn't create the backup.\nPlease backup your UIColors.txt/cel.\nDo you want to continue changing color?",
    //                 (s, e) => { continueChanging = true; }
    //                );

    //            if (!continueChanging)
    //            {
    //                return false;
    //            }
    //        }
    //        return true;
    //    }


    //    public static bool CreateBackup()
    //    {
    //        if (File.Exists(BackupFolder + "\\UIColors.txt") || File.Exists(BackupFolder + "\\UIColors.cel"))
    //        {
    //            return true;
    //        }
    //        try
    //        {
    //            if (IsCelfile)
    //            {
    //                File.Copy(UIColorFilePath, BackupFolder + "\\UIColors.cel", overwrite: false);
    //            }
    //            else
    //            {
    //                File.Copy(UIColorFilePath, BackupFolder + "\\UIColors.txt", overwrite: false);
    //            }
    //            return true;
    //        }
    //        catch (Exception)
    //        {
    //            return false;
    //        }
    //    }

    //    public static (bool, string) Restore()
    //    {
    //        Debug
    //            .WriteLine("BackupFolder : " + BackupFolder);
    //        if (!Directory.Exists(BackupFolder))
    //        {
    //            return (false, "Application couldn't find the backup folder.");
    //        }

    //        string UIColors = null;
    //        if (File.Exists(BackupFolder + "\\UIColors.txt"))
    //        {
    //            UIColors = BackupFolder + "\\UIColors.txt";
    //        }
    //        else if (File.Exists(BackupFolder + "\\UIColors.cel"))
    //        {
    //            UIColors = BackupFolder + "\\UIColors.cel";
    //        }
    //        if (UIColors == null)
    //        {
    //            return (false, "Application couldn't find the backup file.");
    //        }
    //        try
    //        {
    //            if (File.Exists(UIColorFilePath))
    //            {
    //                File.Delete(UIColorFilePath);
    //            }
    //            if (IsCelfile)
    //            {
    //                File.Copy(BackupFolder + "\\UIColors.cel", UIColorFilePath, overwrite: true);
    //            }
    //            else
    //            {
    //                File.Copy(BackupFolder + "\\UIColors.txt", UIColorFilePath, overwrite: true);
    //            }
    //            return (true, "Done");
    //        }
    //        catch (Exception ex)
    //        {
    //            return (false, "Error : " + ex.Message);
    //        }
    //    }

    //    public static async Task ChangeInUIColors(string valueToChange, ColorHolder colorHolder, bool SaveAfterEdit = false, bool backup = true)
    //    {
    //        string newValue = GetValue(colorHolder);
    //        await ChangeInUIColors(valueToChange, newValue, SaveAfterEdit, backup, true);
    //    }

    //    static UIColorSet UIColorsFile;
    //    public static void ReadUIColors()
    //    {
    //        Debug.WriteLine("Reading UI Colors");
    //        UIColorsFile = UIColorParser.Deserialize(File.ReadAllText(UIColorFilePath));
    //    }
    //    public static async Task ChangeInUIColors(string valueToChange, string newValue, bool SaveAfterEdit = false, bool backup = true, bool allThemes = false)
    //    {
    //        if (backup)
    //        {
    //            if (!(await Backup()))
    //            {
    //                return;
    //            }
    //        }
    //        if (UIColorsFile == null)
    //        {
    //            ReadUIColors();
    //        }

    //        if (!UIColorsFile.Colors.ContainsKey(valueToChange))
    //        {
    //            Debug.WriteLine($"Color '{valueToChange}' not found.");
    //            return;
    //        }

    //        try
    //        {
    //            // Parse new RGBA from string "r, g, b, a"
    //            var parts = newValue.Split(',')
    //                                .Select(p => double.Parse(p.Trim()))
    //                                .ToArray();
    //            if (parts.Length != 4)
    //            {
    //                Debug.WriteLine("Invalid RGBA format.");
    //                return;
    //            }

    //            if (allThemes)
    //            {
    //                foreach (var theme in UIColorParser.ThemeOrder)
    //                {
    //                    UIColorsFile.Colors[valueToChange][theme] = parts;
    //                }
    //            }
    //            else
    //            {
    //                var themeName = DesiredThemeToString(UIColorsData.Instance.desiredTheme);
    //                UIColorsFile.Colors[valueToChange][themeName] = parts;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.WriteLine($"Error changing color: {ex}");
    //        }


    //        if (SaveAfterEdit)
    //            SaveUIColors();
    //    }
    //    public static void SaveUIColors()
    //    {
    //        Debug.WriteLine("Saving UI Colors");
    //        if (File.Exists(UIColorFilePath))
    //            File.Delete(UIColorFilePath);
    //        File.WriteAllText(UIColorFilePath, UIColorParser.Serialize(UIColorsFile, IsCelfile));
    //    }
    //    private static UIColorsTheme DesiredThemeToString(DesiredTheme theme)
    //    {
    //        return theme switch
    //        {
    //            DesiredTheme.Light => UIColorsTheme.Light,
    //            DesiredTheme.LightGray => UIColorsTheme.LightGray,
    //            DesiredTheme.DarkGray => UIColorsTheme.DarkGray,
    //            DesiredTheme.Dark => UIColorsTheme.Dark,
    //            _ => UIColorsTheme.Dark
    //        };
    //    }
    //}
}
