using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static lifegame.scripts.Life;

namespace lifegame.scripts
{
    internal class GlobalVars
    {
        private static GlobalVars instance;

        public static GlobalVars Instance
        {
            get
            { 
                if (instance == null) { instance = new GlobalVars(); }
                return instance;
            }
        }
        public void ResetVariables()
        {
            Life_List.Clear();
            foreach (var timer in LifeTimers.Values)
            {
                timer.Stop();
                timer.Dispose();
            }
            LifeTimers.Clear();
        }

        //internal
        public Random random = new Random();
        public string guycase = "default";
        public float interval = 24;
        public bool clonated = false;
        public List<LifeGuy> Life_List = new List<LifeGuy>();
        public Dictionary<Button,Timer> LifeTimers = new Dictionary<Button,Timer>();

        //user
        public Keys pauseKey = Keys.Escape;

        public bool grid = false;
        public bool pause = false;

        public int initquantity = 6;
        public int maxLife = 100;
        public int minLife = 2;
        public float constVelocity = 5;
        public int lifeSize = 50;
    }
}
