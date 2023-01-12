// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Windows;
using Caliburn.Metro.Demo.Controls;
using Caliburn.Micro;

namespace Caliburn.Metro.Demo
{
    public class AppBootstrapper : BootstrapperBase
    {
        private CompositionContainer? container;

        public AppBootstrapper()
        {
            this.Initialize();
        }

        protected override void BuildUp(object instance)
        {
            this.container?.SatisfyImportsOnce(instance);
        }

        /// <summary>
        ///     By default, we are configured to use MEF
        /// </summary>
        protected override void Configure()
        {
            var catalog =
                new AggregateCatalog(
                    AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>());

            this.container = new CompositionContainer(catalog);

            var batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(this.container);
            batch.AddExportedValue(catalog);

            this.container.Compose(batch);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return this.container?.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType)) ?? Enumerable.Empty<object>();
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var export = this.container?.GetExportedValues<object>(contract).FirstOrDefault();

            if (export is not null)
            {
                return export;
            }

            throw new Exception($"Could not locate any instances of contract {contract}.");
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            var startupTasks = this.GetAllInstances(typeof(StartupTask))
                                   .OfType<ExportedDelegate>()
                                   .Select(exportedDelegate => (StartupTask)exportedDelegate.CreateDelegate(typeof(StartupTask))!);

            startupTasks.Apply(s => s());

            await this.DisplayRootViewForAsync<IShell>();
        }
    }
}