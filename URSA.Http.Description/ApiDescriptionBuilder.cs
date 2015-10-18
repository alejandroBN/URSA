﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using URSA.Web.Description;
using URSA.Web.Description.Http;
using URSA.Web.Http.Converters;
using URSA.Web.Http.Description.Hydra;
using URSA.Web.Http.Description.Mapping;
using URSA.Web.Http.Mapping;
using URSA.Web.Mapping;
using IClass = URSA.Web.Http.Description.Hydra.IClass;

namespace URSA.Web.Http.Description
{
    /// <summary>Builds API description.</summary>
    /// <typeparam name="T">Type of the API to describe</typeparam>
    public class ApiDescriptionBuilder<T> : IApiDescriptionBuilder<T> where T : IController
    {
        private static readonly string[] RdfMediaTypes = EntityConverter.MediaTypes;
        private static readonly string[] NonRdfMediaTypes = { JsonConverter.ApplicationJson, XmlConverter.ApplicationXml, XmlConverter.TextXml };

        private readonly IHttpControllerDescriptionBuilder<T> _descriptionBuilder;
        private readonly IXmlDocProvider _xmlDocProvider;
        private readonly IEnumerable<ITypeDescriptionBuilder> _typeDescriptionBuilders;
        private readonly IEnumerable<IServerBehaviorAttributeVisitor> _serverBehaviorAttributeVisitors;
        private Type _specializationType;

        /// <summary>Initializes a new instance of the <see cref="ApiDescriptionBuilder{T}" /> class.</summary>
        /// <param name="descriptionBuilder">Description builder.</param>
        /// <param name="xmlDocProvider">The XML documentation provider.</param>
        /// <param name="typeDescriptionBuilders">Type description builders.</param>
        /// <param name="serverBehaviorAttributeVisitors">Server behavior attribute visitors.</param>
        public ApiDescriptionBuilder(IHttpControllerDescriptionBuilder<T> descriptionBuilder, IXmlDocProvider xmlDocProvider, IEnumerable<ITypeDescriptionBuilder> typeDescriptionBuilders, IEnumerable<IServerBehaviorAttributeVisitor> serverBehaviorAttributeVisitors)
        {
            if (descriptionBuilder == null)
            {
                throw new ArgumentNullException("descriptionBuilder");
            }

            if (xmlDocProvider == null)
            {
                throw new ArgumentNullException("xmlDocProvider");
            }

            if (typeDescriptionBuilders == null)
            {
                throw new ArgumentNullException("typeDescriptionBuilders");
            }

            if (!typeDescriptionBuilders.Any())
            {
                throw new ArgumentOutOfRangeException("typeDescriptionBuilders");
            }

            _descriptionBuilder = descriptionBuilder;
            _xmlDocProvider = xmlDocProvider;
            _typeDescriptionBuilders = typeDescriptionBuilders;
            _serverBehaviorAttributeVisitors = serverBehaviorAttributeVisitors ?? new IServerBehaviorAttributeVisitor[0];
        }

        private Type SpecializationType
        {
            get
            {
                return _specializationType ??
                    (_specializationType = (from @interface in typeof(T).GetInterfaces()
                                            where (@interface.IsGenericType) && (typeof(IController<>).IsAssignableFrom(@interface.GetGenericTypeDefinition()))
                                            from type in @interface.GetGenericArguments()
                                            select type).FirstOrDefault() ?? typeof(object));
            }
        }

        /// <summary>Builds an API description.</summary>
        /// <param name="apiDocumentation">API documentation.</param>
        /// <param name="profiles">Requested media type profiles.</param>
        public void BuildDescription(IApiDocumentation apiDocumentation, IEnumerable<Uri> profiles)
        {
            if (apiDocumentation == null)
            {
                throw new ArgumentNullException("apiDocumentation");
            }

            if (SpecializationType == typeof(object))
            {
                return;
            }

            var typeDescriptionBuilder = GetTypeDescriptionBuilder(profiles);
            var context = DescriptionContext.ForType(apiDocumentation, SpecializationType, typeDescriptionBuilder);
            IClass specializationType = context.BuildTypeDescription();
            BuildDescription(context, specializationType);
        }

