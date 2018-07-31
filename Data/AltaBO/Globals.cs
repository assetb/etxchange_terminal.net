using System;
using System.Collections.Generic;

namespace AltaBO
{
    public static class Globals
    {
        public static int CurrentYear = DateTime.Today.Year;

        public static Dictionary<HolydayEnum, DateTime> Holydays = new Dictionary<HolydayEnum, DateTime>()
        {
            {HolydayEnum.NewYear, new DateTime(DateTime.Now.Year,1,1) },
            {HolydayEnum.NewYearContinium, new DateTime(DateTime.Now.Year,1,2) },
            {HolydayEnum.MerryChristmas, new DateTime(DateTime.Now.Year,1,7) },
            {HolydayEnum.WomansDay, new DateTime(DateTime.Now.Year,3,8) },
            {HolydayEnum.KazNewYear, new DateTime(DateTime.Now.Year,3,21) },
            {HolydayEnum.KazNewYearSecondDay, new DateTime(DateTime.Now.Year,3,22) },
            {HolydayEnum.KazNewYearThirdDay, new DateTime(DateTime.Now.Year,3,23) },
            {HolydayEnum.UnityDay, new DateTime(DateTime.Now.Year,5,1) },
            {HolydayEnum.VictoryDayZero, new DateTime(DateTime.Now.Year,5,8) },
            {HolydayEnum.VictoryDay, new DateTime(DateTime.Now.Year,5,9) },
            {HolydayEnum.CapitalDay, new DateTime(DateTime.Now.Year,7,6) },
            {HolydayEnum.CapitalDay2, new DateTime(DateTime.Now.Year,7,7) },
            {HolydayEnum.ConstitutionDay, new DateTime(DateTime.Now.Year,8,30) },
            {HolydayEnum.IndependentDay, new DateTime(DateTime.Now.Year,12,16) },
            {HolydayEnum.IndependentDayContinium, new DateTime(DateTime.Now.Year,12,19) },
        };
    }
}
