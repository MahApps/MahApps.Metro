using System;
using System.Windows;
using System.Windows.Interactivity;

namespace MahApps.Metro.Behaviours
{
    public static class BehaviorsHelper
    {
        /// <summary>
        /// Add behavior to element if element does NOT contain this behavior.
        /// </summary>
        /// <typeparam name="TElement">Type of the element.</typeparam>
        /// <param name="element">Element.</param>
        /// <param name="behavior">Behavior.</param>
        public static void AddBehavior<TElement>(this TElement element, Behavior<TElement> behavior)
            where TElement : DependencyObject
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (behavior == null)
                throw new ArgumentNullException("behavior");

            BehaviorCollection behaviors = Interaction.GetBehaviors(element);

            // Check if there is NO behavior.
            if (!behaviors.Contains(behavior))
                behaviors.Add(behavior);
        }

        /// <summary>
        /// Remove behavior from element if element contains this behavior.
        /// </summary>
        /// <typeparam name="TElement">Type of the element.</typeparam>
        /// <param name="element">Element.</param>
        /// <param name="behavior">Behavior.</param>
        public static void RemoveBehavior<TElement>(this TElement element, Behavior<TElement> behavior)
            where TElement : DependencyObject
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (behavior == null)
                throw new ArgumentNullException("behavior");

            BehaviorCollection behaviors = Interaction.GetBehaviors(element);

            // Check if there is behavior.
            if (behaviors.Contains(behavior))
                behaviors.Remove(behavior);
        }
    }
}
