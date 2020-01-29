using Newtonsoft.Json;
using ReflectionIT.Universal.Helpers;
using System;

#nullable enable

namespace HbbCompetitiePlanner.Library.Models {

    public class Wedstrijd : ObservableObject, IComparable<Wedstrijd> {

        public int Nummer { get; }

        public Team Team1 { get; set; }

        public Team Team2 { get; set; }

        [JsonIgnore]
        public Poul Poul { get; }
        
        [JsonIgnore]
        public Speelronde Speelronde { get; }
        
        [JsonIgnore]
        public Speelavond? Speelavond { get; set; }

        [JsonIgnore]
        public int BaanNummer { get; set; }

        public DateTime Speeldatum => Speelavond?.Datum ?? new DateTime(1900,1,1);
        public DayOfWeek SpeeldatumDag => Speeldatum.DayOfWeek;

        public Wedstrijd(int nr, Team team1, Team team2, Poul poul, Speelronde speelronde) {
            this.Nummer = nr;
            this.Team1 = team1;
            this.Team2 = team2;
            this.Poul = poul;
            this.Speelronde = speelronde;
        }

        public int CompareTo(Wedstrijd other) {
            return this.Speeldatum.CompareTo(other.Speeldatum);
        }

        public override string ToString() => $"{Speelronde.Nummer} {Nummer} {Team1.Naam} {Team2.Naam}";

    }
}
