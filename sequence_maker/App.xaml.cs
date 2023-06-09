﻿using Prism.Ioc;
using sequence_maker.Services;
using sequence_maker.Views;
using System.Windows;

namespace sequence_maker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILogManager, LogManager>();
        }
    }
}
