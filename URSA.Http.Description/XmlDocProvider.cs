﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace URSA.Web.Http.Description
{
    /// <summary>Provides an XML documentation based on <![CDATA[Jolt]]>.</summary>
    public class XmlDocProvider : IXmlDocProvider
    {
        private static readonly IDictionary<Assembly, XDocument> AssemblyCache = new ConcurrentDictionary<Assembly, XDocument>();
        private static readonly Func<XElement, bool> DefaultAuxPredicate = element => element.Name == "summary"; 

        /// <inheritdoc />
        public string GetDescription(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            EnsureAssemblyDocumentation(type.Assembly);
            string memberName = CreateMemberName(type);
            return GetText(AssemblyCache[type.Assembly], element => (element.Attribute("name") != null) && (element.Attribute("name").Value == memberName));
        }

        /// <inheritdoc />
        public string GetDescription(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            EnsureAssemblyDocumentation(method.DeclaringType.Assembly);
            string memberName = CreateMemberName(method);
            return GetText(
                AssemblyCache[method.DeclaringType.Assembly],
                element => (element.Attribute("name") != null) && (element.Attribute("name").Value == memberName));
        }

        /// <inheritdoc />
        public string GetDescription(MethodInfo method, ParameterInfo parameter)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            EnsureAssemblyDocumentation(method.DeclaringType.Assembly);
            string memberName = CreateMemberName(method);
            return GetText(
                AssemblyCache[method.DeclaringType.Assembly],
                element => (element.Attribute("name") != null) && (element.Attribute("name").Value == memberName),
                element => (element.Name.LocalName == "param") && (element.Attribute("name") != null) && (element.Attribute("name").Value == parameter.Name));
        }

        /// <inheritdoc />
        public string GetDescription(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            EnsureAssemblyDocumentation(property.DeclaringType.Assembly);
            return GetText(
                AssemblyCache[property.DeclaringType.Assembly],
                element => (element.Attribute("name") != null) && (element.Attribute("name").Value == CreateMemberName(property)));
        }

        private static string CreateMemberName(MemberInfo member)
        {
            if (member is ConstructorInfo)
            {
                var parameters = String.Join(",", ((MethodInfo)member).GetParameters().Select(parameter => CreateTypeName(parameter.ParameterType)));
                return String.Format(
                    "M:{0}.#ctor{1}",
                    member.DeclaringType.FullName,
                    (parameters.Length == 0 ? String.Empty : String.Format("({0})", parameters)));
            }

            if (member is PropertyInfo)
            {
                return String.Format("P:{0}.{1}", member.DeclaringType.FullName, member.Name);
            }

            if (member is MethodInfo)
            {
                var parameters = String.Join(",", ((MethodInfo)member).GetParameters().Select(parameter => CreateTypeName(parameter.ParameterType)));
                return String.Format(
                    "M:{0}.{1}{2}",
                    member.DeclaringType.FullName,
                    member.Name,
                    (parameters.Length == 0 ? String.Empty : String.Format("({0})", parameters)));
            }

            if (member is Type)
            {
                return String.Format("T:{0}", ((Type)member).FullName);
            }

            return null;
        }

        private static string CreateTypeName(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.FullName.Replace("&", "@");
            }

            var arguments = type.GetGenericArguments();
            return String.Format(
                "{0}{{{1}}}",
                type.FullName.Substring(0, type.FullName.IndexOf('`')),
                String.Join(",", arguments.Select(argument => CreateTypeName(argument))));
        }

        private static string GetText(XDocument document, Func<XElement, bool> predicate, Func<XElement, bool> auxPredicate = null)
        {
            var element = document.Descendants("member").Where(predicate).FirstOrDefault();
            if (element != null)
            {
                element = element.Descendants().Where(auxPredicate ?? DefaultAuxPredicate).FirstOrDefault();
            }

            return (element == null ? null : GetText(element).Trim('\r', '\n', '\t', ' '));
        }

        private static string GetText(XNode node)
        {
            if (node is XText)
            {
                return ((XText)node).Value;
            }

            var element = (XElement)node;
            if ((element.IsEmpty) && (element.HasAttributes))
            {
                return String.Join(" ", element.Attributes().Select(attribute => attribute.Value));
            }

            return String.Join(String.Empty, element.DescendantNodes().Select(GetText));
        }

        private static void EnsureAssemblyDocumentation(Assembly assembly)
        {
            if (AssemblyCache.ContainsKey(assembly))
            {
                return;
            }

            var documentPath = Path.Combine(AppDomain.CurrentDomain.GetPrimaryAssemblyDirectory(), assembly.GetName().Name + ".xml");
            if (!File.Exists(documentPath))
            {
                AssemblyCache[assembly] = new XDocument();
                return;
            }

            Stream documentStream = null;
            try
            {
                documentStream = File.OpenRead(documentPath);
                var document = XDocument.Load(documentStream);
                AssemblyCache[assembly] = document;
            }
            catch
            {
                AssemblyCache[assembly] = new XDocument();
            }
            finally
            {
                if (documentStream != null)
                {
                    documentStream.Close();
                }
            }
        }
    }
}