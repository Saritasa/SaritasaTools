﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Saritasa.Tools.Messages.Abstractions;
using JetBrains.Annotations;
using Saritasa.Tools.Messages.Configuration;

namespace Saritasa.Tools.Messages.Common
{
    /// <summary>
    /// Locates handler base middleware.
    /// </summary>
    public abstract class BaseHandlerLocatorMiddleware : IMessagePipelineMiddleware
    {
        /// <inheritdoc />
        public string Id { get; set; } = "Locator";

        /// <summary>
        /// Assemblies to search in.
        /// </summary>
        protected Assembly[] Assemblies { get; set; } = new Assembly[0];

        /// <summary>
        /// .ctor
        /// </summary>
        protected BaseHandlerLocatorMiddleware()
        {
        }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="dict">Input parameters as dict.</param>
        protected BaseHandlerLocatorMiddleware(IDictionary<string, string> dict)
        {
            if (dict == null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (dict.ContainsKey("id"))
            {
                Id = dict["id"];
            }
            if (dict.ContainsKey("assemblies"))
            {
                var assemblies = new List<Assembly>();
                foreach (string assemblyFile in dict["assemblies"].Split(';'))
                {
                    Assembly assembly;
                    try
                    {
                        assembly = LoadAssemblyByName(assemblyFile);
                    }
                    catch (Exception ex)
                    {
                        throw new MessagesConfigurationException($"Cannot load assembly {assemblyFile}.", ex);
                    }
                    if (assembly == null)
                    {
                        throw new MessagesConfigurationException($"Cannot load assembly {assemblyFile}.");
                    }
                    assemblies.Add(assembly);
                }
                this.Assemblies = assemblies.ToArray();
            }
            Initialize();
        }

        HandlerSearchMethod handlerSearchMethod = HandlerSearchMethod.ClassAttribute;

        /// <summary>
        /// Uses various combinations to find assembly.
        /// - Tries to load by name.
        /// - Tries to load from currently loaded assemblies (.NET classic only).
        /// </summary>
        /// <param name="name">Assembly name.</param>
        /// <returns>Assembly.</returns>
        internal static Assembly LoadAssemblyByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            if (!System.IO.Path.HasExtension(name))
            {
                name = System.IO.Path.ChangeExtension(name, ".dll");
            }

#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_2 || NETSTANDARD1_6
            var assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(name);
            if (assembly != null)
            {
                return assembly;
            }
#else
            var assembly = Assembly.LoadFrom(name);
            if (assembly != null)
            {
                return assembly;
            }

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .ToDictionary(k => System.IO.Path.GetFileName(k.CodeBase), a => a);
            if (loadedAssemblies.ContainsKey(name))
            {
                return loadedAssemblies[name];
            }
#endif
            return null;
        }

        /// <summary>
        /// What method to use to search command handler class.
        /// </summary>
        public HandlerSearchMethod HandlerSearchMethod
        {
            get
            {
                return handlerSearchMethod;
            }

            set
            {
                if (handlerSearchMethod != value)
                {
                    handlerSearchMethod = value;
                    Initialize();
                }
            }
        }

        /// <inheritdoc />
        public abstract void Handle([NotNull] IMessage message);

        /// <summary>
        /// Initialize middleware.
        /// </summary>
        protected abstract void Initialize();
    }
}