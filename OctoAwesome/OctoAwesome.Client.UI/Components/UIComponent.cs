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
        public bool Visible { get; set; }

        protected List<ComponentContainer> componentContainer = new();

        /// <summary>
        /// Displays the ui
        /// </summary>
        public abstract void Update(GameTime gameTime);

        public void Add(ComponentContainer value)
        {
            if (Match(value))
            {
                OnAdd(value);
                componentContainer.Add(value);
            }
        }
        protected virtual void OnAdd(ComponentContainer value) { }

        public void Remove(ComponentContainer value)
        {
            if (componentContainer.Contains(value))
            {
                OnRemove(value);
                _ = componentContainer.Remove(value);
            }
        }

        protected virtual void OnRemove(ComponentContainer value) { }


        protected virtual bool Match(ComponentContainer value)
        {
            return true;
        }
    }
    public abstract class UIComponent<T> : UIComponent
    {
        public Relay<T> Changes { get; protected set; }

        public UIComponent()
        {
            Changes = new Relay<T>();
        }

    }


    public abstract class UIComponent<TModel, TComponentRecord, TComponent1> : UIComponent<TModel>
        where TComponentRecord : UiComponentRecord<TModel, TComponent1>
        where TComponent1 : Component
    {
        readonly Dictionary<ComponentContainer, UiComponentRecord<TModel, TComponent1>> componentCache = new();

        public sealed override void Update(GameTime gameTime)
        {
            foreach (var cachedComponent in componentCache)
            {
                if (TryUpdate(
                    cachedComponent.Key,
                    cachedComponent.Value.Component1,
                    cachedComponent.Value.Model,
                    out var model))
                {
                    componentCache[cachedComponent.Key] = cachedComponent.Value with { Model = model };
                    Changes.OnNext(model);
                }
            }
        }
        protected abstract bool TryUpdate(ComponentContainer value, TComponent1 component, TModel lastModel, out TModel model);

        protected sealed override void OnAdd(ComponentContainer value)
        {
            componentCache[value] = new UiComponentRecord<TModel, TComponent1>(default, value.GetComponent<TComponent1>());
        }

        protected sealed override void OnRemove(ComponentContainer value)
        {
            if (componentCache.ContainsKey(value))
                _ = componentCache.Remove(value);
        }

        protected override bool Match(ComponentContainer value) => value.ContainsComponent<TComponent1>();

    }
    public abstract class UIComponent<TModel, TComponentRecord, TComponent1, TComponent2> : UIComponent<TModel>
        where TComponentRecord : UiComponentRecord<TModel, TComponent1, TComponent2>
        where TComponent1 : Component
        where TComponent2 : Component
    {
        readonly Dictionary<ComponentContainer, UiComponentRecord<TModel, TComponent1, TComponent2>> componentCache = new();

        public sealed override void Update(GameTime gameTime)
        {
            foreach (var cachedComponent in componentCache)
            {
                if (TryUpdate(
                    cachedComponent.Key,
                    cachedComponent.Value.Component1,
                    cachedComponent.Value.Component2,
                    cachedComponent.Value.Model,
                    out var model))
                {
                    componentCache[cachedComponent.Key] = cachedComponent.Value with { Model = model };
                    Changes.OnNext(model);
                }
            }
        }
        protected abstract bool TryUpdate(ComponentContainer value, TComponent1 component, TComponent2 component2, TModel lastModel, out TModel model);

        protected sealed override void OnAdd(ComponentContainer value)
        {
            componentCache[value] = new UiComponentRecord<TModel, TComponent1, TComponent2>(default, value.GetComponent<TComponent1>(), value.GetComponent<TComponent2>());
        }

        protected sealed override void OnRemove(ComponentContainer value)
        {
            if (componentCache.ContainsKey(value))
                _ = componentCache.Remove(value);
        }
        protected override bool Match(ComponentContainer value) => value.ContainsComponent<TComponent1>() && value.ContainsComponent<TComponent2>();
    }


    public abstract class UIComponent<TModel, TComponentRecord, TComponent1, TComponent2, TComponent3> : UIComponent<TModel>
        where TComponentRecord : UiComponentRecord<TModel, TComponent1, TComponent2, TComponent3>
        where TComponent1 : Component
        where TComponent2 : Component
        where TComponent3 : Component
    {
        readonly Dictionary<ComponentContainer, UiComponentRecord<TModel, TComponent1, TComponent2, TComponent3>> componentCache = new();

        public sealed override void Update(GameTime gameTime)
        {
            foreach (var cachedComponent in componentCache)
            {
                if (TryUpdate(cachedComponent.Key,
                        cachedComponent.Value.Component1,
                        cachedComponent.Value.Component2,
                        cachedComponent.Value.Component3,
                        cachedComponent.Value.Model,
                        out var model))
                {
                    componentCache[cachedComponent.Key] = cachedComponent.Value with { Model = model };
                    Changes.OnNext(model);
                }
            }
        }
        protected abstract bool TryUpdate(ComponentContainer value, TComponent1 component, TComponent2 component2, TComponent3 component3, TModel lastModel, out TModel model);
        protected sealed override void OnAdd(ComponentContainer value)
        {
            componentCache[value]
                = new UiComponentRecord<TModel, TComponent1, TComponent2, TComponent3>(
                    default,
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

    public record UiComponentRecord<TModel, TComponent1>(TModel Model, TComponent1 Component1);
    public record UiComponentRecord<TModel, TComponent1, TComponent2>(TModel Model, TComponent1 Component1, TComponent2 Component2);
    public record UiComponentRecord<TModel, TComponent1, TComponent2, TComponent3>(TModel Model, TComponent1 Component1, TComponent2 Component2, TComponent3 Component3);


}
