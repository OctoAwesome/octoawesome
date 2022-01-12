
using OctoAwesome.Caching;
using OctoAwesome.UI.Components;
using System;
using System.Collections.Generic;

namespace OctoAwesome.UI.Extender
{
    public class ScreenComponentExtensionExtender : BaseExtensionExtender<IScreenComponent>
    {
        private readonly List<Action<IScreenComponent>> simulationExtender;

        public ScreenComponentExtensionExtender()
        {
            simulationExtender = new();
        }

        /// <summary>
        /// Adds a new Extender for the ScreenComponent.
        /// </summary>
        /// <param name="extenderDelegate"></param>
        public override void RegisterExtender<T>(Action<T> extenderDelegate)
        {
            simulationExtender.Add(GenericCaster<Action<IScreenComponent>, Action<T>>.Cast(extenderDelegate));
        }

        /// <summary>
        /// Extend a ScreenComponent
        /// </summary>
        /// <param name="screenComponent">IScreenComponent</param>
        public override void Execute<T>(T screenComponent)
        {
            foreach (var extender in simulationExtender)
                extender(screenComponent);
        }
    }
}
