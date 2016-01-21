using Microsoft.Xna.Framework.Input;
using MonoGameUi;
using System;
using System.Linq;
using System.Collections.Generic;

namespace OctoAwesome.Client.Components
{
    internal class KeyMapper
    {
        private Dictionary<string, Binding> bindings;

        // private Dictionary<Keys, List<Binding>> keyBindings;

        public KeyMapper(IScreenManager manager)
        {
            manager.KeyDown += KeyDown;
            manager.KeyUp += KeyUp;
            manager.KeyPress += KeyPressed;

            bindings = new Dictionary<string, Binding>();
            // keyBindings = new Dictionary<Keys, List<Binding>>();
        }

        public void RegisterKey(string id, Keys key)
        {
            Binding binding;
            if (!bindings.TryGetValue(id, out binding))
            {
                binding = new Binding()
                {
                    Id = id,
                    Keys = new List<Keys>(),
                    Actions = new List<Action<KeyType>>()
                };
                bindings.Add(id, binding);
            }

            binding.Keys.Add(key);
        }

        public void UnregisterKey(string id, Keys key)
        {
            Binding binding;
            if (bindings.TryGetValue(id, out binding))
                binding.Keys.Remove(key);
        }

        public void AddAction(string id, Action<KeyType> action)
        {
            Binding binding;
            if (!bindings.TryGetValue(id, out binding))
            {
                binding = new Binding()
                {
                    Id = id,
                    Keys = new List<Keys>(),
                    Actions = new List<Action<KeyType>>()
                };
                bindings.Add(id, binding);
            }

            binding.Actions.Add(action);
        }

        public void RemoveAction(string id, Action<KeyType> action)
        {
            Binding binding;
            if (bindings.TryGetValue(id, out binding))
                binding.Actions.Remove(action);
        }

        public void KeyPressed(KeyEventArgs args)
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

        private void KeyDown(KeyEventArgs args)
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

        private void KeyUp(KeyEventArgs args)
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

        private class Binding
        {
            public string Id { get; set; }

            public List<Keys> Keys { get; set; }

            public List<Action<KeyType>> Actions { get; set; }
        }

        public enum KeyType
        {
            Down,
            Up,
            Pressed
        }
    }
}
