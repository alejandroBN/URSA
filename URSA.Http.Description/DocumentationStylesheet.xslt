﻿<?xml version="1.0" encoding="utf-8"?>
<!DOCTYPE xsl:stylesheet[
    <!ENTITY rdf 'http://www.w3.org/1999/02/22-rdf-syntax-ns#'>
    <!ENTITY rdfs 'http://www.w3.org/2000/01/rdf-schema#'>
    <!ENTITY owl 'http://www.w3.org/2002/07/owl#'>
    <!ENTITY xsd 'http://www.w3.org/2001/XMLSchema#'>
    <!ENTITY hydra 'http://www.w3.org/ns/hydra/core#'>
    <!ENTITY ursa 'http://alien-mcl.github.io/URSA/vocabulary#'>
    <!ENTITY odata 'http://docs.oasis-open.org/odata/odata/v4.0/'>
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
    xmlns:rdfs="http://www.w3.org/2000/01/rdf-schema#" xmlns:xsd="http://www.w3.org/2001/XMLSchema#" xmlns:hydra="http://www.w3.org/ns/hydra/core#" 
    xmlns:owl="http://www.w3.org/2002/07/owl#" xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" xmlns:ursa="http://alien-mcl.github.io/URSA/vocabulary#">
    <xsl:output method="html" indent="yes" encoding="utf-8" omit-xml-declaration="yes" media-type="text/html" />

    <xsl:template match="/rdf:RDF">
        <xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html></xsl:text>
        <html>
            <head>
                <title></title>
                <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" />
                <style type="text/css">
                    <![CDATA[
                    .list-group-item { word-wrap: break-word; }
                    .list-group-item.list-group-item-property { padding-left:30px; }
                    .list-group-item.list-group-item-property:before { position:absolute; left:12px; top:0px; width:16px; height:100%; content:' '; background:url(/property) center no-repeat; }
                    .list-group-item.list-group-item-method { padding-left:30px; }
                    .list-group-item.list-group-item-method:before { position:absolute; left:15px; top:0px; width:16px; height:100%; content:' '; background:url(/method) center no-repeat; }
                ]]></style>
                <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.2/css/bootstrap.min.css"></link>
                <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery/2.1.3/jquery.min.js"></script>
                <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.2/js/bootstrap.min.js"></script>
                <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/angular.js/1.3.14/angular.min.js"></script>
                <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/angular.js/1.3.14/angular-route.min.js"></script>
                <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/angular.js/1.3.14/angular-sanitize.min.js"></script>
                <script type="text/javascript"><xsl:apply-templates select="hydra:ApiDocumentation" /><![CDATA[
        var flavours = {
            "C#": {
                "http://www.w3.org/2001/XMLSchema#string": "string",
                "http://www.w3.org/2001/XMLSchema#string[]": "string[]",
                "http://www.w3.org/2001/XMLSchema#string<>": "System.Collections.Generic.IEnumerable<string>",
                "http://www.w3.org/2001/XMLSchema#boolean": "bool",
                "http://www.w3.org/2001/XMLSchema#boolean[]": "bool[]",
                "http://www.w3.org/2001/XMLSchema#boolean<>": "System.Collections.Generic.IEnumerable<bool>",
                "http://www.w3.org/2001/XMLSchema#byte": "sbyte",
                "http://www.w3.org/2001/XMLSchema#byte[]": "sbyte[]",
                "http://www.w3.org/2001/XMLSchema#byte<>": "System.Collections.Generic.IEnumerable<sbyte>",
                "http://www.w3.org/2001/XMLSchema#unsignedByte": "byte",
                "http://www.w3.org/2001/XMLSchema#unsignedByte[]": "byte[]",
                "http://www.w3.org/2001/XMLSchema#unsignedByte<>": "System.Collections.Generic.IEnumerable<byte>",
                "http://www.w3.org/2001/XMLSchema#short": "short",
                "http://www.w3.org/2001/XMLSchema#short[]": "short[]",
                "http://www.w3.org/2001/XMLSchema#short<>": "System.Collections.Generic.IEnumerable<short>",
                "http://www.w3.org/2001/XMLSchema#unsignedShort": "ushort",
                "http://www.w3.org/2001/XMLSchema#unsignedShort[]": "ushort[]",
                "http://www.w3.org/2001/XMLSchema#unsignedShort<>": "System.Collections.Generic.IEnumerable<ushort>",
                "http://www.w3.org/2001/XMLSchema#int": "int",
                "http://www.w3.org/2001/XMLSchema#int[]": "int[]",
                "http://www.w3.org/2001/XMLSchema#int<>": "System.Collections.Generic.IEnumerable<int>",
                "http://www.w3.org/2001/XMLSchema#unsignedInt": "uint",
                "http://www.w3.org/2001/XMLSchema#unsignedInt[]": "uint[]",
                "http://www.w3.org/2001/XMLSchema#unsignedInt<>": "System.Collections.Generic.IEnumerable<uint>",
                "http://www.w3.org/2001/XMLSchema#long": "long",
                "http://www.w3.org/2001/XMLSchema#long[]": "long[]",
                "http://www.w3.org/2001/XMLSchema#long<>": "System.Collections.Generic.IEnumerable<long>",
                "http://www.w3.org/2001/XMLSchema#unsignedLong": "ulong",
                "http://www.w3.org/2001/XMLSchema#unsignedLong[]": "ulong[]",
                "http://www.w3.org/2001/XMLSchema#unsignedLong<>": "System.Collections.Generic.IEnumerable<ulong>",
                "http://www.w3.org/2001/XMLSchema#float": "float",
                "http://www.w3.org/2001/XMLSchema#float[]": "float[]",
                "http://www.w3.org/2001/XMLSchema#float<>": "System.Collections.Generic.IEnumerable<float>",
                "http://www.w3.org/2001/XMLSchema#double": "double",
                "http://www.w3.org/2001/XMLSchema#double[]": "double[]",
                "http://www.w3.org/2001/XMLSchema#double<>": "System.Collections.Generic.IEnumerable<double>",
                "http://www.w3.org/2001/XMLSchema#decimal": "decimal",
                "http://www.w3.org/2001/XMLSchema#decimal[]": "decimal>",
                "http://www.w3.org/2001/XMLSchema#decimal<>": "System.Collections.Generic.IEnumerable<decimal>",
                "http://www.w3.org/2001/XMLSchema#dateTime": "System.DateTime",
                "http://www.w3.org/2001/XMLSchema#dateTime[]": "System.DateTime[]",
                "http://www.w3.org/2001/XMLSchema#dateTime<>": "System.Collections.Generic.IEnumerable<System.DateTime>",
                "http://www.w3.org/2001/XMLSchema#hexBinary": "byte[]",
                "http://openguid.net/rdf#guid": "System.Guid",
                "http://openguid.net/rdf#guid[]": "System.Guid[]",
                "http://openguid.net/rdf#guid<>": "System.Collections.Generic.IEnumerable<System.Guid>",
                "<>": function(type) { return type.replace(/(.+)<>$/, "System.Collections.Generic.IEnumerable<$1>"); },
                "default": function(type) { return type.replace(/^javascript:?/, ""); }
            },
            "Java": {
                "http://www.w3.org/2001/XMLSchema#string": "String",
                "http://www.w3.org/2001/XMLSchema#string[]": "String[]",
                "http://www.w3.org/2001/XMLSchema#string<>": "java.lang.Iterable<String>",
                "http://www.w3.org/2001/XMLSchema#boolean": "boolean",
                "http://www.w3.org/2001/XMLSchema#boolean[]": "boolean[]",
                "http://www.w3.org/2001/XMLSchema#boolean<>": "java.lang.Iterable<boolean>",
                "http://www.w3.org/2001/XMLSchema#byte": "byte",
                "http://www.w3.org/2001/XMLSchema#byte[]": "byte[]",
                "http://www.w3.org/2001/XMLSchema#byte<>": "java.lang.Iterable<byte>",
                "http://www.w3.org/2001/XMLSchema#unsignedByte": "short",
                "http://www.w3.org/2001/XMLSchema#unsignedByte[]": "short[]",
                "http://www.w3.org/2001/XMLSchema#unsignedByte<>": "java.lang.Iterable<short>",
                "http://www.w3.org/2001/XMLSchema#short": "short",
                "http://www.w3.org/2001/XMLSchema#short[]": "short[]",
                "http://www.w3.org/2001/XMLSchema#short<>": "java.lang.Iterable<short>",
                "http://www.w3.org/2001/XMLSchema#unsignedShort": "int",
                "http://www.w3.org/2001/XMLSchema#unsignedShort[]": "int[]",
                "http://www.w3.org/2001/XMLSchema#unsignedShort<>": "java.lang.Iterable<int>",
                "http://www.w3.org/2001/XMLSchema#int": "int",
                "http://www.w3.org/2001/XMLSchema#int[]": "int[]",
                "http://www.w3.org/2001/XMLSchema#int<>": "java.lang.Iterable<int>",
                "http://www.w3.org/2001/XMLSchema#unsignedInt": "long",
                "http://www.w3.org/2001/XMLSchema#unsignedInt[]": "long[]",
                "http://www.w3.org/2001/XMLSchema#unsignedInt<>": "java.lang.Iterable<long>",
                "http://www.w3.org/2001/XMLSchema#long": "long",
                "http://www.w3.org/2001/XMLSchema#long[]": "long[]",
                "http://www.w3.org/2001/XMLSchema#long<>": "java.lang.Iterable<long>",
                "http://www.w3.org/2001/XMLSchema#unsignedLong": "long",
                "http://www.w3.org/2001/XMLSchema#unsignedLong[]": "long[]",
                "http://www.w3.org/2001/XMLSchema#unsignedLong<>": "java.lang.Iterable<long>",
                "http://www.w3.org/2001/XMLSchema#float": "float",
                "http://www.w3.org/2001/XMLSchema#float[]": "float[]",
                "http://www.w3.org/2001/XMLSchema#float<>": "java.lang.Iterable<float>",
                "http://www.w3.org/2001/XMLSchema#double": "double",
                "http://www.w3.org/2001/XMLSchema#double[]": "double[]",
                "http://www.w3.org/2001/XMLSchema#double<>": "java.lang.Iterable<double>",
                "http://www.w3.org/2001/XMLSchema#decimal": "double",
                "http://www.w3.org/2001/XMLSchema#decimal[]": "double[]",
                "http://www.w3.org/2001/XMLSchema#decimal<>": "java.lang.Iterable<double>",
                "http://www.w3.org/2001/XMLSchema#dateTime": "java.util.Date",
                "http://www.w3.org/2001/XMLSchema#dateTime[]": "java.util.Date[]",
                "http://www.w3.org/2001/XMLSchema#dateTime<>": "java.lang.Iterable<java.util.Date>",
                "http://www.w3.org/2001/XMLSchema#hexBinary": "byte[]",
                "http://openguid.net/rdf#guid": "java.util.UUID",
                "http://openguid.net/rdf#guid[]": "java.util.UUID[]",
                "http://openguid.net/rdf#guid<>": "java.lang.Iterable<java.util.UUID>",
                "<>": function(type) { return type.replace(/(.+)<>$/, "java.lang.Iterable<$1>"); },
                "default": function(type) { return type.replace(/^javascript:?/, ""); }
            },
            "UML": {
                "http://www.w3.org/2001/XMLSchema#string": "String",
                "http://www.w3.org/2001/XMLSchema#string[]": "String[]",
                "http://www.w3.org/2001/XMLSchema#string<>": "<<collectionOf>> String",
                "http://www.w3.org/2001/XMLSchema#boolean": "Boolean",
                "http://www.w3.org/2001/XMLSchema#boolean[]": "Boolean[]",
                "http://www.w3.org/2001/XMLSchema#boolean<>": "<<collectionOf>> Boolean",
                "http://www.w3.org/2001/XMLSchema#byte": "Integer",
                "http://www.w3.org/2001/XMLSchema#byte[]": "Integer[]",
                "http://www.w3.org/2001/XMLSchema#byte<>": "<<collectionOf>> Integer",
                "http://www.w3.org/2001/XMLSchema#unsignedByte": "Integer",
                "http://www.w3.org/2001/XMLSchema#unsignedByte[]": "Integer[]",
                "http://www.w3.org/2001/XMLSchema#unsignedByte<>": "<<collectionOf>> Integer",
                "http://www.w3.org/2001/XMLSchema#short": "Integer",
                "http://www.w3.org/2001/XMLSchema#short[]": "Integer[]",
                "http://www.w3.org/2001/XMLSchema#short<>": "<<collectionOf>> Integer",
                "http://www.w3.org/2001/XMLSchema#unsignedShort": "Integer",
                "http://www.w3.org/2001/XMLSchema#unsignedShort[]": "Integer[]",
                "http://www.w3.org/2001/XMLSchema#unsignedShort<>": "<<collectionOf>> Integer",
                "http://www.w3.org/2001/XMLSchema#int": "Integer",
                "http://www.w3.org/2001/XMLSchema#int[]": "Integer[]",
                "http://www.w3.org/2001/XMLSchema#int<>": "<<collectionOf>> Integer",
                "http://www.w3.org/2001/XMLSchema#unsignedInt": "Integer",
                "http://www.w3.org/2001/XMLSchema#unsignedInt[]": "Integer[]",
                "http://www.w3.org/2001/XMLSchema#unsignedInt<>": "<<collectionOf>> Integer",
                "http://www.w3.org/2001/XMLSchema#long": "Integer",
                "http://www.w3.org/2001/XMLSchema#long[]": "Integer[]",
                "http://www.w3.org/2001/XMLSchema#long<>": "<<collectionOf>> Integer",
                "http://www.w3.org/2001/XMLSchema#unsignedLong": "Integer",
                "http://www.w3.org/2001/XMLSchema#unsignedLong[]": "Integer[]",
                "http://www.w3.org/2001/XMLSchema#unsignedLong<>": "<<collectionOf>> Integer",
                "http://www.w3.org/2001/XMLSchema#float": "Real",
                "http://www.w3.org/2001/XMLSchema#float[]": "Real[]",
                "http://www.w3.org/2001/XMLSchema#float<>": "<<collectionOf>> Real",
                "http://www.w3.org/2001/XMLSchema#double": "UnlimitedNatural",
                "http://www.w3.org/2001/XMLSchema#double[]": "UnlimitedNatural[]",
                "http://www.w3.org/2001/XMLSchema#double<>": "<<collectionOf>> UnlimitedNatural",
                "http://www.w3.org/2001/XMLSchema#decimal": "UnlimitedNatural",
                "http://www.w3.org/2001/XMLSchema#decimal[]": "UnlimitedNatural[]",
                "http://www.w3.org/2001/XMLSchema#decimal<>": "<<collectionOf>> UnlimitedNatural",
                "http://www.w3.org/2001/XMLSchema#dateTime": "DateTime",
                "http://www.w3.org/2001/XMLSchema#dateTime[]": "DateTime[]",
                "http://www.w3.org/2001/XMLSchema#dateTime<>": "<<collectionOf>> DateTime",
                "http://www.w3.org/2001/XMLSchema#hexBinary": "Binary",
                "http://openguid.net/rdf#guid": "GUID",
                "http://openguid.net/rdf#guid[]": "GUID[]",
                "http://openguid.net/rdf#guid<>": "<<collectionOf>> GUID",
                "<>": function(type) { return type.replace(/(.+)<>$/, "<<collectionOf>> $1"); },
                "default": function(type) { return type.replace(/^javascript:?/, ""); }
            }
        };
        
        var mapType = function(type, flavour, supportedClasses, currentClass) {
            var flavour = flavours[flavour];
            result = flavour[type];
            if (result !== undefined) {
                return result;
            }
            
            result = flavour["default"](type);
            for (var index = 0; index < supportedClasses.length; index++) {
                var supportedClass = supportedClasses[index];
                if (result.indexOf(supportedClass["@id"]) === 0) {
                    result = supportedClass.label + result.substr(supportedClass["@id"].length);
                    break;
                }
            }

            if (result.indexOf("<>") === result.length - 2) {
                result = flavour["<>"](result);
            }
            
            if (currentClass) {
                var currentNamespace = (currentClass = flavour["default"](currentClass)).match(/[.#/]([a-zA-Z_$]+[a-zA-Z_$0-9]*)$/);
                if (currentNamespace !== null) {
                    result = result.replace(currentClass.replace(currentNamespace[1], ""), "");
                }
            }
            
            return result;
        };
        
        var createMethod = function(supportedClass, supportedOperation, currentFlavour, format, supportedClasses) {
            format = !!format;
            var returns = "void";
            var parameters = "";
            if (supportedOperation.returns.length > 0) {
                var returnedTypes = [];
                var hasTotalEntities = false;
                for (var index = 0; index < supportedOperation.mappings.length; index++) {
                    var mapping = supportedOperation.mappings[index];
                    if ((mapping.property["@id"] == window.odata + "$top") || (mapping.property["@id"] == window.odata + "$skip")) {
                        hasTotalEntities = true;
                        switch (currentFlavour) {
                            case "UML": parameters += (format ? "&lt;&lt;out&gt;&gt;" : "<<out>>") + " totalEntities: Integer, "; break;
                            case "Java": parameters += "int[] totalEntities, "; break;
                            case "C#": parameters += "out int totalEntities, "; break;
                        }
                        
                        break;
                    }
                }
                
                for (var index = (hasTotalEntities ? 1 : 0); index < supportedOperation.returns.length; index++) {
                    returnedTypes.push(supportedOperation.returns[index]);
                }
                
                returns = "";
                if (returnedTypes.length > 1) {
                    returns = supportedOperation.label + "Result";
                }
                else {
                    returns = mapType(returnedTypes[0]["@id"], currentFlavour, supportedClasses, supportedClass["@id"]);
                }
            }
                
            if ((supportedOperation.template) && (supportedOperation.mappings)) {
                for (var index = 0; index < supportedOperation.mappings.length; index++) {
                    var mapping = supportedOperation.mappings[index];
                    var parameterType = "object";
                    if (mapping.property) {
                        parameterType = mapType(mapping.property.type, currentFlavour, supportedClasses, supportedClass["@id"]);
                    }
                        
                    if (format) {
                        parameterType = parameterType.replace(/</g, "&lt;").replace(/>/g, "&gt;");
                    }

                    parameters += (currentFlavour === "UML" ? mapping.variable.replace("$", "") + ": " + parameterType : parameterType + " " + mapping.variable.replace("$", "")) + ", ";
                }
                    
                parameters = parameters.substr(0, parameters.length - 2);
            }
                
            var arguments = "";
            for (var index = 0; index < supportedOperation.expects.length; index++) {
                var expected = supportedOperation.expects[index];
                var expectedType = mapType(expected.type, currentFlavour, supportedClasses, supportedClass["@id"]);
                if (format) {
                    expectedType = expectedType.replace(/</g, "&lt;").replace(/>/g, "&gt;");
                }
                
                arguments += (currentFlavour === "UML" ? expected.variable + ": " + expectedType : expectedType + " " + expected.variable) + ", ";
            }
                
            if (arguments.length > 0) {
                arguments = arguments.substr(0, arguments.length - 2);
            }

            var methodName = (currentFlavour === "Java" ? supportedOperation.label.charAt(0).toLowerCase() + supportedOperation.label.substr(1) : supportedOperation.label);
            var result = (currentFlavour === "UML" ? (format ? "<b>" : "") + methodName + (format ? "</b>" : "") + 
                "(" + parameters + ((parameters.length > 0) && (arguments.length > 0) ? ", " : "") + arguments + ")" + 
                (returns === "void" ? "" : ": " + (format ? "<i>" + returns.replace(/</g, "&lt;").replace(/>/g, "&gt;") + "</i>" : returns)) :
                (format ? "<i>" + returns.replace(/</g, "&lt;").replace(/>/g, "&gt;") + "</i>" : returns) + " " + 
                (format ? "<b>" : "") + methodName + (format ? "</b>" : "") + 
                "(" + parameters + ((parameters.length > 0) && (arguments.length > 0) ? ", " : "") + arguments + ")");
            return result;
        };
            
        var createProperty = function(supportedClass, supportedProperty, currentFlavour, format, supportedClasses) {
            format = !!format;
            var type = mapType(supportedProperty.type, currentFlavour, supportedClasses, supportedClass["@id"]);
            var result = (format ? "<i>" + type.replace(/</g, "&lt;").replace(/>/g, "&gt;") + "</i>" : type) + " " + (format ? "<b>" : "") + 
                (currentFlavour === "Java" ? supportedProperty.label.charAt(0).toLowerCase() + supportedProperty.label.substr(1) : supportedProperty.label) +
                (format ? "</b>" : "");
            switch (currentFlavour) {
                case "C#": result += " { " + (supportedProperty.readable ? "get; " : "") + (supportedProperty.writeable ? "set; " : "") + "}"; break;
                case "Java": result = (supportedProperty.readable ? "@Getter " : "") + (supportedProperty.writeable ? "@Setter " : "") + " " + result; break;
                case "UML": result = ((supportedProperty.readable) && (supportedProperty.writeable) ? "<<property>>" : (supportedProperty.readable ? "<<readonly>>" : "<<writeonly>>")) + " " + result; break; 
            }
            
            return result;
        };
        
        var createMember = function(supportedClass, supportedMember, flavour, format, supportedClasses) {
            if ((supportedClass == null) || (supportedMember == null)) {
                return "";
            }
            
            return (supportedMember["@type"] === hydra.supportedProperty ?
                createProperty(supportedClass, supportedMember, flavour, format, supportedClasses) :
                createMethod(supportedClass, supportedMember, flavour, format, supportedClasses));
        };
        
        var defaultFlavour = "C#";
        var apiDocumentation = angular.module("ApiDocumentation", ["ngRoute", "ngSanitize"]).
        constant("supportedClasses", window.supportedClasses).
        controller("MainMenu", ["$scope", function($scope) {
            $scope.currentFlavour = defaultFlavour;
            $scope.changeFlavour = function(newFlavour) {
                $scope.$emit("FlavourChanged", $scope.currentFlavour = newFlavour);
                event.preventDefault();
                event.stopPropagation();
            };
        }]).
        controller("SupportedClasses", ["$scope", "supportedClasses", function($scope, supportedClasses) {
            $scope.supportedClasses = supportedClasses;
            $scope.createMember = function(supportedClass, supportedMember, format) {
                return createMember(supportedClass, supportedMember, $scope.currentFlavour, format, supportedClasses);
            };
            $scope.currentFlavour = defaultFlavour;
            $scope.showMember = function(event, supportedClass, supportedMember) {
                $scope.$emit("MemberSelected", supportedClass, supportedMember);
                event.preventDefault();
                event.stopPropagation();
            };
            $scope.$root.$on("FlavourChanged", function(e, newFlavour) {
                $scope.currentFlavour = newFlavour;
            });
        }]).
        controller("MemberDescription", ["$scope", "supportedClasses", function($scope, supportedClasses) {
            $scope.currentClass = null;
            $scope.currentMember = null;
            $scope.currentFlavour = defaultFlavour;
            $scope.createMember = function(supportedClass, supportedMember, format) {
                return createMember(supportedClass, supportedMember, $scope.currentFlavour, format, supportedClasses);
            };
            $scope.hasTotalEntities = false;
            $scope.xsd = window.xsd;
            $scope.mapType = function(supportedClass, type) {
                if ((supportedClass === undefined) || (supportedClass == null) || (type === undefined) || (type == null)) {
                    return "";
                }
                
                return mapType(type, $scope.currentFlavour, supportedClasses, $scope.currentClass["@id"]);
            };
            $scope.isMethod = function(member) {
                return (member) && (member['@type'] === hydra.supportedOperation);
            };
            $scope.returns = function(member) {
                return (member) && (((member.returns) && (member.returns.length > 0)) || (member.type));
            };
            $scope.variableName = function(variableName) {
                return variableName.replace("$", "");
            };
            $scope.$root.$on("MemberSelected", function(e, currentClass, currentMember) {
                $scope.currentClass = currentClass;
                $scope.currentMember = currentMember;
                $scope.hasTotalEntities = false;
                $("html,body").animate({ scrollTop: 0 }, "fast");
                if (currentMember.mappings) {
                    for (var index = 0; index < currentMember.mappings.length; index++) {
                        var mapping = currentMember.mappings[index];
                        if ((mapping.property["@id"] == window.odata + "$top") || (mapping.property["@id"] == window.odata + "$skip")) {
                            $scope.hasTotalEntities = true;
                            break;
                        }
                    }
                }
            });
            $scope.$root.$on("FlavourChanged", function(e, newFlavour) {
                $scope.currentFlavour = newFlavour;
            });
        }]);
                ]]></script>
            </head>
            <body ng-app="ApiDocumentation">
                <nav class="navbar navbar-default" ng-controller="MainMenu">
                    <div class="container-fluid">
                        <div class="navbar-header">
                            <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#main-menu" aria-expanded="false">
                                <span class="sr-only">Toggle navigation</span>
                                <span class="icon-bar"></span>
                                <span class="icon-bar"></span>
                                <span class="icon-bar"></span>
                            </button>
                            <span class="navbar-brand">API Documentation</span>
                        </div>
                        <div class="collapse navbar-collapse" id="main-menu">
                            <ul class="nav navbar-nav">
                                <li ng-class="currentFlavour === 'C#' ? 'active' : ''"><a href="#" ng-click="changeFlavour('C#')">C#</a></li>
                                <li ng-class="currentFlavour === 'Java' ? 'active' : ''"><a href="#" ng-click="changeFlavour('Java')">Java</a></li>
                                <li ng-class="currentFlavour === 'UML' ? 'active' : ''"><a href="#" ng-click="changeFlavour('UML')">UML</a></li>
                            </ul>
                        </div>
                    </div>
                </nav>
                <div class="container-fluid">
                    <div class="col-sm-4 col-xs-12" ng-controller="SupportedClasses">
                        <nav class="panel-group" id="SupportedClasses">
                            <div class="panel panel-default" ng-repeat="supportedClass in supportedClasses">
                                <div class="panel-heading">
                                    <h4 class="panel-title">
                                        <a data-toggle="collapse" data-parent="#SupportedClasses" ng-attr-href="#collapse{{{{ $index }}}}">{{ supportedClass.label }}</a>
                                    </h4>
                                </div>
                                <div id="collapse{{{{ $index }}}}" class="panel-collapse collapse">
                                    <div class="panel-body">
                                        <ul class="list-group">
                                            <a href="#" class="list-group-item list-group-item-property" ng-repeat="supportedProperty in supportedClass.supportedProperties" ng-click="showMember($event, supportedClass, supportedProperty)">
                                                {{ createMember(supportedClass, supportedProperty) }}
                                            </a>
                                            <a href="#" class="list-group-item list-group-item-method" ng-repeat="supportedOperation in supportedClass.supportedOperations" ng-click="showMember($event, supportedClass, supportedOperation)">
                                                {{ createMember(supportedClass, supportedOperation) }}
                                            </a>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </nav>
                    </div>
                    <div class="col-sm-8 col-xs-12" ng-controller="MemberDescription" ng-show="currentMember">
                        <div class="panel panel-default">
                            <div class="panel-heading" ng-bind-html="createMember(currentClass, currentMember, true)"></div>
                            <div class="panel-body">
                                <p ng-show="currentMember.description">{{ currentMember.description }}</p>
                                <table class="table" ng-show="isMethod(currentMember)">
                                    <tr>
                                        <th>Name</th>
                                        <th>Type</th>
                                        <th>Description</th>
                                    </tr>
                                    <tr ng-show="hasTotalEntities">
                                        <td>totalEntities</td>
                                        <xsl:element name="td">
                                            <xsl:attribute name="title"><xsl:value-of select="'&xsd;int'" /></xsl:attribute>
                                            <xsl:text>{{ mapType(currentClass, xsd.int) }}</xsl:text>
                                        </xsl:element>
                                        <td>Number of total entities in the collection.</td>
                                    </tr>
                                    <tr ng-repeat="mapping in currentMember.mappings">
                                        <td>{{ variableName(mapping.variable) }}</td>
                                        <td ng-attr-title="mapping.property ? mapping.property.type : ''">{{ mapping.property ? mapType(currentClass, mapping.property.type) : "object" }}</td>
                                        <td>{{ mapping.description }}</td>
                                    </tr>
                                    <tr ng-repeat="expected in currentMember.expects">
                                        <td>{{ variableName(expected.variable) }}</td>
                                        <td ng-attr-title="expected.type">{{ mapType(currentClass, expected.type) }}</td>
                                        <td>{{ expected.description }}</td>
                                    </tr>
                                </table>
                                <p ng-show="returns(currentMember)">
                                    {{ currentMember.type ? "Type:" : "Returns:" }}
                                    <span ng-show="returns(currentMember)">{{ mapType(currentClass, currentMember.type || currentMember.returns[0]["@id"]) }}</span>
                                    <span ng-show="returns(currentMember) &amp;&amp; currentMember.returns[0].description"><br />{{ currentMember.returns[0].description }}</span>
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </body>
        </html>
    </xsl:template>

    <xsl:template match="hydra:ApiDocumentation">
        window.xsd = new String("<xsl:value-of select="'&xsd;'"/>");
        xsd.int = xsd + "int";
        window.hydra = new String("<xsl:value-of select="'hydra'"/>");
        hydra.supportedProperty = hydra + "supportedProperty";
        window.ursa = new String("<xsl:value-of select="'&ursa;'"/>");
        window.odata = new String("<xsl:value-of select="'&odata;'"/>");
        var supportedClasses = [];<xsl:for-each select="hydra:supportedClass"><xsl:variable name="id" select="@rdf:resource" /><xsl:if test="$id != '&hydra;ApiDocumentation'">
        supportedClasses.push(<xsl:call-template name="hydra-Class">
                <xsl:with-param name="class" select="/rdf:RDF/hydra:Class[@rdf:about = $id]|/rdf:RDF/*[rdf:type/@rdf:resource = '&hydra;Class' and @rdf:about = $id]" />
            </xsl:call-template>);
        supportedClasses[supportedClasses.length - 1].supportedOperations.pop();</xsl:if></xsl:for-each>
    </xsl:template>

    <xsl:template name="hydra-Class">
        <xsl:param name="class" />
            {
                "@id": "<xsl:value-of select="$class/@rdf:about" />",
                "label": "<xsl:value-of select="$class/rdfs:label" />",
                "supportedProperties": [<xsl:for-each select="$class/hydra:supportedProperty">
                        <xsl:variable name="id" select="@rdf:resource" />
                        <xsl:variable name="supportedProperty" select="/rdf:RDF/hydra:SupportedProperty[@rdf:about = $id]|/rdf:RDF/*[rdf:type/@rdf:resource = '&hydra;SupportedProperty' and @rdf:about = $id]" />
                        <xsl:variable name="property" select="/rdf:RDF/rdf:Property[@rdf:about = $supportedProperty/hydra:property/@rdf:resource]|/rdf:RDF/*[rdf:type/@rdf:resource = '&rdf;Property' and @rdf:about = $supportedProperty/hydra:property/@rdf:resource]" />
                        <xsl:call-template name="hydra-SupportedProperty">
                            <xsl:with-param name="supportedProperty" select="$supportedProperty" />
                        </xsl:call-template><xsl:if test="position() != last()">,</xsl:if>
                    </xsl:for-each>
                ],
                    "supportedOperations": [<xsl:for-each select="/rdf:RDF/hydra:Operation[@rdf:about = $class/*/@rdf:resource]|/rdf:RDF/*[rdf:type/@rdf:resource = '&hydra;Operation' and @rdf:about = $class/*/@rdf:resource]">
                        <xsl:variable name="id" select="@rdf:resource|@rdf:about" />
                        <xsl:variable name="localName" select="local-name($class/*[@rdf:resource = $id])" />
                        <xsl:variable name="namespace" select="namespace-uri($class/*[@rdf:resource = $id])" />
                        <xsl:variable name="name">
                            <xsl:choose>
                                <xsl:when test="$localName != 'supportedProperty' and $namespace != '&hydra;'">
                                    <xsl:value-of select="concat($namespace, $localName)" />
                                </xsl:when>
                            </xsl:choose>
                        </xsl:variable>
                        <xsl:call-template name="hydra-SupportedOperation">
                            <xsl:with-param name="supportedOperation" select="/rdf:RDF/hydra:Operation[@rdf:about = $id]|/rdf:RDF/*[rdf:type/@rdf:resource = '&hydra;Operation' and @rdf:about = $id]" />
                            <xsl:with-param name="template" select="/rdf:RDF/hydra:IriTemplate[@rdf:about = $name]|/rdf:RDF/*[rdf:type/@rdf:resource = '&hydra;IriTemplate' and @rdf:about = $name]" />
                        </xsl:call-template>,</xsl:for-each
                    ><xsl:for-each select="$class/*[@rdf:resource]">
                        <xsl:variable name="templatedLink" select="concat(namespace-uri(current()), local-name(current()))" />
                        <xsl:variable name="iriTemplate" select="@rdf:resource" />
                        <xsl:for-each select="/rdf:RDF/hydra:TemplatedLink[@rdf:about = $templatedLink]/hydra:supportedOperation">
                            <xsl:variable name="operation" select="@rdf:resource" />
                            <xsl:for-each select="/rdf:RDF/hydra:Operation[@rdf:about = $operation]|/rdf:RDF/*[rdf:type/@rdf:resource = '&hydra;Operation' and @rdf:about = $operation]">
                                <xsl:variable name="id" select="@rdf:resource|@rdf:about" />
                                <xsl:call-template name="hydra-SupportedOperation">
                                    <xsl:with-param name="supportedOperation" select="/rdf:RDF/hydra:Operation[@rdf:about = $id]|/rdf:RDF/*[rdf:type/@rdf:resource = '&hydra;Operation' and @rdf:about = $id]" />
                                    <xsl:with-param name="template" select="/rdf:RDF/hydra:IriTemplate[@rdf:about = $iriTemplate]|/rdf:RDF/*[rdf:type/@rdf:resource = '&hydra;IriTemplate' and @rdf:about = $iriTemplate]" />
                                </xsl:call-template>,</xsl:for-each
                        ></xsl:for-each>
                    </xsl:for-each>
                    <xsl:for-each select="$class/hydra:supportedProperty[@rdf:resource]">
                        <xsl:variable name="supportedProperty" select="/rdf:RDF/hydra:SupportedProperty[@rdf:about = current()/@rdf:resource]" />
                        <xsl:for-each select="$supportedProperty/*[@rdf:resource]">
                            <xsl:variable name="templatedLink" select="concat(namespace-uri(current()), local-name(current()))" />
                            <xsl:variable name="iriTemplate" select="@rdf:resource" />
                            <xsl:for-each select="/rdf:RDF/hydra:TemplatedLink[@rdf:about = $templatedLink]/hydra:supportedOperation">
                                <xsl:variable name="operation" select="@rdf:resource" />
                                <xsl:for-each select="/rdf:RDF/hydra:Operation[@rdf:about = $operation]|/rdf:RDF/*[rdf:type/@rdf:resource = '&hydra;Operation' and @rdf:about = $operation]">
                                    <xsl:variable name="id" select="@rdf:resource|@rdf:about" />
                                    <xsl:call-template name="hydra-SupportedOperation">
                                        <xsl:with-param name="supportedOperation" select="/rdf:RDF/hydra:Operation[@rdf:about = $id]|/rdf:RDF/*[rdf:type/@rdf:resource = '&hydra;Operation' and @rdf:about = $id]" />
                                        <xsl:with-param name="template" select="/rdf:RDF/hydra:IriTemplate[@rdf:about = $iriTemplate]|/rdf:RDF/*[rdf:type/@rdf:resource = '&hydra;IriTemplate' and @rdf:about = $iriTemplate]" />
                                    </xsl:call-template>,</xsl:for-each
                            ></xsl:for-each>
                        </xsl:for-each>
                    </xsl:for-each>
                    {
                    }
                ]
            }</xsl:template>

    <xsl:template name="hydra-SupportedProperty">
        <xsl:param name="supportedProperty" />
        <xsl:variable name="property" select="/rdf:RDF/rdf:Property[@rdf:about = $supportedProperty/hydra:property/@rdf:resource]|/rdf:RDF/*[rdf:type/@rdf:resource = '&rdf;Property' and @rdf:about = $supportedProperty/hydra:property/@rdf:resource]" />
                    {
                        "@id": "<xsl:value-of select="$supportedProperty/@rdf:about" />",
                        "@type": hydra.supportedProperty,
                        "label": "<xsl:value-of select="$property/rdfs:label" />",<xsl:if test="$property/rdfs:comment">
                        "description": "<xsl:value-of select="$property/rdfs:comment/text()"/>",</xsl:if>
                        "type": "<xsl:call-template name="type-description"><xsl:with-param name="type" select="$property/rdfs:range/@rdf:nodeID | $property/rdfs:range/@rdf:resource" /></xsl:call-template>",
                        "property": "<xsl:value-of select="$property/@rdf:about" />",
                        "readable": <xsl:choose><xsl:when test="$supportedProperty/hydra:readable"><xsl:value-of select="$supportedProperty/hydra:readable"/></xsl:when><xsl:otherwise>false</xsl:otherwise></xsl:choose>,
                        "writeable": <xsl:choose><xsl:when test="$supportedProperty/hydra:writeable"><xsl:value-of select="$supportedProperty/hydra:writeable"/></xsl:when><xsl:otherwise>false</xsl:otherwise></xsl:choose>
                    }</xsl:template>
    
    <xsl:template name="hydra-SupportedOperation">
        <xsl:param name="supportedOperation" />
        <xsl:param name="template" />
        <xsl:variable name="isEnumerable">
            <xsl:choose>
                <xsl:when test="/rdf:RDF/rdf:Property[@rdf:about = /rdf:RDF/hydra:IriTemplateMapping[@rdf:about = $template/hydra:mapping/@rdf:resource]/hydra:property/@rdf:resource and rdf:type/@rdf:resource = '&owl;InverseFunctionalProperty']">false</xsl:when>
                <xsl:otherwise>true</xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <xsl:variable name="hasTotalEntities" select="/rdf:RDF/*[@rdf:about = $template/hydra:mapping/@rdf:resource]/hydra:property/@rdf:resource = '&odata;$top' or /rdf:RDF/*[@rdf:about = $template/hydra:mapping/@rdf:resource]/hydra:property/@rdf:resource = '&odata;$skip'" />
                    {
                        "@id": "<xsl:value-of select="$supportedOperation/@rdf:about" />",
                        "@type": hydra.supportedOperation,
                        "label": "<xsl:value-of select="$supportedOperation/rdfs:label" />",<xsl:if test="$template">
                        "template": "<xsl:value-of select="$template/hydra:template" />",</xsl:if><xsl:if test="$supportedOperation/rdfs:comment">
                        "description": "<xsl:value-of select="$supportedOperation/rdfs:comment/text()"/>",</xsl:if>
                        "returns": [<xsl:if test="$hasTotalEntities">
                            {
                                "@id": "<xsl:value-of select="'&xsd;int'" />",
                                "description": "Total items in collection"
                            }<xsl:if test="$supportedOperation/hydra:returns">, </xsl:if></xsl:if
                            ><xsl:for-each select="$supportedOperation/hydra:returns"
                            ><xsl:variable name="current" select="@rdf:nodeID"
                            /><xsl:variable name="resource" select="/rdf:RDF/*[@rdf:nodeID = $current]" />
                            {
                                "@id": "<xsl:call-template name="type-description"><xsl:with-param name="type" select="$resource/rdfs:subClassOf/@rdf:resource | $resource/rdfs:subClassOf/@rdf:nodeID" /></xsl:call-template>",
                                "description": "<xsl:value-of select="$resource/rdfs:comment" />"
                            }<xsl:if test="position() != last()">, </xsl:if></xsl:for-each>
                        ],
                        "expects": [<xsl:for-each select="$supportedOperation/hydra:expects">
                            {   <xsl:variable name="current" select="@rdf:nodeID" />
                                <xsl:variable name="resource" select="/rdf:RDF/*[@rdf:nodeID = $current]" />
                                "variable": "<xsl:value-of select="$resource/rdfs:label" />",
                                "type": "<xsl:call-template name="type-description"><xsl:with-param name="type" select="$resource/rdfs:subClassOf/@rdf:resource | $resource/rdfs:subClassOf/@rdf:nodeID" /></xsl:call-template>",
                                "description": "<xsl:value-of select="$resource/rdfs:comment" />"
                            }<xsl:if test="position() != last()">, </xsl:if></xsl:for-each>
                        ],
                        "mappings": [<xsl:for-each select="/rdf:RDF/*[@rdf:about = $template/hydra:mapping/@rdf:resource]">
                            {
                                "required": <xsl:choose><xsl:when test="hydra:required"><xsl:value-of select="hydra:required"/></xsl:when><xsl:otherwise>false</xsl:otherwise></xsl:choose>,
                                "variable": "<xsl:value-of select="hydra:variable" />",
                                "description": "<xsl:value-of select="rdfs:comment" />"<xsl:if test="hydra:property">,
                                "property": { <xsl:variable name="current" select="hydra:property/@rdf:resource" /><xsl:variable name="targetProperty" select="/rdf:RDF/rdf:Property[@rdf:about = $current]|/rdf:RDF/*[rdf:type[@rdf:resource = '&rdf;Property'] and @rdf:about = $current]" />
                                    "@id": "<xsl:value-of select="hydra:property/@rdf:resource" />",
                                    "type": "<xsl:value-of select="/rdf:RDF/*[@rdf:nodeID = $targetProperty/rdfs:range/@rdf:nodeID]/rdfs:subClassOf/@rdf:resource | $targetProperty/rdfs:range/@rdf:resource" />",
                                }</xsl:if>
                            }<xsl:if test="position() != last()">,</xsl:if></xsl:for-each>
                        ],
                        "method": [<xsl:for-each select="$supportedOperation/hydra:method">
                            "<xsl:value-of select="." />"<xsl:if test="position() != last()">,</xsl:if></xsl:for-each>
                        ]
                    }</xsl:template>

    <xsl:template name="type">
        <xsl:param name="type" />
        <xsl:param name="isEnumerable" />
        <xsl:variable name="typeDescription" select="/rdf:RDF/*[@rdf:about = $type]" />
        <xsl:choose>
            <xsl:when test="$typeDescription/rdfs:subClassOf[@rdf:resource = '&rdf;List']">
                <xsl:variable name="itemType">
                    <xsl:choose>
                        <xsl:when test="/rdf:RDF/owl:Restriction[@rdf:nodeID = $typeDescription/rdfs:subClassOf/@rdf:nodeID]/owl:onProperty[@rdf:resource = '&rdf;first']">
                            <xsl:value-of select="/rdf:RDF/owl:Restriction[@rdf:nodeID = $typeDescription/rdfs:subClassOf/@rdf:nodeID]/owl:allValuesFrom/@rdf:resource" />
                        </xsl:when>
                        <xsl:otherwise><xsl:value-of select="'&owl;Thing'" /></xsl:otherwise>
                    </xsl:choose>
                </xsl:variable><xsl:value-of select="$itemType" />[]</xsl:when>
            <xsl:otherwise><xsl:value-of select="$type" /><xsl:if test="$isEnumerable = 'true'">&lt;&gt;</xsl:if></xsl:otherwise>
        </xsl:choose>
    </xsl:template>

    <xsl:template name="type-description">
        <xsl:param name="type" />
        <xsl:variable name="typeDescription" select="/rdf:RDF/*[@rdf:about = $type | @rdf:nodeID = $type]" />
        <xsl:choose>
            <xsl:when test="$typeDescription/rdfs:subClassOf[@rdf:resource = '&rdf;List']">
                <xsl:choose>
                    <xsl:when test="/rdf:RDF/*[@rdf:about = $typeDescription/rdfs:subClassOf/@rdf:resource or @rdf:nodeID = $typeDescription/rdfs:subClassOf/@rdf:nodeID]/owl:allValuesFrom[@rdf:resource]">
                        <xsl:value-of select="concat(/rdf:RDF/*[@rdf:about = $typeDescription/rdfs:subClassOf/@rdf:resource or @rdf:nodeID = $typeDescription/rdfs:subClassOf/@rdf:nodeID]/owl:allValuesFrom/@rdf:resource, '[]')" />
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select="concat($typeDescription/@rdf:about, '[]')" />
                    </xsl:otherwise>
                </xsl:choose>
            </xsl:when>
            <xsl:when test="$typeDescription/rdfs:subClassOf[@rdf:resource = '&hydra;Collection']">
                <xsl:choose>
                    <xsl:when test="/rdf:RDF/*[@rdf:about = $typeDescription/rdfs:subClassOf/@rdf:resource or @rdf:nodeID = $typeDescription/rdfs:subClassOf/@rdf:nodeID]/owl:allValuesFrom[@rdf:resource]">
                        <xsl:value-of select="concat(/rdf:RDF/*[@rdf:about = $typeDescription/rdfs:subClassOf/@rdf:resource or @rdf:nodeID = $typeDescription/rdfs:subClassOf/@rdf:nodeID]/owl:allValuesFrom/@rdf:resource, '&lt;&gt;')" />
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select="concat($typeDescription/@rdf:about, '&lt;&gt;')" />
                    </xsl:otherwise>
                </xsl:choose>
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="$typeDescription/@rdf:about" />
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
</xsl:stylesheet>