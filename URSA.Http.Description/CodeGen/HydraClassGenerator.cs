﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using RomanticWeb;
using RomanticWeb.Entities;
using RomanticWeb.Vocabularies;
using URSA.Web.Http.Converters;
using URSA.Web.Http.Description;
using URSA.Web.Http.Description.CodeGen;
using URSA.Web.Http.Description.Entities;
using URSA.Web.Http.Description.Hydra;
using URSA.Web.Http.Description.Mapping;
using URSA.Web.Http.Description.Owl;
using URSA.Web.Http.Reflection;
using IClass = URSA.Web.Http.Description.Hydra.IClass;
using ICollection = URSA.Web.Http.Description.Hydra.ICollection;

namespace URSA.CodeGen
{
    /// <summary>Provides a basic implementation of the <see cref="IClassGenerator" />.</summary>
    public class HydraClassGenerator : IClassGenerator
    {
        private static readonly string EntityClassTemplate = new StreamReader(typeof(HydraClassGenerator).Assembly.GetManifestResourceStream("URSA.Web.Http.Description.CodeGen.Templates.Entity.cs")).ReadToEnd();
        private static readonly string ClientClassTemplate = new StreamReader(typeof(HydraClassGenerator).Assembly.GetManifestResourceStream("URSA.Web.Http.Description.CodeGen.Templates.Client.cs")).ReadToEnd();
        private static readonly string ResponseClassTemplate = new StreamReader(typeof(HydraClassGenerator).Assembly.GetManifestResourceStream("URSA.Web.Http.Description.CodeGen.Templates.Response.cs")).ReadToEnd();
        private static readonly string PropertyTemplate = new StreamReader(typeof(HydraClassGenerator).Assembly.GetManifestResourceStream("URSA.Web.Http.Description.CodeGen.Templates.Property.cs")).ReadToEnd();
        private static readonly string OperationTemplate = new StreamReader(typeof(HydraClassGenerator).Assembly.GetManifestResourceStream("URSA.Web.Http.Description.CodeGen.Templates.Operation.cs")).ReadToEnd();
        private static readonly IDictionary<IResource, string> Namespaces = new ConcurrentDictionary<IResource, string>();
        private static readonly IDictionary<IResource, string> Names = new ConcurrentDictionary<IResource, string>();
        private static readonly string[] KnownDataTypeNamespaces = new[] { XsdUriParser.Xsd, OGuidUriParser.OGuid };

        private readonly IEnumerable<IUriParser> _uriParsers;
        private readonly IUriParser _hydraUriParser;

        /// <summary>Initializes a new instance of the <see cref="HydraClassGenerator"/> class.</summary>
        /// <param name="uriParsers">The URI parsers.</param>
        public HydraClassGenerator(IEnumerable<IUriParser> uriParsers)
        {
            if (uriParsers == null)
            {
                throw new ArgumentNullException("uriParsers");
            }

            _hydraUriParser = (_uriParsers = uriParsers).FirstOrDefault(parser => parser is HydraUriParser);
        }

        /// <inheritdoc />
        public IDictionary<string, string> CreateCode(IClass supportedClass)
        {
            var name = CreateName(supportedClass);
            var @namespace = CreateNamespace(supportedClass);
            var result = new Dictionary<string, string>();
            var properties = AnalyzeProperties(supportedClass).Replace("\r\n        \r\n", "\r\n");
            var operations = AnalyzeOperations(supportedClass, result);
            var mapping = String.Empty;
            if ((_hydraUriParser != null) && (_hydraUriParser.IsApplicable(supportedClass.Id.Uri) == UriParserCompatibility.None))
            {
                mapping = String.Format("\r\n    [RomanticWeb.Mapping.Attributes.Class(\"{0}\")]", supportedClass.Id);
            }

            var classProperties = Regex.Replace(properties.Replace("public new ", "public "), "\\[[^\\]]+\\]\r\n        ", String.Empty);
            var interfaceProperties = Regex.Replace(Regex.Replace(properties.Replace("        public ", "        "), "        private[^\r\n]*", String.Empty), "get { return _[^;]+; } ", "get;");
            if (interfaceProperties.Length > 0)
            {
                interfaceProperties = Regex.Replace(interfaceProperties, "(" + Environment.NewLine + "){3,}", Environment.NewLine + Environment.NewLine).Substring(Environment.NewLine.Length);
            }

            result[name + ".cs"] = String.Format(EntityClassTemplate, @namespace, name, classProperties, interfaceProperties, mapping).Replace("{\r\n\r\n", "{\r\n");
            result[name + "Client.cs"] = String.Format(ClientClassTemplate, @namespace, name, operations);
            return result;
        }

