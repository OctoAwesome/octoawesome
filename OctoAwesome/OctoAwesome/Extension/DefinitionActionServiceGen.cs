
using System;
using System.Linq;

class Template1
{
    string Main()
    {
        var ret = $$"""
        using OctoAwesome.Definitions;
        using System;

        namespace OctoAwesome.Extension;
        partial class DefinitionActionService
        {
        """;

        for (int i = 0; i < 14; i++)
        {
            var ts = string.Join(", ", Enumerable.Range(1, i + 1).Select(x => "T" + x));
            var tParams = string.Join(", ", Enumerable.Range(1, i + 1).Select(x => "T" + x + " param" + x));
            var paramNames = string.Join(", ", Enumerable.Range(1, i + 1).Select(x => "param" + x));

            ret +=
                $$"""
                    ///<inheritdoc>
                    public void Action<{{ts}}>(string actionName, IDefinition definition, {{tParams}})
                        => Action(actionName, manager.GetUniqueKeyByDefinition(definition), definition, {{paramNames}});
                    ///<inheritdoc>
                    public void Action<{{ts}}>(string actionName, string definitionKey, {{tParams}})
                        => Action(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), {{paramNames}});

                    ///<inheritdoc>
                    private void Action<{{ts}}>(string actionName, string definitionKey, IDefinition definition,{{tParams}})
                    {
                        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
                            return;
                        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
                            return;

                        foreach (var item in actions)
                        {
                            if (item is Action<IDefinition,{{ts}}> act)
                                act.Invoke(definition, {{paramNames}});
                        }
                    }

                    ///<inheritdoc>
                    public TRet? Function<TRet,{{ts}}>(string actionName, IDefinition definition, TRet? defaultValue, {{tParams}})
                        => Function<TRet, {{ts}}>(actionName, manager.GetUniqueKeyByDefinition(definition), definition, defaultValue, {{paramNames}});
                    ///<inheritdoc>
                    public TRet? Function<TRet,{{ts}}>(string actionName, string definitionKey, TRet? defaultValue, {{tParams}})
                        => Function<TRet, {{ts}}>(actionName, definitionKey, manager.GetDefinitionByUniqueKey(definitionKey), defaultValue, {{paramNames}});
                    ///<inheritdoc>
                    public TRet? Function<TRet,{{ts}}>(string actionName, string definitionKey, IDefinition definition, TRet? defaultValue, {{tParams}})
                    {
                        if(!methodsPerDefinition.TryGetValue(actionName, out var definitionDelegates))
                            return defaultValue;
                        if(!definitionDelegates.TryGetValue(definitionKey, out var actions))
                            return defaultValue;

                        TRet? lastResult = default;
                        foreach (var item in actions)
                        {
                            if (item is Func<TRet?, IDefinition,{{ts}}, TRet> act)
                                lastResult = act.Invoke(lastResult, definition, {{paramNames}});
                        }
                        return lastResult;
                    }

                """;
        }
        ret += "}";
        return ret;
    }
}