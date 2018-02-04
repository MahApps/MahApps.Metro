using System.Diagnostics;
using System.Windows;
using System.Windows.Interactivity;

namespace MahApps.Metro.Behaviours
{
    using System.ComponentModel;

    public class StylizedBehaviors
    {
        public static readonly DependencyProperty BehaviorsProperty
            = DependencyProperty.RegisterAttached("Behaviors",
                                                  typeof(StylizedBehaviorCollection),
                                                  typeof(StylizedBehaviors),
                                                  new FrameworkPropertyMetadata(null, OnPropertyChanged));
        
        [Category(AppName.MahApps)]
        public static StylizedBehaviorCollection GetBehaviors(DependencyObject uie)
        {
            return (StylizedBehaviorCollection)uie.GetValue(BehaviorsProperty);
        }

        public static void SetBehaviors(DependencyObject uie, StylizedBehaviorCollection value)
        {
            uie.SetValue(BehaviorsProperty, value);
        }
        
        private static void OnPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            var uie = dpo as FrameworkElement;
            if (uie == null)
            {
                return;
            }

            var newBehaviors = e.NewValue as StylizedBehaviorCollection;
            var oldBehaviors = e.OldValue as StylizedBehaviorCollection;
            if (newBehaviors == oldBehaviors)
            {
                return;
            }

            BehaviorCollection itemBehaviors = Interaction.GetBehaviors(uie);

            uie.Unloaded -= FrameworkElementUnloaded;

            if (oldBehaviors != null)
            {
                foreach (var behavior in oldBehaviors)
                {
                    int index = GetIndexOf(itemBehaviors, behavior);
                    if (index >= 0)
                    {
                        itemBehaviors.RemoveAt(index);
                    }
                }
            }

            if (newBehaviors != null)
            {
                foreach (var behavior in newBehaviors)
                {
                    int index = GetIndexOf(itemBehaviors, behavior);
                    if (index < 0)
                    {
                        var clone = (Behavior)behavior.Clone();
                        SetOriginalBehavior(clone, behavior);
                        itemBehaviors.Add(clone);
                    }
                }
            }

            if (itemBehaviors.Count > 0)
            {
                uie.Unloaded += FrameworkElementUnloaded;
            }
            uie.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private static void Dispatcher_ShutdownStarted(object sender, System.EventArgs e)
        {
            Debug.WriteLine("Dispatcher.ShutdownStarted");
        }

        private static void FrameworkElementUnloaded(object sender, RoutedEventArgs e)
        {
            // BehaviorCollection doesn't call Detach, so we do this
            var uie = sender as FrameworkElement;
            if (uie == null)
            {
                return;
            }
            BehaviorCollection itemBehaviors = Interaction.GetBehaviors(uie);
            foreach (var behavior in itemBehaviors) {
                behavior.Detach();
            }
            uie.Loaded += FrameworkElementLoaded;
        }

        private static void FrameworkElementLoaded(object sender, RoutedEventArgs e)
        {
            var uie = sender as FrameworkElement;
            if (uie == null)
            {
                return;
            }
            uie.Loaded -= FrameworkElementLoaded;
            BehaviorCollection itemBehaviors = Interaction.GetBehaviors(uie);
            foreach (var behavior in itemBehaviors)
            {
                behavior.Attach(uie);
            }
        }

        private static int GetIndexOf(BehaviorCollection itemBehaviors, Behavior behavior)
        {
            int index = -1;

            Behavior orignalBehavior = GetOriginalBehavior(behavior);

            for (int i = 0; i < itemBehaviors.Count; i++)
            {
                Behavior currentBehavior = itemBehaviors[i];
                if (currentBehavior == behavior || currentBehavior == orignalBehavior)
                {
                    index = i;
                    break;
                }

                Behavior currentOrignalBehavior = GetOriginalBehavior(currentBehavior);
                if (currentOrignalBehavior == behavior || currentOrignalBehavior == orignalBehavior)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private static readonly DependencyProperty OriginalBehaviorProperty
            = DependencyProperty.RegisterAttached("OriginalBehavior",
                                                  typeof(Behavior),
                                                  typeof(StylizedBehaviors),
                                                  new UIPropertyMetadata(null));

        private static Behavior GetOriginalBehavior(DependencyObject obj)
        {
            return obj.GetValue(OriginalBehaviorProperty) as Behavior;
        }

        private static void SetOriginalBehavior(DependencyObject obj, Behavior value)
        {
            obj.SetValue(OriginalBehaviorProperty, value);
        }
    }
}
