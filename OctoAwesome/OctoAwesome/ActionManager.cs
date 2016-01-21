using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Komponente im Pluginmodell: Ermöglicht simple Events zum Eingreifen in den Programmablauf
    /// </summary>
    public class ActionManager
    {
        private Dictionary<string, ActionInfo> actions;
        
        public ActionManager()
        {
            actions = new Dictionary<string, ActionInfo>();
        }

        /// <summary>
        /// Führt eine Aktion aus
        /// </summary>
        /// <param name="actionName">Name der auszuführenden Aktion</param>
        /// <param name="parameters">Die Argumente, die an die Callbacks übergeben werden</param>
        public void Do(string actionName, object[] parameters)
        {
            foreach (var action in actions.Where(a => a.Key == actionName))
            {
                if (action.Value.Types.Length == parameters.Length)
                {
                    bool exit = false;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (!action.Value.Types[i].IsInstanceOfType(parameters[i]))
                        {
                            exit = true;
                            break;
                        }
                    }
                    if (!exit)
                        action.Value.Action(parameters);
                }
            }                
        }

        /// <summary>
        /// Registriert ein callback für eine Aktion
        /// </summary>
        /// <param name="actioName">Name der Aktion</param>
        /// <param name="action">Wird ausgeführt, wenn die Aktion ausgeführt wird</param>
        /// <param name="types">Erwartete Typen der Parameter. Die Aktion wird nur ausgeführt, wenn diese Typen mit den Typen der Parameter übereinstimmen</param>
        public void Add(string actioName, Action<object[]> action, Type[] types)
        {
            actions.Add(actioName, new ActionInfo(action, types));
        }
    }

    internal class ActionInfo
    {
        public ActionInfo(Action<object[]> action, Type[] types)
        {
            Action = action;
            Types = types;
        }

        public Action<object[]> Action { get; set; }

        public Type[] Types { get; set; }
    }
}
