using System;
using engenious;

using OctoAwesome.EntityComponents;

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace OctoAwesome.Components
{
    /// <summary>
    /// Baseclass of all SimulationComponents who extend the <see cref="Simulation"/>.
    /// </summary>
    public abstract class SimulationComponent : Component
    {
        /// <summary>
        /// Update method of this <see cref="Component"/>.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public abstract void Update(GameTime gameTime);

    }

    /// <summary>
    /// Base class for SimulationComponents who extend the <see cref="Simulation"/> and hold values of specific type.
    /// </summary>
    /// <typeparam name="T">The type of the values to hold.</typeparam>
    public abstract class SimulationComponent<T> : SimulationComponent, IHoldComponent<T>
    {
        /// <summary>
        /// Entities simulated by this <see cref="SimulationComponent{T}"/>.
        /// </summary>
        protected readonly List<T> values = new();

        /// <inheritdoc />
        public void Add(T value)
        {
            if (Match(value))
            {
                OnAdd(value);
                values.Add(value);
            }
        }

        /// <summary>
        /// Checks whether the given entity can be added to the component.
        /// </summary>
        /// <param name="value">The entity to check.</param>
        /// <returns>A value indicating whether the given entity can be added to the component.</returns>
        protected virtual bool Match(T value)
        {
            return true;
        }

        /// <summary>
        /// Internal method called when a new entity is added.
        /// </summary>
        /// <param name="value">The entity to add.</param>
        protected abstract void OnAdd(T value);

        /// <inheritdoc />
        public void Remove(T value)
        {
            if (values.Contains(value))
            {
                OnRemove(value);
                values.Remove(value);
            }
        }

        /// <summary>
        /// Is called during <see cref="Remove(T)"/>.
        /// </summary>
        /// <param name="value">Instance of <typeparamref name="T"/> to remove.</param>
        protected abstract void OnRemove(T value);
    }

    /// <summary>
    /// Base class for SimulationComponents who extend the <see cref="Simulation"/> and hold values of specific type.
    /// </summary>
    /// <typeparam name="TContainer">The type of the component container.</typeparam>
    /// <typeparam name="TCachedContainer">
    /// The type which wraps the <typeparamref name="TContainer"/> container and caches <typeparamref name="TComponent"/>.
    /// </typeparam>
    /// <typeparam name="TComponent">The type of the component to cache for each container.</typeparam>
    public abstract class SimulationComponent<TContainer, TCachedContainer, TComponent> : SimulationComponent, IHoldComponent<TContainer>
        where TContainer : IComponentContainer
        where TCachedContainer : SimulationComponentRecord<TContainer, TComponent>
        where TComponent : Component
    {
        /// <summary>
        /// Entities simulated by this <see cref="SimulationComponent{T, S, C1}"/>.
        /// </summary>
        protected readonly List<TCachedContainer> values = new();

        /// <inheritdoc />
        public void Add(TContainer value)
        {
            if (Match(value))
            {
                values.Add(OnAdd(value));
            }
        }

        /// <summary>
        /// Is called during <see cref="Add(TContainer)"/> to convert <paramref name="value"/> from <typeparamref name="TContainer"/> to <typeparamref name="TCachedContainer"/>
        /// </summary>
        /// <param name="value">instance of <typeparamref name="TContainer"/> that is passed to <see cref="Add(TContainer)"/></param>
        /// <returns>Converted <paramref name="value"/> as <typeparamref name="TCachedContainer"/></returns>
        protected virtual TCachedContainer OnAdd(TContainer value)
        {
            var component = value.GetComponent<TComponent>();
            Debug.Assert(component != null, nameof(component) + " != null");
            return (TCachedContainer)new SimulationComponentRecord<TContainer, TComponent>(value,
                component);
        }

        /// <inheritdoc />
        public void Remove(TContainer value)
        {
            OnRemove(value);
            values.RemoveAll(c => Compare(c, value));
        }

        /// <summary>
        /// Is called during <see cref="Remove(TContainer)"/> to convert <paramref name="value"/> from <typeparamref name="TContainer"/> to <typeparamref name="TCachedContainer"/>
        /// </summary>
        /// <param name="value">instance of <typeparamref name="TContainer"/> that is passed to <see cref="Add(TContainer)"/></param>
        /// <returns>Converted <paramref name="value"/> as <typeparamref name="TCachedContainer"/></returns>
        protected virtual void OnRemove(TContainer value) { }

        /// <summary>
        /// Checks whether the given cached container matches the given container.
        /// </summary>
        /// <param name="left">The cached container.</param>
        /// <param name="right">The container.</param>
        /// <returns>A value indicating whether the given cached container matches the given container.</returns>
        protected virtual bool Compare(TCachedContainer left, TContainer right)
            => Equals(left.Value, right);

        /// <summary>
        /// Checks whether the given entity can be added to the component.
        /// </summary>
        /// <param name="value">The entity to check.</param>
        /// <returns>A value indicating whether the given entity can be added to the component.</returns>
        protected virtual bool Match(TContainer value)
            => value.ContainsComponent<TComponent>();

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            foreach (var value in values)
                UpdateValue(gameTime, value);
        }

        /// <summary>
        /// Method to update contained entity.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="value">The entity to update.s</param>
        protected abstract void UpdateValue(GameTime gameTime, TCachedContainer value);

    }

    /// <summary>
    /// Base class for SimulationComponents who extend the <see cref="Simulation"/> and hold values of specific type.
    /// </summary>
    /// <typeparam name="TContainer">The type of the component container.</typeparam>
    /// <typeparam name="TCachedContainer">
    /// The type which wraps the <typeparamref name="TContainer"/> container and caches <typeparamref name="TComponent1"/>.
    /// </typeparam>
    /// <typeparam name="TComponent1">The type of the first component to cache for each container.</typeparam>
    /// <typeparam name="TComponent2">The type of the second component to cache for each container.</typeparam>
    public abstract class SimulationComponent<TContainer, TCachedContainer, TComponent1, TComponent2> : SimulationComponent, IHoldComponent<TContainer>
        where TContainer : IComponentContainer
        where TCachedContainer : SimulationComponentRecord<TContainer, TComponent1, TComponent2>
        where TComponent1 : Component
        where TComponent2 : Component
    {
        /// <summary>
        /// Entities simulated by this <see cref="SimulationComponent{T, S, C1, C2}"/>.
        /// </summary>
        protected readonly List<TCachedContainer> values = new();

        /// <inheritdoc />
        public void Add(TContainer value)
        {
            if (Match(value))
            {
                values.Add(OnAdd(value));
            }
        }

        /// <summary>
        /// Is called during <see cref="Add(TContainer)"/> to convert <paramref name="value"/> from <typeparamref name="TContainer"/> to <typeparamref name="TCachedContainer"/>
        /// </summary>
        /// <param name="value">instance of <typeparamref name="TContainer"/> that is passed to <see cref="Add(TContainer)"/></param>
        /// <returns>Converted <paramref name="value"/> as <typeparamref name="TCachedContainer"/></returns>
        protected virtual TCachedContainer OnAdd(TContainer value)
        {
            var component1 = value.GetComponent<TComponent1>();
            var component2 = value.GetComponent<TComponent2>();
            Debug.Assert(component1 != null, nameof(component1) + " != null");
            Debug.Assert(component2 != null, nameof(component2) + " != null");
            return (TCachedContainer)new SimulationComponentRecord<TContainer, TComponent1, TComponent2>(value,
                component1, component2);
        }

        /// <inheritdoc />
        public void Remove(TContainer value)
        {
            OnRemove(value);
            values.RemoveAll(c => Compare(c, value));
        }

        /// <summary>
        /// Is called during <see cref="Remove(TContainer)"/> to convert <paramref name="value"/> from <typeparamref name="TContainer"/> to <typeparamref name="TCachedContainer"/>
        /// </summary>
        /// <param name="value">instance of <typeparamref name="TContainer"/> that is passed to <see cref="Add(TContainer)"/></param>
        /// <returns>Converted <paramref name="value"/> as <typeparamref name="TCachedContainer"/></returns>
        protected virtual void OnRemove(TContainer value) { }

        /// <summary>
        /// Checks whether the given cached container matches the given container.
        /// </summary>
        /// <param name="left">The cached container.</param>
        /// <param name="right">The container.</param>
        /// <returns>A value indicating whether the given cached container matches the given container.</returns>
        protected virtual bool Compare(TCachedContainer left, TContainer right)
            => Equals(left.Value, right);

        /// <summary>
        /// Checks whether the given entity can be added to the component.
        /// </summary>
        /// <param name="value">The entity to check.</param>
        /// <returns>A value indicating whether the given entity can be added to the component.</returns>
        protected virtual bool Match(TContainer value)
            => value.ContainsComponent<TComponent1>()
                && value.ContainsComponent<TComponent2>();

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            //TODO: Change (Collection was modified in Multiplayer)
            foreach (var value in values)
                UpdateValue(gameTime, value);
        }

        /// <summary>
        /// Method to update contained entity.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="value">The entity to update.s</param>
        protected abstract void UpdateValue(GameTime gameTime, TCachedContainer value);

    }

    /// <summary>
    /// Base class for SimulationComponents who extend the <see cref="Simulation"/> and hold values of specific type.
    /// </summary>
    /// <typeparam name="TContainer">The type of the values to add and be transformed to <typeparamref name="TCachedContainer"/>.</typeparam>
    /// <typeparam name="TCachedContainer">The type of the values to hold.</typeparam>
    /// <typeparam name="TComponent1">The type of the first component to cache for each container.</typeparam>
    /// <typeparam name="TComponent2">The type of the second component to cache for each container.</typeparam>
    /// <typeparam name="TComponent3">The type of the third component to cache for each container.</typeparam>
    public abstract class SimulationComponent<TContainer, TCachedContainer, TComponent1, TComponent2, TComponent3> : SimulationComponent, IHoldComponent<TContainer>
        where TContainer : IComponentContainer
        where TCachedContainer : SimulationComponentRecord<TContainer, TComponent1, TComponent2, TComponent3>
        where TComponent1 : Component
        where TComponent2 : Component
        where TComponent3 : Component
    {
        /// <summary>
        /// Entities simulated by this <see cref="SimulationComponent{T, S, C1, C2, C3}"/>.
        /// </summary>
        protected readonly List<TCachedContainer> values = new();

        /// <inheritdoc />
        public void Add(TContainer value)
        {
            if (Match(value))
            {
                values.Add(OnAdd(value));
            }
        }


        /// <summary>
        /// Is called during <see cref="Add(TContainer)"/> to convert <paramref name="value"/> from <typeparamref name="TContainer"/> to <typeparamref name="TCachedContainer"/>
        /// </summary>
        /// <param name="value">instance of <typeparamref name="TContainer"/> that is passed to <see cref="Add(TContainer)"/></param>
        /// <returns>Converted <paramref name="value"/> as <typeparamref name="TCachedContainer"/></returns>
        protected virtual TCachedContainer OnAdd(TContainer value)
        {
            var component1 = value.GetComponent<TComponent1>();
            var component2 = value.GetComponent<TComponent2>();
            var component3 = value.GetComponent<TComponent3>();
            Debug.Assert(component1 != null, nameof(component1) + " != null");
            Debug.Assert(component2 != null, nameof(component2) + " != null");
            Debug.Assert(component3 != null, nameof(component3) + " != null");
            return (TCachedContainer)new SimulationComponentRecord<TContainer, TComponent1, TComponent2, TComponent3>(
                value, component1, component2, component3);
        }

        /// <inheritdoc />
        public void Remove(TContainer value)
        {
            OnRemove(value);
            values.RemoveAll(c => Compare(c, value));
        }

        /// <summary>
        /// Is called during <see cref="Remove(TContainer)"/> to convert <paramref name="value"/> from <typeparamref name="TContainer"/> to <typeparamref name="TCachedContainer"/>
        /// </summary>
        /// <param name="value">instance of <typeparamref name="TContainer"/> that is passed to <see cref="Add(TContainer)"/></param>
        /// <returns>Converted <paramref name="value"/> as <typeparamref name="TCachedContainer"/></returns>
        protected virtual void OnRemove(TContainer value) { }


        /// <summary>
        /// Checks whether the given cached container matches the given container.
        /// </summary>
        /// <param name="left">The cached container.</param>
        /// <param name="right">The container.</param>
        /// <returns>A value indicating whether the given cached container matches the given container.</returns>
        protected virtual bool Compare(TCachedContainer left, TContainer right)
            => Equals(left.Value, right);

        /// <summary>
        /// Checks whether the given entity can be added to the component.
        /// </summary>
        /// <param name="value">The entity to check.</param>
        /// <returns>A value indicating whether the given entity can be added to the component.</returns>
        protected virtual bool Match(TContainer value)
            => value.ContainsComponent<TComponent1>()
                && value.ContainsComponent<TComponent2>()
                && value.ContainsComponent<TComponent3>();

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            //TODO: change
            foreach (var value in values)
                UpdateValue(gameTime, value);
        }

        /// <summary>
        /// Method to update contained entity.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="value">The entity to update.s</param>
        protected abstract void UpdateValue(GameTime gameTime, TCachedContainer value);

    }

    /// <summary>
    /// Record for caching a single component of a component container.
    /// </summary>
    /// <param name="Value">The component container.</param>
    /// <param name="Component">The component to cache.</param>
    /// <typeparam name="TContainer">The type of the component container.</typeparam>
    /// <typeparam name="TComponent">The type of the component to cache.</typeparam>
    public record SimulationComponentRecord<TContainer, TComponent>(TContainer Value, TComponent Component);

    /// <summary>
    /// Record for caching two components of a component container.
    /// </summary>
    /// <param name="Value">The component container.</param>
    /// <param name="Component1">The first component to cache.</param>
    /// <param name="Component2">The second component to cache.</param>
    /// <typeparam name="TContainer">The type of the component container.</typeparam>
    /// <typeparam name="TComponent1">The type of the first component to cache.</typeparam>
    /// <typeparam name="TComponent2">The type of the second component to cache.</typeparam>
    public record SimulationComponentRecord<TContainer, TComponent1, TComponent2>(TContainer Value, TComponent1 Component1, TComponent2 Component2);

    /// <summary>
    /// Record for caching three components of a component container.
    /// </summary>
    /// <param name="Value">The component container.</param>
    /// <param name="Component1">The first component to cache.</param>
    /// <param name="Component2">The second component to cache.</param>
    /// <param name="Component3">The third component to cache.</param>
    /// <typeparam name="TContainer">The type of the component container.</typeparam>
    /// <typeparam name="TComponent1">The type of the first component to cache.</typeparam>
    /// <typeparam name="TComponent2">The type of the second component to cache.</typeparam>
    /// <typeparam name="TComponent3">The type of the third component to cache.</typeparam>
    public record SimulationComponentRecord<TContainer, TComponent1, TComponent2, TComponent3>(TContainer Value, TComponent1 Component1, TComponent2 Component2, TComponent3 Component3);
}

