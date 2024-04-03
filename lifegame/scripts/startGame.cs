using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static lifegame.scripts.Life;

namespace lifegame.scripts
{
    internal class startGame
    {
        Processors processors = new Processors();
        public void Start(mainForm form)
        {
            createLifeGuy(GlobalVars.Instance.initquantity, form);
        }
        private void createLifeGuy(int initquantity, mainForm mainform)
        {
            for (int quantity = 0; quantity < initquantity; quantity++)
            {
                processors.ModifyLife(mainform);
            }
        }
        public void ClearGameElements(mainForm form)
        {
            GlobalVars.Instance.ResetVariables();

            List<Control> controlsToRemove = new List<Control>();

            foreach (Control control in form.Controls)
            {
                if (control is LifeGuy || control is Button)
                {
                    controlsToRemove.Add(control);
                }
            }
            foreach (Control control in controlsToRemove)
            {
                form.Controls.Remove(control);
            }
        }


    }
}
