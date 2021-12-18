using System;
using System.Collections.Generic;
using System.Linq;
using engenious.UI;
using KeyEventArgs = engenious.UI.KeyEventArgs;
using Keys = engenious.Input.Keys;

namespace OctoAwesome.Client.Components
{
    internal class KeyMapper
    {
        private Dictionary<string, Binding> bindings;

        public Dictionary<string, Binding> Bindings { get { return bindings; } }

        private ISettings settings;

        public KeyMapper(BaseScreenComponent manager, ISettings settings)
        {
            manager.KeyDown += KeyDown;
            manager.KeyUp += KeyUp;
            manager.KeyPress += KeyPressed;

            this.settings = settings;

            bindings = new Dictionary<string, Binding>();
        }

        /// <summary>
        /// Registers a new Binding
        /// </summary>
        /// <param name="id">The ID - guideline: ModName:Action</param>
        /// <param name="displayName">The Display name</param>
        public void RegisterBinding(string id, string displayName)
        {
            if (bindings.ContainsKey(id))
                return;
            bindings.Add(id, new Binding(id, displayName));
        }

        /// <summary>
        /// Removes a Binding
        /// </summary>
        /// <param name="id">The ID</param>
        public void UnregisterBinding(string id)
        {
            if (bindings.ContainsKey(id))
                bindings.Remove(id);
        }

        /// <summary>
        /// Adds a Key to a Binding
        /// </summary>
        /// <param name="id">The ID of the Binding</param>
        /// <param name="key">The Key</param>
        public void AddKey(string id, Keys key)
        {
            if (bindings.TryGetValue(id, out var binding))
            {
                if (!binding.Keys.Contains(key))
                    binding.Keys.Add(key);
            }
        }

        /// <summary>
        /// Removes a Key from a Binding
        /// </summary>
        /// <param name="id">The ID of the Binding</param>
        /// <param name="key">The Key</param>
        public void RemoveKey(string id, Keys key)
        {
            if (bindings.TryGetValue(id, out var binding))
            {
                if (binding.Keys.Contains(key)) binding.Keys.Remove(key);
            }
        }

        /// <summary>
        /// Adds an Action to a Binding
        /// </summary>
        /// <param name="id">The ID of the Binding</param>
        /// <param name="action">The Action</param>
        public void AddAction(string id, Action<KeyType> action)
        {
            if (bindings.TryGetValue(id, out var binding))
            {
                if (!binding.Actions.Contains(action)) binding.Actions.Add(action);
            }
        }

        /// <summary>
        /// Removes an Action from a Binding
        /// </summary>
        /// <param name="id">The ID of the Binding</param>
        /// <param name="action">The Action</param>
        public void RemoveAction(string id, Action<KeyType> action)
        {
            if (bindings.TryGetValue(id, out var binding))
            {
                if (binding.Actions.Contains(action)) binding.Actions.Remove(action);
            }
        }

        /// <summary>
        /// Sets the DisplayName of a Binding
        /// </summary>
        /// <param name="id">The ID of the Binding</param>
        /// <param name="displayName">The new DisplayName</param>
        public void SetDisplayName(string id, string displayName)
        {
            if (bindings.TryGetValue(id, out var binding))
                binding.DisplayName = displayName;
        }

        /// <summary>
        /// Loads KeyBindings from the App.config file or fallbacks to default values.
        /// </summary>
        /// <param name="standardKeys"></param>
        public void LoadFromConfig(Dictionary<string, Keys> standardKeys)
        {
            foreach (var id in standardKeys.Keys)
            {
                if (settings.KeyExists("KeyMapper-" + id))
                {
                    try
                    {
                        string val = settings.Get<string>("KeyMapper-" + id);
                        Keys key = (Keys)Enum.Parse(typeof(Keys), val);
                        AddKey(id, key);
                    }
                    catch
                    {
                        AddKey(id, standardKeys[id]);
                    }
                }
                else
                    AddKey(id, standardKeys[id]);
            }
        }

        public List<Binding> GetBindings()
        {
            List<Binding> bindings = new List<Binding>();
            foreach (var binding in Bindings)
                bindings.Add(binding.Value);
            return bindings;
        }


        #region KeyEvents

        protected void KeyPressed(KeyEventArgs args)
        {
            var result = bindings.Values.Where(b => b.Keys.Contains(args.Key));
            foreach (var binding in result)
            {
                foreach (var action in binding.Actions)
                {
                    action(KeyType.Pressed);
                }
            }
        }

        protected void KeyDown(KeyEventArgs args)
        {
            var result = bindings.Values.Where(b => b.Keys.Contains(args.Key));
            foreach (var binding in result)
            {
                foreach (var action in binding.Actions)
                {
                    action(KeyType.Down);
                }
            }
        }

        protected void KeyUp(KeyEventArgs args)
        {
            var result = bindings.Values.Where(b => b.Keys.Contains(args.Key));
            foreach (var binding in result)
            {
                foreach (var action in binding.Actions)
                {
                    action(KeyType.Up);
                }
            }
        }

        #endregion

        public class Binding
        {
            public string Id { get; }

            public string DisplayName { get; set; }

            public List<Keys> Keys { get; }

            public List<Action<KeyType>> Actions { get; }

            public Binding(string id, string displayName)
            {
                Id = id;
                DisplayName = displayName;
                Keys = new List<Keys>();
                Actions = new List<Action<KeyType>>();
            }
        }

        public enum KeyType
        {
            Down,
            Up,
            Pressed,
        }
    }
}