        /// <inheritdoc />
        public string CreateNamespace(IResource resource)
        {
            string result;
            if (Namespaces.TryGetValue(resource, out result))
            {
                return result;
            }

            ParseUri(resource);
            return Namespaces[resource];
        }

        /// <inheritdoc />
        public string CreateName(IResource resource)
        {
            string result;
            if (Names.TryGetValue(resource, out result))
            {
                return result;
            }

            if (resource.Is(resource.Context.Mappings.MappingFor<Web.Http.Description.Rdfs.IClass>().Classes.First().Uri))
            {
                CreateName(resource.AsEntity<IClass>());
            }

            ParseUri(resource);
            return Names[resource];
        }

        private string CreateListName(IClass @class)
        {
            IRestriction itemTypeRestriction;
            IEntity itemType = null;
            @class.SuperClasses().Any(restriction => (restriction.Is(Owl.Restriction)) && ((itemTypeRestriction = restriction.AsEntity<IRestriction>()).OnProperty != null) &&
                (itemTypeRestriction.OnProperty.Id == Rdf.first) && ((itemType = itemTypeRestriction.AllValuesFrom) != null));
            if (itemType != null)
            {
                Namespaces[@class] = typeof(IList<>).Namespace;
                return Names[@class] = String.Format("IList<{0}>", CreateName(itemType.AsEntity<IClass>()));
            }

            Namespaces[@class] = typeof(IList).Namespace;
            return Names[@class] = "IList";
        }

        private string CreateCollectionName(IClass @class)
        {
            IRestriction itemTypeRestriction;
            IEntity itemType = null;
            @class.SuperClasses().Any(restriction => (restriction.Is(Owl.Restriction)) && ((itemTypeRestriction = restriction.AsEntity<IRestriction>()).OnProperty != null) &&
                (AbsoluteUriComparer.Default.Equals(itemTypeRestriction.OnProperty.Id.Uri, @class.Context.Mappings.MappingFor<ICollection>().PropertyFor("Members").Uri)) && 
                ((itemType = itemTypeRestriction.AllValuesFrom) != null));
            if (itemType != null)
            {
                Namespaces[@class] = typeof(IEnumerable<>).Namespace;
                return Names[@class] = String.Format("ICollection<{0}>", CreateName(itemType.AsEntity<IClass>()));
            }

            Namespaces[@class] = typeof(ICollection).Namespace;
            return Names[@class] = "ICollection";
        }

        private string CreateName(IClass @class)
        {
            if (Names.ContainsKey(@class))
            {
                return Names[@class];
            }

            if (@class.IsClass(Rdf.List))
            {
                return CreateListName(@class);
            }

            if (@class.IsClass(@class.Context.Mappings.MappingFor<ICollection>().Classes.First().Uri))
            {
                return CreateCollectionName(@class);
            }

            if (@class.Id is BlankId)
            {
                foreach (var superClass in @class.SubClassOf)
                {
                    var result = Names[@class] = CreateName(superClass.AsEntity<IClass>());
                    Namespaces[@class] = CreateNamespace(superClass.AsEntity<IClass>());
                    return result;
                }
            }

            ParseUri(@class);
            return Names[@class];
        }

        private string CreateName(IOperation operation, string method)
        {
            string result = operation.Label;
            if (operation.Method.Count > 1)
            {
                result = MemberInfoExtensions.PopularNameMappings
                    .Where(item => item.Value.ToString() == method).Select(item => item.Key).FirstOrDefault() ?? method.ToUpperCamelCase();
            }

            if (String.IsNullOrEmpty(result))
            {
                result = CreateName(operation);
            }

            return result;
        }

