using engenious;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace OctoAwesome;
public class InteractService
{
    Dictionary<string, Action<GameTime, ComponentContainer, ComponentContainer>> registeredActions = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="action">Name of the Class to interact with, first component container is the interactor and second component container the target</param>
    public void Register(string key, Action<GameTime, ComponentContainer, ComponentContainer> action)
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrAddDefault(registeredActions, key, out var exists);
        if (exists)
        {
            val += action;
        }
        else
        {
            val = action;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="action">Name of the Class to interact with, first component container is the interactor and second component container the target</param>
    public void Unregister(string key, Action<GameTime, ComponentContainer, ComponentContainer> action)
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrAddDefault(registeredActions, key, out var exists);
        if (exists)
        {
            val -= action;
        }
    }

    public void Interact(string key, GameTime gameTime, ComponentContainer interactor, ComponentContainer target)
    {
        if (registeredActions.TryGetValue(key, out var action))
        {
            action(gameTime, interactor, target);
        }
    }

}
