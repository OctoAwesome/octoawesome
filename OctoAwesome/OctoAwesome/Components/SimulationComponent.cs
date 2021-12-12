using engenious;
using System.Collections.Generic;

namespace OctoAwesome.Components
{
    /// <summary>
    /// Baseclass of all SimulationComponents who extend the <see cref="Simulation"/>
    /// </summary>
    public abstract class SimulationComponent : Component
    {

        /// <summary>
        /// Updatemethod of this Component
        /// </summary>
        /// <param name="gameTime">The current gametime</param>
        public abstract void Update(GameTime gameTime);

    }

    /// <summary>
    /// Basisklasse für Simulationskomponenten
    /// </summary>
    public abstract class SimulationComponent<T> : SimulationComponent, IHoldComponent<T>
    {
        /// <summary>
        /// Entities die durch diese Simulationkomponete simuliert werden
        /// </summary>
        protected readonly List<T> values = new();

        /// <summary>
        /// Konstruktor
        /// </summary>
        public SimulationComponent()
        {

        }

        /// <summary>
        /// Fügt eine neue Entity der Simulationskomponente hinzu
        /// </summary>
        /// <param name="value">Neue Entity</param>
        public void Add(T value)
        {
            if (Match(value))
            {
                OnAdd(value);
                values.Add(value);
            }
        }

        /// <summary>
        /// Führt ein Vergleich durch, ob diese Entity in die Komponente eingefügt werden kann
        /// </summary>
        /// <param name="value">Vergleichsentity</param>
        /// <returns>Ergebnis des Vergleiches</returns>
        protected virtual bool Match(T value)
        {
            return true;
        }

        /// <summary>
        /// Internes Event, für das hinzufügen einer Entity
        /// </summary>
        /// <param name="value">Neue Entity</param>
        /// <returns>Ergebnis</returns>
        protected abstract void OnAdd(T value);

        /// <summary>
        /// Entfernt eine Entity aus der Simulationskomponente
        /// </summary>
        /// <param name="value"></param>
        public void Remove(T value)
        {
            if (values.Contains(value))
            {
                OnRemove(value);
                values.Remove(value);
            }
        }

        /// <summary>
        /// Internes Event, für das entfernen einer Entity
        /// </summary>
        /// <param name="value">Neue Entity</param>
        /// <returns>Ergebnis</returns>
        protected abstract void OnRemove(T value);
    }
    /// <summary>
    /// Basisklasse für Simulationskomponenten
    /// </summary>
    public abstract class SimulationComponent<TContainer, TCachedContainer, TComponent> : SimulationComponent, IHoldComponent<TContainer>
        where TContainer : IComponentContainer
        where TCachedContainer : SimulationComponentRecord<TContainer, TComponent>
        where TComponent : Component
    {
        protected readonly List<TCachedContainer> values = new();
        
        /// <summary>
        /// Adds a new value of <typeparamref name="T"/> to this Component
        /// </summary>
        /// <param name="value">an instance of <typeparamref name="T"/> to add</param>
        public void Add(TContainer value)
        {
            if (Match(value))
            {
                values.Add(OnAdd(value));
            }
        }
        

        /// <summary>
        /// Is called during <see cref="Add(T)"/> to convert <paramref name="value"/> from <typeparamref name="T"/> to <typeparamref name="S"/>
        /// </summary>
        /// <param name="value">instance of <typeparamref name="T"/> that is passed to <see cref="Add(T)"/></param>
        /// <returns>Converted <paramref name="value"/> as <typeparamref name="S"/></returns>
        protected virtual TCachedContainer OnAdd(TContainer value)
            => (TCachedContainer)new SimulationComponentRecord<TContainer, TComponent>(value, value.GetComponent<TComponent>());
        
        /// <summary>
        /// Removes an instance of <typeparamref name="T"/> that is previouse added with <see cref="Add(T)"/>
        /// </summary>
        /// <param name="value">an instance of <typeparamref name="T"/> to Remove</param>
        public void Remove(TContainer value)
        {
            OnRemove(value);
            values.RemoveAll(c => Compare(c, value));
        }
        /// <summary>
        /// Is called during <see cref="Remove(T)"/> to convert <paramref name="value"/> from <typeparamref name="T"/> to <typeparamref name="S"/>
        /// </summary>
        /// <param name="value">instance of <typeparamref name="T"/> that is passed to <see cref="Add(T)"/></param>
        /// <returns>Converted <paramref name="value"/> as <typeparamref name="S"/></returns>
        protected virtual void OnRemove(TContainer value) { }

        protected virtual bool Compare(TCachedContainer left, TContainer right)
            => Equals(left.Value, right);
        /// <summary>
        /// Führt ein Vergleich durch, ob diese Entity in die Komponente eingefügt werden kann
        /// </summary>
        /// <param name="value">Vergleichsentity</param>
        /// <returns>Ergebnis des Vergleiches</returns>
        protected virtual bool Match(TContainer value)
            => value.ContainsComponent<TComponent>();
        
        /// <summary>
        /// Updatemethode der Entity
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        public override void Update(GameTime gameTime)
        {
            foreach (var value in values)
                UpdateValue(gameTime, value);
        }
        /// <summary>
        /// Internes Event, für das Updaten der Simulationskomponente
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        /// <param name="value">Entity die geupdatet werden muss</param>
        protected abstract void UpdateValue(GameTime gameTime, TCachedContainer value);

    }