        private void ParseUri(IResource resource)
        {
            var uriParser = (from parser in _uriParsers
                             orderby parser.IsApplicable(resource.Id.Uri) descending
                             select parser).FirstOrDefault();

            if (uriParser == null)
            {
                throw new InvalidOperationException(String.Format("Cannot find a suitable parser for resource uri '{0}'.", resource.Id.Uri));
            }

            string @namespace;
            Names[resource] = uriParser.Parse(resource.Id.Uri, out @namespace);
            Namespaces[resource] = @namespace;
        }

        private string AnalyzeProperties(IClass supportedClass)
        {
            var properties = new StringBuilder(512);
            foreach (var property in supportedClass.SupportedProperties)
            {
                string typeName;
                string getter;
                string attributes;
                string propertyName = AnalyzeProperty(supportedClass, property, out typeName, out getter, out attributes);
                properties.AppendFormat(
                    PropertyTemplate,
                    propertyName,
                    property.Readable ? getter : String.Empty,
                    property.Writeable ? " set; " : String.Empty,
                    typeName,
                    (propertyName == "Id" ? "new " : String.Empty),
                    attributes);
            }

            return properties.ToString();
        }

        private string AnalyzeProperty(IClass supportedClass, ISupportedProperty property, out string typeName, out string getter, out string attributes)
        {
            var propertyName = (!String.IsNullOrEmpty(property.Property.Label) ? property.Property.Label : CreateName(property.Property.AsEntity<IResource>()));
            typeName = AnalyzePropertyType(property);
            attributes = String.Empty;
            bool singleValue = typeName.IndexOf("<") == -1;
            if ((!property.Writeable) && (!singleValue))
            {
                var targetType = Regex.Replace(
                    Regex.Replace(typeName, "ICollection|IList(?=<)", "List"),
                    "System.Collections.IList",
                    typeof(ArrayList).FullName);
                attributes = String.Format("private {0} _{1} = new {2}();{3}{3}        ", typeName, propertyName.ToLowerCamelCase(), targetType, Environment.NewLine);
            }

            if ((_hydraUriParser != null) && (_hydraUriParser.IsApplicable(property.Property.Id.Uri) == UriParserCompatibility.None))
            {
                attributes += String.Format("[RomanticWeb.Mapping.Attributes.{0}(\"{1}\")]", (singleValue ? "Property" : "Collection"), property.Property.Id);
            }

            getter = ((!property.Writeable) && (!singleValue) ?
                String.Format(" get {{ return _{0}; }} ", propertyName.ToLowerCamelCase()) :
                " get;" + (property.Writeable ? String.Empty : " "));
            return propertyName;
        }

        private string AnalyzePropertyType(ISupportedProperty property)
        {
            var propertyTypeNamespace = "System";
            var propertyTypeName = "Object";
            if (!property.Property.Range.Any())
            {
                return String.Format("{0}.{1}", propertyTypeNamespace, propertyTypeName);
            }

            IClass propertyType = property.Property.Range.First().AsEntity<IClass>();
            propertyTypeName = CreateName(propertyType);
            propertyTypeNamespace = CreateNamespace(propertyType);
            return String.Format("{0}.{1}", propertyTypeNamespace, propertyTypeName);
        }

        private string AnalyzeOperations(IClass supportedClass, IDictionary<string, string> classes)
        {
            var operations = new StringBuilder(1024);
            var supportedOperations = supportedClass.GetSupportedOperations();
            foreach (var operationDescriptor in supportedOperations)
            {
                AnalyzeOperation(supportedClass, operationDescriptor.Operation, operationDescriptor.IriTemplate, operations, classes);
            }

            return operations.ToString();
        }

