using OctoAwesome.Definitions;
using System;

namespace OctoAwesome.Extension;
partial class DefinitionActionService
{    ///<inheritdoc>
    public void Action<T1>(string actionName, IDefinition definition, T1 param1)
        => Action(actionName, manager.GetUniqueKeyByDefinition(definition), definition, param1);
    ///<inheritdoc>
    public void Action<T1>(string actionName, string definitionKey, T1 param1)
        => Action(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), param1);

    ///<inheritdoc>
    private void Action<T1>(string actionName, string definitionKey, IDefinition definition,T1 param1)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return;

        foreach (var item in actions)
        {
            if (item is Action<IDefinition,T1> act)
                act.Invoke(definition, param1);
        }
    }

    ///<inheritdoc>
    public TRet? Function<TRet,T1>(string actionName, IDefinition definition, TRet? defaultValue, T1 param1)
        => Function<TRet, T1>(actionName, manager.GetUniqueKeyByDefinition(definition), definition, defaultValue, param1);
    ///<inheritdoc>
    public TRet? Function<TRet,T1>(string actionName, string definitionKey, TRet? defaultValue, T1 param1)
        => Function<TRet, T1>(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), defaultValue, param1);
    ///<inheritdoc>
    public TRet? Function<TRet,T1>(string actionName, string definitionKey, IDefinition definition, TRet? defaultValue, T1 param1)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return defaultValue;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return defaultValue;

        TRet? lastResult = default;
        foreach (var item in actions)
        {
            if (item is Func<TRet?, IDefinition,T1, TRet> act)
                lastResult = act.Invoke(lastResult, definition, param1);
        }
        return lastResult;
    }
    ///<inheritdoc>
    public void Action<T1, T2>(string actionName, IDefinition definition, T1 param1, T2 param2)
        => Action(actionName, manager.GetUniqueKeyByDefinition(definition), definition, param1, param2);
    ///<inheritdoc>
    public void Action<T1, T2>(string actionName, string definitionKey, T1 param1, T2 param2)
        => Action(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), param1, param2);

    ///<inheritdoc>
    private void Action<T1, T2>(string actionName, string definitionKey, IDefinition definition,T1 param1, T2 param2)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return;

        foreach (var item in actions)
        {
            if (item is Action<IDefinition,T1, T2> act)
                act.Invoke(definition, param1, param2);
        }
    }

    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2>(string actionName, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2)
        => Function<TRet, T1, T2>(actionName, manager.GetUniqueKeyByDefinition(definition), definition, defaultValue, param1, param2);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2>(string actionName, string definitionKey, TRet? defaultValue, T1 param1, T2 param2)
        => Function<TRet, T1, T2>(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), defaultValue, param1, param2);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2>(string actionName, string definitionKey, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return defaultValue;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return defaultValue;

        TRet? lastResult = default;
        foreach (var item in actions)
        {
            if (item is Func<TRet?, IDefinition,T1, T2, TRet> act)
                lastResult = act.Invoke(lastResult, definition, param1, param2);
        }
        return lastResult;
    }
    ///<inheritdoc>
    public void Action<T1, T2, T3>(string actionName, IDefinition definition, T1 param1, T2 param2, T3 param3)
        => Action(actionName, manager.GetUniqueKeyByDefinition(definition), definition, param1, param2, param3);
    ///<inheritdoc>
    public void Action<T1, T2, T3>(string actionName, string definitionKey, T1 param1, T2 param2, T3 param3)
        => Action(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), param1, param2, param3);

    ///<inheritdoc>
    private void Action<T1, T2, T3>(string actionName, string definitionKey, IDefinition definition,T1 param1, T2 param2, T3 param3)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return;

        foreach (var item in actions)
        {
            if (item is Action<IDefinition,T1, T2, T3> act)
                act.Invoke(definition, param1, param2, param3);
        }
    }

    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3>(string actionName, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3)
        => Function<TRet, T1, T2, T3>(actionName, manager.GetUniqueKeyByDefinition(definition), definition, defaultValue, param1, param2, param3);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3>(string actionName, string definitionKey, TRet? defaultValue, T1 param1, T2 param2, T3 param3)
        => Function<TRet, T1, T2, T3>(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), defaultValue, param1, param2, param3);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3>(string actionName, string definitionKey, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return defaultValue;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return defaultValue;

        TRet? lastResult = default;
        foreach (var item in actions)
        {
            if (item is Func<TRet?, IDefinition,T1, T2, T3, TRet> act)
                lastResult = act.Invoke(lastResult, definition, param1, param2, param3);
        }
        return lastResult;
    }
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4>(string actionName, IDefinition definition, T1 param1, T2 param2, T3 param3, T4 param4)
        => Action(actionName, manager.GetUniqueKeyByDefinition(definition), definition, param1, param2, param3, param4);
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4>(string actionName, string definitionKey, T1 param1, T2 param2, T3 param3, T4 param4)
        => Action(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), param1, param2, param3, param4);

    ///<inheritdoc>
    private void Action<T1, T2, T3, T4>(string actionName, string definitionKey, IDefinition definition,T1 param1, T2 param2, T3 param3, T4 param4)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return;

        foreach (var item in actions)
        {
            if (item is Action<IDefinition,T1, T2, T3, T4> act)
                act.Invoke(definition, param1, param2, param3, param4);
        }
    }

    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4>(string actionName, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4)
        => Function<TRet, T1, T2, T3, T4>(actionName, manager.GetUniqueKeyByDefinition(definition), definition, defaultValue, param1, param2, param3, param4);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4>(string actionName, string definitionKey, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4)
        => Function<TRet, T1, T2, T3, T4>(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), defaultValue, param1, param2, param3, param4);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4>(string actionName, string definitionKey, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return defaultValue;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return defaultValue;

        TRet? lastResult = default;
        foreach (var item in actions)
        {
            if (item is Func<TRet?, IDefinition,T1, T2, T3, T4, TRet> act)
                lastResult = act.Invoke(lastResult, definition, param1, param2, param3, param4);
        }
        return lastResult;
    }
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5>(string actionName, IDefinition definition, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        => Action(actionName, manager.GetUniqueKeyByDefinition(definition), definition, param1, param2, param3, param4, param5);
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5>(string actionName, string definitionKey, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        => Action(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), param1, param2, param3, param4, param5);

    ///<inheritdoc>
    private void Action<T1, T2, T3, T4, T5>(string actionName, string definitionKey, IDefinition definition,T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return;

        foreach (var item in actions)
        {
            if (item is Action<IDefinition,T1, T2, T3, T4, T5> act)
                act.Invoke(definition, param1, param2, param3, param4, param5);
        }
    }

    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5>(string actionName, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        => Function<TRet, T1, T2, T3, T4, T5>(actionName, manager.GetUniqueKeyByDefinition(definition), definition, defaultValue, param1, param2, param3, param4, param5);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5>(string actionName, string definitionKey, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        => Function<TRet, T1, T2, T3, T4, T5>(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), defaultValue, param1, param2, param3, param4, param5);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5>(string actionName, string definitionKey, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return defaultValue;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return defaultValue;

        TRet? lastResult = default;
        foreach (var item in actions)
        {
            if (item is Func<TRet?, IDefinition,T1, T2, T3, T4, T5, TRet> act)
                lastResult = act.Invoke(lastResult, definition, param1, param2, param3, param4, param5);
        }
        return lastResult;
    }
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6>(string actionName, IDefinition definition, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        => Action(actionName, manager.GetUniqueKeyByDefinition(definition), definition, param1, param2, param3, param4, param5, param6);
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6>(string actionName, string definitionKey, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        => Action(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), param1, param2, param3, param4, param5, param6);

    ///<inheritdoc>
    private void Action<T1, T2, T3, T4, T5, T6>(string actionName, string definitionKey, IDefinition definition,T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return;

        foreach (var item in actions)
        {
            if (item is Action<IDefinition,T1, T2, T3, T4, T5, T6> act)
                act.Invoke(definition, param1, param2, param3, param4, param5, param6);
        }
    }

    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6>(string actionName, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        => Function<TRet, T1, T2, T3, T4, T5, T6>(actionName, manager.GetUniqueKeyByDefinition(definition), definition, defaultValue, param1, param2, param3, param4, param5, param6);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6>(string actionName, string definitionKey, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        => Function<TRet, T1, T2, T3, T4, T5, T6>(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), defaultValue, param1, param2, param3, param4, param5, param6);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6>(string actionName, string definitionKey, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return defaultValue;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return defaultValue;

        TRet? lastResult = default;
        foreach (var item in actions)
        {
            if (item is Func<TRet?, IDefinition,T1, T2, T3, T4, T5, T6, TRet> act)
                lastResult = act.Invoke(lastResult, definition, param1, param2, param3, param4, param5, param6);
        }
        return lastResult;
    }
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7>(string actionName, IDefinition definition, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
        => Action(actionName, manager.GetUniqueKeyByDefinition(definition), definition, param1, param2, param3, param4, param5, param6, param7);
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7>(string actionName, string definitionKey, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
        => Action(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), param1, param2, param3, param4, param5, param6, param7);

    ///<inheritdoc>
    private void Action<T1, T2, T3, T4, T5, T6, T7>(string actionName, string definitionKey, IDefinition definition,T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return;

        foreach (var item in actions)
        {
            if (item is Action<IDefinition,T1, T2, T3, T4, T5, T6, T7> act)
                act.Invoke(definition, param1, param2, param3, param4, param5, param6, param7);
        }
    }

    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7>(string actionName, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7>(actionName, manager.GetUniqueKeyByDefinition(definition), definition, defaultValue, param1, param2, param3, param4, param5, param6, param7);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7>(string actionName, string definitionKey, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7>(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), defaultValue, param1, param2, param3, param4, param5, param6, param7);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7>(string actionName, string definitionKey, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return defaultValue;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return defaultValue;

        TRet? lastResult = default;
        foreach (var item in actions)
        {
            if (item is Func<TRet?, IDefinition,T1, T2, T3, T4, T5, T6, T7, TRet> act)
                lastResult = act.Invoke(lastResult, definition, param1, param2, param3, param4, param5, param6, param7);
        }
        return lastResult;
    }
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7, T8>(string actionName, IDefinition definition, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
        => Action(actionName, manager.GetUniqueKeyByDefinition(definition), definition, param1, param2, param3, param4, param5, param6, param7, param8);
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7, T8>(string actionName, string definitionKey, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
        => Action(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), param1, param2, param3, param4, param5, param6, param7, param8);

    ///<inheritdoc>
    private void Action<T1, T2, T3, T4, T5, T6, T7, T8>(string actionName, string definitionKey, IDefinition definition,T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return;

        foreach (var item in actions)
        {
            if (item is Action<IDefinition,T1, T2, T3, T4, T5, T6, T7, T8> act)
                act.Invoke(definition, param1, param2, param3, param4, param5, param6, param7, param8);
        }
    }

    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8>(string actionName, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7, T8>(actionName, manager.GetUniqueKeyByDefinition(definition), definition, defaultValue, param1, param2, param3, param4, param5, param6, param7, param8);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8>(string actionName, string definitionKey, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7, T8>(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), defaultValue, param1, param2, param3, param4, param5, param6, param7, param8);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8>(string actionName, string definitionKey, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return defaultValue;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return defaultValue;

        TRet? lastResult = default;
        foreach (var item in actions)
        {
            if (item is Func<TRet?, IDefinition,T1, T2, T3, T4, T5, T6, T7, T8, TRet> act)
                lastResult = act.Invoke(lastResult, definition, param1, param2, param3, param4, param5, param6, param7, param8);
        }
        return lastResult;
    }
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string actionName, IDefinition definition, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
        => Action(actionName, manager.GetUniqueKeyByDefinition(definition), definition, param1, param2, param3, param4, param5, param6, param7, param8, param9);
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string actionName, string definitionKey, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
        => Action(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), param1, param2, param3, param4, param5, param6, param7, param8, param9);

    ///<inheritdoc>
    private void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string actionName, string definitionKey, IDefinition definition,T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return;

        foreach (var item in actions)
        {
            if (item is Action<IDefinition,T1, T2, T3, T4, T5, T6, T7, T8, T9> act)
                act.Invoke(definition, param1, param2, param3, param4, param5, param6, param7, param8, param9);
        }
    }

    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9>(string actionName, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9>(actionName, manager.GetUniqueKeyByDefinition(definition), definition, defaultValue, param1, param2, param3, param4, param5, param6, param7, param8, param9);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9>(string actionName, string definitionKey, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9>(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), defaultValue, param1, param2, param3, param4, param5, param6, param7, param8, param9);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9>(string actionName, string definitionKey, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return defaultValue;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return defaultValue;

        TRet? lastResult = default;
        foreach (var item in actions)
        {
            if (item is Func<TRet?, IDefinition,T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet> act)
                lastResult = act.Invoke(lastResult, definition, param1, param2, param3, param4, param5, param6, param7, param8, param9);
        }
        return lastResult;
    }
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string actionName, IDefinition definition, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10)
        => Action(actionName, manager.GetUniqueKeyByDefinition(definition), definition, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10);
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string actionName, string definitionKey, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10)
        => Action(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), param1, param2, param3, param4, param5, param6, param7, param8, param9, param10);

    ///<inheritdoc>
    private void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string actionName, string definitionKey, IDefinition definition,T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return;

        foreach (var item in actions)
        {
            if (item is Action<IDefinition,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> act)
                act.Invoke(definition, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10);
        }
    }

    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string actionName, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(actionName, manager.GetUniqueKeyByDefinition(definition), definition, defaultValue, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string actionName, string definitionKey, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), defaultValue, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string actionName, string definitionKey, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return defaultValue;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return defaultValue;

        TRet? lastResult = default;
        foreach (var item in actions)
        {
            if (item is Func<TRet?, IDefinition,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet> act)
                lastResult = act.Invoke(lastResult, definition, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10);
        }
        return lastResult;
    }
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string actionName, IDefinition definition, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11)
        => Action(actionName, manager.GetUniqueKeyByDefinition(definition), definition, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11);
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string actionName, string definitionKey, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11)
        => Action(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11);

    ///<inheritdoc>
    private void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string actionName, string definitionKey, IDefinition definition,T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return;

        foreach (var item in actions)
        {
            if (item is Action<IDefinition,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> act)
                act.Invoke(definition, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11);
        }
    }

    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string actionName, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(actionName, manager.GetUniqueKeyByDefinition(definition), definition, defaultValue, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string actionName, string definitionKey, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), defaultValue, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string actionName, string definitionKey, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return defaultValue;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return defaultValue;

        TRet? lastResult = default;
        foreach (var item in actions)
        {
            if (item is Func<TRet?, IDefinition,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRet> act)
                lastResult = act.Invoke(lastResult, definition, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11);
        }
        return lastResult;
    }
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string actionName, IDefinition definition, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12)
        => Action(actionName, manager.GetUniqueKeyByDefinition(definition), definition, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12);
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string actionName, string definitionKey, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12)
        => Action(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12);

    ///<inheritdoc>
    private void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string actionName, string definitionKey, IDefinition definition,T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return;

        foreach (var item in actions)
        {
            if (item is Action<IDefinition,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> act)
                act.Invoke(definition, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12);
        }
    }

    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string actionName, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(actionName, manager.GetUniqueKeyByDefinition(definition), definition, defaultValue, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string actionName, string definitionKey, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), defaultValue, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string actionName, string definitionKey, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return defaultValue;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return defaultValue;

        TRet? lastResult = default;
        foreach (var item in actions)
        {
            if (item is Func<TRet?, IDefinition,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet> act)
                lastResult = act.Invoke(lastResult, definition, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12);
        }
        return lastResult;
    }
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string actionName, IDefinition definition, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13)
        => Action(actionName, manager.GetUniqueKeyByDefinition(definition), definition, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13);
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string actionName, string definitionKey, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13)
        => Action(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13);

    ///<inheritdoc>
    private void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string actionName, string definitionKey, IDefinition definition,T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return;

        foreach (var item in actions)
        {
            if (item is Action<IDefinition,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> act)
                act.Invoke(definition, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13);
        }
    }

    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string actionName, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(actionName, manager.GetUniqueKeyByDefinition(definition), definition, defaultValue, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string actionName, string definitionKey, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), defaultValue, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string actionName, string definitionKey, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return defaultValue;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return defaultValue;

        TRet? lastResult = default;
        foreach (var item in actions)
        {
            if (item is Func<TRet?, IDefinition,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRet> act)
                lastResult = act.Invoke(lastResult, definition, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13);
        }
        return lastResult;
    }
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string actionName, IDefinition definition, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14)
        => Action(actionName, manager.GetUniqueKeyByDefinition(definition), definition, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14);
    ///<inheritdoc>
    public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string actionName, string definitionKey, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14)
        => Action(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14);

    ///<inheritdoc>
    private void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string actionName, string definitionKey, IDefinition definition,T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return;

        foreach (var item in actions)
        {
            if (item is Action<IDefinition,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> act)
                act.Invoke(definition, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14);
        }
    }

    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string actionName, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(actionName, manager.GetUniqueKeyByDefinition(definition), definition, defaultValue, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string actionName, string definitionKey, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14)
        => Function<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), defaultValue, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14);
    ///<inheritdoc>
    public TRet? Function<TRet,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string actionName, string definitionKey, IDefinition definition, TRet? defaultValue, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14)
    {
        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
            return defaultValue;
        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
            return defaultValue;

        TRet? lastResult = default;
        foreach (var item in actions)
        {
            if (item is Func<TRet?, IDefinition,T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRet> act)
                lastResult = act.Invoke(lastResult, definition, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14);
        }
        return lastResult;
    }
}