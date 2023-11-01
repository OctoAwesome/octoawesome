namespace OctoAwesome.Graph;

[Nooson]
public partial class SourceNode : Node
{
    public bool IsOn { get; set; } = true;

    public override int Update(int state)
    {
        return IsOn ? state + 100 : state;
    }
}
