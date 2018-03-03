using OctoAwesome.CodeExtensions;
using OctoAwesome.Common;
using OctoAwesome.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Definition Manager, der Typen aus Erweiterungen nachlädt.
    /// </summary>
    public class DefinitionManager : IDefinitionManager
    {
        private List<IDefinition> definitions;

        private List<IItemDefinition> itemDefinitions;

        private List<IBlockDefinition> blockDefinitions;

        private List<IEntityDefinition> entityDefinitions;

        private List<IResourceDefinition> resourceDefinitions;

        private List<IInventoryableDefinition> inventoryableDefinitions;

        private Dictionary<Type, List<IDefinition>> typeddefinitions;

        private Dictionary<string, IDefinition> nameddefinitions;

        private IExtensionResolver extensionResolver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extensionResolver"></param>
        public DefinitionManager(IExtensionResolver extensionResolver)
        {
            this.extensionResolver = extensionResolver;

            definitions = extensionResolver.GetDefinitions<IDefinition>().ToList();
            typeddefinitions = new Dictionary<Type, List<IDefinition>>();
            nameddefinitions = new Dictionary<string, IDefinition>();
            blockDefinitions = new List<IBlockDefinition>();
            itemDefinitions = new List<IItemDefinition>();
            entityDefinitions = new List<IEntityDefinition>();
            resourceDefinitions = new List<IResourceDefinition>();
            inventoryableDefinitions = new List<IInventoryableDefinition>();
            List<string> names = new List<string>();
            int unnameddefinitionsCounter = 0;
            foreach(IDefinition definition in definitions)
            {
                if (definition is IBlockDefinition blockdefinition)
                    blockDefinitions.Add(blockdefinition);

                if (definition is IItemDefinition itemdefinition)
                    itemDefinitions.Add(itemdefinition);

                if (definition is IEntityDefinition entitydefinition)
                    entityDefinitions.Add(entitydefinition);

                if (definition is IInventoryableDefinition inventoryable)
                    inventoryableDefinitions.Add(inventoryable);

                if (definition is IResourceDefinition resourcedefinition)
                    resourceDefinitions.Add(resourcedefinition);

                int namecounter = 0;
                string name = definition.Name;
                // TODO: throw exception ?
                if (name.IsNullOrEmpty()) name = $"noname[{unnameddefinitionsCounter++}]";
                while (nameddefinitions.ContainsKey(name))
                    name = $"{definition.Name}[{namecounter++}]";
                names.Add(name);
                nameddefinitions.Add(name, definition);

                Type type = null;
                if (definition is IParsedDefinition parsed)
                    type = parsed.AssociatedType;
                if(type == null)
                    type = definition.GetType();
                if (typeddefinitions.ContainsKey(type))
                    typeddefinitions[type].Add(definition);
                else typeddefinitions.Add(type, new List<IDefinition>() { definition });

            };
            FileStream stream = File.Create(Path.Combine(Directory.GetCurrentDirectory(), "loadeddifinitions.txt"));
            try
            {
                //TODO: mehr information falls notwendig
                // eventuel für UI zugägnlich mahcen bzw. auslaggern
                string totalstring = string.Join(Environment.NewLine, names);
                stream.Write(Encoding.UTF8.GetBytes(totalstring), 0, totalstring.Length);
            }
            catch(Exception)
            {
                //TODO: loggen
            }
            finally
            {
                stream.Flush();
                stream.Dispose();
                stream.Close();
            }
        }
        /// <summary>
        /// Liefert eine Liste von Defintions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDefinition> GetDefinitions()
        {
            return definitions;
        }
        /// <summary>
        /// Liefert eine Liste aller bekannten Item Definitions (inkl. Blocks, Resources, Tools)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IItemDefinition> GetItemDefinitions()
        {
            return itemDefinitions;
        }
        /// <summary>
        /// Returns all <see cref="IResourceDefinition"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IResourceDefinition> GetResourceDefinitions()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Liefert eine Liste der bekannten Blocktypen.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IBlockDefinition> GetBlockDefinitions()
        {
            return blockDefinitions;
        }
        /// <summary>
        /// Return all <see cref="IEntityDefinition"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IEntityDefinition> GetEntityDefinitions()
        {
            return entityDefinitions;
        }
        /// <summary>
        /// Return all <see cref="IInventoryableDefinition"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IInventoryableDefinition> GetInventoryableDefinitions()
        {
            return inventoryableDefinitions;
        }
        /// <summary>
        /// Return definiton or null by index
        /// </summary>
        /// <typeparam name="T">Type of <see cref="IDefinition"/></typeparam>
        /// <param name="index">Index of <see cref="IDefinition"/></param>
        /// <returns></returns>
        public T GetDefinitionByIndex<T>(ushort index) where T : class, IDefinition
        {
            if (index == 0)
                return null;

            return definitions[(index & Blocks.TypeMask) - 1] as T;
        }
        /// <summary>
        /// Return definiton or null by name
        /// </summary>
        /// <typeparam name="T">Type of <see cref="IDefinition"/></typeparam>
        /// <param name="name">Name of <see cref="IDefinition"/></param>
        /// <returns></returns>
        public T GetDefinitionByName<T>(string name) where T : class, IDefinition
        {
            if (nameddefinitions.TryGetValue(name, out IDefinition definition))
                return definition as T;
            return null;
        }
        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <param name="definition">BlockDefinition</param>
        /// <returns>Index der Block Definition</returns>
        public ushort GetDefinitionIndex(IDefinition definition)
        {
            if (definition == null) return 0;
            return (ushort)(definitions.IndexOf(definition) + 1);
        }
        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <typeparam name="T">BlockDefinition Type</typeparam>
        /// <returns>Index der Block Definition</returns>
        public IList<ushort> GetDefinitionIndex<T>() where T : IDefinition
        {
            //TODO: methode ist nicht robust... ->
            if (typeddefinitions.TryGetValue(typeof(T), out List<IDefinition> definitions))
                return definitions.Select(d => GetDefinitionIndex(d)).ToList();
            else return new List<ushort>() { 0 };
        }
        /// <summary>
        /// Returns the index of a definition by name
        /// </summary>
        /// <param name="name">Name of <see cref="IDefinition"/></param>
        /// <returns></returns>
        public ushort GetDefinitionIndexByName(string name)
        {
            return GetDefinitionIndex(GetDefinitionByName<IDefinition>(name));
        }
        /// <summary>
        /// Gibt die Liste von Instanzen des angegebenen Definition Interfaces zurück.
        /// </summary>
        /// <typeparam name="T">Typ der <see cref="IDefinition"/></typeparam>
        /// <returns>Auflistung von Instanzen</returns>
        public IEnumerable<T> GetDefinitions<T>() where T : IDefinition
        {
            if (typeddefinitions.TryGetValue(typeof(T), out List<IDefinition> definitions))
                return definitions.Cast<T>();
            return new List<T>();
        }
    }
}
