// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// One row of the sample lists: a name, where the person worked, and the letter their surname
    /// starts with, which is what the grouped samples put them in groups by.
    /// </summary>
    public sealed class Person
    {
        public Person(string name, string company)
        {
            this.Name = name;
            this.Company = company;
        }

        public string Name { get; }

        public string Company { get; }

        public string Initial => this.Name.Substring(this.Name.LastIndexOf(' ') + 1, 1);
    }

    /// <summary>
    /// People the samples show in a list. They are the ones the rest of this gallery already names,
    /// so a reader coming from another page recognises them.
    /// </summary>
    public static class SamplePeople
    {
        public static IReadOnlyList<Person> All { get; } = new List<Person>
                                                           {
                                                               new("Charles Babbage", "Analytical Engine"),
                                                               new("John Backus", "IBM Research"),
                                                               new("Tim Berners-Lee", "CERN"),
                                                               new("Alonzo Church", "Princeton University"),
                                                               new("Edgar Codd", "IBM San Jose"),
                                                               new("Edsger Dijkstra", "Eindhoven University"),
                                                               new("Anders Hejlsberg", "Microsoft"),
                                                               new("Grace Hopper", "Remington Rand"),
                                                               new("Barbara Liskov", "MIT"),
                                                               new("Ada Lovelace", "Analytical Engine"),
                                                               new("Dennis Ritchie", "Bell Labs"),
                                                               new("Alan Turing", "Bletchley Park"),
                                                               new("Niklaus Wirth", "ETH Zürich"),
                                                               new("Konrad Zuse", "Zuse KG")
                                                           };
    }
}
