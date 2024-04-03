using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static lifegame.scripts.Life;

namespace lifegame.scripts
{
    internal class Timers
    {
        public void InitializeTimer(LifeGuy lifeG)
        {
            Timer timer = new Timer();
            timer.Interval = (int)GlobalVars.Instance.interval;
            timer.Tick += (sender, e) => UpdateTimer(sender, e, lifeG, timer);
            timer.Start();
        }
        private void UpdateTimer(object sender,EventArgs e, LifeGuy lifeG, Timer t )
        {
            if (GlobalVars.Instance.pause)
            {
                return;
            }
            if(!GlobalVars.Instance.grid)
            {
                lifeG.Movement();
                lifeG.BorderColliders();
                lifeG.GuyColliders();
            }
            else
            {
                lifeG.GridMovement();
                lifeG.GridBorderColliders();
                lifeG.GuyColliders();
            }
            if (lifeG == null) { t.Stop(); }
        }
    }
}
