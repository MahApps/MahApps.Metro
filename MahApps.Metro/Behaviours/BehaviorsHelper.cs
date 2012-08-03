namespace MahApps.Metro.Behaviours
{
    using System;
    using System.Windows;
    using System.Windows.Interactivity;

    public static class BehaviorsHelper
    {
        public static void AddBehavior<TElement>(this TElement element, Behavior<TElement> behavior)
            where TElement : DependencyObject
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (behavior == null)
                throw new ArgumentNullException("behavior");

            BehaviorCollection behaviors = Interaction.GetBehaviors(element);

            // Check if behavior is already there.
            if (!behaviors.Contains(behavior))
                behaviors.Add(behavior);
        }

        public static void RemoveBehavior<TElement>(this TElement element, Behavior<TElement> behavior)
            where TElement : DependencyObject
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (behavior == null)
                throw new ArgumentNullException("behavior");

            BehaviorCollection behaviors = Interaction.GetBehaviors(element);

            // Check if behavior is there.
            if (behaviors.Contains(behavior))
                behaviors.Remove(behavior);
        }
    }
}