    /// <summary>
    /// Basisklasse für Simulationskomponenten
    /// </summary>
    public abstract class SimulationComponent<TContainer, TCachedContainer, TComponent1, TComponent2> : SimulationComponent, IHoldComponent<TContainer>
        where TContainer : IComponentContainer
        where TCachedContainer : SimulationComponentRecord<TContainer, TComponent1, TComponent2>
        where TComponent1 : Component
        where TComponent2 : Component
    {
        protected readonly List<TCachedContainer> values = new();
        /// <summary>
        /// Adds a new value of <typeparamref name="T"/> to this Component
        /// </summary>
        /// <param name="value">an instance of <typeparamref name="T"/> to add</param>
        public void Add(TContainer value)
        {
            if (Match(value))
            {
                values.Add(OnAdd(value));
            }
        }

        /// <summary>
        /// Is called during <see cref="Add(T)"/> to convert <paramref name="value"/> from <typeparamref name="T"/> to <typeparamref name="S"/>
        /// </summary>
        /// <param name="value">instance of <typeparamref name="T"/> that is passed to <see cref="Add(T)"/></param>
        /// <returns>Converted <paramref name="value"/> as <typeparamref name="S"/></returns>
        protected virtual TCachedContainer OnAdd(TContainer value)
            => (TCachedContainer)new SimulationComponentRecord<TContainer, TComponent1, TComponent2>(value, value.GetComponent<TComponent1>(), value.GetComponent<TComponent2>());
        
        /// <summary>
        /// Removes an instance of <typeparamref name="T"/> that is previouse added with <see cref="Add(T)"/>
        /// </summary>
        /// <param name="value">an instance of <typeparamref name="T"/> to Remove</param>
        public void Remove(TContainer value)
        {
            OnRemove(value);
            values.RemoveAll(c => Compare(c, value));
        }

        /// <summary>
        /// Is called during <see cref="Remove(T)"/> to convert <paramref name="value"/> from <typeparamref name="T"/> to <typeparamref name="S"/>
        /// </summary>
        /// <param name="value">instance of <typeparamref name="T"/> that is passed to <see cref="Add(T)"/></param>
        /// <returns>Converted <paramref name="value"/> as <typeparamref name="S"/></returns>
        protected virtual void OnRemove(TContainer value) { }

        protected virtual bool Compare(TCachedContainer left, TContainer right)
            => Equals(left.Value, right);

        /// <summary>
        /// Führt ein Vergleich durch, ob diese Entity in die Komponente eingefügt werden kann
        /// </summary>
        /// <param name="value">Vergleichsentity</param>
        /// <returns>Ergebnis des Vergleiches</returns>
        protected virtual bool Match(TContainer value)
            => value.ContainsComponent<TComponent1>()
                && value.ContainsComponent<TComponent2>();

        /// <summary>
        /// Updatemethode der Entity
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        public override void Update(GameTime gameTime)
        {
            //TODO: Ändern (Collection was modified in Multiplayer)
            foreach (var value in values)
                UpdateValue(gameTime, value);
        }

        /// <summary>
        /// Internes Event, für das Updaten der Simulationskomponente
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        /// <param name="value">Entity die geupdatet werden muss</param>
        protected abstract void UpdateValue(GameTime gameTime, TCachedContainer value);

    }

