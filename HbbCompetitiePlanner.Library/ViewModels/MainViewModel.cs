﻿using HbbCompetitiePlanner.Library.Models;
using Newtonsoft.Json;
using ReflectionIT.Universal.Helpers;
using System;
using System.Windows.Input;

namespace HbbCompetitiePlanner.Library.ViewModels {

    public class MainViewModel : ObservableObject {

        public static MainViewModel Current { get; } = new MainViewModel();

        public ICommand StartCommand { get; }

        public Competitie? Competitie { get; private set; }
        public string? Input { get; set; }
        public string? Output { get; private set; }

        private MainViewModel() {
            this.StartCommand = new RelayCommand(OnStart);
        }

        private void OnStart() {
            if (string.IsNullOrEmpty(Input)) {
                CreateSampleCompetitie();
            } else {
                this.Competitie = JsonConvert.DeserializeObject<Competitie>(Input);
            }

            if (this.Competitie != null) {
                this.Competitie.CreateSpeelrondes();
                this.Competitie.VerdeelWedstrijdenOverSpeelavonden(false);
                this.Competitie.VerdeelWedstrijdenOverSpeelavonden(true);

                this.Competitie.Sorteren();

                this.Output = JsonConvert.SerializeObject(this.Competitie, Formatting.Indented);
                OnPropertyChanged(nameof(Output));
                OnPropertyChanged(nameof(Competitie));
            }
            // TODO: https://bitbucket.org/rasmuszimmer/wpf-jsonviewer-usercontrol/src/master/
        }

        private void CreateSampleCompetitie() {

            this.Competitie = new Competitie {
                Naam = "2019-2020"
            };

            // Speelavonden
            for (int i = 0; i < 14; i++) {
                // Woensdag
                this.Competitie.Speelavonden.Add(new Speelavond {
                    Datum = new DateTime(2020, 9, 16).AddDays(i * 7),
                    AantalBanen = 11
                });

                // Donderdag
                this.Competitie.Speelavonden.Add(new Speelavond {
                    Datum = new DateTime(2020, 9, 17).AddDays(i * 7),
                    AantalBanen = 12
                });
            }

            for (int i = 0; i < 14; i++) {
                // Woensdag
                this.Competitie.Speelavonden.Add(new Speelavond {
                    Datum = new DateTime(2021, 1, 13).AddDays(i * 7),
                    AantalBanen = 13,
                    IsTweedeDeel = true,
                });

                // Donderdag
                this.Competitie.Speelavonden.Add(new Speelavond {
                    Datum = new DateTime(2021, 1, 14).AddDays(i * 7),
                    AantalBanen = 11,
                    IsTweedeDeel = true,
                });
            }

            int teamNr = 1;

            // Pouls
            for (int i = 1; i < 8; i++) {
                var p = new Poul {
                    Naam = $"Klasse {i}",
                    Nummer = i,
                    IsHalveCompetitie = (i == 1),
                };
                this.Competitie.Pouls.Add(p);

                for (int t = 1; t <= (i < 3 ? 7 : 8); t++) {
                    p.Teams.Add(new Team() {
                        Naam = $"Team {t}.{p.Nummer}",
                        Nr = teamNr++,
                        TrainingsAvond = t < 3 ? DayOfWeek.Tuesday : DayOfWeek.Wednesday,
                        VoorkeursAvond = (t % 2 == 0) ? DayOfWeek.Wednesday : DayOfWeek.Thursday,
                    });
                }
            }

            this.Input = JsonConvert.SerializeObject(this.Competitie, Formatting.Indented);
            OnPropertyChanged(nameof(Input));
        }
    }
}