        // TODO: Add support for header out parameters like totalItems.
        private void AnalyzeOperation(IClass supportedClass, IOperation operation, IIriTemplate template, StringBuilder operations, IDictionary<string, string> classes)
        {
            foreach (var method in operation.Method)
            {
                var bodyArguments = new StringBuilder(256);
                var uriArguments = new StringBuilder(256);
                var parameters = new StringBuilder(256);
                var accept = new StringBuilder(256);
                var contentType = new StringBuilder(256);
                var returns = "void";
                var operationName = CreateName(operation, method);
                var uri = operation.Id.Uri.ToRelativeUri().ToString();
                var returnedType = String.Empty;
                var isReturns = String.Empty;
                bool isContentRangeHeaderParameterAdded = false;
                if (template != null)
                {
                    uri = template.Template;
                    isContentRangeHeaderParameterAdded = AnalyzeTemplate(ref uri, template.Mappings, parameters, uriArguments);
                }

                AnalyzeBody(operation.Expects, parameters, bodyArguments, contentType, operation.MediaTypes);
                if (parameters.Length > 2)
                {
                    parameters.Remove(parameters.Length - 2, 2);
                }

                if (operation.Returns.Any())
                {
                    isReturns = "var result = ";
                    returns = AnalyzeResult(operationName, operation.Returns, classes, accept, operation.MediaTypes);
                    returnedType = String.Format("<{0}>", returns);
                }

                if (accept.Length == 0)
                {
                    accept.Append("            var accept = new string[0];");
                }

                if (contentType.Length == 0)
                {
                    contentType.Append("            var contentType = new string[0];");
                }

                var isStatic = ((operation.Expects.Any(expected => expected == supportedClass)) && (method == "POST") ? " static" : String.Empty);
                operations.AppendFormat(
                    OperationTemplate,
                    isStatic,
                    returns,
                    operationName,
                    parameters,
                    method,
                    uri,
                    uriArguments,
                    bodyArguments,
                    isReturns,
                    returnedType,
                    accept,
                    contentType,
                    (isContentRangeHeaderParameterAdded ? "            totalEntities = (int)uriArguments[\"totalEntities\"];" + Environment.NewLine : String.Empty),
                    (isReturns.Length > 0 ? "            return result;" + Environment.NewLine : String.Empty));
            }
        }

        private bool AnalyzeTemplate(ref string template, IEnumerable<IIriTemplateMapping> mappings, StringBuilder parameters, StringBuilder uriArguments)
        {
            bool expectContentRangeHeader = false;
            bool isContentRangeHeaderParameterAdded = false;
            foreach (var mapping in mappings)
            {
                bool isMultipleValueMapping = Regex.IsMatch(template, mapping.Variable + "\\*[,}]");
                var variableName = mapping.Variable.ToLowerCamelCase();
                var templateSafeVariableName = Regex.Replace(variableName, "\\W", match => String.Join(String.Empty, match.Value.Select(@char => "%" + Convert.ToUInt32(@char).ToString("X"))));
                var safeVariableName = Regex.Replace(variableName, "\\W", "_");
                template = template.Replace(mapping.Variable, templateSafeVariableName);
                IClass expected = null;
                if (mapping.Property != null)
                {
                    expectContentRangeHeader |= (mapping.Property.Id.Uri.ToString() == DescriptionBuildingServerBahaviorAttributeVisitor<PropertyInfo>.OData + "$skip") ||
                        (mapping.Property.Id.Uri.ToString() == DescriptionBuildingServerBahaviorAttributeVisitor<PropertyInfo>.OData + "$take");
                    if ((mapping.Property.Range.Any()) && ((expected = mapping.Property.Range.First().AsEntity<IClass>()) != null) && 
                        (expected.Id is BlankId) && (expected.SubClassOf.Any()))
                    {
                        expected = expected.SubClassOf.First().AsEntity<IClass>();
                    }
                }

                var @namespace = "System";
                var name = "Object";
                if (expected != null)
                {
                    @namespace = CreateNamespace(expected);
                    name = CreateName(expected);
                }

                if ((expectContentRangeHeader) && (!isContentRangeHeaderParameterAdded))
                {
                    parameters.Insert(0, "out System.Int32 totalEntities, ");
                    uriArguments.AppendFormat("            uriArguments[\"totalEntities\"] = totalEntities = 0;{0}", Environment.NewLine);
                    isContentRangeHeaderParameterAdded = true;
                }

                parameters.AppendFormat(isMultipleValueMapping ? "System.Collections.Generic.IEnumerable<{0}.{1}> {2}, " : "{0}.{1} {2}, ", @namespace, name, safeVariableName);
                uriArguments.AppendFormat("            uriArguments[\"{1}\"] = {0};{2}", safeVariableName, templateSafeVariableName, Environment.NewLine);
            }

            return isContentRangeHeaderParameterAdded;
        }

