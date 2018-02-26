using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OctoAwesome.Entities;
using System.IO;

namespace OctoAwesome
{
    public abstract class SimulationComponent : Component
    {
        public abstract void Update(GameTime gameTime);
        public abstract void Register(Entity entity);
        public abstract void Remove(Entity entity);
    }

    /// <summary>
    /// Basisklasse für Simulationskomponenten
    /// </summary>
    public abstract class OSimulationComponent : Component
    {
        /// <summary>
        /// Entities die durch diese Simulationkomponete simuliert werden
        /// </summary>
        protected List<Entity> entities = new List<Entity>();

        private List<Type[]> componentFilter = new List<Type[]>();

        /// <summary>
        /// Konstruktor
        /// </summary>
        public OSimulationComponent()
        {
            // TODO: Refelct Attributes
            foreach (EntityFilterAttribute attribute in GetType().GetCustomAttributes(typeof(EntityFilterAttribute), false))
            {
                foreach (var entityComponentType in attribute.EntityComponentTypes)
                {
                    if (!typeof(EntityComponent).IsAssignableFrom(entityComponentType))
                        throw new NotSupportedException();

                    componentFilter.Add(attribute.EntityComponentTypes);
                }
            }
        }

        /// <summary>
        /// Fügt eine neue Entity der Simulationskomponente hinzu
        /// </summary>
        /// <param name="entity">Neue Entity</param>
        public void Add(Entity entity)
        {
            if (Match(entity) && AddEntity(entity))
            {
                entities.Add(entity);
            }
        }

        /// <summary>
        /// Führt ein Vergleich durch, ob diese Entity in die Komponente eingefügt werden kann
        /// </summary>
        /// <param name="entity">Vergleichsentity</param>
        /// <returns>Ergebnis des Vergleiches</returns>
        protected virtual bool Match(Entity entity)
        {
            if (componentFilter.Count == 0)
                return true;

            return componentFilter.Any(
                x => x.All(
                    t => entity.Components.Any(
                        c => t.IsAssignableFrom(c.GetType()))));
        }

        /// <summary>
        /// Internes Event, für das hinzufügen einer Entity
        /// </summary>
        /// <param name="entity">Neue Entity</param>
        /// <returns>Ergebnis</returns>
        protected abstract bool AddEntity(Entity entity);

        /// <summary>
        /// Entfernt eine Entity aus der Simulationskomponente
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(Entity entity)
        {
            if (entities.Contains(entity))
            {
                RemoveEntity(entity);
                entities.Remove(entity);
            }
        }

        /// <summary>
        /// Internes Event, für das entfernen einer Entity
        /// </summary>
        /// <param name="entity">Neue Entity</param>
        /// <returns>Ergebnis</returns>
        protected abstract void RemoveEntity(Entity entity);

        /// <summary>
        /// Updatemethode der Entity
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        public abstract void Update(GameTime gameTime);
    }

    /// <summary>
    /// Basisklasse für Simulationskomponenten
    /// </summary>
    public abstract class OSimulationComponent<C1> : OSimulationComponent where C1 : EntityComponent
    {
        /// <summary>
        /// Führt ein Vergleich durch, ob diese Entity in die Komponente eingefügt werden kann
        /// </summary>
        /// <param name="entity">Vergleichsentity</param>
        /// <returns>Ergebnis des Vergleiches</returns>
        protected override bool Match(Entity entity) 
            => entity.Components.ContainsComponent<C1>();

        /// <summary>
        /// Updatemethode der Entity
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        public override void Update(GameTime gameTime)
        {
            foreach (var entity in entities)
                UpdateEntity(gameTime, entity, entity.Components.GetComponent<C1>());
        }

        /// <summary>
        /// Internes Event, für das Updaten der Simulationskomponente
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        /// <param name="entity">Entity die geupdatet werden muss</param>
        /// <param name="component1">Komponente 1</param>
        protected abstract void UpdateEntity(GameTime gameTime, Entity entity, C1 component1);
    }

    /// <summary>
    /// Basisklasse für Simulationskomponenten
    /// </summary>
    public abstract class OSimulationComponent<C1, C2> : OSimulationComponent
        where C1 : EntityComponent
        where C2 : EntityComponent
    {

        /// <summary>
        /// Führt ein Vergleich durch, ob diese Entity in die Komponente eingefügt werden kann
        /// </summary>
        /// <param name="entity">Vergleichsentity</param>
        /// <returns>Ergebnis des Vergleiches</returns>
        protected override bool Match(Entity entity) 
            => entity.Components.ContainsComponent<C1>()
                && entity.Components.ContainsComponent<C2>();

        /// <summary>
        /// Updatemethode der Entity
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        public override void Update(GameTime gameTime)
        {
            //TODO: Ändern
            foreach (var entity in entities.ToArray())
                UpdateEntity(gameTime, entity, entity.Components.GetComponent<C1>(), entity.Components.GetComponent<C2>());
        }

        /// <summary>
        /// Internes Event, für das Updaten der Simulationskomponente
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        /// <param name="entity">Entity die geupdatet werden muss</param>
        /// <param name="component1">Komponente 1</param>
        /// <param name="component2">Komponente 2</param>
        protected abstract void UpdateEntity(GameTime gameTime, Entity entity, C1 component1, C2 component2);
    }
    /// <summary>
    /// Basisklasse für Simulationskomponenten
    /// </summary>
    public abstract class OSimulationComponent<C1, C2, C3> : OSimulationComponent
        where C1 : EntityComponent
        where C2 : EntityComponent
        where C3 : EntityComponent
    {

        /// <summary>
        /// Führt ein Vergleich durch, ob diese Entity in die Komponente eingefügt werden kann
        /// </summary>
        /// <param name="entity">Vergleichsentity</param>
        /// <returns>Ergebnis des Vergleiches</returns>
        protected override bool Match(Entity entity) 
            => entity.Components.ContainsComponent<C1>()
                && entity.Components.ContainsComponent<C2>()
                && entity.Components.ContainsComponent<C3>();

        /// <summary>
        /// Updatemethode der Entity
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        public override void Update(GameTime gameTime)
        {
            //TODO: Ändern
            foreach (var entity in entities.ToArray())
                UpdateEntity(gameTime, entity, entity.Components.GetComponent<C1>(), entity.Components.GetComponent<C2>(), entity.Components.GetComponent<C3>());
        }

        /// <summary>
        /// Internes Event, für das Updaten der Simulationskomponente
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        /// <param name="entity">Entity die geupdatet werden muss</param>
        /// <param name="component1">Komponente 1</param>
        /// <param name="component2">Komponente 2</param>
        protected abstract void UpdateEntity(GameTime gameTime, Entity entity, C1 component1, C2 component2, C3 component3);
    }
}
