using engenious;

using OctoAwesome.Components;
using OctoAwesome.Rx;

using System;
using System.Collections.Generic;

namespace OctoAwesome.UI.Components
{
    /// <summary>
    /// Component to provide a ui for the screen
    /// </summary>
    public abstract class UIComponent : Component, IHoldComponent<ComponentContainer>
    {
        /// <summary>
        /// Gets or sets if this component should be shown
        /// </summary>
        public bool Show { get; protected set; }

        /// <summary>
        /// Gets or sets the key for this component that should be unique
        /// </summary>
        public string PrimaryUiKey { get; protected set; }

        /// <summary>
        /// Gets or sets the relay that can be used to subscribe to an event to know if the ui component has changed
        /// </summary>
        public Relay<Unit> Changes { get; protected set; }

        /// <summary>
        /// Unique list of component container for this ui component
        /// </summary>
        protected HashSet<ComponentContainer> componentContainers = new();

        /// <summary>
        /// The dictionary which contains the disposables for each <see cref="ComponentContainer"/> so we can cirumvent memory leaks
        /// </summary>
        protected Dictionary<ComponentContainer, IDisposable> componentContainerSubs = new Dictionary<ComponentContainer, IDisposable>();


        /// <summary>
        /// Initializes a new instance of the<see cref="UIComponent" /> class
        /// </summary>
        public UIComponent()
        {
            Changes = new Relay<Unit>();
        }

        /// <summary>
        /// Displays the ui
        /// </summary>
        public abstract void Update(GameTime gameTime);

        /// <inheritdoc/>
        public void Add(ComponentContainer value)
        {

            if (!componentContainers.Contains(value) && Match(value))
            {
                OnAdd(value);
                componentContainers.Add(value);

                var uiMappingComponent = value.GetComponent<UiMappingComponent>();
                componentContainerSubs[value] = uiMappingComponent.Changed.Subscribe(UiMappingChanged);
            }
        }
        /// <summary>
        /// Invoked before a value was added to this component
        /// </summary>
        /// <param name="value">The instance of type <see cref="ComponentContainer"/> that will be added.</param>
        protected virtual void OnAdd(ComponentContainer value) { }

        /// <inheritdoc/>
        public void Remove(ComponentContainer value)
        {
            if (componentContainers.Contains(value))
            {
                if (componentContainerSubs.TryGetValue(value, out var dispose))
                {
                    dispose.Dispose();
                    componentContainerSubs.Remove(value);
                }
                OnRemove(value);
                _ = componentContainers.Remove(value);
            }
        }

        /// <summary>
        /// Invoked before a value was removed to this component
        /// </summary>
        /// <param name="value">The instance of type <see cref="ComponentContainer"/> that will be removed.</param>
        protected virtual void OnRemove(ComponentContainer value) { }

        /// <summary>
        /// Checks if the <see cref="ComponentContainer"/> can be added.
        /// <see cref="ComponentContainer"/> can only be added, if it contains the required Components
        /// </summary>
        /// <param name="value">The <see cref="ComponentContainer"/> that should be checked for a component</param>
        /// <returns><see langword="true"/></returns>
        protected virtual bool Match(ComponentContainer value) => true;

        /// <summary>
        /// Checks if a change has occured and sets the values accordingly
        /// </summary>
        /// <param name="e">The tuple containing the values that identifies a record for this component</param>
        protected virtual void UiMappingChanged((ComponentContainer container, string screenKey, bool show) e)
        {
            bool isSame = PrimaryUiKey == e.screenKey && e.show == Show;
            if (isSame)
                return;
            PrimaryUiKey = e.screenKey;
            Show = e.show;
        }
    }