    /// <summary>
    /// Basisklasse für Simulationskomponenten
    /// </summary>
    public abstract class SimulationComponent<TContainer, TCachedContainer, TComponent1, TComponent2, TComponent3> : SimulationComponent, IHoldComponent<TContainer>
        where TContainer : IComponentContainer
        where TCachedContainer : SimulationComponentRecord<TContainer, TComponent1, TComponent2, TComponent3>
        where TComponent1 : Component
        where TComponent2 : Component
        where TComponent3 : Component
    {
        protected readonly List<TCachedContainer> values = new();
        
        /// <summary>
        /// Adds a new value of <typeparamref name="T"/> to this Component
        /// </summary>
        /// <param name="value">an instance of <typeparamref name="T"/> to add</param>
        public void Add(TContainer value)
        {
            if (Match(value))
            {
                values.Add(OnAdd(value));
            }
        }


        /// <summary>
        /// Is called during <see cref="Add(T)"/> to convert <paramref name="value"/> from <typeparamref name="T"/> to <typeparamref name="S"/>
        /// </summary>
        /// <param name="value">instance of <typeparamref name="T"/> that is passed to <see cref="Add(T)"/></param>
        /// <returns>Converted <paramref name="value"/> as <typeparamref name="S"/></returns>
        protected virtual TCachedContainer OnAdd(TContainer value)
            => (TCachedContainer)new SimulationComponentRecord<TContainer, TComponent1, TComponent2, TComponent3>(value, value.GetComponent<TComponent1>(), value.GetComponent<TComponent2>(), value.GetComponent<TComponent3>());
        
        /// <summary>
        /// Removes an instance of <typeparamref name="T"/> that is previouse added with <see cref="Add(T)"/>
        /// </summary>
        /// <param name="value">an instance of <typeparamref name="T"/> to Remove</param>
        public void Remove(TContainer value)
        {
            OnRemove(value);
            values.RemoveAll(c => Compare(c, value));
        }

        /// <summary>
        /// Is called during <see cref="Remove(T)"/> to convert <paramref name="value"/> from <typeparamref name="T"/> to <typeparamref name="S"/>
        /// </summary>
        /// <param name="value">instance of <typeparamref name="T"/> that is passed to <see cref="Add(T)"/></param>
        /// <returns>Converted <paramref name="value"/> as <typeparamref name="S"/></returns>
        protected virtual void OnRemove(TContainer value) { }
        protected virtual bool Compare(TCachedContainer left, TContainer right)
            => Equals(left.Value, right);

        /// <summary>
        /// Führt ein Vergleich durch, ob diese Entity in die Komponente eingefügt werden kann
        /// </summary>
        /// <param name="value">Vergleichsentity</param>
        /// <returns>Ergebnis des Vergleiches</returns>
        protected virtual bool Match(TContainer value)
            => value.ContainsComponent<TComponent1>()
                && value.ContainsComponent<TComponent2>()
                && value.ContainsComponent<TComponent3>();

        /// <summary>
        /// Updatemethode der Entity
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        public override void Update(GameTime gameTime)
        {
            //TODO: Ändern
            foreach (var value in values)
                UpdateValue(gameTime, value);
        }

        /// <summary>
        /// Internes Event, für das Updaten der Simulationskomponente
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        /// <param name="value">Entity die geupdatet werden muss</param>
        protected abstract void UpdateValue(GameTime gameTime, TCachedContainer value);

    }
    public record SimulationComponentRecord<TContainer, TComponent>(TContainer Value, TComponent Component);
    public record SimulationComponentRecord<TContainer, TComponent1, TComponent2>(TContainer Value, TComponent1 Component1, TComponent2 Component2);
    public record SimulationComponentRecord<TContainer, TComponent1, TComponent2, TComponent3>(TContainer Value, TComponent1 Component1, TComponent2 Component2, TComponent3 Component3);
}

