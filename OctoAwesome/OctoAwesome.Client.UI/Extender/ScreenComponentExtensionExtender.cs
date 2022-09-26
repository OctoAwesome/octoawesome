
using OctoAwesome.Caching;
using OctoAwesome.UI.Components;
using System;
using System.Collections.Generic;

namespace OctoAwesome.UI.Extender
{
    /// <summary>
    /// Class for managing screen component extensions
    /// </summary>
    public class ScreenComponentExtensionExtender : BaseExtensionExtender<IScreenComponent>
    {
        private readonly List<Action<IScreenComponent>> simulationExtender;        

        /// <summary>
        /// Initializes a new instance of the<see cref="ScreenComponentExtensionExtender" /> class
        /// </summary>
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
            simulationExtender.Add(GenericCaster< Action<T>, Action<IScreenComponent>>.Cast(extenderDelegate)!);
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
