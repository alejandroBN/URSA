﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using URSA.Security;
using URSA.Web.Converters;

namespace URSA.Web.Http.Testing
{
    [ExcludeFromCodeCoverage]
    public abstract class ConverterTest<T, TI> where T : class, IConverter
    {
        protected const string OperationName = "PostStrings";

        protected static readonly HttpUrl BaseUrl = (HttpUrl)UrlParser.Parse("http://temp.org/api/test/");

        private const string ContentType = "text/plain";

        protected virtual string SingleEntityContentType { get { return ContentType; } }

        protected virtual string MultipleEntitiesContentType { get { return ContentType; } }

        protected virtual string SingleEntityBody { get { return String.Format(CultureInfo.InvariantCulture, "{0}", SingleEntity); } }

        protected virtual string MultipleEntitiesBody { get { return String.Join("\r\n", MultipleEntities.Select(entity => String.Format(CultureInfo.InvariantCulture, "{0}", entity))); } }

        protected virtual bool SupportsSingleInstances { get { return true; } }

        protected virtual bool SupportsMultipleInstances { get { return true; } }

        protected virtual bool SupportsSerialization { get { return true; } }

        protected virtual bool SupportsDeserialization { get { return true; } }

        protected abstract TI SingleEntity { get; }

        protected abstract TI[] MultipleEntities { get; }

        protected T Converter { get; private set; }

        [TestMethod]
        public virtual void it_should_not_acknowledge_the_converter_as_a_match_against_incompatible_type_when_serializing()
        {
            if ((!SupportsSerialization) || (!SupportsSingleInstances))
            {
                return;
            }

            var result = CanConvertFrom<object>("POST", OperationName, SingleEntityContentType, SingleEntityBody);

            result.Should().Be(CompatibilityLevel.None);
        }

