using ReflectionIT.Universal.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HbbCompetitiePlanner.Library.Models {

    public class Competitie : ObservableObject {

        public string Naam { get; set; }

        public List<Speelavond> Speelavonden { get; } = new List<Speelavond>();
        public List<Poul> Pouls { get; } = new List<Poul>();

        internal void VerdeelWedstrijdenOverSpeelavonden(bool isTweedeDeel) {
            var wedstrijden = this.Pouls.SelectMany(p => p.Speelrondes.SelectMany(r => r.Wedstrijden).Where(w => w.Speelronde.IsTweedeDeel == isTweedeDeel)).OrderBy(w => w.Speelronde.Nummer).ThenBy(w => w.Poul.Nummer).ToList();
            var weken = this.Speelavonden.Where(a => a.IsTweedeDeel == isTweedeDeel).ToLookup(a => a.Datum.GetWeekNumber());

            //var totaalAantalBanen = this.Speelavonden.Sum(a => a.IsTweedeDeel == isTweedeDeel ? a.AantalBanen : 0);
            //var gemiddeldAantalBanenPerWeek = Math.Ceiling((double)totaalAantalBanen / weken.Count());

            foreach (var week in weken) {
                int nr = 0;
                var avonden = week.ToDictionary(a => a.Datum.DayOfWeek);

                while (avonden.Count > 0 && nr < wedstrijden.Count) {
                    var wedstrijd = wedstrijden[nr++];

                    if (wedstrijd.Speelavond is null && avonden.ContainsKey(wedstrijd.Team1.VoorkeursAvond)) {
                        var avond = avonden[wedstrijd.Team1.VoorkeursAvond];
                        var baanNr = avond.Wedstrijden.Count + 1;
                        wedstrijd.Speelavond = avond;
                        wedstrijd.BaanNummer = baanNr;
                        avond.Wedstrijden.Add(baanNr, wedstrijd);
                        if (avond.Wedstrijden.Count == avond.AantalBanen) {
                            avonden.Remove(avond.Datum.DayOfWeek);
                        }
                    }
                }

            }
        }
    }
}
