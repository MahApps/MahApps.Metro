﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroDemo.Models
{
    public class Animals
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        //public override bool Equals(object obj)
        //{
        //    if (obj == null)
        //        return false;

        //    if (obj is Animals animal)
        //        return animal.Id == Id;

        //    return false;
        //}
        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}

        public static IList<Animals> GenerateDate()
        {
            return new List<Animals>
            {
                new Animals { Id = 1, Name = "African elephant"},
                new Animals { Id = 2, Name = "Ant"},
                new Animals { Id = 3, Name = "Antelope"},
                new Animals { Id = 4, Name = "Aphid"},
                new Animals { Id = 5, Name = "Arctic wolf"},
                new Animals { Id = 6, Name = "Badger"},
                new Animals { Id = 7, Name = "Bald eagle"},
                new Animals { Id = 8, Name = "Bat"},
                new Animals { Id = 9, Name = "Bear"},
                new Animals { Id = 10, Name = "Bee"},
                new Animals { Id = 11, Name = "Beetle"},
                new Animals { Id = 12, Name = "Bengal tiger"},
                new Animals { Id = 13, Name = "Bison"},
                new Animals { Id = 14, Name = "Butterfly"},
                new Animals { Id = 15, Name = "Camel"},
                new Animals { Id = 16, Name = "Cat"},
                new Animals { Id = 17, Name = "Caterpillar"},
                new Animals { Id = 18, Name = "Chicken"},
                new Animals { Id = 19, Name = "Chimpanzee"},
                new Animals { Id = 20, Name = "Chipmunk"},
                new Animals { Id = 21, Name = "Cicada"},
                new Animals { Id = 22, Name = "Clam"},
                new Animals { Id = 23, Name = "Cockroach"},
                new Animals { Id = 24, Name = "Cormorant"},
                new Animals { Id = 25, Name = "Cow"},
                new Animals { Id = 26, Name = "Coyote"},
                new Animals { Id = 27, Name = "Crab"},
                new Animals { Id = 28, Name = "Crow"},
                new Animals { Id = 29, Name = "Cuckoo"},
                new Animals { Id = 30, Name = "Deer"},
                new Animals { Id = 31, Name = "Dog"},
                new Animals { Id = 32, Name = "Dolphin"},
                new Animals { Id = 33, Name = "Donkey"},
                new Animals { Id = 34, Name = "Dove"},
                new Animals { Id = 35, Name = "Dragonfly"},
                new Animals { Id = 36, Name = "Duck"},
                new Animals { Id = 37, Name = "Elephant"},
                new Animals { Id = 38, Name = "Elk"},
                new Animals { Id = 39, Name = "Finch"},
                new Animals { Id = 40, Name = "Fish"},
                new Animals { Id = 41, Name = "Flamingo"},
                new Animals { Id = 42, Name = "Flea"},
                new Animals { Id = 43, Name = "Fly"},
                new Animals { Id = 44, Name = "Fox"},
                new Animals { Id = 45, Name = "Frigatebird"},
                new Animals { Id = 46, Name = "Giraffe"},
                new Animals { Id = 47, Name = "Goat"},
                new Animals { Id = 48, Name = "Goldfish"},
                new Animals { Id = 49, Name = "Goose"},
                new Animals { Id = 50, Name = "Gorilla"},
                new Animals { Id = 51, Name = "Grasshopper"},
                new Animals { Id = 52, Name = "Great horned owl"},
                new Animals { Id = 53, Name = "Guinea pig"},
                new Animals { Id = 54, Name = "Hamster"},
                new Animals { Id = 55, Name = "Hare"},
                new Animals { Id = 56, Name = "Hawk"},
                new Animals { Id = 57, Name = "Hedgehog"},
                new Animals { Id = 58, Name = "Hippopotamus"},
                new Animals { Id = 59, Name = "Hornbill"},
                new Animals { Id = 60, Name = "Horse"},
                new Animals { Id = 61, Name = "Horse-fly"},
                new Animals { Id = 62, Name = "Howler monkey"},
                new Animals { Id = 63, Name = "Hummingbird"},
                new Animals { Id = 64, Name = "Hyena"},
                new Animals { Id = 65, Name = "Ibis"},
                new Animals { Id = 66, Name = "Jackal"},
                new Animals { Id = 67, Name = "Jellyfish"},
                new Animals { Id = 68, Name = "Kangaroo"},
                new Animals { Id = 69, Name = "Koala"},
                new Animals { Id = 70, Name = "Ladybugs(NAmE) /ladybirds(BrE)"},
                new Animals { Id = 71, Name = "Leopard"},
                new Animals { Id = 72, Name = "Lion"},
                new Animals { Id = 73, Name = "Lizard"},
                new Animals { Id = 74, Name = "Lobster"},
                new Animals { Id = 75, Name = "Lynxes"},
                new Animals { Id = 76, Name = "Mantis"},
                new Animals { Id = 77, Name = "Marten"},
                new Animals { Id = 78, Name = "Mole"},
                new Animals { Id = 79, Name = "Monkey"},
                new Animals { Id = 80, Name = "Mosquito"},
                new Animals { Id = 81, Name = "Moth"},
                new Animals { Id = 82, Name = "Mouse"},
                new Animals { Id = 83, Name = "Octopus"},
                new Animals { Id = 84, Name = "Okapi"},
                new Animals { Id = 85, Name = "Orangutan"},
                new Animals { Id = 86, Name = "Otter"},
                new Animals { Id = 87, Name = "Owl"},
                new Animals { Id = 88, Name = "Ox"},
                new Animals { Id = 89, Name = "Oyster"},
                new Animals { Id = 90, Name = "Panda"},
                new Animals { Id = 91, Name = "Parrot"},
                new Animals { Id = 92, Name = "Pelecaniformes"},
                new Animals { Id = 93, Name = "Pelican"},
                new Animals { Id = 94, Name = "Penguin"},
                new Animals { Id = 95, Name = "Pig"},
                new Animals { Id = 96, Name = "Pigeon"},
                new Animals { Id = 97, Name = "Porcupine"},
                new Animals { Id = 98, Name = "Possum"},
                new Animals { Id = 99, Name = "Puma"},
                new Animals { Id = 100, Name = "Rabbit"},
                new Animals { Id = 101, Name = "Raccoon"},
                new Animals { Id = 102, Name = "Rat"},
                new Animals { Id = 103, Name = "Raven"},
                new Animals { Id = 104, Name = "Red dear"},
                new Animals { Id = 105, Name = "Red panda"},
                new Animals { Id = 106, Name = "Red squirrel"},
                new Animals { Id = 107, Name = "Reindeer"},
                new Animals { Id = 108, Name = "Rhinoceros"},
                new Animals { Id = 109, Name = "Robin"},
                new Animals { Id = 110, Name = "Sandpiper"},
                new Animals { Id = 111, Name = "Sea turtle"},
                new Animals { Id = 112, Name = "Seahorse"},
                new Animals { Id = 113, Name = "Seal"},
                new Animals { Id = 114, Name = "Shark"},
                new Animals { Id = 115, Name = "Sheep"},
                new Animals { Id = 116, Name = "Shell"},
                new Animals { Id = 117, Name = "Shrimp"},
                new Animals { Id = 118, Name = "Snake"},
                new Animals { Id = 119, Name = "Sparrow"},
                new Animals { Id = 120, Name = "Squid"},
                new Animals { Id = 121, Name = "Squirrel"},
                new Animals { Id = 122, Name = "Squirrel monkey"},
                new Animals { Id = 123, Name = "Starfish"},
                new Animals { Id = 124, Name = "Stork"},
                new Animals { Id = 125, Name = "Swallow"},
                new Animals { Id = 126, Name = "Swan"},
                new Animals { Id = 127, Name = "Termite"},
                new Animals { Id = 128, Name = "Tern"},
                new Animals { Id = 129, Name = "Tick"},
                new Animals { Id = 130, Name = "Tiger"},
                new Animals { Id = 131, Name = "Turkey"},
                new Animals { Id = 132, Name = "Turtle"},
                new Animals { Id = 133, Name = "Walrus"},
                new Animals { Id = 134, Name = "Wasp"},
                new Animals { Id = 135, Name = "Whale"},
                new Animals { Id = 136, Name = "Whitefly"},
                new Animals { Id = 137, Name = "Wild boar"},
                new Animals { Id = 138, Name = "Wolf"},
                new Animals { Id = 139, Name = "Wombat"},
                new Animals { Id = 140, Name = "Woodpecker"},
                new Animals { Id = 141, Name = "Zebra"},
            };
        }
    }
}
