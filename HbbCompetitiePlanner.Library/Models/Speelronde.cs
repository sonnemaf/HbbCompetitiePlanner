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

        internal void AddWedstrijd(Wedstrijd wedstrijd, bool wisselUitThuis) {
            if (wedstrijd.Poul.IsHalveCompetitie && wedstrijd.Speelronde.IsTweedeDeel) {
                return;
            }

            if (wisselUitThuis) {
                if (wedstrijd.Team1.AantalThuisWedstrijden > wedstrijd.Team2.AantalThuisWedstrijden) {
                    //if (Extensions.Random.Next(1, 1000) > 500) {
                    (wedstrijd.Team1, wedstrijd.Team2) = (wedstrijd.Team2, wedstrijd.Team1);
                }
                wedstrijd.Team1.AantalThuisWedstrijden++;
            }

            this.Wedstrijden.Add(wedstrijd);
            wedstrijd.Team1.Wedstrijden.Add(wedstrijd);
            wedstrijd.Team2.Wedstrijden.Add(wedstrijd);
        }
    }
}

