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
        private Dictionary<string, Action<object[]>> actions;
        
        public ActionManager()
        {
            actions = new Dictionary<string, Action<object[]>>();
        }

        /// <summary>
        /// Führt eine Aktion aus
        /// </summary>
        /// <param name="actionName">Name der auszuführenden Aktion</param>
        /// <param name="parameters">Die Argumente, die an die Callbacks übergeben werden</param>
        public void Do(string actionName, object[] parameters)
        {
            foreach (var action in actions.Where(a => a.Key == actionName))
                action.Value(parameters);
        }

        /// <summary>
        /// Registriert ein callback für eine Aktion
        /// </summary>
        /// <param name="actioName">Name der Aktion</param>
        /// <param name="action">Wird ausgeführt, wenn die Aktion ausgeführt wird</param>
        public void Add(string actioName, Action<object[]> action)
        {
            actions.Add(actioName, action);
        }
    }
}