        private static void BuildOperationMediaType(IEnumerable<IClass> resources, OperationInfo operation, bool requiresRdf)
        {
            foreach (var resource in resources)
            {
                foreach (var mediaType in operation.UnderlyingMethod.GetCustomAttributes<AsMediaTypeAttribute>())
                {
                    resource.MediaTypes.Add(mediaType.MediaType);
                }

                if (resource.MediaTypes.Count != 0)
                {
                    continue;
                }

                foreach (var mediaType in (requiresRdf ? RdfMediaTypes : NonRdfMediaTypes))
                {
                    resource.MediaTypes.Add(mediaType);
                }
            }
        }

        private static Rdfs.IProperty GetMappingProperty(DescriptionContext context, ParameterInfo parameter)
        {
            IClass description = context[context.Type];
            Rdfs.IProperty resultCandidate = null;
            IResource parameterType = null;
            foreach (var supportedProperty in description.SupportedProperties)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(supportedProperty.Property.Label, parameter.Name))
                {
                    return supportedProperty.Property;
                }

                if (parameterType == null)
                {
                    parameterType = (context.ContainsType(parameter.ParameterType) ? context[parameter.ParameterType] : context.BuildTypeDescription());
                }

                if (supportedProperty.Property.Range.Any(range => (range.Id == parameterType.Id) || 
                    ((range is IClass) && (((IClass)range).SubClassOf.Any(subClass => subClass.Id == parameterType.Id)))))
                {
                    resultCandidate = supportedProperty.Property;
                }
            }

