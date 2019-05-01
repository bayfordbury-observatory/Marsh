using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AstroLib;

namespace MarshControl {

    public static class Globals {

        public static bool testing = false;
        public static bool SunUp = true;

        public static string targetname = "None selected";
        public static string targettype = "-";
        public static double targetRA = 0;
        public static double targetDec = 0;

        public static EquatorialCoords Target = new EquatorialCoords(0,0);
        public static EquatorialCoords Scope= new EquatorialCoords(0,0);

        public static HorizontalCoords ScopeHor = new HorizontalCoords(180, 0);
        public static HorizontalCoords SunHor = new HorizontalCoords(180, 0);

        public static Location loc = new Location(51.774849.ToRad(), 0.095656.ToRad(), 60);

        public static double targetRAnow = 0;
        public static double targetDecnow = 0;

        public static EquatorialCoords TargetNow = new EquatorialCoords(0, 0);

        //public static double RA = 0;
        public static double HA = 0;

        public static double HAdeg = 0;
        public static double DECdeg = 0;
        public static bool reloadSettings = true;

        public static int LoadProgress = 0;
        public static string Status = "Opening load window";
    }

}
