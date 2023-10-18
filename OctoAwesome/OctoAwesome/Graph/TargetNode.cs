using System;

namespace OctoAwesome.Graph;

public class TargetNode : Node
{
    public bool IsOn
    {
        get => isOn; private set
        {
            if (isOn == value)
                return;
            isOn = value;

            StateHasChanged.Invoke(isOn, this.BlockInfo);
        }
    }

    public Action<bool, BlockInfo> StateHasChanged;
    private bool isOn;

    public override int Update(int state)
    {
        IsOn = state >= 50;
        //if (IsOn)
        //    Console.WriteLine("Lamp is now on");
        return IsOn ? state - 50 : state;
    }
}