            return resultCandidate;
        }

        private ITypeDescriptionBuilder GetTypeDescriptionBuilder(IEnumerable<Uri> profiles)
        {
            if ((profiles == null) || (!profiles.Any()))
            {
                return _typeDescriptionBuilders.FirstOrDefault();
            }

            var result = (from descriptionBuilder in _typeDescriptionBuilders
                          from supportedProfile in descriptionBuilder.SupportedProfiles
                          join requestedProfile in profiles on supportedProfile equals requestedProfile into matchingProfiles
                          orderby matchingProfiles.Count() descending
                          select descriptionBuilder).FirstOrDefault();

            if (result == null)
            {
                throw new InvalidOperationException(String.Format("No matching profile ({0}) type description builder found.", String.Join("', '", profiles)));
            }

            return result;
        }

        private void BuildDescription(DescriptionContext context, IClass specializationType)
        {
            context.ApiDocumentation.SupportedClasses.Add(specializationType);
            var description = _descriptionBuilder.BuildDescriptor();
            foreach (OperationInfo<Verb> operation in description.Operations)
            {
                IIriTemplate template;
                var operationDefinition = BuildOperation(context, operation, out template);
                if (template != null)
                {
                    ITemplatedLink templatedLink = context.ApiDocumentation.Context.Create<ITemplatedLink>(template.Id.Uri.AbsoluteUri.Replace("#template", "#withTemplate"));
                    templatedLink.Operations.Add(operationDefinition);
                    context.ApiDocumentation.Context.Store.ReplacePredicateValues(
                        specializationType.Id,
                        Node.ForUri(templatedLink.Id.Uri),
                        () => new Node[] { Node.ForUri(template.Id.Uri) },
                        specializationType.Id.Uri);
                }
                else
                {
                    specializationType.SupportedOperations.Add(operationDefinition);
                }
            }
        }

        private IOperation BuildOperation(DescriptionContext context, OperationInfo<Verb> operation, out IIriTemplate template)
        {
            IOperation result = operation.AsOperation(context.ApiDocumentation);
            result.Label = operation.UnderlyingMethod.Name;
            result.Description = _xmlDocProvider.GetDescription(operation.UnderlyingMethod);
            result.Method.Add(_descriptionBuilder.GetMethodVerb(operation.UnderlyingMethod).ToString());
            template = BuildTemplate(context, operation, result);
            bool isRdfRequired;
            bool requiresRdf = false;
            var arguments = operation.Arguments.Where(parameter => parameter.Source is FromBodyAttribute).Select(parameter => parameter.Parameter);
            var results = operation.Results.Where(output => output.Target is ToBodyAttribute).Select(parameter => parameter.Parameter);

            foreach (var value in arguments)
            {
                var childContext = context.ForType(value.ParameterType);
                var expected = childContext.BuildTypeDescription(out isRdfRequired);
                expected = childContext.SubClass(expected);
                expected.Label = value.Name ?? "instance";
                result.Expects.Add(expected);
                requiresRdf |= isRdfRequired;
            }

            foreach (var value in results)
            {
                var childContext = context.ForType(value.ParameterType);
                var returned = childContext.BuildTypeDescription(out isRdfRequired);
                returned = childContext.SubClass(returned);
                result.Returns.Add(returned);
                requiresRdf |= isRdfRequired;
            }

            BuildOperationMediaType(result.Returns, operation, requiresRdf);
            BuildOperationMediaType(result.Expects, operation, requiresRdf);
            return result;
        }

        private IIriTemplate BuildTemplate(DescriptionContext context, OperationInfo<Verb> operation, IOperation operationDocumentation)
        {
            IEnumerable<ArgumentInfo> parameterMapping;
            var uriTemplate = _descriptionBuilder.GetOperationUriTemplate(operation.UnderlyingMethod, out parameterMapping);
            if (String.IsNullOrEmpty(uriTemplate))
            {
                return null;
            }

            IIriTemplate template = null;
            var templateUri = operationDocumentation.Id.Uri.AddFragment("template");
            var templateMappings = from mapping in parameterMapping where !(mapping.Source is FromBodyAttribute) select BuildTemplateMapping(context, templateUri, operation, mapping);
            foreach (var templateMapping in templateMappings)
            {
                if (template == null)
                {
                    template = context.ApiDocumentation.Context.Create<IIriTemplate>(templateUri);
                    template.Template = uriTemplate;
                }

                template.Mappings.Add(templateMapping);
            }

            return template;
        }

        private IIriTemplateMapping BuildTemplateMapping(DescriptionContext context, Uri templateUri, OperationInfo<Verb> operation, ArgumentInfo mapping)
        {
            IIriTemplateMapping templateMapping = context.ApiDocumentation.Context.Create<IIriTemplateMapping>(templateUri.AddFragment(mapping.VariableName));
            templateMapping.Variable = mapping.VariableName;
            templateMapping.Required = (mapping.Parameter.ParameterType.IsValueType) && (!mapping.Parameter.HasDefaultValue);
            templateMapping.Description = _xmlDocProvider.GetDescription(operation.UnderlyingMethod, mapping.Parameter);
            var linqBehaviors = mapping.Parameter.GetCustomAttributes<LinqServerBehaviorAttribute>(true);
            if (linqBehaviors.Any())
            {
                IClass range = (context.ContainsType(typeof(int)) ? context[typeof(int)] : context.TypeDescriptionBuilder.BuildTypeDescription(context.ForType(typeof(int))));
                range = context.SubClass(range);
                range.SingleValue = range.SingleValue;
                range.MediaTypes.AddRange(range.MediaTypes);
                foreach (var visitor in _serverBehaviorAttributeVisitors)
                {
                    linqBehaviors.Accept(visitor, templateMapping);
                }

                if (templateMapping.Property != null)
                {
                    templateMapping.Property.Range.Add(range);
                }
            }
            else if (context.Type != typeof(object))
            {
                templateMapping.Property = GetMappingProperty(context, mapping.Parameter);
            }

            return templateMapping;
        }
    }
}