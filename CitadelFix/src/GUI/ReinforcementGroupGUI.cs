using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CitadelFix.Util;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace CitadelFix.GUI;


public class ReinforcementGroupEntry : IFlatListItem
{
    public bool Visible => true;

    public string GroupName {get; set;} = "Reinforcement Group";

    private LoadedTexture _texture;

    public ReinforcementGroupEntry(string groupName = null){
        GroupName = groupName ?? "Reinforcement Group";
    }

    public void Compose(ICoreClientAPI capi){
        _texture?.Dispose();
        _texture = new TextTextureUtil(capi).GenTextTexture(GroupName, CairoFont.WhiteDetailText());
    }

    public void Dispose()
    {
        _texture?.Dispose();
    }

    public void RenderListEntryTo(ICoreClientAPI capi, float dt, double x, double y, double cellWidth, double cellHeight)
    {
        if(_texture == null){
            this.Compose(capi);
        }

        double xPadding = GuiElement.scaled(4);

        capi.Render.Render2DTexturePremultipliedAlpha(_texture.TextureId, x + xPadding, y, _texture.Width, _texture.Height);
    }
}

public class ReinforcementGroupGUI : CitadelGui
{
    
    public override string ToggleKeyCombinationCode => "reinforcementGroupGui";

    public override KeyCombination KeybindCombo => new KeyCombination() {
        KeyCode = (int)GlKeys.BracketRight,
        Ctrl = true
    };

    public override string KeybindName => "CitadelFix Group Overhaul";

    public override HotkeyType KeybindType => HotkeyType.GUIOrOtherControls;

    public List<ReinforcementGroupEntry> ReinforcementGroups {get; set;} = new();

    private const string DROPDOWN_KEY = "reinforcementGroupSelect";

    private string _selection = null;

    private GuiTab[] _tabs = new GuiTab[2]{
        new GuiTab(){
            Name = "Roles",
            DataInt = 0
        },
        new GuiTab(){
            Name = "Members",
            DataInt = 1
        }
    };


    public ReinforcementGroupGUI(ICoreClientAPI capi) : base(capi)
    {
        for(int i = 0; i < 100; i++){
            ReinforcementGroups.Add(new ReinforcementGroupEntry($"Reinforcement Group {i}"));
        }
        this.Compose();
    }

    private void Compose(){
        // Auto-sized dialog at the center of the screen
        ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);

        // Create the content bounds
        ElementBounds contentBounds = ElementBounds.Fixed(
            GuiStyle.ElementToDialogPadding * 0.5f, 
            GuiStyle.TitleBarHeight + GuiStyle.ElementToDialogPadding, 
            GuiElement.scaled(500), 
            GuiElement.scaled(300)
        );

        // Create the background bounds
        ElementBounds bgBounds = ElementBounds.Fill
            //.WithFixedPadding(GuiStyle.ElementToDialogPadding)
            .WithSizing(ElementSizing.FitToChildren)
            .WithChildren(contentBounds);

        ElementBounds dropdownBounds = ElementBounds.Fixed(0, 0, contentBounds.fixedWidth * 0.6f, GuiStyle.ElementToDialogPadding+GuiStyle.HalfPadding)
            .WithParent(contentBounds);

        ElementBounds innerContentBounds = ElementBounds.Fixed(
            0, 
            dropdownBounds.fixedHeight + GuiStyle.ElementToDialogPadding + GuiStyle.HalfPadding + 1, 
            contentBounds.fixedWidth - GuiStyle.ElementToDialogPadding * 0.5f, 
            contentBounds.fixedHeight - (GuiStyle.TitleBarHeight + GuiStyle.ElementToDialogPadding) - GuiStyle.HalfPadding
        ).WithParent(contentBounds);

        ElementBounds tabsBounds = innerContentBounds.FlatCopy()
            .WithFixedHeight(GuiStyle.ElementToDialogPadding)
            .WithFixedPosition(innerContentBounds.fixedX, innerContentBounds.fixedY - GuiStyle.ElementToDialogPadding-1);


        // Lastly, create the dialog
        var composer = capi.Gui.CreateCompo(ToggleKeyCombinationCode, dialogBounds)
            .AddShadedDialogBG(bgBounds)
            .AddDialogTitleBar("Reinforcement Group Management", () => this.TryClose());
        
        composer.BeginChildElements(contentBounds);

            var keysAndVals = ReinforcementGroups.Select(x => x.GroupName).ToArray();
            _selection = keysAndVals[0];
            composer.AddDropDown(keysAndVals, keysAndVals, 0, (val, selected) => {
                if(selected){
                    _selection = val;
                    composer.GetDynamicText("selectedGroup").Text = "Selected Group: " + _selection;
                    composer.GetDynamicText("selectedGroup").RecomposeText();
                }
            }, dropdownBounds);
            //ElementBounds.Fixed(0, 0, innerContentBounds.fixedWidth, GuiStyle.ElementToDialogPadding)
            composer.AddHorizontalTabs(_tabs, tabsBounds, (tabIndex) => {

            }, CairoFont.WhiteDetailText(), CairoFont.WhiteSmallText());
            composer.AddInset(innerContentBounds);
            //
            composer.BeginChildElements(innerContentBounds.ForkBoundingParent());
            //composer.AddStaticText("Selected Group: " + _selection, CairoFont.WhiteDetailText(), );
                composer.AddDynamicText("Selected Group: " + _selection, CairoFont.WhiteDetailText(), ElementBounds.Fill.WithAlignment(EnumDialogArea.CenterMiddle), "selectedGroup");
            composer.EndChildElements();
            //composer.AddIf(_selection != null);
            //composer.EndIf();