        [TestMethod]
        public virtual void it_should_throw_when_instance_being_serialized_mismatches_the_converter_supported_type()
        {
            if ((!SupportsSerialization) || (!SupportsSingleInstances))
            {
                return;
            }

            Converter.Invoking(instance => ConvertFrom("POST", OperationName, SingleEntityContentType, (object)Encoding.UTF8)).ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public virtual void it_should_test_serialization_compatibility()
        {
            if ((!SupportsSerialization) || (!SupportsSingleInstances))
            {
                return;
            }

            var result = CanConvertFrom("POST", OperationName, SingleEntityContentType, SingleEntity);

            result.Should().NotBe(CompatibilityLevel.None);
        }

        [TestMethod]
        public virtual void it_should_throw_when_no_given_type_is_provided_for_serialization_compatibility_test()
        {
            if (!SupportsSerialization)
            {
                return;
            }

            Converter.Invoking(converter => converter.CanConvertFrom(null, null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("givenType");
        }

        [TestMethod]
        public virtual void it_should_throw_when_no_response_is_provided_for_serialization_compatibility_test()
        {
            if (!SupportsSerialization)
            {
                return;
            }

            Converter.Invoking(converter => converter.CanConvertFrom(typeof(TI), null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public virtual void it_should_throw_when_no_response_is_provided_for_serialization()
        {
            if (!SupportsSerialization)
            {
                return;
            }

            Converter.Invoking(converter => converter.ConvertFrom(SingleEntity, null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public virtual void it_should_throw_when_no_given_type_is_provided_for_serialization()
        {
            if (!SupportsSerialization)
            {
                return;
            }

            Converter.Invoking(converter => converter.ConvertFrom(null, SingleEntity, null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("givenType");
        }

        [TestMethod]
        public virtual void it_should_serialize_an_entity_to_message()
        {
            if ((!SupportsSerialization) || (!SupportsSingleInstances))
            {
                return;
            }

            var content = ConvertFrom("POST", OperationName, SingleEntityContentType, SingleEntity);
            AssertSingleEntityMessage(content);
        }

        [TestMethod]
        public virtual void it_should_serialize_array_of_entities_to_message()
        {
            if ((!SupportsSerialization) || (!SupportsMultipleInstances))
            {
                return;
            }

            var content = ConvertFrom("POST", OperationName, MultipleEntitiesContentType, MultipleEntities);
            AssertMultipleEntitiesMessage(content);
        }

        [TestMethod]
        public virtual void it_should_do_nothing_if_the_instance_being_serialized_is_null()
        {
            if ((!SupportsSerialization) || (!SupportsSingleInstances))
            {
                return;
            }

            ConvertFrom("POST", OperationName, SingleEntityContentType, null).Should().BeNullOrEmpty();
        }

        [TestMethod]
        public virtual void it_should_test_deserialization_compatibility()
        {
            if ((!SupportsDeserialization) || (!SupportsSingleInstances))
            {
                return;
            }

            var result = CanConvertTo<TI>("POST", OperationName, SingleEntityContentType, SingleEntityBody);

            result.Should().NotBe(CompatibilityLevel.None);
        }

        [TestMethod]
        public virtual void it_should_not_acknowledge_the_converter_as_a_match_against_incompatible_type_when_deserializing()
        {
            if ((!SupportsDeserialization) || (!SupportsSingleInstances))
            {
                return;
            }

            var result = CanConvertTo<object>("POST", OperationName, SingleEntityContentType, SingleEntityBody);

            result.Should().Be(CompatibilityLevel.None);
        }

        [TestMethod]
        public virtual void it_should_throw_when_no_expected_type_is_provided_for_deserialization_compatibility_test()
        {
            Converter.Invoking(converter => converter.CanConvertTo(null, null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("expectedType");
        }

        [TestMethod]
        public virtual void it_should_throw_when_no_request_is_provided_for_deserialization_compatibility_test()
        {
            Converter.Invoking(converter => converter.CanConvertTo(typeof(TI), null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public virtual void it_should_throw_when_no_request_is_provided_for_deserialization()
        {
            Converter.Invoking(converter => converter.ConvertTo<TI>((IRequestInfo)null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public virtual void it_should_throw_when_no_expected_type_is_provided_for_deserialization()
        {
            Converter.Invoking(converter => converter.ConvertTo(null, (IRequestInfo)null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("expectedType");
        }

        [TestMethod]
        public virtual void it_should_deserialize_message_body_as_an_entity()
        {
            if ((!SupportsDeserialization) || (!SupportsSingleInstances))
            {
                return;
            }

            var result = ConvertBodyTo<TI>("POST", OperationName, SingleEntityContentType, SingleEntityBody);
            AssertSingleEntity(result);
        }

        [TestMethod]
        public virtual void it_should_deserialize_message_as_an_entity()
        {
            if ((!SupportsDeserialization) || (!SupportsSingleInstances))
            {
                return;
            }

            var result = ConvertTo<TI>("POST", OperationName, SingleEntityContentType, SingleEntityBody);
            AssertSingleEntity(result);
        }

        [TestMethod]
        public virtual void it_should_deserialize_message_body_as_an_array_of_entities()
        {
            if ((!SupportsDeserialization) || (!SupportsMultipleInstances))
            {
                return;
            }

            var result = ConvertBodyTo<TI[]>("POST", OperationName, MultipleEntitiesContentType, MultipleEntitiesBody);
            AssertMultipleEntities(result);
        }

        [TestMethod]
        public virtual void it_should_deserialize_message_as_an_array_of_entities()
        {
            if ((!SupportsDeserialization) || (!SupportsMultipleInstances))
            {
                return;
            }

            var result = ConvertTo<TI[]>("POST", OperationName, MultipleEntitiesContentType, MultipleEntitiesBody);
            AssertMultipleEntities(result);
        }

        [TestMethod]
        public virtual void it_should_throw_when_no_given_type_is_provided_for_string_deserialization()
        {
            Converter.Invoking(instance => instance.ConvertTo(null, SingleEntityBody)).ShouldThrow<ArgumentNullException>().Which.ParamName.Should().Be("expectedType");
        }

        [TestInitialize]
        public void Setup()
        {
            Converter = CreateInstance();
        }

        [TestCleanup]
        public void Teardown()
        {
            Converter = null;
        }

        protected virtual void AssertSingleEntityMessage(string result)
        {
            result.Should().Be(SingleEntityBody);
        }

        protected virtual void AssertMultipleEntitiesMessage(string result)
        {
            result.Should().Be(MultipleEntitiesBody);
        }

        protected virtual void AssertSingleEntity(TI result)
        {
            result.Should().Be(SingleEntity);
        }

        protected virtual void AssertMultipleEntities(TI[] result)
        {
            result.Should().NotBeNull();
            result.Should().HaveCount(MultipleEntities.Length);
            result.First().Should().Be(MultipleEntities.First());
            result.Last().Should().Be(MultipleEntities.Last());
        }

        protected virtual T CreateInstance()
        {
            return (T)typeof(T).GetConstructor(new Type[0]).Invoke(null);
        }

        protected CompatibilityLevel CanConvertFrom<TT>(string method, string handler, string mediaType, TT body)
        {
            return Converter.CanConvertFrom<TT>(MakeResponse(MakeRequest(method, handler, mediaType)));
        }

        protected string ConvertFrom<TT>(string method, string handler, string mediaType, TT body)
        {
            var response = MakeResponse(MakeRequest(method, handler, mediaType));
            Converter.ConvertFrom(body, response);
            response.Body.Seek(0, SeekOrigin.Begin);
            return new StreamReader(response.Body).ReadToEnd();
        }

        protected string ConvertFrom(string method, string handler, string mediaType, object body)
        {
            var response = MakeResponse(MakeRequest(method, handler, mediaType));
            Converter.ConvertFrom(typeof(TI), body, response);
            response.Body.Seek(0, SeekOrigin.Begin);
            return new StreamReader(response.Body).ReadToEnd();
        }

        protected TT ConvertBodyTo<TT>(string method, string handler, string mediaType, string body)
        {
            return Converter.ConvertTo<TT>(body);
        }

        protected TT ConvertTo<TT>(string method, string handler, string mediaType, string body)
        {
            return Converter.ConvertTo<TT>(MakeRequest(method, handler, mediaType, body));
        }

        protected CompatibilityLevel CanConvertTo<TT>(string method, string handler, string mediaType, string body)
        {
            return Converter.CanConvertTo<TT>(MakeRequest(method, handler, mediaType, body));
        }

        protected RequestInfo MakeRequest(string method, string handler, string mediaType, string body = null)
        {
            var headers = new HeaderCollection { ContentType = mediaType };
            if (body != null)
            {
                headers.ContentLength = body.Length;
            }

            headers.Accept = mediaType;
            return new RequestInfo(
                Verb.Parse(method),
                (HttpUrl)BaseUrl.AddSegment(handler),
                (body != null ? new MemoryStream(Encoding.UTF8.GetBytes(body)) : new MemoryStream()),
                new BasicClaimBasedIdentity(),
                headers);
        }

        protected ResponseInfo MakeResponse(RequestInfo request)
        {
            return new StringResponseInfo(null, request);
        }
    }
}