    /// <summary>
    /// Component to provide a ui for the screen
    /// </summary>
    public abstract class UIComponent<TComponentRecord, TComponent1> : UIComponent
        where TComponentRecord : UiComponentRecord<TComponent1>
        where TComponent1 : Component
    {
        /// <summary>
        /// The cached components.
        /// </summary>
        protected readonly Dictionary<ComponentContainer, UiComponentRecord<TComponent1>> componentCache = new();

        /// <inheritdoc/>
        public sealed override void Update(GameTime gameTime)
        {
            foreach (var cachedComponent in componentCache)
            {
                if (TryUpdate(
                    cachedComponent.Key,
                    cachedComponent.Value.Component1))
                {
                    Changes.OnNext(new Unit());
                }
            }
        }
        /// <summary>
        /// Invoked for each component in <see cref="componentCache"/> for each update
        /// </summary>
        /// <param name="value">The component container which containes the components</param>
        /// <param name="component">The component that is needed for the update check of type<typeparamref name="TComponent1"/> </param>
        /// <returns>If the component has been updated</returns>
        protected abstract bool TryUpdate(ComponentContainer value, TComponent1 component);

        /// <inheritdoc/>
        protected sealed override void OnAdd(ComponentContainer value) => componentCache[value] = new UiComponentRecord<TComponent1>(value.GetComponent<TComponent1>());

        /// <inheritdoc/>
        protected sealed override void OnRemove(ComponentContainer value)
        {
            if (componentCache.ContainsKey(value))
                _ = componentCache.Remove(value);
        }

        /// <summary>
        /// Checks if the <see cref="ComponentContainer"/> can be added.
        /// <see cref="ComponentContainer"/> can only be added, if it contains the required Components
        /// </summary>
        /// <param name="value">The <see cref="ComponentContainer"/> that should be checked for <typeparamref name="TComponent1"/></param>
        /// <returns>True if the <see cref="ComponentContainer"/> contains <typeparamref name="TComponent1"/></returns>
        protected override bool Match(ComponentContainer value) => value.ContainsComponent<TComponent1>();

    }

    /// <summary>
    /// Component to provide a ui for the screen
    /// </summary>
    public abstract class UIComponent<TComponentRecord, TComponent1, TComponent2> : UIComponent
        where TComponentRecord : UiComponentRecord<TComponent1, TComponent2>
        where TComponent1 : Component
        where TComponent2 : Component
    {
        /// <summary>
        /// The cached components.
        /// </summary>
        protected readonly Dictionary<ComponentContainer, UiComponentRecord<TComponent1, TComponent2>> componentCache = new();

        /// <inheritdoc/>
        public sealed override void Update(GameTime gameTime)
        {
            foreach (var cachedComponent in componentCache)
            {
                if (TryUpdate(
                    cachedComponent.Key,
                    cachedComponent.Value.Component1,
                    cachedComponent.Value.Component2))
                {
                    Changes.OnNext(new Unit());
                }
            }
        }
        /// <summary>
        /// Invoked for each component in <see cref="componentCache"/> for each update
        /// </summary>
        /// <param name="value">The component container which containes the components</param>
        /// <param name="component">The component that is needed for the update check of type<typeparamref name="TComponent1"/> </param>
        /// <param name="component2">The component that is needed for the update check of type <typeparamref name="TComponent2"/> </param>
        /// <returns>If the component has been updated</returns>
        protected abstract bool TryUpdate(ComponentContainer value, TComponent1 component, TComponent2 component2);

        /// <inheritdoc/>
        protected sealed override void OnAdd(ComponentContainer value)
        {
            componentCache[value] = new UiComponentRecord<TComponent1, TComponent2>(value.GetComponent<TComponent1>(), value.GetComponent<TComponent2>());
        }

        /// <inheritdoc/>
        protected sealed override void OnRemove(ComponentContainer value)
        {
            if (componentCache.ContainsKey(value))
                _ = componentCache.Remove(value);
        }

        /// <summary>
        /// Checks if the <see cref="ComponentContainer"/> can be added.
        /// <see cref="ComponentContainer"/> can only be added, if it contains the required Components
        /// </summary>
        /// <param name="value">The <see cref="ComponentContainer"/> that should be checked for <typeparamref name="TComponent1"/> and <typeparamref name="TComponent2"/></param>
        /// <returns>True if the <see cref="ComponentContainer"/> contains <typeparamref name="TComponent1"/> and <typeparamref name="TComponent2"/></returns>
        protected override bool Match(ComponentContainer value) => value.ContainsComponent<TComponent1>() && value.ContainsComponent<TComponent2>();
    }


