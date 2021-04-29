// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using MahApps.Metro;

namespace Caliburn.Metro.Demo.Services
{
    [Export(typeof(IServiceLocator))]
    public class MefServiceLocator : IServiceLocator
    {
        private readonly CompositionContainer compositionContainer;

        [ImportingConstructor]
        public MefServiceLocator(CompositionContainer compositionContainer)
        {
            this.compositionContainer = compositionContainer;
        }

        public T GetInstance<T>()
            where T : class
        {
            try
            {
                return this.compositionContainer.GetExportedValue<T>();
            }
            catch (Exception exception)
            {
                throw new MahAppsException($"Could not locate any instances of contract {typeof(T)}.", exception);
            }
        }
    }
}