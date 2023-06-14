using engenious;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome;
public class InteractService
{
    Dictionary<string, List<Action<GameTime, ComponentContainer, ComponentContainer>>> registeredActions = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="action">Name of the Class to interact with, first component container is the interactor and second component container the target</param>
    public void Register(string key, Action<GameTime, ComponentContainer, ComponentContainer> action)
    {
        if (registeredActions.TryGetValue(key, out var list))
        {
            list.Add(action);
        }
        else
        {
            registeredActions[key] = new List<Action<GameTime, ComponentContainer, ComponentContainer>> { action };
        }
    }

    public void Interact(string key, GameTime gameTime, ComponentContainer interactor, ComponentContainer target)
    {
        if(registeredActions.TryGetValue(key, out var actions))
        {
            foreach (var action in actions)
            { 
                action(gameTime, interactor, target);
            }


        }
    }

}