    /// <summary>
    /// Component to provide a ui for the screen
    /// </summary>
    public abstract class UIComponent<TComponentRecord, TComponent1, TComponent2, TComponent3> : UIComponent
        where TComponentRecord : UiComponentRecord<TComponent1, TComponent2, TComponent3>
        where TComponent1 : Component
        where TComponent2 : Component
        where TComponent3 : Component
    {
        /// <summary>
        /// The cached components.
        /// </summary>
        protected readonly Dictionary<ComponentContainer, UiComponentRecord<TComponent1, TComponent2, TComponent3>> componentCache = new();

        /// <inheritdoc/>
        public sealed override void Update(GameTime gameTime)
        {
            foreach (var cachedComponent in componentCache)
            {
                if (TryUpdate(cachedComponent.Key,
                        cachedComponent.Value.Component1,
                        cachedComponent.Value.Component2,
                        cachedComponent.Value.Component3))
                {
                    Changes.OnNext(new Unit());
                }
            }
        }
        /// <summary>
        /// Invoked for each component in <see cref="componentCache"/> for each update
        /// </summary>
        /// <param name="value">The component container which containes the components</param>
        /// <param name="component">The component that is needed for the update check of type<typeparamref name="TComponent1"/> </param>
        /// <param name="component2">The component that is needed for the update check of type <typeparamref name="TComponent2"/> </param>
        /// <param name="component3">The component that is needed for the update check of type <typeparamref name="TComponent3"/> </param>
        /// <returns>If the component has been updated</returns>
        protected abstract bool TryUpdate(ComponentContainer value, TComponent1 component, TComponent2 component2, TComponent3 component3);

        /// <inheritdoc/>
        protected sealed override void OnAdd(ComponentContainer value)
        {
            componentCache[value]
                = new UiComponentRecord<TComponent1, TComponent2, TComponent3>(
                    value.GetComponent<TComponent1>(),
                    value.GetComponent<TComponent2>(),
                    value.GetComponent<TComponent3>());
        }

        /// <inheritdoc/>
        protected sealed override void OnRemove(ComponentContainer value)
        {
            if (componentCache.ContainsKey(value))
                _ = componentCache.Remove(value);
        }

        /// <summary>
        /// Checks if the <see cref="ComponentContainer"/> can be added.
        /// <see cref="ComponentContainer"/> can only be added, if it contains the required Components
        /// </summary>
        /// <param name="value">The <see cref="ComponentContainer"/> that should be checked for <typeparamref name="TComponent1"/> and <typeparamref name="TComponent2"/> and <typeparamref name="TComponent3"/></param>
        /// <returns>True if the <see cref="ComponentContainer"/> contains <typeparamref name="TComponent1"/> and <typeparamref name="TComponent2"/> and <typeparamref name="TComponent3"/></returns>
        protected override bool Match(ComponentContainer value)
            => value.ContainsComponent<TComponent1>() && value.ContainsComponent<TComponent2>() && value.ContainsComponent<TComponent3>();
    }

    /// <summary>
    /// Record wrapper to hold a single cached component.
    /// </summary>
    /// <param name="Component1">The cached component.</param>
    /// <typeparam name="TComponent1">The type of the component to cache.</typeparam>
    public record UiComponentRecord<TComponent1>(TComponent1 Component1);

    /// <summary>
    /// Record wrapper to hold two cached components.
    /// </summary>
    /// <param name="Component1">The first cached component.</param>
    /// <typeparam name="TComponent1">The type of the first component to cache.</typeparam>
    /// <param name="Component2">The second cached component.</param>
    /// <typeparam name="TComponent2">The type of the second component to cache.</typeparam>
    public record UiComponentRecord<TComponent1, TComponent2>(TComponent1 Component1, TComponent2 Component2);

    /// <summary>
    /// Record wrapper to hold three cached components.
    /// </summary>
    /// <param name="Component1">The first cached component.</param>
    /// <typeparam name="TComponent1">The type of the first component to cache.</typeparam>
    /// <param name="Component2">The second cached component.</param>
    /// <typeparam name="TComponent2">The type of the second component to cache.</typeparam>
    /// <param name="Component3">The third cached component.</param>
    /// <typeparam name="TComponent3">The type of the third component to cache.</typeparam>
    public record UiComponentRecord<TComponent1, TComponent2, TComponent3>(TComponent1 Component1, TComponent2 Component2, TComponent3 Component3);


}
