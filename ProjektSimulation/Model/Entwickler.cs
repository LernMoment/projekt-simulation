﻿using Gurock.SmartInspect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektSimulation.Model
{
    public enum Beschaeftigung
    {
        Lernen,
        Planen,
        Entwickeln
    }

    public class Entwickler : NotifyPropertyChangeModelBase
    {
        public string Name { get; }

        private Beschaeftigung aktuelleAufgabe;
        public Beschaeftigung AktuelleAufgabe
        {
            get { return aktuelleAufgabe; }
            private set
            {
                if (value != aktuelleAufgabe)
                {
                    aktuelleAufgabe = value;
                    RaisePropertyChanged("AktuelleAufgabe");
                }
            }
        }

        public Entwickler(string name)
        {
            Name = name;
            AktuelleAufgabe = Beschaeftigung.Lernen;
        }

        public async Task Arbeiten(IEnumerable<Projekt> projekte)
        {
            foreach (Projekt projekt in projekte)
            {
                if (!projekt.IstInBearbeitung)
                {
                    projekt.IstInBearbeitung = true;
                    await Planen(projekt);
                    await Entwickeln(projekt);
                    projekt.IstInBearbeitung = false;
                }
            }
        }

        private async Task Planen(Projekt projekt)
        {
            SiAuto.Main.EnterThread("Entwickler.Planen-Task");

            if ((projekt.Status == ProjektStatus.Definition) 
                || (projekt.Status == ProjektStatus.Test))
            {
                projekt.Status = ProjektStatus.Planung;
                projekt.ZuletztAktiverEntwickler = Name;
                AktuelleAufgabe = Beschaeftigung.Planen;

                // Eigentlich kann der Entwickler gar nicht planen ;-)
                await Task.Delay(TimeSpan.FromSeconds(5));

                SiAuto.Main.LogObject("Entwickler " + Name + " hat das folgende Projekt geplant", projekt);
                AktuelleAufgabe = Beschaeftigung.Lernen;
            }

            SiAuto.Main.LeaveThread("Entwickler.Planen-Task");
        }

        private async Task Entwickeln(Projekt projekt)
        {
            SiAuto.Main.EnterThread("Entwickler.Entwickeln-Task");

            if (projekt.Status == ProjektStatus.Planung)
            {
                projekt.Status = ProjektStatus.Entwicklung;
                AktuelleAufgabe = Beschaeftigung.Entwickeln;
                projekt.ZuletztAktiverEntwickler = Name;

                await Task.Delay(TimeSpan.FromSeconds(15));

                SiAuto.Main.LogObject("Entwickler " + Name + " hat das folgende Projekt entwickelt", projekt);
                AktuelleAufgabe = Beschaeftigung.Lernen;
            }

            SiAuto.Main.LeaveThread("Entwickler.Entwickeln-Task");
        }
    }
}
