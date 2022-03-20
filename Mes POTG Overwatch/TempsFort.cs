using System;

namespace Mes_POTG_Overwatch
{
    public class TempsFort
    {
        public string Titre { get; set; }
        public DateTime Date { get; set; }
        public Héro Héro { get; set; }
        public bool IsPOTG { get; set; }
        public string Path { get; set; }
        public string ImageOfHero { get; set; }
    }

    public enum Héro
    {
        null_,
        Ana,
        Ange,
        Ashe,
        Baptiste,
        Bastion,
        Bouldozer,
        Brigitte,
        Chacal,
        Chopper,
        D_VA,
        Doomfist,
        Echo,
        Fatale,
        Faucheur,
        Genji,
        Hanzo,
        Lucio,
        Mccree,
        Mei,
        Moira,
        Orisa,
        Pharah,
        Reinhardt,
        Sigma,
        Soldat_76,
        Sombra,
        Symettra,
        Torbjorn,
        Tracer,
        Winston,
        Zarya,
        Zenyatta
    }
}