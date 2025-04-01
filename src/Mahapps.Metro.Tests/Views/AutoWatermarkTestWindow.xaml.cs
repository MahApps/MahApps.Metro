﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Tests.Views
{
    public partial class AutoWatermarkTestWindow : TestWindow
    {
        public AutoWatermarkTestWindow()
        {
            this.InitializeComponent();
        }
    }

    public class AutoWatermarkTestModel
    {
        [Display(Prompt = "AutoWatermark")]
        public string TextBoxText { get; set; }

        [Display(Prompt = "AutoWatermark")]
        public object ComboBoxSelectedObject { get; set; }

        [Display(Prompt = "AutoWatermark")]
        public double? NumericUpDownValue { get; set; }

        [Display(Prompt = "AutoWatermark")]
        public DateTime? DatePickerDate { get; set; }

        [Display(Prompt = "AutoWatermark")]
        public HotKey HotKey { get; set; }

        public AutoWatermarkTestSubModel SubModel { get; set; } = new AutoWatermarkTestSubModel();

        public ObservableCollection<AutoWatermarkTestSubModel> CollectionProperty { get; set; } = new ObservableCollection<AutoWatermarkTestSubModel>(new[] { new AutoWatermarkTestSubModel() });
    }

    public class AutoWatermarkTestSubModel
    {
        [Display(Prompt = "AutoWatermark")]
        public string TextBoxText { get; set; }
    }
}