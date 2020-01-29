using Newtonsoft.Json;
using ReflectionIT.Universal.Helpers;
using System;
using System.Collections.Generic;

namespace HbbCompetitiePlanner.Library.Models {

    public class Team : ObservableObject {

        public string? Naam { get; set; }
        public int Nr { get; set; }
        public DayOfWeek VoorkeursAvond { get; set; }
        public DayOfWeek TrainingsAvond { get; set; }

        [JsonIgnore]
        public List<Wedstrijd> Wedstrijden { get; } = new List<Wedstrijd>();

        public override string? ToString() => Naam;

        [JsonIgnore]
        public int AantalThuisWedstrijden { get; set; } 

    }
}
