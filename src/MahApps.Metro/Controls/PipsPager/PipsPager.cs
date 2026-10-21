// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// Ported from PipsPager.cpp of microsoft-ui-xaml, Copyright (c) Microsoft Corporation, MIT License.

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.ValueBoxes;
using AutomationStrings = MahApps.Metro.Lang.Automation;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A row of pips, one per page, to show and to pick the current page of a paged collection,
    /// with an optional button on either end to go a page back or on.
    /// </summary>
    [TemplatePart(Name = PART_PreviousPageButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_NextPageButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_ScrollViewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PART_Pips, Type = typeof(ItemsControl))]
    [StyleTypedProperty(Property = nameof(PreviousButtonStyle), StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = nameof(NextButtonStyle), StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = nameof(SelectedPipStyle), StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = nameof(NormalPipStyle), StyleTargetType = typeof(Button))]
    public class PipsPager : Control
    {
        private const string PART_PreviousPageButton = "PART_PreviousPageButton";
        private const string PART_NextPageButton = "PART_NextPageButton";
        private const string PART_ScrollViewer = "PART_ScrollViewer";
        private const string PART_Pips = "PART_Pips";

        private Button? previousPageButton;
        private Button? nextPageButton;
        private ScrollViewer? scrollViewer;
        private ItemsControl? pips;
        private Size defaultPipSize;
        private Size selectedPipSize;
        private bool scrollToSelectedPipPending;
        private bool isPointerOver;
        private bool isFocused;

        static PipsPager()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PipsPager), new FrameworkPropertyMetadata(typeof(PipsPager)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipsPager"/> class.
        /// </summary>
        public PipsPager()
        {
            this.SetValue(TemplateSettingsPropertyKey, new PipsPagerTemplateSettings());
            this.UpdatePipsItems(this.NumberOfPages, this.MaxVisiblePips);
        }

        /// <summary>Identifies the <see cref="NumberOfPages"/> dependency property.</summary>
        public static readonly DependencyProperty NumberOfPagesProperty
            = DependencyProperty.Register(nameof(NumberOfPages),
                                          typeof(int),
                                          typeof(PipsPager),
                                          new PropertyMetadata(-1, (d, _) => ((PipsPager)d).OnNumberOfPagesChanged()));

        /// <summary>
        /// Gets or sets the number of pages. -1, the default, stands for an unknown number, then a new pip
        /// comes up whenever the last one is selected.
        /// </summary>
        public int NumberOfPages
        {
            get => (int)this.GetValue(NumberOfPagesProperty);
            set => this.SetValue(NumberOfPagesProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedPageIndex"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedPageIndexProperty
            = DependencyProperty.Register(nameof(SelectedPageIndex),
                                          typeof(int),
                                          typeof(PipsPager),
                                          new FrameworkPropertyMetadata(0,
                                                                        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                        (d, e) => ((PipsPager)d).OnSelectedPageIndexChanged((int)e.NewValue)));

        /// <summary>
        /// Gets or sets the index of the selected page, counted from zero. A value outside the pages is
        /// pulled back to the nearest one.
        /// </summary>
        public int SelectedPageIndex
        {
            get => (int)this.GetValue(SelectedPageIndexProperty);
            set => this.SetValue(SelectedPageIndexProperty, value);
        }

        /// <summary>Identifies the <see cref="MaxVisiblePips"/> dependency property.</summary>
        public static readonly DependencyProperty MaxVisiblePipsProperty
            = DependencyProperty.Register(nameof(MaxVisiblePips),
                                          typeof(int),
                                          typeof(PipsPager),
                                          new PropertyMetadata(5, (d, _) => ((PipsPager)d).OnMaxVisiblePipsChanged()));

        /// <summary>
        /// Gets or sets how many pips are seen at once. More of them scroll, so that the selected one is in the middle.
        /// </summary>
        public int MaxVisiblePips
        {
            get => (int)this.GetValue(MaxVisiblePipsProperty);
            set => this.SetValue(MaxVisiblePipsProperty, value);
        }

        /// <summary>Identifies the <see cref="Orientation"/> dependency property.</summary>
        public static readonly DependencyProperty OrientationProperty
            = DependencyProperty.Register(nameof(Orientation),
                                          typeof(Orientation),
                                          typeof(PipsPager),
                                          new PropertyMetadata(Orientation.Horizontal, (d, _) => ((PipsPager)d).OnOrientationChanged()));

        /// <summary>
        /// Gets or sets whether the pips run from left to right or from top to bottom.
        /// </summary>
        public Orientation Orientation
        {
            get => (Orientation)this.GetValue(OrientationProperty);
            set => this.SetValue(OrientationProperty, value);
        }

        private static readonly DependencyPropertyKey PipOrientationPropertyKey
            = DependencyProperty.RegisterAttachedReadOnly("PipOrientation",
                                                          typeof(Orientation),
                                                          typeof(PipsPager),
                                                          new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>Identifies the <see cref="GetPipOrientation(DependencyObject)"/> attached property.</summary>
        public static readonly DependencyProperty PipOrientationProperty = PipOrientationPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the orientation of the pager an element sits in, as the pager hands it down to its pips and arrows.
        /// A style of a pip turns on it rather than on a binding to the pager, since a binding to an ancestor
        /// is not there yet when the pager measures a pip it made on its own.
        /// </summary>
        public static Orientation GetPipOrientation(DependencyObject element)
        {
            return (Orientation)element.GetValue(PipOrientationProperty);
        }

        /// <summary>Identifies the <see cref="PreviousButtonVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty PreviousButtonVisibilityProperty
            = DependencyProperty.Register(nameof(PreviousButtonVisibility),
                                          typeof(PipsPagerButtonVisibility),
                                          typeof(PipsPager),
                                          new PropertyMetadata(PipsPagerButtonVisibility.Collapsed, (d, _) => ((PipsPager)d).UpdateNavigationButtonStates()));

        /// <summary>
        /// Gets or sets when the button to the previous page is shown.
        /// </summary>
        public PipsPagerButtonVisibility PreviousButtonVisibility
        {
            get => (PipsPagerButtonVisibility)this.GetValue(PreviousButtonVisibilityProperty);
            set => this.SetValue(PreviousButtonVisibilityProperty, value);
        }

        /// <summary>Identifies the <see cref="NextButtonVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty NextButtonVisibilityProperty
            = DependencyProperty.Register(nameof(NextButtonVisibility),
                                          typeof(PipsPagerButtonVisibility),
                                          typeof(PipsPager),
                                          new PropertyMetadata(PipsPagerButtonVisibility.Collapsed, (d, _) => ((PipsPager)d).UpdateNavigationButtonStates()));

        /// <summary>
        /// Gets or sets when the button to the next page is shown.
        /// </summary>
        public PipsPagerButtonVisibility NextButtonVisibility
        {
            get => (PipsPagerButtonVisibility)this.GetValue(NextButtonVisibilityProperty);
            set => this.SetValue(NextButtonVisibilityProperty, value);
        }

        /// <summary>Identifies the <see cref="PreviousButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty PreviousButtonStyleProperty
            = DependencyProperty.Register(nameof(PreviousButtonStyle),
                                          typeof(Style),
                                          typeof(PipsPager),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style of the button to the previous page.
        /// </summary>
        public Style? PreviousButtonStyle
        {
            get => (Style?)this.GetValue(PreviousButtonStyleProperty);
            set => this.SetValue(PreviousButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="NextButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty NextButtonStyleProperty
            = DependencyProperty.Register(nameof(NextButtonStyle),
                                          typeof(Style),
                                          typeof(PipsPager),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style of the button to the next page.
        /// </summary>
        public Style? NextButtonStyle
        {
            get => (Style?)this.GetValue(NextButtonStyleProperty);
            set => this.SetValue(NextButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedPipStyle"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedPipStyleProperty
            = DependencyProperty.Register(nameof(SelectedPipStyle),
                                          typeof(Style),
                                          typeof(PipsPager),
                                          new PropertyMetadata(null, (d, _) => ((PipsPager)d).OnPipStyleChanged()));

        /// <summary>
        /// Gets or sets the style of the pip of the selected page.
        /// </summary>
        public Style? SelectedPipStyle
        {
            get => (Style?)this.GetValue(SelectedPipStyleProperty);
            set => this.SetValue(SelectedPipStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="NormalPipStyle"/> dependency property.</summary>
        public static readonly DependencyProperty NormalPipStyleProperty
            = DependencyProperty.Register(nameof(NormalPipStyle),
                                          typeof(Style),
                                          typeof(PipsPager),
                                          new PropertyMetadata(null, (d, _) => ((PipsPager)d).OnPipStyleChanged()));

        /// <summary>
        /// Gets or sets the style of the pips of the pages that are not selected.
        /// </summary>
        public Style? NormalPipStyle
        {
            get => (Style?)this.GetValue(NormalPipStyleProperty);
            set => this.SetValue(NormalPipStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="WrapMode"/> dependency property.</summary>
        public static readonly DependencyProperty WrapModeProperty
            = DependencyProperty.Register(nameof(WrapMode),
                                          typeof(PipsPagerWrapMode),
                                          typeof(PipsPager),
                                          new PropertyMetadata(PipsPagerWrapMode.None, (d, _) => ((PipsPager)d).UpdateNavigationButtonStates()));

        /// <summary>
        /// Gets or sets whether the buttons go round from the last page to the first and back.
        /// </summary>
        public PipsPagerWrapMode WrapMode
        {
            get => (PipsPagerWrapMode)this.GetValue(WrapModeProperty);
            set => this.SetValue(WrapModeProperty, value);
        }

        private static readonly DependencyPropertyKey TemplateSettingsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(TemplateSettings),
                                                  typeof(PipsPagerTemplateSettings),
                                                  typeof(PipsPager),
                                                  new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="TemplateSettings"/> dependency property.</summary>
        public static readonly DependencyProperty TemplateSettingsProperty = TemplateSettingsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets what the template binds to, the pips among it.
        /// </summary>
        public PipsPagerTemplateSettings TemplateSettings => (PipsPagerTemplateSettings)this.GetValue(TemplateSettingsProperty);

        /// <summary>
        /// Occurs when another page is selected, once the index is within the pages.
        /// </summary>
        public event EventHandler<PipsPagerSelectedIndexChangedEventArgs>? SelectedIndexChanged;

        private bool IsWrapEnabled => this.WrapMode == PipsPagerWrapMode.Wrap;

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            if (this.previousPageButton is not null)
            {
                this.previousPageButton.Click -= this.OnPreviousButtonClicked;
            }

            if (this.nextPageButton is not null)
            {
                this.nextPageButton.Click -= this.OnNextButtonClicked;
            }

            if (this.pips is not null)
            {
                this.pips.ItemContainerGenerator.StatusChanged -= this.OnPipsStatusChanged;
                this.pips.RequestBringIntoView -= OnPipsRequestBringIntoView;
            }

            if (this.scrollViewer is not null)
            {
                this.scrollViewer.LayoutUpdated -= this.OnScrollViewerLayoutUpdated;
            }

            this.scrollToSelectedPipPending = false;

            base.OnApplyTemplate();

            this.previousPageButton = this.GetTemplateChild(PART_PreviousPageButton) as Button;
            if (this.previousPageButton is not null)
            {
                SetDefaultAutomationName(this.previousPageButton, AutomationStrings.PipsPagerPreviousPageButtonText);
                this.previousPageButton.Click += this.OnPreviousButtonClicked;
            }

            this.nextPageButton = this.GetTemplateChild(PART_NextPageButton) as Button;
            if (this.nextPageButton is not null)
            {
                SetDefaultAutomationName(this.nextPageButton, AutomationStrings.PipsPagerNextPageButtonText);
                this.nextPageButton.Click += this.OnNextButtonClicked;
            }

            this.scrollViewer = this.GetTemplateChild(PART_ScrollViewer) as ScrollViewer;

            this.pips = this.GetTemplateChild(PART_Pips) as ItemsControl;
            if (this.pips is not null)
            {
                this.pips.ItemContainerGenerator.StatusChanged += this.OnPipsStatusChanged;
                this.pips.RequestBringIntoView += OnPipsRequestBringIntoView;
            }

            this.UpdatePipsItems(this.NumberOfPages, this.MaxVisiblePips);
            this.UpdatePipSizes();
            this.SetScrollViewerMaxSize();
            this.UpdateNavigationButtonStates();
            this.UpdateSelectedPip();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new PipsPagerAutomationPeer(this);
        }

        /// <summary>
        /// The pip of the selected page, if it is there.
        /// </summary>
        internal FrameworkElement? GetSelectedPip()
        {
            return this.pips is { } itemsControl ? GetPip(itemsControl.ItemContainerGenerator.ContainerFromIndex(this.SelectedPageIndex)) : null;
        }

        /// <summary>
        /// Moves the focus to the pip before or after the focused one. At the first and the last pip it stays,
        /// the arrows are reached by tab. From an arrow it goes the way WPF does.
        /// </summary>
        private void MoveFocusByPips(int step)
        {
            if (Keyboard.FocusedElement is not Visual focused)
            {
                return;
            }

            if (this.pips is { } itemsControl && itemsControl.IsAncestorOf(focused))
            {
                var index = itemsControl.ItemContainerGenerator.IndexFromContainer(itemsControl.ContainerFromElement(focused));
                var target = index >= 0 && index + step >= 0 && index + step < itemsControl.Items.Count ? GetPip(itemsControl.ItemContainerGenerator.ContainerFromIndex(index + step)) : null;
                target?.Focus();
            }
            else if (focused is UIElement element)
            {
                var horizontal = this.Orientation == Orientation.Horizontal;
                var direction = step < 0
                    ? horizontal ? FocusNavigationDirection.Left : FocusNavigationDirection.Up
                    : horizontal ? FocusNavigationDirection.Right : FocusNavigationDirection.Down;
                element.MoveFocus(new TraversalRequest(direction));
            }
        }

        /// <inheritdoc />
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // Tunnelled, since the keyboard navigation of WPF takes the arrow keys on their way up and moves the focus by the axis of the key.
            // Left and up go to the previous pip, right and down to the next one, whichever way the pager runs.
            var step = e.Key switch
            {
                Key.Left or Key.Up => -1,
                Key.Right or Key.Down => 1,
                _ => 0
            };

            if (step != 0)
            {
                this.MoveFocusByPips(step);
                e.Handled = true;
            }

            base.OnPreviewKeyDown(e);
        }

        /// <inheritdoc />
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.isPointerOver = true;
            this.UpdateNavigationButtonStates();
        }

        /// <inheritdoc />
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.isPointerOver = false;
            this.UpdateNavigationButtonStates();
        }

        /// <inheritdoc />
        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnPreviewGotKeyboardFocus(e);

            // Focus that comes from outside the pips, by tab or by code, goes to the pip of the selected page. A click
            // keeps its pip, and the pips are one tab stop (KeyboardNavigation.TabNavigation in the template).
            if (this.pips is { } itemsControl
                && e.NewFocus is Visual newFocus
                && itemsControl.IsAncestorOf(newFocus)
                && !(e.OldFocus is Visual oldFocus && itemsControl.IsAncestorOf(oldFocus))
                && Mouse.LeftButton != MouseButtonState.Pressed
                && this.GetSelectedPip() is { } selectedPip
                && !ReferenceEquals(selectedPip, newFocus))
            {
                e.Handled = true;
                selectedPip.Focus();
            }
        }

        /// <inheritdoc />
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            if (e.NewFocus is ButtonBase)
            {
                // A button takes the focus when it is pressed with the mouse. WinUI counts only the focus that
                // didn't come from the pointer, so that a click doesn't keep the buttons up after the pointer left.
                this.isFocused = this.IsKeyboardFocusWithin && Mouse.LeftButton != MouseButtonState.Pressed;
                this.UpdateNavigationButtonStates();
            }
        }

        /// <inheritdoc />
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);

            // The event bubbles up from every pip and arrow, also when the focus only moves on to another one of them.
            if (e.NewFocus is Visual newFocus && this.IsAncestorOf(newFocus))
            {
                return;
            }

            this.isFocused = false;
            this.UpdateNavigationButtonStates();
        }

        private static void SetDefaultAutomationName(DependencyObject button, string name)
        {
            if (string.IsNullOrEmpty(AutomationProperties.GetName(button)))
            {
                AutomationProperties.SetName(button, name);
            }
        }

        private void RaiseSelectedIndexChanged()
        {
            this.SelectedIndexChanged?.Invoke(this, new PipsPagerSelectedIndexChangedEventArgs());
        }

        private void OnNumberOfPagesChanged()
        {
            var numberOfPages = this.NumberOfPages;
            this.UpdatePipsItems(numberOfPages, this.MaxVisiblePips);
            this.SetScrollViewerMaxSize();

            if (this.SelectedPageIndex > numberOfPages - 1 && numberOfPages > -1)
            {
                this.SetCurrentValue(SelectedPageIndexProperty, numberOfPages - 1);
            }
            else
            {
                this.UpdateSelectedPip();
                this.UpdateNavigationButtonStates();
            }
        }

        private void OnSelectedPageIndexChanged(int newValue)
        {
            // A value that was pulled back meanwhile is not the one to tell about, only the final valid one raises the event.
            if (newValue != this.SelectedPageIndex)
            {
                return;
            }

            // Ensure that SelectedPageIndex ends up within the pages. Without any pages there is no upper bound.
            // The value is set again rather than coerced, so that only the valid one raises the event.
            var numberOfPages = this.NumberOfPages;
            if (this.SelectedPageIndex > numberOfPages - 1 && numberOfPages > 0)
            {
                this.SetCurrentValue(SelectedPageIndexProperty, numberOfPages - 1);
            }
            else if (this.SelectedPageIndex < 0)
            {
                this.SetCurrentValue(SelectedPageIndexProperty, 0);
            }
            else
            {
                if (numberOfPages < 0)
                {
                    this.UpdatePipsItems(numberOfPages, this.MaxVisiblePips);
                }

                this.UpdateSelectedPip();
                this.UpdateNavigationButtonStates();
                this.RaiseSelectedIndexChanged();
                (UIElementAutomationPeer.FromElement(this) as PipsPagerAutomationPeer)?.RaiseSelectionChanged();
            }
        }

        private void OnMaxVisiblePipsChanged()
        {
            var numberOfPages = this.NumberOfPages;
            if (numberOfPages < 0)
            {
                this.UpdatePipsItems(numberOfPages, this.MaxVisiblePips);
            }

            this.SetScrollViewerMaxSize();
            this.UpdateSelectedPip();
            this.UpdateNavigationButtonStates();
        }

        private void OnOrientationChanged()
        {
            this.SetValue(PipOrientationPropertyKey, this.Orientation);
            this.UpdatePipSizes();
            this.SetScrollViewerMaxSize();
            this.UpdateSelectedPip();
        }

        private void OnPipStyleChanged()
        {
            this.UpdatePipSizes();
            this.SetScrollViewerMaxSize();
            this.UpdateSelectedPip();
        }

        /// <summary>
        /// The room a pip takes with the given style, margin included. It is measured on a pip made just for it,
        /// since the pips in use wear the style of their page and change with the selection.
        /// </summary>
        private Size GetDesiredPipSize(Style? style)
        {
            if (this.pips is null)
            {
                return default;
            }

            var pip = new Button { Style = style };

            // Hung into the logical tree for the time of the measure only, so that the style finds its resources.
            this.AddLogicalChild(pip);
            try
            {
                pip.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                return pip.DesiredSize;
            }
            finally
            {
                this.RemoveLogicalChild(pip);
            }
        }

        private void UpdatePipSizes()
        {
            this.defaultPipSize = this.GetDesiredPipSize(this.NormalPipStyle);
            this.selectedPipSize = this.GetDesiredPipSize(this.SelectedPipStyle);
        }

        /// <summary>
        /// The size of the window on the pips along the pager: the room of as many pips as may be seen,
        /// and of one selected pip among them.
        /// </summary>
        internal static double CalculateScrollViewerSize(double defaultPipSize, double selectedPipSize, int numberOfPages, int maxVisiblePips)
        {
            maxVisiblePips = Math.Max(0, maxVisiblePips);
            if (maxVisiblePips == 0 || numberOfPages == 0)
            {
                return 0;
            }

            // an unknown number of pages shows as many pips as may be seen
            var numberOfPagesToDisplay = numberOfPages > 0 ? Math.Min(maxVisiblePips, numberOfPages) : maxVisiblePips;
            return defaultPipSize * (numberOfPagesToDisplay - 1) + selectedPipSize;
        }

        private void SetScrollViewerMaxSize()
        {
            if (this.scrollViewer is not { } viewer)
            {
                return;
            }

            if (this.Orientation == Orientation.Horizontal)
            {
                viewer.MaxWidth = CalculateScrollViewerSize(this.defaultPipSize.Width, this.selectedPipSize.Width, this.NumberOfPages, this.MaxVisiblePips);
                viewer.MaxHeight = Math.Max(this.defaultPipSize.Height, this.selectedPipSize.Height);
            }
            else
            {
                viewer.MaxHeight = CalculateScrollViewerSize(this.defaultPipSize.Height, this.selectedPipSize.Height, this.NumberOfPages, this.MaxVisiblePips);
                viewer.MaxWidth = Math.Max(this.defaultPipSize.Width, this.selectedPipSize.Width);
            }
        }

        /// <summary>
        /// A pip that takes the focus must not scroll the pips by itself, the selected one is kept in the middle instead.
        /// </summary>
        private static void OnPipsRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Scrolls to the selected pip once the layout is there, the containers and the extent of the scroll viewer
        /// are not before.
        /// </summary>
        private void ScrollToCenterOfViewport()
        {
            if (this.scrollViewer is not { } viewer || this.scrollToSelectedPipPending)
            {
                return;
            }

            this.scrollToSelectedPipPending = true;
            viewer.LayoutUpdated += this.OnScrollViewerLayoutUpdated;
        }

        private void OnScrollViewerLayoutUpdated(object? sender, EventArgs e)
        {
            if (this.scrollViewer is not { } viewer || this.pips is not { } itemsControl)
            {
                return;
            }

            viewer.LayoutUpdated -= this.OnScrollViewerLayoutUpdated;
            this.scrollToSelectedPipPending = false;

            if (itemsControl.ItemContainerGenerator.ContainerFromIndex(this.SelectedPageIndex) is not FrameworkElement container
                || !container.IsArrangeValid
                || !itemsControl.IsArrangeValid)
            {
                return;
            }

            // the point is in the coordinates of the pips, those of the scrolled content
            var center = container.TranslatePoint(new Point(container.ActualWidth / 2, container.ActualHeight / 2), itemsControl);
            if (this.Orientation == Orientation.Horizontal)
            {
                viewer.ScrollToHorizontalOffset(center.X - viewer.ViewportWidth / 2);
                viewer.ScrollToVerticalOffset(0);
            }
            else
            {
                viewer.ScrollToVerticalOffset(center.Y - viewer.ViewportHeight / 2);
                viewer.ScrollToHorizontalOffset(0);
            }
        }

        private void UpdatePipsItems(int numberOfPages, int maxVisualIndicators)
        {
            var pipsPagerItems = this.TemplateSettings.PipsPagerItems;
            var pipsListSize = pipsPagerItems.Count;

            if (numberOfPages == 0 || maxVisualIndicators == 0)
            {
                pipsPagerItems.Clear();
            }
            else if (numberOfPages < 0)
            {
                // an unknown number of pages, a negative number of visible pips counts as none
                var minNumberOfElements = Math.Max(this.SelectedPageIndex + 1, Math.Max(0, maxVisualIndicators));
                if (minNumberOfElements > pipsListSize)
                {
                    for (var i = pipsListSize; i < minNumberOfElements; i++)
                    {
                        pipsPagerItems.Add(i + 1);
                    }
                }
                else if (this.SelectedPageIndex == pipsListSize - 1)
                {
                    pipsPagerItems.Add(pipsListSize + 1);
                }
            }
            else if (pipsListSize < numberOfPages)
            {
                for (var i = pipsListSize; i < numberOfPages; i++)
                {
                    pipsPagerItems.Add(i + 1);
                }
            }
            else
            {
                for (var i = numberOfPages; i < pipsListSize; i++)
                {
                    pipsPagerItems.RemoveAt(pipsPagerItems.Count - 1);
                }
            }
        }

        private void OnPipsStatusChanged(object? sender, EventArgs e)
        {
            if (this.pips?.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                this.UpdateSelectedPip();
            }
        }

        /// <summary>
        /// Gives every pip the style for its page and makes it select that page when clicked.
        /// </summary>
        private void UpdateSelectedPip()
        {
            var itemsControl = this.pips;
            if (itemsControl is null)
            {
                return;
            }

            var generator = itemsControl.ItemContainerGenerator;
            var selectedPageIndex = this.SelectedPageIndex;
            for (var i = 0; i < itemsControl.Items.Count; i++)
            {
                if (GetPip(generator.ContainerFromIndex(i)) is { } pip)
                {
                    pip.Style = i == selectedPageIndex ? this.SelectedPipStyle : this.NormalPipStyle;

                    AutomationProperties.SetName(pip, $"{AutomationStrings.PipsPagerPageText} {i + 1}");
#if NETCOREAPP
                    // SetPositionInSet and SetSizeOfSet only exist from .NET Framework 4.7.2 on, so net462 skips them.
                    AutomationProperties.SetPositionInSet(pip, i + 1);
                    AutomationProperties.SetSizeOfSet(pip, this.NumberOfPages);
#endif

                    if (pip is ButtonBase button)
                    {
                        button.Click -= this.OnPipClicked;
                        button.Click += this.OnPipClicked;
                    }
                }
            }

            if (this.NumberOfPages != 0 && this.MaxVisiblePips > 0)
            {
                this.ScrollToCenterOfViewport();
            }
        }

        /// <summary>
        /// The pip is the root of what the item template made in the container.
        /// </summary>
        private static FrameworkElement? GetPip(DependencyObject? container)
        {
            if (container is not FrameworkElement element)
            {
                return null;
            }

            element.ApplyTemplate();
            return VisualTreeHelper.GetChildrenCount(element) > 0 ? VisualTreeHelper.GetChild(element, 0) as FrameworkElement : null;
        }

        private void OnPipClicked(object sender, RoutedEventArgs e)
        {
            if (this.pips is { } itemsControl && sender is DependencyObject pip)
            {
                var index = itemsControl.ItemContainerGenerator.IndexFromContainer(itemsControl.ContainerFromElement(pip));
                if (index >= 0)
                {
                    this.SetCurrentValue(SelectedPageIndexProperty, index);
                }
            }
        }

        private void OnPreviousButtonClicked(object sender, RoutedEventArgs e)
        {
            // The buttons are hidden then, but a template could leave them on,
            // and they shouldn't go to a page that isn't there.
            var numberOfPages = this.NumberOfPages;
            if (numberOfPages == 0 || numberOfPages == 1)
            {
                return;
            }

            var newPageIndex = Math.Max(0, this.SelectedPageIndex - 1);

            if (this.IsWrapEnabled && numberOfPages > -1 && this.SelectedPageIndex == 0)
            {
                newPageIndex = numberOfPages - 1;
            }

            this.SetCurrentValue(SelectedPageIndexProperty, newPageIndex);
        }

        private void OnNextButtonClicked(object sender, RoutedEventArgs e)
        {
            // The buttons are hidden then, but a template could leave them on,
            // and they shouldn't go to a page that isn't there.
            var numberOfPages = this.NumberOfPages;
            if (numberOfPages == 0 || numberOfPages == 1)
            {
                return;
            }

            var newPageIndex = numberOfPages > -1 ? Math.Min(this.SelectedPageIndex + 1, numberOfPages - 1) : this.SelectedPageIndex + 1;

            if (this.IsWrapEnabled && this.SelectedPageIndex == numberOfPages - 1)
            {
                newPageIndex = 0;
            }

            this.SetCurrentValue(SelectedPageIndexProperty, newPageIndex);
        }

        private void UpdateNavigationButtonStates()
        {
            var selectedPageIndex = this.SelectedPageIndex;
            var numberOfPages = this.NumberOfPages;

            this.UpdateIndividualNavigationButtonState(this.previousPageButton, selectedPageIndex == 0, this.PreviousButtonVisibility);
            this.UpdateIndividualNavigationButtonState(this.nextPageButton, selectedPageIndex == numberOfPages - 1, this.NextButtonVisibility);
        }

        /// <summary>
        /// What the visual states of WinUI do: a collapsed button is gone and off, one with nowhere to go is
        /// hidden and off, one shown only on pointer over is hidden while neither the pointer nor the focus is there.
        /// </summary>
        private void UpdateIndividualNavigationButtonState(Button? button, bool hiddenOnEdgeCondition, PipsPagerButtonVisibility visibility)
        {
            if (button is null)
            {
                return;
            }

            if (visibility == PipsPagerButtonVisibility.Collapsed)
            {
                button.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                button.SetCurrentValue(IsEnabledProperty, BooleanBoxes.FalseBox);
                return;
            }

            var numberOfPages = this.NumberOfPages;
            var isGenerallyVisible = (!hiddenOnEdgeCondition || (this.IsWrapEnabled && numberOfPages > 1))
                                     && numberOfPages != 0
                                     && this.MaxVisiblePips > 0;

            button.SetCurrentValue(VisibilityProperty, Visibility.Visible);
            button.SetCurrentValue(IsEnabledProperty, BooleanBoxes.Box(isGenerallyVisible));

            var isShown = (visibility == PipsPagerButtonVisibility.Visible || this.isPointerOver || this.isFocused) && isGenerallyVisible;
            button.SetCurrentValue(OpacityProperty, isShown ? 1d : 0d);
        }
    }
}
