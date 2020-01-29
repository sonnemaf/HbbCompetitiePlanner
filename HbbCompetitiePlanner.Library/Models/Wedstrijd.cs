using Newtonsoft.Json;
using ReflectionIT.Universal.Helpers;
using System;

namespace HbbCompetitiePlanner.Library.Models {

    public class Wedstrijd : ObservableObject {

        public int Nummer { get; }

        public Team Team1 { get; }

        public Team Team2 { get; }

        [JsonIgnore]
        public Poul Poul { get; }
        
        [JsonIgnore]
        public Speelronde Speelronde { get; }
        
        [JsonIgnore]
        public Speelavond Speelavond { get; set; }

        [JsonIgnore]
        public int BaanNummer { get; set; }

        public DateTime Speeldatum => Speelavond.Datum;
        public DayOfWeek SpeeldatumDag => Speelavond.Datum.DayOfWeek;

        public Wedstrijd(int nr, Team team1, Team team2, Poul poul, Speelronde speelronde) {
            this.Nummer = nr;
            this.Team1 = team1;
            this.Team2 = team2;
            this.Poul = poul;
            this.Speelronde = speelronde;
        }

        

    }
}
