using ReflectionIT.Universal.Helpers;
using System;
using System.Collections.Generic;

namespace HbbCompetitiePlanner.Library.Models {

    public class Speelronde : ObservableObject, IComparable<Speelronde> {

        public int Nummer { get; set; }
        public bool IsTweedeDeel => Nummer >= 1000;
        public List<Wedstrijd> Wedstrijden { get; } = new List<Wedstrijd>();

        public int CompareTo(Speelronde other) {
            return this.Nummer.CompareTo(other.Nummer);
        }
    }
}
