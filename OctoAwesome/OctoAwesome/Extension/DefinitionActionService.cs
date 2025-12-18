using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Extension;
public partial class DefinitionActionService
{
    private readonly Dictionary<string, Dictionary<string, List<Delegate>>> methodsPerDefinition = [];
    private readonly IDefinitionManager manager;

    public DefinitionActionService(IDefinitionManager manager)
    {
        this.manager = manager;
    }

    public void Register(string methodName, string definitionId, Delegate method)
    {
        ref var definitionDelegates = ref CollectionsMarshal.GetValueRefOrAddDefault(methodsPerDefinition, methodName, out var exists);
        if (!exists)
        {
            definitionDelegates = new();
        }
        ref var delegates = ref CollectionsMarshal.GetValueRefOrAddDefault(definitionDelegates!, definitionId, out exists);
        if (!exists)
            delegates = new();
        delegates!.Add(method);
    }

    public void RegisterMultiple(string methodName, Delegate method, params string[] definitionIds)
    {
        ref var definitionDelegates = ref CollectionsMarshal.GetValueRefOrAddDefault(methodsPerDefinition, methodName, out var exists);
        if (!exists)
        {
            definitionDelegates = new();
        }

        foreach (string definitionId in definitionIds)
        {
            ref var delegates = ref CollectionsMarshal.GetValueRefOrAddDefault(definitionDelegates!, definitionId, out exists);
            if (!exists)
                delegates = new();
            delegates!.Add(method);
        }
    }

    public bool IsRegistered(string methodName, string definitionId)
    {
        ref var definitionDelegates = ref CollectionsMarshal.GetValueRefOrNullRef(methodsPerDefinition, methodName);
        return Unsafe.IsNullRef(ref definitionDelegates)
            ? false
            : definitionDelegates.ContainsKey(definitionId);
    }


}
