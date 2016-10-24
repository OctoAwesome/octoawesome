using System;
using System.Collections;

namespace OctoAwesome.Ecs
{
    public class Entity
    {
        public bool Complete;
        public EntityManager Manager;
        public Component[] Components;

        public T Get<T>() where T : Component, new()
        {
            return (T) Components[ComponentRegistry<T>.Index];
        }

        public Entity Add<T>() where T : Component, new()
        {
            Manager.Add<T>(this);
            return this;
        }

        public Entity Add<T>(Action<T> action, bool actionIfAlreadyExists = false, bool throwOnExists = false) where T : Component, new()
        {
            Manager.Add(this, action, actionIfAlreadyExists, throwOnExists);
            return this;
        }
    }
}