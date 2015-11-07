﻿/*globals xsd, rdf, rdfs, owl, guid, hydra, ursa */
(function() {
    "use strict";
    var invalidArgumentPassed = "Invalid {0} passed.";
    window.xsd = new String("http://www.w3.org/2001/XMLSchema#"); //jshint ignore:line
    xsd.string = xsd + "string";
    xsd.boolean = xsd + "boolean";
    xsd.byte = xsd + "byte";
    xsd.unsignedByte = xsd + "unsignedByte";
    xsd.short = xsd + "short";
    xsd.unsignedShort = xsd + "unsignedShort";
    xsd.int = xsd + "int";
    xsd.unsignedInt = xsd + "unsignedInt";
    xsd.long = xsd + "long";
    xsd.unsignedLong = xsd + "unsignedLong";
    xsd.integer = xsd + "integer";
    xsd.nonPositiveInteger = xsd + "nonPositiveInteger";
    xsd.nonNegativeInteger = xsd + "nonNegativeInteger";
    xsd.positiveInteger = xsd + "positiveInteger";
    xsd.negativeInteger = xsd + "negativeInteger";
    xsd.float = xsd + "float";
    xsd.double = xsd + "double";
    xsd.decimal = xsd + "decimal";
    xsd.dateTime = xsd + "dateTime";
    xsd.time = xsd + "time";
    xsd.date = xsd + "date";
    xsd.gYear = xsd + "gYear";
    xsd.gMonth = xsd + "gMonth";
    xsd.gDay = xsd + "gDay";
    xsd.gYearMonth = xsd + "gYearMonth";
    window.rdf = new String("http://www.w3.org/1999/02/22-rdf-syntax-ns#"); //jshint ignore:line
    rdf.Property = rdf + "Property";
    window.rdfs = new String("http://www.w3.org/2000/01/rdf-schema#"); //jshint ignore:line
    rdfs.Class = rdfs + "Class";
    rdfs.subClassOf = rdfs + "subClassOf";
    rdfs.range = rdfs + "range";
    rdfs.label = rdfs + "label";
    rdfs.comment = rdfs + "comment";
    window.owl = new String("http://www.w3.org/2002/07/owl#"); //jshint ignore:line
    owl.onProperty = owl + "onProperty";
    owl.minCardinality = owl + "minCardinality";
    owl.maxCardinality = owl + "maxCardinality";
    owl.InverseFunctionalProperty = owl + "InverseFunctionalProperty";
    owl.Restriction = owl + "Restriction";
    window.hydra = new String("http://www.w3.org/ns/hydra/core#"); //jshint ignore:line
    hydra.Resource = hydra + "Resource";
    hydra.Class = hydra + "Class";
    hydra.Operation = hydra + "Operation";
    hydra.SupportedProperty = hydra + "SupportedProperty";
    hydra.Link = hydra + "Link";
    hydra.TemplatedLink = hydra + "TemplatedLink";
    hydra.IriTemplate = hydra + "IriTemplate";
    hydra.IriTemplateMapping = hydra + "IriTemplateMapping";
    hydra.ApiDocumentation = hydra + "ApiDocumentation";
    hydra.CreateResourceOperation = hydra + "CreateResourceOperation";
    hydra.ReplaceResourceOperation = hydra + "ReplaceResourceOperation";
    hydra.DeleteResourceOperation = hydra + "DeleteResourceOperation";
    hydra.entrypoint = hydra + "entrypoint";
    hydra.property = hydra + "property";
    hydra.supportedProperty = hydra + "supportedProperty";
    hydra.supportedOperation = hydra + "supportedOperation";
    hydra.supportedClass = hydra + "supportedClass";
    hydra.readable = hydra + "readable";
    hydra.writeable = hydra + "writeable";
    hydra.required = hydra + "required";
    hydra.expects = hydra + "expects";
    hydra.returns = hydra + "returns";
    hydra.method = hydra + "method";
    hydra.statusCode = hydra + "statusCode";
    hydra.operation = hydra + "operation";
    hydra.template = hydra + "template";
    hydra.mapping = hydra + "mapping";
    hydra.variable = hydra + "variable";
    hydra.title = hydra + "title";
    hydra.description = hydra + "description";
    window.ursa = new String("http://alien-mcl.github.io/URSA/vocabulary#"); //jshint ignore:line
    ursa.singleValue = ursa + "singleValue";
    ursa.mediaType = ursa + "mediaType";
    window.shacl = new String("http://www.w3.org/ns/shacl#"); //jshint ignore:line
    window.guid = new String("http://openguid.net/rdf#"); //jshint ignore:line
    guid.guid = guid + "guid";
    window.namespace = function(ns) {
        if ((ns === undefined) || (ns === null) || ((typeof(ns) !== "string") && (!(ns instanceof String)))) {
            return window;
        }

        ns = (ns instanceof String ? ns.toString() : ns);
        var parts = ns.split(".");
        var current = window;
        for (var index = 0; index < parts.length; index++) {
            if ((current[parts[index]] === undefined) || (current[parts[index]] === null)) {
                current = current[parts[index]] = {};
            }
            else {
                current = current[parts[index]];
            }

            current.__namespace = true;
        }

        return current;
    };
    var maxResolveDepth = 4;
    var is = function(type) {
        if ((this.prototype === undefined) || (this.prototype === null)) {
            return false;
        }

        if (this.prototype instanceof type) {
            return true;
        }

        return is.call(this.prototype, type);
    };

    var resolve = function(type, target, result, depth) {
        if (depth > maxResolveDepth) {
            return result;
        }

        for (var property in target) {
            if ((target.hasOwnProperty(property)) && (target[property] !== undefined) && (target[property] !== null)) {
                if ((typeof(target[property]) === "object") && (target[property].__namespace)) {
                    resolve(type, target[property], result, depth + 1);
                }
                else if ((typeof(target[property]) === "function") && (result.indexOf(target[property]) === -1) && (is.call(target[property], type))) {
                    result.push(target[property]);
                }
            }
        }

        return result;
    };
    window.container = {
        resolve: function(type) {
            if (typeof(type) !== "function") {
                throw new Error(invalidArgumentPassed.replace("{0}", "type"));
            }

            var result = [];
            return resolve(type, window, result, 0);
        }
    };
    String.format = function(format) {
        if ((format === undefined) || (format === null)) {
            return format;
        }

        var result = format;
        for (var index = 1; index < arguments.length; index++) {
            result = result.replace(new RegExp("\\{" + (index - 1) + "\\}", "g"), arguments[index]);
        }

        return result;
    };
}());