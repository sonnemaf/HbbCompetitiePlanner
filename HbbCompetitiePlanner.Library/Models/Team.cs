using ReflectionIT.Universal.Helpers;
using System;

namespace HbbCompetitiePlanner.Library.Models {

    public class Team : ObservableObject {

        public string Naam { get; set; }
        public int Nr { get; set; }
        public DayOfWeek VoorkeursAvond { get; set; }
        public DayOfWeek TrainingsAvond { get; set; }

    }
}
