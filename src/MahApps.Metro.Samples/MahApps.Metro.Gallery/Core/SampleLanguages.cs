// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// One row of the sample tables: a programming language, who designed it, the year it first
    /// appeared and the paradigm it is mostly filed under, which is what the grouped samples group by.
    /// </summary>
    public sealed class Language
    {
        public Language(int id, string name, string designer, int year, string paradigm, bool isInUse, int influence)
        {
            this.Id = id;
            this.Name = name;
            this.Designer = designer;
            this.Year = year;
            this.Paradigm = paradigm;
            this.IsInUse = isInUse;
            this.Influence = influence;
        }

        public int Id { get; }

        public string Name { get; }

        public string Designer { get; }

        public int Year { get; }

        public string Paradigm { get; }

        public bool IsInUse { get; }

        /// <summary>
        /// Whether it is still written today, in the words a badge shows.
        /// </summary>
        public string Status => this.IsInUse ? "In use" : "Historic";

        /// <summary>
        /// How much the languages after it took from it, one to five stars.
        /// </summary>
        public int Influence { get; }

        /// <summary>
        /// What the round badge in front of the name shows.
        /// </summary>
        public string Monogram => this.Name.Length > 2 ? this.Name.Substring(0, 2) : this.Name;
    }

    /// <summary>
    /// Languages the samples show in a table, oldest first, which is the order the source has them in.
    /// </summary>
    public static class SampleLanguages
    {
        /// <summary>The year the timeline in the custom cells starts with.</summary>
        public static double FirstYear { get; } = 1950;

        /// <summary>The year the timeline in the custom cells ends with.</summary>
        public static double LastYear { get; } = 2020;

        public static IReadOnlyList<Language> All { get; } = new List<Language>
                                                             {
                                                                 new(1, "Fortran", "John Backus", 1957, "Imperative", true, 5),
                                                                 new(2, "Lisp", "John McCarthy", 1958, "Functional", true, 5),
                                                                 new(3, "COBOL", "Grace Hopper", 1959, "Imperative", true, 3),
                                                                 new(4, "Simula", "Ole-Johan Dahl", 1967, "Object-oriented", false, 4),
                                                                 new(5, "Pascal", "Niklaus Wirth", 1970, "Imperative", false, 4),
                                                                 new(6, "C", "Dennis Ritchie", 1972, "Imperative", true, 5),
                                                                 new(7, "Smalltalk", "Alan Kay", 1972, "Object-oriented", false, 4),
                                                                 new(8, "ML", "Robin Milner", 1973, "Functional", false, 4),
                                                                 new(9, "CLU", "Barbara Liskov", 1975, "Object-oriented", false, 3),
                                                                 new(10, "C++", "Bjarne Stroustrup", 1985, "Object-oriented", true, 4),
                                                                 new(11, "Haskell", "Paul Hudak", 1990, "Functional", true, 4),
                                                                 new(12, "Python", "Guido van Rossum", 1991, "Object-oriented", true, 3),
                                                                 new(13, "C#", "Anders Hejlsberg", 2000, "Object-oriented", true, 3),
                                                                 new(14, "F#", "Don Syme", 2005, "Functional", true, 2)
                                                             };
    }
}
