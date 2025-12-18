using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Extension;


/// <summary>
/// Service for managing and invoking actions associated with definitions.
/// </summary>
public partial class DefinitionActionService
{
    private readonly Dictionary<string, Dictionary<string, List<Delegate>>> methodsPerDefinition = [];
    private readonly IDefinitionManager manager;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefinitionActionService"/> class.
    /// </summary>
    /// <param name="manager">The definition manager.</param>
    public DefinitionActionService(IDefinitionManager manager)
    {
        this.manager = manager;
    }

    /// <summary>
    /// Registers a method for a specific definition.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="definitionId">The ID of the definition.</param>
    /// <param name="method">The method to register.</param>
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

    /// <summary>
    /// Registers a method for multiple definitions.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="method">The method to register.</param>
    /// <param name="definitionIds">The IDs of the definitions.</param>
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

    /// <summary>
    /// Checks if a method is registered for a specific definition.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="definitionId">The ID of the definition.</param>
    /// <returns><c>true</c> if the method is registered; otherwise, <c>false</c>.</returns>
    public bool IsRegistered(string methodName, string definitionId)
    {
        ref var definitionDelegates = ref CollectionsMarshal.GetValueRefOrNullRef(methodsPerDefinition, methodName);
        return Unsafe.IsNullRef(ref definitionDelegates)
            ? false
            : definitionDelegates.ContainsKey(definitionId);
    }
}
