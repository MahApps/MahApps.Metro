// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MetroDemo.Models
{
    /// <summary>
    /// The content used by the TransitioningContentControl examples. It is a plain data object, so the
    /// same instance can be the content of more than one control at a time.
    /// </summary>
    public class TransitionContent
    {
        public TransitionContent(int number)
        {
            this.Number = number;
        }

        public int Number { get; }

        public string Text => $"Content {this.Number}";

        /// <summary>
        /// Used by the example template to alternate the background, which makes the transition easier to follow.
        /// </summary>
        public bool IsEven => this.Number % 2 == 0;
    }
}