        composer.EndChildElements();

        this.SingleComposer = composer.Compose();
    }

    /*
    private void UpdateScrollbar(){
        this._scrollbar.Bounds.CalcWorldBounds();
        this._scrollbar.SetHeights((float)this._listClipBounds.fixedHeight, (float)this._list.insideBounds.fixedHeight);
    }
    */

}



/*
        double guiPadding = Constants.GuiPadding;
        double guiElementSpacing = Constants.GuiElementSpacing;
        ElementBounds bounds1 = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.LeftTop).WithFixedAlignmentOffset(GuiStyle.DialogToScreenPadding, GuiStyle.DialogToScreenPadding);
        ElementBounds bounds2 = ElementBounds.Fill.WithFixedPadding(guiPadding);
        bounds2.BothSizing = ElementSizing.FitToChildren;
        int num1 = 3;
        double guiLabelHeight = Constants.GuiLabelHeight;
        int height1 = 30;
        int num2 = 240;
        double num3 = (double) (num2 * 2) + guiElementSpacing;
        int width1 = 14;
        int amount = 4;
        double height2 = Math.Min(GuiElement.scaled(400.0), (double) this._rapi.FrameHeight - GuiElement.scaled(100.0 + ((double) height1 + guiLabelHeight + (double) num1 + guiElementSpacing * guiPadding) * 2.0));
        int num4 = 20;
        int width2 = 120;
        ElementBounds bounds3 = ElementBounds.Fixed(0.0, 20.0 + guiElementSpacing, (double) num2, guiLabelHeight);
        ElementBounds bounds4 = bounds3.BelowCopy(fixedDeltaY: (double) num1).WithFixedHeight((double) height1);
        ElementBounds bounds5 = bounds3.RightCopy(guiElementSpacing);
        ElementBounds bounds6 = bounds5.BelowCopy(fixedDeltaY: (double) num1).WithFixedHeight((double) height1);
        ElementBounds bounds7 = bounds4.BelowCopy(fixedDeltaY: guiElementSpacing).WithFixedSize(num3 - (double) width2 - guiElementSpacing, guiLabelHeight);
        ElementBounds bounds8 = bounds7.BelowCopy(fixedDeltaY: (double) num1).WithFixedHeight((double) height1);
        ElementBounds bounds9 = bounds8.RightCopy(guiElementSpacing, -1.0, fixedDeltaHeight: 2.0).WithFixedWidth((double) width2);
        ElementBounds bounds10 = bounds8.BelowCopy(fixedDeltaY: guiElementSpacing).WithFixedSize((double) num2, guiLabelHeight);
        ElementBounds bounds11 = bounds10.BelowCopy(fixedDeltaY: (double) num1).WithFixedSize((double) (num2 - amount * 2 - width1), height2);
        ElementBounds bounds12 = bounds11.ForkBoundingParent();
        ElementBounds bounds13 = bounds11.FlatCopy().FixedGrow((double) amount);
        ElementBounds bounds14 = bounds13.CopyOffsetedSibling(bounds11.fixedWidth + guiElementSpacing).WithFixedWidth((double) width1);
        ElementBounds bounds15 = bounds10.RightCopy(guiElementSpacing);
        ElementBounds bounds16 = bounds15.BelowCopy(fixedDeltaY: (double) num1).WithFixedSize(bounds11.fixedWidth, bounds11.fixedHeight);
        ElementBounds bounds17 = bounds16.ForkBoundingParent();
        ElementBounds bounds18 = bounds16.FlatCopy().FixedGrow((double) amount);
        ElementBounds bounds19 = bounds18.CopyOffsetedSibling(bounds16.fixedWidth + guiElementSpacing).WithFixedWidth((double) width1);
        ElementBounds elementBounds = bounds16;
        double num5 = guiPadding;
        double fixedDeltaX = (double) (num2 / 2);
        double fixedDeltaY = num5;
        ElementBounds bounds20 = elementBounds.BelowCopy(fixedDeltaX, fixedDeltaY).WithFixedSize((double) (num2 / 2), 30.0);
        ElementBounds bounds21 = bounds12.BelowCopy(fixedDeltaY: guiPadding - 5.0).WithFixedSize((double) num4, (double) num4);
        ElementBounds bounds22 = bounds21.RightCopy((double) num1, -1.0).WithFixedWidth((double) (num2 - num4));
        ElementBounds bounds23 = bounds21.BelowCopy(fixedDeltaY: guiElementSpacing);
        ElementBounds bounds24 = bounds23.RightCopy((double) num1, -1.0).WithFixedWidth(bounds22.fixedWidth);
        CairoFont font1 = CairoFont.WhiteSmallText();
        CairoFont font2 = CairoFont.TextInput();
        this.ClearComposers();
        this.SingleComposer = this.capi.Gui.CreateCompo(this._composerKey, bounds1)
            .AddShadedDialogBG(bounds2)
            .AddDialogTitleBar("Select targets to overlay", (Action) (() => this.TryClose()))
            .BeginChildElements(bounds2).AddStaticText("Scan radius (blocks)", font1, bounds3)
                .AddNumberInput(bounds4, (Action<string>) (value => this.HandleValueChanged("maxRange", value)), font2, "maxRange")
                .AddStaticText("Label radius (blocks)", font1, bounds5)
                .AddNumberInput(bounds6, (Action<string>) (value => this.HandleValueChanged("labelRange", value)), font2, "labelRange")
                .AddStaticText("Search for a block or entity", font1, bounds7).AddAutoSizeHoverText("Search by asset code for exact matches!", font1, num2, bounds7)
                .AddTextInput(bounds8, (Action<string>) (value => this.HandleValueChanged("search", value)), font2, "search")
                .AddSmallButton("Saved groups", new ActionConsumable(this.HandleSavedGroupsClicked), bounds9, EnumButtonStyle.Small)
                .AddStaticText("Results", font1, bounds10)
                .AddStaticText("Active targets", font1, bounds15)
                .BeginClip(bounds12).AddInset(bounds13)
                    .AddCellList<BlockCodeGroup>(bounds11, new OnRequireCell<BlockCodeGroup>(this.ResultListCellCreator), new List<BlockCodeGroup>(), "searchResults")
                .EndClip()
                .AddVerticalScrollbar(new Action<float>(this.OnNewResultsScrollbarValue), bounds14, "searchResultsScrollbar")
                .BeginClip(bounds17)
                    .AddInset(bounds18)
                    .AddCellList<BlockCodeGroup>(bounds16, new OnRequireCell<BlockCodeGroup>(this.ActiveListCellCreator), new List<BlockCodeGroup>(), "activeBlocks")
                .EndClip()
                .AddVerticalScrollbar(new Action<float>(this.OnNewActiveScrollbarValue), bounds19, "selectedResultsScrollbar")
                .AddSwitch((Action<bool>) (value => this.HandleToggleCheckbox("shouldGroupFar", value)), bounds21, "shouldGroupFar", (double) (num4 - 4), 2.0)
                .AddStaticText("Group far results?", font1, bounds22)
                .AddSwitch((Action<bool>) (value => this.HandleToggleCheckbox("showDebug", value)), bounds23, "showDebug", (double) (num4 - 4), 2.0)
                .AddStaticText("Show debug info on overlay?", font1, bounds24)
                .AddSmallButton("Scan", new ActionConsumable(this.HandleScanClicked), bounds20, EnumButtonStyle.Small, "scanButton")
            .EndChildElements()
            .Compose();
        this.SingleComposer.GetTextInput("search").SetPlaceHolderText("Block/Entity name or asset code");
        GuiElementNumberInput numberInput1 = this.SingleComposer.GetNumberInput("maxRange");
        numberInput1.SetPlaceHolderText("# Blocks");
        numberInput1.SetValue((float) this._modSystem.Config.MaxSearchRadius);
        GuiElementNumberInput numberInput2 = this.SingleComposer.GetNumberInput("labelRange");
        numberInput2.SetPlaceHolderText("# Blocks");
        numberInput2.SetValue((float) this._modSystem.Config.MaxLabelRadius);
        this.SingleComposer.GetSwitch("shouldGroupFar").SetValue(this._modSystem.Config.ShouldCompactFarGroups);
        this.SingleComposer.GetSwitch("showDebug").SetValue(this._modSystem.Config.ShouldShowDebugText);
        this._searchResultsList = this.SingleComposer.GetCellList<BlockCodeGroup>("searchResults");
        this._activeBlocksList = this.SingleComposer.GetCellList<BlockCodeGroup>("activeBlocks");
        this._searchResultsList.unscaledCellSpacing = 0;
        this._activeBlocksList.unscaledCellSpacing = 0;
        this._searchResultsScrollbar = this.SingleComposer.GetScrollbar("searchResultsScrollbar");
        this._searchResultsClipBounds = bounds12;
        this._searchResultsListBounds = bounds11;
        this._selectedResultsScrollbar = this.SingleComposer.GetScrollbar("selectedResultsScrollbar");
        this._selectedResultsClipBounds = bounds17;
        this._selectedResultsListBounds = bounds16;
        this._activeBlocksList.ReloadCells(this._modSystem.SearchGroups.ToList<BlockCodeGroup>());
        this.UpdateSearchResultsScrollbar();
        this.UpdateSelectedResultsScrollbar();
        */