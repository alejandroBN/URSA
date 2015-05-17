﻿using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using URSA.Web.Description;
using URSA.Web.Mapping;

namespace URSA.Web.Http.Tests.Testing
{
    /// <summary>Provides useful <see cref="MethodInfo" /> extensions.</summary>
    public static class MethodInfoExtensions
    {
        private static readonly string[] PopularIdentifierPropertyNames = { "Id", "Identifier", "Identity", "Key" };

        /// <summary>Converts a method into an operation descriptor..</summary>
        /// <typeparam name="T">Type of the protocol specific command.</typeparam>
        /// <param name="method">The method.</param>
        /// <param name="baseUri">The call URI.</param>
        /// <param name="verb">The verb.</param>
        /// <param name="callUri">Call URI.</param>
        /// <param name="values">Call values.</param>
        /// <returns>Operation descriptor.</returns>
        public static OperationInfo<T> ToOperationInfo<T>(this MethodInfo method, string baseUri, T verb, out string callUri, params object[] values)
        {
            var methodUri = method.GetCustomAttributes<RouteAttribute>().Select(attribute => attribute.Uri.ToString()).FirstOrDefault() ?? method.Name.ToLower();
            var actualCallUri = baseUri + methodUri;
            var queryString = String.Empty;
            var arguments = method.GetParameters()
                .Where(parameter => !parameter.IsOut)
                .Select((parameter, index) =>
                {
                    string uriTemplate = null;
                    var target = parameter.GetParameterTarget();
                    if (target is FromUriAttribute)
                    {
                        uriTemplate = (methodUri += String.Format("/{0}/{{?{0}}}", parameter.Name));
                        actualCallUri += String.Format("/{0}/{1}", parameter.Name, values[index]);
                    }
                    else if (target is FromQueryStringAttribute)
                    {
                        queryString += (queryString.Length == 0 ? "?" : "&") + String.Format("{0}={{?{0}}}", parameter.Name);
                        uriTemplate = methodUri + queryString;
                    }

                    return (ValueInfo)new ArgumentInfo(parameter, target, uriTemplate, parameter.Name);
                })
                .Concat(method.GetParameters()
                .Where(parameter => parameter.IsOut)
                .Select(parameter => (ValueInfo)new ResultInfo(parameter, parameter.GetResultTarget(), null, null)));
            if (method.ReturnParameter != null)
            {
                arguments = arguments.Concat(new[] { new ResultInfo(method.ReturnParameter, method.ReturnParameter.GetResultTarget(), null, null) });
            }

            callUri = actualCallUri + queryString;
            var queryStringParameters = Regex.Matches(callUri, "[?&]([^=]+)=[^&]+").Cast<Match>();
            var queryStringRegex = (queryStringParameters.Any() ? "[?&](" + String.Join("|", queryStringParameters.Select(item => item.Groups[1].Value)) + ")=[^&]+" : String.Empty);
            return new OperationInfo<T>(method, new Uri(methodUri, UriKind.RelativeOrAbsolute), callUri, new Regex("^" + methodUri + queryStringRegex + "$"), verb, arguments.ToArray());
        }

        private static ParameterSourceAttribute GetParameterTarget(this ParameterInfo parameter)
        {
            var explicitSetting = parameter.GetCustomAttribute<ParameterSourceAttribute>(true);
            if (explicitSetting != null)
            {
                return explicitSetting;
            }

            if ((typeof(Guid) == parameter.ParameterType) || (typeof(DateTime) == parameter.ParameterType) ||
                ((IsIdentity(parameter.ParameterType)) &&
                (PopularIdentifierPropertyNames.Contains(parameter.Name, StringComparer.OrdinalIgnoreCase))))
            {
                return FromUriAttribute.For(parameter);
            }

            if ((!parameter.ParameterType.IsValueType) && (typeof(string) != parameter.ParameterType) &&
                (!((parameter.ParameterType.IsEnumerable()) && (IsNumber(parameter.ParameterType.GetItemType())))))
            {
                return FromBodyAttribute.For(parameter);
            }

            return FromQueryStringAttribute.For(parameter);
        }

        private static ResultTargetAttribute GetResultTarget(this ParameterInfo parameter)
        {
            var explicitSetting = parameter.GetCustomAttribute<ResultTargetAttribute>(true);
            if (explicitSetting != null)
            {
                return explicitSetting;
            }

            if ((typeof(Guid).MakeByRefType() == parameter.ParameterType) || (typeof(DateTime).MakeByRefType() == parameter.ParameterType) ||
                ((IsIdentity(parameter.ParameterType)) &&
                (PopularIdentifierPropertyNames.Contains(parameter.Name, StringComparer.OrdinalIgnoreCase))))
            {
                return new ToHeaderAttribute(Header.Location);
            }

            return new ToBodyAttribute();
        }

        private static bool IsIdentity(Type type)
        {
            return ((typeof(Int32) == type) || (typeof(UInt32) == type) || (typeof(Int64) == type) || (typeof(UInt64) == type) ||
                (typeof(Int32).MakeByRefType() == type) || (typeof(UInt32).MakeByRefType() == type) || (typeof(Int64).MakeByRefType() == type) || (typeof(UInt64).MakeByRefType() == type));
        }

        private static bool IsNumber(Type type)
        {
            return (typeof(SByte) == type) || (typeof(Byte) == type) || (typeof(Int16) == type) || (typeof(UInt16) == type) ||
                (typeof(Int32) == type) || (typeof(UInt32) == type) || (typeof(Int64) == type) || (typeof(UInt64) == type) ||
                (typeof(Single) == type) || (typeof(Double) == type) || (typeof(Decimal) == type);
        }
    }
}