        private void AnalyzeBody(
            IEnumerable<IClass> expects,
            StringBuilder parameters,
            StringBuilder bodyArguments,
            StringBuilder contentType,
            IEnumerable<string> operationMediaTypes)
        {
            IEnumerable<string> validMediaTypes = new List<string>();
            var validMediaTypesList = (IList<string>)validMediaTypes;
            foreach (var expected in expects)
            {
                string variableName = expected.Label.ToLowerCamelCase();
                string @namespace;
                string name = AnalyzeType(expected, out @namespace, validMediaTypesList);
                parameters.AppendFormat(
                    "{3}{0}.{1}{4} {2}, ",
                    @namespace,
                    name,
                    variableName,
                    (!expected.IsCollection() ? String.Empty : "System.Collections.Generic.IEnumerable<"),
                    (!expected.IsCollection() ? String.Empty : ">"));
                bodyArguments.AppendFormat(", {0}", variableName);
            }

            if ((validMediaTypesList.Count == 0 ? validMediaTypes = operationMediaTypes : validMediaTypes).Any())
            {
                contentType.AppendFormat(
                    "            var contentType = new string[] {{\r\n                {0} }};",
                    String.Join(",\r\n                ", validMediaTypes.Select(mediaType => String.Format("\"{0}\"", mediaType))));
            }
        }

        private string AnalyzeResult(
            string operationName,
            IEnumerable<IClass> returns,
            IDictionary<string, string> classes,
            StringBuilder accept,
            IEnumerable<string> operationMediaTypes)
        {
            string result;
            string @namespace;
            string name;
            IEnumerable<string> validMediaTypes = new List<string>();
            var validMediaTypesList = (IList<string>)validMediaTypes;

            if (returns.Count() > 1)
            {
                StringBuilder includes = new StringBuilder();
                result = String.Format("{0}Result", operationName);
                if (includes.ToString().IndexOf(result) == -1)
                {
                    var properties = new StringBuilder(256);
                    foreach (var returned in returns)
                    {
                        name = AnalyzeType(returned, out @namespace, validMediaTypesList);
                        properties.AppendFormat(PropertyTemplate, name, " get;", String.Empty, @namespace, String.Empty, String.Empty);
                    }

                    includes.AppendFormat(ResponseClassTemplate, result, properties);
                    classes[result + ".cs"] = includes.ToString();
                }
            }
            else
            {
                name = AnalyzeType(returns.First(), out @namespace, validMediaTypesList);
                if ((!name.StartsWith("ICollection")) && (!name.StartsWith("IList")) && (!(validMediaTypesList.Count == 0 ? operationMediaTypes : validMediaTypes)
                    .Join(EntityConverter.MediaTypes, outer => outer, inner => inner, (outer, inner) => inner).Any()))
                {
                    name = name.TrimStart('I');
                }

                result = String.Format("{0}.{1}", @namespace, name);
            }

            if ((validMediaTypesList.Count == 0 ? validMediaTypes = operationMediaTypes : validMediaTypes).Any())
            {
                accept.AppendFormat(
                    "            var accept = new string[] {{\r\n                {0} }};",
                    String.Join(",\r\n                ", validMediaTypes.Select(mediaType => String.Format("\"{0}\"", mediaType))));
            }

            return result;
        }

        private string AnalyzeType(IClass @class, out string @namespace, IList<string> validMediaTypes = null)
        {
            if (validMediaTypes != null)
            {
                validMediaTypes.AddRange(@class.MediaTypes);
            }

            var name = CreateName(@class);
            @namespace = CreateNamespace(@class);
            return name;
        }
    }
}