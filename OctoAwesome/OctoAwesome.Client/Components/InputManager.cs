using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace OctoAwesome.Client.Components
{
    public class InputManager
    {
        #region Singleton

        private static InputManager _instance;

        public static InputManager Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new InputManager();
                return _instance;
            }
        }

        #endregion

        private Dictionary<string, KeyBinding> bindings; 

        private InputManager()
        {
            bindings = new Dictionary<string, KeyBinding>();
        }

        public void AddBinding(string name, Keys key, Action callback)
        {
            if(bindings.ContainsKey(name))
                throw new Exception("name already registered");

            KeyBinding newBinding = new KeyBinding();
            newBinding.key = key;
            newBinding.action = callback;

            bindings.Add(name, newBinding);
        }

        public void RemoveBinding(string name)
        {
            if (!bindings.ContainsKey(name))
                return;

            bindings.Remove(name);
        }

        public void ChangeKey(string name, Keys key)
        {
            if (!bindings.ContainsKey(name))
                throw new Exception("no matching binding found");

            bindings[name].key = key;
        }

        public void UpdateInput()
        {
            KeyboardState state = Keyboard.GetState();
            foreach (Keys key in state.GetPressedKeys())
            {
                bindings.FirstOrDefault(b => b.Value.key == key).Value.action();
            }
        }
    }

    class KeyBinding
    {
        public Keys key;
        public Action action;
    }
}
