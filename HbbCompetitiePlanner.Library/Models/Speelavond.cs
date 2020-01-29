using ReflectionIT.Universal.Helpers;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HbbCompetitiePlanner.Library.Models {

    public class Speelavond : ObservableObject {

        public DateTime Datum { get; set; }
        public int AantalBanen { get; set; }

        public bool IsTweedeDeel { get; set; }

        //[JsonIgnore]
        //public DayOfWeek Weekdag => Datum.DayOfWeek;

        /// <summary>
        /// Key = BaanNummer
        /// </summary>
        public Dictionary<int, Wedstrijd> Wedstrijden { get; } = new Dictionary<int, Wedstrijd>();

    }
}
