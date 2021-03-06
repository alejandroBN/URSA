﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using URSA.ComponentModel;
using URSA.Web.Converters;

namespace URSA.Configuration
{
    /// <summary>Describes the base URSA configuration section.</summary>
    public class UrsaConfigurationSection : ConfigurationSection
    {
        /// <summary>Defines a configuration section group name for URSA framework.</summary>
        public const string ConfigurationSectionGroupName = "ursa";

        internal const string ConfigurationSectionName = "core";
        internal const string ConfigurationSection = ConfigurationSectionGroupName + "/" + ConfigurationSectionName;
        private const string DefaultInstallerAssemblyNameMask = "URSA*";
        private const string InstallerAssemblyNameMaskAttribute = "installerAssemblyNameMask";
        private const string ServiceProviderTypeNameAttribute = "serviceProviderType";
        private const string ConverterProviderTypeNameAttribute = "converterProviderType";
        private const string ControllerActivatorTypeNameAttribute = "controllerActivatorType";

        /// <summary>Gets or sets the installer assembly name mask.</summary>
        [ConfigurationProperty(InstallerAssemblyNameMaskAttribute)]
        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Design", "CA0000:ExcludeFromCodeCoverage", Justification = "Wrapper without testable logic.")]
        public string InstallerAssemblyNameMask
        {
            get { return (string)this[InstallerAssemblyNameMaskAttribute]; }
            set { this[InstallerAssemblyNameMaskAttribute] = value; }
        }

        /// <summary>Gets or sets the service provider type.</summary>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Design", "CA0000:ExcludeFromCodeCoverage", Justification = "Wrapper without testable logic.")]
        public Type ServiceProviderType
        {
            get { return Type.GetType(ServiceProviderTypeName); }
            set { ServiceProviderTypeName = (value != null ? value.AssemblyQualifiedName : null); }
        }

        /// <summary>Gets or sets the converter provider type.</summary>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Design", "CA0000:ExcludeFromCodeCoverage", Justification = "Wrapper without testable logic.")]
        public Type ConverterProviderType
        {
            get { return Type.GetType(ConverterProviderTypeName); }
            set { ConverterProviderTypeName = (value != null ? value.AssemblyQualifiedName : null); }
        }

        /// <summary>Gets or sets the controller activator type.</summary>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Design", "CA0000:ExcludeFromCodeCoverage", Justification = "Wrapper without testable logic.")]
        public Type ControllerActivatorType
        {
            get { return Type.GetType(ControllerActivatorTypeName); }
            set { ControllerActivatorTypeName = (value != null ? value.AssemblyQualifiedName : null); }
        }

        /// <summary>Gets or sets the component provider.</summary>
        internal static IComponentProvider ComponentProvider { get; set; }

        [ConfigurationProperty(ServiceProviderTypeNameAttribute)]
        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Design", "CA0000:ExcludeFromCodeCoverage", Justification = "Wrapper without testable logic.")]
        private string ServiceProviderTypeName
        {
            get { return (string)this[ServiceProviderTypeNameAttribute]; }
            set { this[ServiceProviderTypeNameAttribute] = value; }
        }

        [ConfigurationProperty(ConverterProviderTypeNameAttribute)]
        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Design", "CA0000:ExcludeFromCodeCoverage", Justification = "Wrapper without testable logic.")]
        private string ConverterProviderTypeName
        {
            get { return (string)this[ConverterProviderTypeNameAttribute]; }
            set { this[ConverterProviderTypeNameAttribute] = value; }
        }

        [ConfigurationProperty(ControllerActivatorTypeNameAttribute)]
        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Design", "CA0000:ExcludeFromCodeCoverage", Justification = "Wrapper without testable logic.")]
        private string ControllerActivatorTypeName
        {
            get { return (string)this[ControllerActivatorTypeNameAttribute]; }
            set { this[ControllerActivatorTypeNameAttribute] = value; }
        }

        /// <summary>Initializes the component provider configured.</summary>
        /// <returns>Implementation of the <see cref="IComponentProvider" /> pointed in the configuration with installers loaded.</returns>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Design", "CA0000:ExcludeFromCodeCoverage", Justification = "Helper method used for initialization, which may proove to be difficult for testing.")]
        public static IComponentProvider InitializeComponentProvider()
        {
            if (ComponentProvider != null)
            {
                return ComponentProvider;
            }

            UrsaConfigurationSection configuration = (UrsaConfigurationSection)ConfigurationManager.GetSection(UrsaConfigurationSection.ConfigurationSection);
            if (configuration == null)
            {
                throw new InvalidOperationException(String.Format("Cannot instantiate a '{0}' without a proper configuration.", typeof(IComponentProvider)));
            }

            string installerAssemblyNameMask = configuration.InstallerAssemblyNameMask ?? DefaultInstallerAssemblyNameMask;
            var componentProviderCtor = ConfigurationSectionExtensions.GetProvider<IComponentProvider>(null, configuration.ServiceProviderType);
            var container = (IComponentProvider)componentProviderCtor.Invoke(null);
            var assemblies = GetInstallerAssemblies(installerAssemblyNameMask).Concat(new[] { Assembly.GetExecutingAssembly() });
            container.Install(assemblies);
            container.RegisterAll<IConverter>(assemblies);
            var converterProvider = (IConverterProvider)(configuration.GetProvider<IConverterProvider>(configuration.ConverterProviderType ?? typeof(DefaultConverterProvider))).Invoke(null);
            converterProvider.Initialize(container.ResolveAll<IConverter>());
            container.Register(converterProvider);
            ComponentProvider = container;
            return ComponentProvider;
        }

        /// <summary>Gets the default installer assemblies.</summary>
        /// <returns>Enumeration of assemblies used by the container initialization.</returns>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Design", "CA0000:ExcludeFromCodeCoverage", Justification = "Helper method used for initialization, which may proove to be difficult for testing.")]
        public static IEnumerable<Assembly> GetInstallerAssemblies()
        {
            UrsaConfigurationSection configuration =
                (UrsaConfigurationSection)ConfigurationManager.GetSection(UrsaConfigurationSection.ConfigurationSection) ??
                new UrsaConfigurationSection();
            return GetInstallerAssemblies(configuration.InstallerAssemblyNameMask ?? DefaultInstallerAssemblyNameMask);
        }

        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Design", "CA0000:ExcludeFromCodeCoverage", Justification = "Method uses local file system, which may proove to be diffucult for testing.")]
        private static IEnumerable<Assembly> GetInstallerAssemblies(string mask)
        {
            var assemblyNameRegex = new Regex(Regex.Escape(mask).Replace("\\*", ".*").Replace("\\?", "."));
            var appDomainAssemblies = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                      where assemblyNameRegex.IsMatch(assembly.FullName)
                                      select assembly;
            var appDomainAssemblyFiles = from assembly in appDomainAssemblies
                                         select Path.GetFileNameWithoutExtension(assembly.CodeBase);
            var fileAssemblies =
                from filePath in Directory.GetFiles(AppDomain.CurrentDomain.GetPrimaryAssemblyDirectory(), mask + ".dll")
                let fileName = Path.GetFileNameWithoutExtension(filePath)
                where !appDomainAssemblyFiles.Contains(fileName, StringComparer.OrdinalIgnoreCase)
                select Assembly.LoadFrom(filePath);
            return appDomainAssemblies.Concat(fileAssemblies).Distinct();
        }
    }
}