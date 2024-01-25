using System;
using System.Collections.Generic;
using CitadelFix.GUI;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace CitadelFix;


public abstract class CitadelGui : GuiDialog
{
    protected CitadelGui(ICoreClientAPI capi) : base(capi)
    {
    }

    public abstract KeyCombination KeybindCombo { get; }
    public abstract string KeybindName { get; }
    public abstract HotkeyType KeybindType { get; }

    public virtual bool KeybindHandler(KeyCombination comb) {
        if(this.IsOpened()){
            this.TryClose();
        }else{
            this.TryOpen();
        }
        return true;
    }
}


public class CitadelGUISystem : ModSystem
{

    internal ICoreClientAPI capi;
    internal Dictionary<string, GuiDialog> dialogs = new();

    public override bool ShouldLoad(EnumAppSide forSide)
    {
        return forSide == EnumAppSide.Client;
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        capi = api;
        registerDialog(new ReinforcementGroupGUI(api));
    }

    internal void registerDialog(CitadelGui dialog){
        this.dialogs[dialog.GetType().Name] = dialog;
        capi.Input.RegisterHotKey(
            dialog.ToggleKeyCombinationCode,
            dialog.KeybindName,
            (GlKeys)dialog.KeybindCombo.KeyCode,
            dialog.KeybindType,
            dialog.KeybindCombo.Alt,
            dialog.KeybindCombo.Ctrl,
            dialog.KeybindCombo.Shift
        );
        capi.Input.SetHotKeyHandler(dialog.ToggleKeyCombinationCode, dialog.KeybindHandler);
    }

    internal T getDialog<T>() where T : CitadelGui {
        return this.dialogs[typeof(T).Name] == null ? null : this.dialogs[typeof(T).Name] as T;
    }
}