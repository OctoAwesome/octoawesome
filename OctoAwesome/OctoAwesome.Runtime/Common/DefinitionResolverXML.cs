using OctoAwesome.CodeExtensions;
using OctoAwesome.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
namespace OctoAwesome.Runtime.Common
{
    //class DependencyProperty<T> where T : IConvertible
    //{
    //    public string Name { get; }
    //    public Type Type { get; }
    //    public T Value { get; private set; }
    //    private DependencyProperty(string name, Type type)
    //    {
    //        Type = type;
    //        Name = name;
    //    }
    //    public static DependencyProperty<T> Register(string name, Type type)
    //    {
    //        return new DependencyProperty<T>(name, type);
    //    }
    //    public void Parse(string value)
    //    {
    //        if (value.IsNullOrEmpty())
    //            return;
    //        try
    //        {
    //            Value = (T) Convert.ChangeType(value, Type);
    //        }
    //        catch
    //        {

    //        }
    //    }
    //}
    class NamespaceDeclaration
    {
        public string Assembly { get; private set; }
        public string Namespace { get; private set; }
        public string Token { get; private set; }


        public static bool ResolveNamespaceDeclaration(string token, string value, out NamespaceDeclaration declaration)
        {
            declaration = null;
            string assembly = string.Empty;
            string space = string.Empty;
            string[] splittedvalue = value.Split(StringSplitOptions.RemoveEmptyEntries, ';', ':');

            if (splittedvalue.Length == 0)
                return false;
            int index = Array.IndexOf(splittedvalue, "assembly");
            if (splittedvalue.Length >= index + 1)
                assembly = splittedvalue[index + 1];

            index = Array.IndexOf(splittedvalue, "namespace");
            if (splittedvalue.Length >= index + 1)
                space = splittedvalue[index + 1];

            if (space.IsNullOrEmpty())
                return false;

            declaration = new NamespaceDeclaration() { Token = token, Assembly = assembly, Namespace = space };
            return true;
        }
        public static bool ResolveNamespaceDeclaration(XAttribute attribute, out NamespaceDeclaration declaration)
        {
            return ResolveNamespaceDeclaration(attribute.Name.LocalName, attribute.Value, out declaration);
        }
    }
    public sealed class DefinitionResolverXML
    {
        //private Dictionary<string, NamespaceDeclaration> declarations = new Dictionary<string, NamespaceDeclaration>();
        public DefinitionResolverXML()
        {
        }
        public void Resolve(Stream stream, IExtensionLoader extensionLoader)
        {
            XDocument document = XDocument.Load(stream);
            Resolve(document, extensionLoader);
        }
        public void Resolve(XDocument document, IExtensionLoader extensionLoader)
        {
            //foreach(XAttribute attribute in document.Root.Attributes())
            //{
            //    if(attribute.IsNamespaceDeclaration && !declarations.ContainsKey(attribute.Name.LocalName) &&
            //        NamespaceDeclaration.ResolveNamespaceDeclaration(attribute, out NamespaceDeclaration declaration))
            //        declarations.Add(declaration.Token, declaration);
            //}

            foreach (XElement element in document.Root.Elements())
                if (ResolveElement(element, out IDefinition definition))
                    extensionLoader.RegisterDefinition(definition);
        }
        private bool ResolveElement(XElement element, out IDefinition definition)
        {
            XMLSerializedEntityDefinition def = new XMLSerializedEntityDefinition();
            try
            {
                def.EntityType = GetTypeFromElement(element);
                foreach(XAttribute attribute in element.Attributes())
                {
                    switch(attribute.Name.LocalName.ToLower())
                    {
                        case "mass":
                        def.Mass = Convert.ToSingle(attribute.Value);
                        break;
                        case "radius":
                        def.Radius = Convert.ToSingle(attribute.Value);
                        break;
                        case "height":
                        def.Height = Convert.ToSingle(attribute.Value);
                        break;
                        case "rotationz":
                        def.RotationZ = Convert.ToSingle(attribute.Value);
                        break;
                        case "stacklimit":
                        def.StackLimit = Convert.ToInt32(attribute.Value);
                        break;
                        case "volumeperunit":
                        def.VolumePerUnit = Convert.ToDecimal(attribute.Value);
                        break;
                        case "isinventoryable":
                        def.IsInventoryable = Convert.ToBoolean(attribute.Value);
                        break;
                        case "name":
                        def.Name = attribute.Value;
                        break;
                        case "icon":
                        def.Icon = attribute.Value;
                        break;
                        default:
                        def.Add(attribute.Name.LocalName.ToLower(), GetObjectFromAttribute(attribute.Value));
                        break;
                    }
                }
                foreach(XElement subelement in element.Elements())
                {

                }
                definition = def;
                return true;
            }
            catch(Exception exception)
            {
                //TODO:loggen
                definition = null;
                return false;
            }
        }
        private Type GetTypeFromElement(XElement element)
        {
            if (NamespaceDeclaration.ResolveNamespaceDeclaration("", element.Name.NamespaceName, out NamespaceDeclaration declaration))
            {
                Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.Equals(declaration.Assembly)).FirstOrDefault();
                if (assembly != null)
                    return assembly.GetType(declaration.Namespace + "." + element.Name.LocalName, false, true);
            }

            return null;
        }
        private object GetObjectFromAttribute(string value)
        {
            return new object();
        }
    }
}
