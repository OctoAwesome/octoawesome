using engenious;

using OctoAwesome.Components;
using OctoAwesome.Database;
using OctoAwesome.Rx;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.UI.Components
{
    /// <summary>
    /// Component to provide a ui for the screen
    /// </summary>
    public abstract class UIComponent : Component, IHoldComponent<ComponentContainer>
    {
        public bool Show { get; protected set; }
        public string PrimaryUiKey { get; protected set; }


        protected HashSet<ComponentContainer> componentContainers = new();
        protected Dictionary<ComponentContainer, IDisposable> componentContainerSubs = new Dictionary<ComponentContainer, IDisposable>();
        public Relay<Unit> Changes { get; protected set; }


        public UIComponent()
        {
            Changes = new Relay<Unit>();
        }

        /// <summary>
        /// Displays the ui
        /// </summary>
        public abstract void Update(GameTime gameTime);

        public void Add(ComponentContainer value)
        {

            if (!componentContainers.Contains(value) && Match(value))
            {
                OnAdd(value);
                componentContainers.Add(value);

                var uiMappingComponent =value.GetComponent<UiMappingComponent>();
                componentContainerSubs[value] = uiMappingComponent.Changed.Subscribe(UiMappingChanged);
            }
        }
        protected virtual void OnAdd(ComponentContainer value) { }

        public void Remove(ComponentContainer value)
        {
            if (componentContainers.Contains(value))
            {
                if(componentContainerSubs.TryGetValue(value, out var dispose))
                {
                    dispose.Dispose();
                    componentContainerSubs.Remove(value);
                }
                OnRemove(value);
                _ = componentContainers.Remove(value);
            }
        }

        protected virtual void OnRemove(ComponentContainer value) { }

        protected virtual bool Match(ComponentContainer value) => true;

        protected virtual void UiMappingChanged((ComponentContainer container, string screenKey, bool show) e) 
        {
            PrimaryUiKey = e.screenKey;
            Show = e.show;
        }
    }


    public abstract class UIComponent<TComponentRecord, TComponent1> : UIComponent
        where TComponentRecord : UiComponentRecord<TComponent1>
        where TComponent1 : Component
    {
        protected readonly Dictionary<ComponentContainer, UiComponentRecord<TComponent1>> componentCache = new();

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
        protected abstract bool TryUpdate(ComponentContainer value, TComponent1 component);

        protected sealed override void OnAdd(ComponentContainer value) => componentCache[value] = new UiComponentRecord<TComponent1>(value.GetComponent<TComponent1>());

        protected sealed override void OnRemove(ComponentContainer value)
        {
            if (componentCache.ContainsKey(value))
                _ = componentCache.Remove(value);
        }

        protected override bool Match(ComponentContainer value) => value.ContainsComponent<TComponent1>();

    }
    public abstract class UIComponent<TComponentRecord, TComponent1, TComponent2> : UIComponent
        where TComponentRecord : UiComponentRecord<TComponent1, TComponent2>
        where TComponent1 : Component
        where TComponent2 : Component
    {
        protected readonly Dictionary<ComponentContainer, UiComponentRecord<TComponent1, TComponent2>> componentCache = new();

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
        protected abstract bool TryUpdate(ComponentContainer value, TComponent1 component, TComponent2 component2);

        protected sealed override void OnAdd(ComponentContainer value)
        {
            componentCache[value] = new UiComponentRecord<TComponent1, TComponent2>(value.GetComponent<TComponent1>(), value.GetComponent<TComponent2>());
        }

        protected sealed override void OnRemove(ComponentContainer value)
        {
            if (componentCache.ContainsKey(value))
                _ = componentCache.Remove(value);
        }
        protected override bool Match(ComponentContainer value) => value.ContainsComponent<TComponent1>() && value.ContainsComponent<TComponent2>();
    }


    public abstract class UIComponent<TComponentRecord, TComponent1, TComponent2, TComponent3> : UIComponent
        where TComponentRecord : UiComponentRecord<TComponent1, TComponent2, TComponent3>
        where TComponent1 : Component
        where TComponent2 : Component
        where TComponent3 : Component
    {
        protected readonly Dictionary<ComponentContainer, UiComponentRecord<TComponent1, TComponent2, TComponent3>> componentCache = new();

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
        protected abstract bool TryUpdate(ComponentContainer value, TComponent1 component, TComponent2 component2, TComponent3 component3);
        protected sealed override void OnAdd(ComponentContainer value)
        {
            componentCache[value]
                = new UiComponentRecord<TComponent1, TComponent2, TComponent3>(
                    value.GetComponent<TComponent1>(),
                    value.GetComponent<TComponent2>(),
                    value.GetComponent<TComponent3>());
        }

        protected sealed override void OnRemove(ComponentContainer value)
        {
            if (componentCache.ContainsKey(value))
                _ = componentCache.Remove(value);
        }

        protected override bool Match(ComponentContainer value)
            => value.ContainsComponent<TComponent1>() && value.ContainsComponent<TComponent2>() && value.ContainsComponent<TComponent3>();
    }

    public record UiComponentRecord<TComponent1>(TComponent1 Component1);
    public record UiComponentRecord<TComponent1, TComponent2>(TComponent1 Component1, TComponent2 Component2);
    public record UiComponentRecord<TComponent1, TComponent2, TComponent3>(TComponent1 Component1, TComponent2 Component2, TComponent3 Component3);


}
