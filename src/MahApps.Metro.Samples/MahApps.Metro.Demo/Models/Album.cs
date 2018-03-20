using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace MetroDemo.Models
{
    public class Album : INotifyPropertyChanged
    {
        private int _albumId;
        private int _genreId;
        private int _artistId;
        private string _title;
        private decimal _price;
        private Genre _genre;
        private Artist _artist;

        public int AlbumId
        {
            get { return _albumId; }
            set
            {
                if (value == _albumId) return;
                _albumId = value;
                OnPropertyChanged();
            }
        }

        [DisplayName("Genre")]
        public int GenreId
        {
            get { return _genreId; }
            set
            {
                if (value == _genreId) return;
                _genreId = value;
                OnPropertyChanged();
            }
        }

        [DisplayName("Artist")]
        public int ArtistId
        {
            get { return _artistId; }
            set
            {
                if (value == _artistId) return;
                _artistId = value;
                OnPropertyChanged();
            }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                if (value == this.isSelected) return;
                this.isSelected = value;
                this.OnPropertyChanged();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public decimal Price
        {
            get { return _price; }
            set
            {
                if (value == _price) return;
                _price = value;
                OnPropertyChanged();
            }
        }

        [DisplayName("Album Art URL")]
        public string AlbumArtUrl { get; set; }

        public virtual Genre Genre
        {
            get { return _genre; }
            set
            {
                if (Equals(value, _genre)) return;
                _genre = value;
                OnPropertyChanged();
            }
        }

        public virtual Artist Artist
        {
            get { return _artist; }
            set
            {
                if (Equals(value, _artist)) return;
                _artist = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class SampleData
    {
        public static List<Genre> Genres { get; set; }
        public static List<Artist> Artists { get; set; }
        public static List<Album> Albums { get; set; }
        public static void Seed()
        {
            if (Genres != null)
                return;

            Genres = new List<Genre>
            {
                new Genre { Name = "Rock" },
                new Genre { Name = "Jazz" },
                new Genre { Name = "Metal" },
                new Genre { Name = "Alternative" },
                new Genre { Name = "Disco" },
                new Genre { Name = "Blues" },
                new Genre { Name = "Latin" },
                new Genre { Name = "Reggae" },
                new Genre { Name = "Pop" },
                new Genre { Name = "Classical" }
            };

            Artists = new List<Artist>
            {
                new Artist { Name = "Aaron Copland & London Symphony Orchestra" },
                new Artist { Name = "Aaron Goldberg" },
                new Artist { Name = "AC/DC" },
                new Artist { Name = "Accept" },
                new Artist { Name = "Adrian Leaper & Doreen de Feis" },
                new Artist { Name = "Aerosmith" },
                new Artist { Name = "Aisha Duo" },
                new Artist { Name = "Alanis Morissette" },
                new Artist { Name = "Alberto Turco & Nova Schola Gregoriana" },
                new Artist { Name = "Alice In Chains" },
                new Artist { Name = "Amy Winehouse" },
                new Artist { Name = "Anita Ward" },
                new Artist { Name = "Antônio Carlos Jobim" },
                new Artist { Name = "Apocalyptica" },
                new Artist { Name = "Audioslave" },
                new Artist { Name = "Barry Wordsworth & BBC Concert Orchestra" },
                new Artist { Name = "Berliner Philharmoniker & Hans Rosbaud" },
                new Artist { Name = "Berliner Philharmoniker & Herbert Von Karajan" },
                new Artist { Name = "Billy Cobham" },
                new Artist { Name = "Black Label Society" },
                new Artist { Name = "Black Sabbath" },
                new Artist { Name = "Boston Symphony Orchestra & Seiji Ozawa" },
                new Artist { Name = "Britten Sinfonia, Ivor Bolton & Lesley Garrett" },
                new Artist { Name = "Bruce Dickinson" },
                new Artist { Name = "Buddy Guy" },
                new Artist { Name = "Caetano Veloso" },
                new Artist { Name = "Cake" },
                new Artist { Name = "Calexico" },
                new Artist { Name = "Cássia Eller" },
                new Artist { Name = "Chic" },
                new Artist { Name = "Chicago Symphony Orchestra & Fritz Reiner" },
                new Artist { Name = "Chico Buarque" },
                new Artist { Name = "Chico Science & Nação Zumbi" },
                new Artist { Name = "Choir Of Westminster Abbey & Simon Preston" },
                new Artist { Name = "Chris Cornell" },
                new Artist { Name = "Christopher O'Riley" },
                new Artist { Name = "Cidade Negra" },
                new Artist { Name = "Cláudio Zoli" },
                new Artist { Name = "Creedence Clearwater Revival" },
                new Artist { Name = "David Coverdale" },
                new Artist { Name = "Deep Purple" },
                new Artist { Name = "Dennis Chambers" },
                new Artist { Name = "Djavan" },
                new Artist { Name = "Donna Summer" },
                new Artist { Name = "Dread Zeppelin" },
                new Artist { Name = "Ed Motta" },
                new Artist { Name = "Edo de Waart & San Francisco Symphony" },
                new Artist { Name = "Elis Regina" },
                new Artist { Name = "English Concert & Trevor Pinnock" },
                new Artist { Name = "Eric Clapton" },
                new Artist { Name = "Eugene Ormandy" },
                new Artist { Name = "Faith No More" },
                new Artist { Name = "Falamansa" },
                new Artist { Name = "Foo Fighters" },
                new Artist { Name = "Frank Zappa & Captain Beefheart" },
                new Artist { Name = "Fretwork" },
                new Artist { Name = "Funk Como Le Gusta" },
                new Artist { Name = "Gerald Moore" },
                new Artist { Name = "Gilberto Gil" },
                new Artist { Name = "Godsmack" },
                new Artist { Name = "Gonzaguinha" },
                new Artist { Name = "Göteborgs Symfoniker & Neeme Järvi" },
                new Artist { Name = "Guns N' Roses" },
                new Artist { Name = "Gustav Mahler" },
                new Artist { Name = "Incognito" },
                new Artist { Name = "Iron Maiden" },
                new Artist { Name = "James Levine" },
                new Artist { Name = "Jamiroquai" },
                new Artist { Name = "Jimi Hendrix" },
                new Artist { Name = "Joe Satriani" },
                new Artist { Name = "Jorge Ben" },
                new Artist { Name = "Jota Quest" },
                new Artist { Name = "Judas Priest" },
                new Artist { Name = "Julian Bream" },
                new Artist { Name = "Kent Nagano and Orchestre de l'Opéra de Lyon" },
                new Artist { Name = "Kiss" },
                new Artist { Name = "Led Zeppelin" },
                new Artist { Name = "Legião Urbana" },
                new Artist { Name = "Lenny Kravitz" },
                new Artist { Name = "Les Arts Florissants & William Christie" },
                new Artist { Name = "London Symphony Orchestra & Sir Charles Mackerras" },
                new Artist { Name = "Luciana Souza/Romero Lubambo" },
                new Artist { Name = "Lulu Santos" },
                new Artist { Name = "Marcos Valle" },
                new Artist { Name = "Marillion" },
                new Artist { Name = "Marisa Monte" },
                new Artist { Name = "Martin Roscoe" },
                new Artist { Name = "Maurizio Pollini" },
                new Artist { Name = "Mela Tenenbaum, Pro Musica Prague & Richard Kapp" },
                new Artist { Name = "Men At Work" },
                new Artist { Name = "Metallica" },
                new Artist { Name = "Michael Tilson Thomas & San Francisco Symphony" },
                new Artist { Name = "Miles Davis" },
                new Artist { Name = "Milton Nascimento" },
                new Artist { Name = "Mötley Crüe" },
                new Artist { Name = "Motörhead" },
                new Artist { Name = "Nash Ensemble" },
                new Artist { Name = "Nicolaus Esterhazy Sinfonia" },
                new Artist { Name = "Nirvana" },
                new Artist { Name = "O Terço" },
                new Artist { Name = "Olodum" },
                new Artist { Name = "Orchestra of The Age of Enlightenment" },
                new Artist { Name = "Os Paralamas Do Sucesso" },
                new Artist { Name = "Ozzy Osbourne" },
                new Artist { Name = "Page & Plant" },
                new Artist { Name = "Paul D'Ianno" },
                new Artist { Name = "Pearl Jam" },
                new Artist { Name = "Pink Floyd" },
                new Artist { Name = "Queen" },
                new Artist { Name = "R.E.M." },
                new Artist { Name = "Raul Seixas" },
                new Artist { Name = "Red Hot Chili Peppers" },
                new Artist { Name = "Roger Norrington, London Classical Players" },
                new Artist { Name = "Royal Philharmonic Orchestra & Sir Thomas Beecham" },
                new Artist { Name = "Rush" },
                new Artist { Name = "Santana" },
                new Artist { Name = "Scholars Baroque Ensemble" },
                new Artist { Name = "Scorpions" },
                new Artist { Name = "Sergei Prokofiev & Yuri Temirkanov" },
                new Artist { Name = "Sir Georg Solti & Wiener Philharmoniker" },
                new Artist { Name = "Skank" },
                new Artist { Name = "Soundgarden" },
                new Artist { Name = "Spyro Gyra" },
                new Artist { Name = "Stevie Ray Vaughan & Double Trouble" },
                new Artist { Name = "Stone Temple Pilots" },
                new Artist { Name = "System Of A Down" },
                new Artist { Name = "Temple of the Dog" },
                new Artist { Name = "Terry Bozzio, Tony Levin & Steve Stevens" },
                new Artist { Name = "The 12 Cellists of The Berlin Philharmonic" },
                new Artist { Name = "The Black Crowes" },
                new Artist { Name = "The Cult" },
                new Artist { Name = "The Doors" },
                new Artist { Name = "The King's Singers" },
                new Artist { Name = "The Police" },
                new Artist { Name = "The Posies" },
                new Artist { Name = "The Rolling Stones" },
                new Artist { Name = "The Who" },
                new Artist { Name = "Tim Maia" },
                new Artist { Name = "Ton Koopman" },
                new Artist { Name = "U2" },
                new Artist { Name = "UB40" },
                new Artist { Name = "Van Halen" },
                new Artist { Name = "Various Artists" },
                new Artist { Name = "Velvet Revolver" },
                new Artist { Name = "Vinícius De Moraes" },
                new Artist { Name = "Wilhelm Kempff" },
                new Artist { Name = "Yehudi Menuhin" },
                new Artist { Name = "Yo-Yo Ma" },
                new Artist { Name = "Zeca Pagodinho" }
            };

            int i = 0;
            Artists.ForEach(x => x.ArtistId = ++i);

            Albums = new List<Album>
                {
                    new Album {Title = "The Best Of Men At Work", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Men At Work"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "A Copland Celebration, Vol. I", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Aaron Copland & London Symphony Orchestra"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Worlds", Genre = Genres.First(g => g.Name == "Jazz"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Aaron Goldberg"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "For Those About To Rock We Salute You", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "AC/DC"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Let There Be Rock", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "AC/DC"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Balls to the Wall", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Accept"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Restless and Wild", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Accept"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Górecki: Symphony No. 3", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Adrian Leaper & Doreen de Feis"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Big Ones", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Aerosmith"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Quiet Songs", Genre = Genres.First(g => g.Name == "Jazz"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Aisha Duo"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Jagged Little Pill", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Alanis Morissette"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Facelift", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Alice In Chains"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Frank", Genre = Genres.First(g => g.Name == "Pop"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Amy Winehouse"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Ring My Bell", Genre = Genres.First(g => g.Name == "Disco"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Anita Ward"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Chill: Brazil (Disc 2)", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Antônio Carlos Jobim"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Warner 25 Anos", Genre = Genres.First(g => g.Name == "Jazz"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Antônio Carlos Jobim"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Plays Metallica By Four Cellos", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Apocalyptica"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Revelations", Genre = Genres.First(g => g.Name == "Alternative"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Audioslave"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Audioslave", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Audioslave"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Last Night of the Proms", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Barry Wordsworth & BBC Concert Orchestra"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Sibelius: Finlandia", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Berliner Philharmoniker & Hans Rosbaud"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Mozart: Symphonies Nos. 40 & 41", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Berliner Philharmoniker & Herbert Von Karajan"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Best Of Billy Cobham", Genre = Genres.First(g => g.Name == "Jazz"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Billy Cobham"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Alcohol Fueled Brewtality Live! [Disc 1]", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Black Label Society"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Alcohol Fueled Brewtality Live! [Disc 2]", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Black Label Society"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Black Sabbath Vol. 4 (Remaster)", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Black Sabbath"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Black Sabbath", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Black Sabbath"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Carmina Burana", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Boston Symphony Orchestra & Seiji Ozawa"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "A Soprano Inspired", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Britten Sinfonia, Ivor Bolton & Lesley Garrett"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Chemical Wedding", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Bruce Dickinson"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Prenda Minha", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Caetano Veloso"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Sozinho Remix Ao Vivo", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Caetano Veloso"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Cake: B-Sides and Rarities", Genre = Genres.First(g => g.Name == "Alternative"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Cake"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Carried to Dust (Bonus Track Version)", Genre = Genres.First(g => g.Name == "Alternative"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Calexico"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Cássia Eller - Sem Limite [Disc 1]", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Cássia Eller"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Le Freak", Genre = Genres.First(g => g.Name == "Disco"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Chic"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Scheherazade", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Chicago Symphony Orchestra & Fritz Reiner"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Minha Historia", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Chico Buarque"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Afrociberdelia", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Chico Science & Nação Zumbi"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Da Lama Ao Caos", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Chico Science & Nação Zumbi"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Carry On", Genre = Genres.First(g => g.Name == "Alternative"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Chris Cornell"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "SCRIABIN: Vers la flamme", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Christopher O'Riley"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Acústico MTV [Live]", Genre = Genres.First(g => g.Name == "Reggae"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Cidade Negra"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Cidade Negra - Hits", Genre = Genres.First(g => g.Name == "Reggae"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Cidade Negra"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Na Pista", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Cláudio Zoli"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Chronicle, Vol. 1", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Creedence Clearwater Revival"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Chronicle, Vol. 2", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Creedence Clearwater Revival"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Into The Light", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "David Coverdale"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Come Taste The Band", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Deep Purple"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Deep Purple In Rock", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Deep Purple"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Fireball", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Deep Purple"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Machine Head", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Deep Purple"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "MK III The Final Concerts [Disc 1]", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Deep Purple"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Purpendicular", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Deep Purple"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Slaves And Masters", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Deep Purple"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Stormbringer", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Deep Purple"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Battle Rages On", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Deep Purple"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Final Concerts (Disc 2)", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Deep Purple"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Outbreak", Genre = Genres.First(g => g.Name == "Jazz"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Dennis Chambers"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Djavan Ao Vivo - Vol. 02", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Djavan"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Djavan Ao Vivo - Vol. 1", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Djavan"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "MacArthur Park Suite", Genre = Genres.First(g => g.Name == "Disco"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Donna Summer"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Un-Led-Ed", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Dread Zeppelin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Best of Ed Motta", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Ed Motta"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Adams, John: The Chairman Dances", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Edo de Waart & San Francisco Symphony"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Elis Regina-Minha História", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Elis Regina"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Pachelbel: Canon & Gigue", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "English Concert & Trevor Pinnock"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Unplugged", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Eric Clapton"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Cream Of Clapton", Genre = Genres.First(g => g.Name == "Blues"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Eric Clapton"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Unplugged", Genre = Genres.First(g => g.Name == "Blues"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Eric Clapton"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Respighi:Pines of Rome", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Eugene Ormandy"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Strauss: Waltzes", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Eugene Ormandy"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "King For A Day Fool For A Lifetime", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Faith No More"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Deixa Entrar", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Falamansa"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "In Your Honor [Disc 1]", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Foo Fighters"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "In Your Honor [Disc 2]", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Foo Fighters"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Colour And The Shape", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Foo Fighters"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Bongo Fury", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Frank Zappa & Captain Beefheart"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Roda De Funk", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Funk Como Le Gusta"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Quanta Gente Veio Ver (Live)", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Gilberto Gil"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Quanta Gente Veio ver--Bônus De Carnaval", Genre = Genres.First(g => g.Name == "Jazz"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Gilberto Gil"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Faceless", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Godsmack"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Meus Momentos", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Gonzaguinha"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Nielsen: The Six Symphonies", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Göteborgs Symfoniker & Neeme Järvi"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Appetite for Destruction", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Guns N' Roses"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Use Your Illusion I", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Guns N' Roses"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Use Your Illusion II", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Guns N' Roses"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Blue Moods", Genre = Genres.First(g => g.Name == "Jazz"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Incognito"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "A Matter of Life and Death", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Brave New World", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Fear Of The Dark", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Live At Donington 1992 (Disc 1)", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Live At Donington 1992 (Disc 2)", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Rock In Rio [CD2]", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Number of The Beast", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The X Factor", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Virtual XI", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "A Real Dead One", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "A Real Live One", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Live After Death", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "No Prayer For The Dying", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Piece Of Mind", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Powerslave", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Rock In Rio [CD1]", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Rock In Rio [CD2]", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Seventh Son of a Seventh Son", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Somewhere in Time", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Number of The Beast", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Iron Maiden", Genre = Genres.First(g => g.Name == "Blues"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Iron Maiden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Mascagni: Cavalleria Rusticana", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "James Levine"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Emergency On Planet Earth", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Jamiroquai"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Are You Experienced?", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Jimi Hendrix"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Surfing with the Alien (Remastered)", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Joe Satriani"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Jorge Ben Jor 25 Anos", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Jorge Ben"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Jota Quest-1995", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Jota Quest"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Living After Midnight", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Judas Priest"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Weill: The Seven Deadly Sins", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Kent Nagano and Orchestre de l'Opéra de Lyon"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Greatest Kiss", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Kiss"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Unplugged [Live]", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Kiss"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "BBC Sessions [Disc 1] [Live]", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Led Zeppelin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "BBC Sessions [Disc 2] [Live]", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Led Zeppelin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Coda", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Led Zeppelin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Houses Of The Holy", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Led Zeppelin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "In Through The Out Door", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Led Zeppelin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "IV", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Led Zeppelin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Led Zeppelin I", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Led Zeppelin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Led Zeppelin II", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Led Zeppelin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Led Zeppelin III", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Led Zeppelin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Physical Graffiti [Disc 1]", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Led Zeppelin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Physical Graffiti [Disc 2]", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Led Zeppelin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Presence", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Led Zeppelin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Song Remains The Same (Disc 1)", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Led Zeppelin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Song Remains The Same (Disc 2)", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Led Zeppelin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Mais Do Mesmo", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Legião Urbana"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Greatest Hits", Genre = Genres.First(g => g.Name == "Reggae"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Lenny Kravitz"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Greatest Hits", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Lenny Kravitz"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Greatest Hits", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Lenny Kravitz"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Tchaikovsky: The Nutcracker", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "London Symphony Orchestra & Sir Charles Mackerras"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Duos II", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Luciana Souza/Romero Lubambo"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Chill: Brazil (Disc 1)", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Marcos Valle"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Misplaced Childhood", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Marillion"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Barulhinho Bom", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Marisa Monte"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Szymanowski: Piano Works, Vol. 1", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Martin Roscoe"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "...And Justice For All", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Metallica"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Black Album", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Metallica"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Garage Inc. (Disc 1)", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Metallica"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Garage Inc. (Disc 2)", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Metallica"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Load", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Metallica"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Master Of Puppets", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Metallica"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "ReLoad", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Metallica"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Ride The Lightning", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Metallica"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "St. Anger", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Metallica"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Berlioz: Symphonie Fantastique", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Michael Tilson Thomas & San Francisco Symphony"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Prokofiev: Romeo & Juliet", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Michael Tilson Thomas & San Francisco Symphony"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Miles Ahead", Genre = Genres.First(g => g.Name == "Jazz"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Miles Davis"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Essential Miles Davis [Disc 1]", Genre = Genres.First(g => g.Name == "Jazz"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Miles Davis"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Essential Miles Davis [Disc 2]", Genre = Genres.First(g => g.Name == "Jazz"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Miles Davis"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Milton Nascimento Ao Vivo", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Milton Nascimento"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Minas", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Milton Nascimento"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Motley Crue Greatest Hits", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Mötley Crüe"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Ace Of Spades", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Motörhead"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Mozart: Chamber Music", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Nash Ensemble"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Best of Beethoven", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Nicolaus Esterhazy Sinfonia"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Nevermind", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Nirvana"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Compositores", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "O Terço"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Olodum", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Olodum"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Bach: The Brandenburg Concertos", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Orchestra of The Age of Enlightenment"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Acústico MTV", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Os Paralamas Do Sucesso"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Arquivo II", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Os Paralamas Do Sucesso"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Arquivo Os Paralamas Do Sucesso", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Os Paralamas Do Sucesso"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Tribute", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Ozzy Osbourne"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Bark at the Moon (Remastered)", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Ozzy Osbourne"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Blizzard of Ozz", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Ozzy Osbourne"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Diary of a Madman (Remastered)", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Ozzy Osbourne"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "No More Tears (Remastered)", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Ozzy Osbourne"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Speak of the Devil", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Ozzy Osbourne"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Walking Into Clarksdale", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Page & Plant"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Beast Live", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Paul D'Ianno"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Live On Two Legs [Live]", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Pearl Jam"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Riot Act", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Pearl Jam"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Ten", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Pearl Jam"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Vs.", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Pearl Jam"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Dark Side Of The Moon", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Pink Floyd"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Greatest Hits I", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Queen"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Greatest Hits II", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Queen"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "News Of The World", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Queen"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "New Adventures In Hi-Fi", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "R.E.M."), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Raul Seixas", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Raul Seixas"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "By The Way", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Red Hot Chili Peppers"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Californication", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Red Hot Chili Peppers"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Purcell: The Fairy Queen", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Roger Norrington, London Classical Players"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Haydn: Symphonies 99 - 104", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Royal Philharmonic Orchestra & Sir Thomas Beecham"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Retrospective I (1974-1980)", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Rush"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Santana - As Years Go By", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Santana"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Santana Live", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Santana"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Supernatural", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Santana"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Handel: The Messiah (Highlights)", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Scholars Baroque Ensemble"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Prokofiev: Symphony No.1", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Sergei Prokofiev & Yuri Temirkanov"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Wagner: Favourite Overtures", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Sir Georg Solti & Wiener Philharmoniker"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Maquinarama", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Skank"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "O Samba Poconé", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Skank"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "A-Sides", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Soundgarden"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Heart of the Night", Genre = Genres.First(g => g.Name == "Jazz"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Spyro Gyra"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Morning Dance", Genre = Genres.First(g => g.Name == "Jazz"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Spyro Gyra"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "In Step", Genre = Genres.First(g => g.Name == "Blues"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Stevie Ray Vaughan & Double Trouble"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Core", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Stone Temple Pilots"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Mezmerize", Genre = Genres.First(g => g.Name == "Metal"), Price = 8.99M, Artist = Artists.First(a => a.Name == "System Of A Down"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Temple of the Dog", Genre = Genres.First(g => g.Name == "Alternative"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Temple of the Dog"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "[1997] Black Light Syndrome", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Terry Bozzio, Tony Levin & Steve Stevens"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "South American Getaway", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "The 12 Cellists of The Berlin Philharmonic"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Live [Disc 1]", Genre = Genres.First(g => g.Name == "Blues"), Price = 8.99M, Artist = Artists.First(a => a.Name == "The Black Crowes"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Live [Disc 2]", Genre = Genres.First(g => g.Name == "Blues"), Price = 8.99M, Artist = Artists.First(a => a.Name == "The Black Crowes"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Beyond Good And Evil", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "The Cult"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Doors", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "The Doors"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "English Renaissance", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "The King's Singers"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Police Greatest Hits", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "The Police"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Every Kind of Light", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "The Posies"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Hot Rocks, 1964-1971 (Disc 1)", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "The Rolling Stones"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "No Security", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "The Rolling Stones"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Voodoo Lounge", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "The Rolling Stones"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "My Generation - The Very Best Of The Who", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "The Who"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Serie Sem Limite (Disc 1)", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Tim Maia"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Serie Sem Limite (Disc 2)", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Tim Maia"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Bach: Toccata & Fugue in D Minor", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Ton Koopman"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Achtung Baby", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "U2"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "B-Sides 1980-1990", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "U2"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "How To Dismantle An Atomic Bomb", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "U2"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Pop", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "U2"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Rattle And Hum", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "U2"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Best Of 1980-1990", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "U2"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "War", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "U2"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Zooropa", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "U2"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "UB40 The Best Of - Volume Two [UK]", Genre = Genres.First(g => g.Name == "Reggae"), Price = 8.99M, Artist = Artists.First(a => a.Name == "UB40"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Diver Down", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Van Halen"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "The Best Of Van Halen, Vol. I", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Van Halen"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Van Halen III", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Van Halen"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Van Halen", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Van Halen"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Axé Bahia 2001", Genre = Genres.First(g => g.Name == "Pop"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Various Artists"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Sambas De Enredo 2001", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Various Artists"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Vozes do MPB", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Various Artists"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Contraband", Genre = Genres.First(g => g.Name == "Rock"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Velvet Revolver"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Vinicius De Moraes", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Vinícius De Moraes"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Bach: Goldberg Variations", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Wilhelm Kempff"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Bartok: Violin & Viola Concertos", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Yehudi Menuhin"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Bach: The Cello Suites", Genre = Genres.First(g => g.Name == "Classical"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Yo-Yo Ma"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                    new Album {Title = "Ao Vivo [IMPORT]", Genre = Genres.First(g => g.Name == "Latin"), Price = 8.99M, Artist = Artists.First(a => a.Name == "Zeca Pagodinho"), AlbumArtUrl = "/Content/Images/placeholder.gif"},
                };


            var r = new Random(Environment.TickCount);
            Albums.ForEach(x => x.Price = Convert.ToDecimal(r.NextDouble() * 20d));

            var albumsGroupedByArtist = Albums.GroupBy(a => a.Artist);
            foreach (var grouping in albumsGroupedByArtist)
            {
                grouping.Key.Albums = grouping.ToList();
            }
        }
    }
}