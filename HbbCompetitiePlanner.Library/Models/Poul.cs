using Newtonsoft.Json;
using ReflectionIT.Universal.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace HbbCompetitiePlanner.Library.Models {

    public class Poul : ObservableObject {

        public string? Naam { get; set; }
        public int Nummer { get; set; }
        public bool IsHalveCompetitie { get; set; }
        public List<Team> Teams { get; } = new List<Team>();
        public List<Speelronde> Speelrondes { get; } = new List<Speelronde>();

        public int CreateSpeelrondes(int nr) {
            // Source: https://stackoverflow.com/questions/1293058/round-robin-tournament-algorithm-in-c-sharp/1293174#1293174

            this.Speelrondes.Clear();

            List<Team?> listTeam = new List<Team?>(this.Teams); // Copy
            if (listTeam.Count % 2 != 0) {
                listTeam.Add(null);
            }
            var numTeams = listTeam.Count;

            int numDays = (numTeams - 1);
            int halfSize = numTeams / 2;

            List<Team> teams = new List<Team>();

            teams.AddRange(listTeam.Skip(halfSize).Take(halfSize));
            teams.AddRange(listTeam.Skip(1).Take(halfSize - 1).ToArray().Reverse());

            int teamsSize = teams.Count;

            for (int day = 0; day < numDays; day++) {

                var ronde1 = new Speelronde() {
                    Nummer = day + 1,
                };
                var ronde2 = new Speelronde() {
                    Nummer = ronde1.Nummer + 1000,
                };

                this.Speelrondes.Add(ronde1);
                if (!IsHalveCompetitie) {
                    this.Speelrondes.Add(ronde2);
                }

                int teamIdx = day % teamsSize;
                var secondT = listTeam[0];
                if (teams[teamIdx] is object && secondT is object) {
                    ronde1.AddWedstrijd(new Wedstrijd(nr, teams[teamIdx], secondT, this, ronde1), true);
                    ronde2.AddWedstrijd(new Wedstrijd(1000 + nr++, ronde1.Wedstrijden.Last().Team2, ronde1.Wedstrijden.Last().Team1, this, ronde2), false);
                }

                for (int idx = 1; idx < halfSize; idx++) {
                    int firstTeam = (day + idx) % teamsSize;
                    int secondTeam = (day + teamsSize - idx) % teamsSize;
                    if (teams[firstTeam] is object && teams[secondTeam] is object) {
                        ronde1.AddWedstrijd(new Wedstrijd(nr, teams[firstTeam], teams[secondTeam], this, ronde1), true);
                        ronde2.AddWedstrijd(new Wedstrijd(1000 + nr++, ronde1.Wedstrijden.Last().Team2, ronde1.Wedstrijden.Last().Team1, this, ronde2), false);
                    }
                }

            }
            this.Speelrondes.Sort();

            return nr;
        }

        [JsonIgnore]
        public IEnumerable<Wedstrijd> Wedstrijden {
            get {
                return this.Speelrondes.SelectMany(s => s.Wedstrijden);
            }
        }

        [JsonIgnore]
        public string TeamNamen => string.Join(", ", Teams.Select(t => t.NaamAndSpeelavond));

    }
}
