using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public class ActionManager
    {
        private Dictionary<string, Action<object[]>> actions;

        public ActionManager()
        {
            actions = new Dictionary<string, Action<object[]>>();
        }

        public void Do(string actionName, object[] parameters)
        {
            foreach (var action in actions.Where(a => a.Key == actionName))
                action.Value(parameters);
        }

        public void Add(string actioName, Action<object[]> action)
        {
            actions.Add(actioName, action);
        }
    }
}
