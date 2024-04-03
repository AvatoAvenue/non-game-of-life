using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static lifegame.scripts.Life;

namespace lifegame.scripts
{
    internal class Processors
    {
        Timers timers = new Timers();
        public Color SetColor(int number)
        {
            Color color;

            switch (GlobalVars.Instance.guycase)
            {
                case "default":
                case "Exact":
                    color = number % 2 == 0 ? Color.LightSkyBlue : Color.Salmon;
                    break;
                case "par":
                    color = Color.LightSkyBlue;
                    break;
                case "impar":
                    color = Color.Salmon;
                    break;
                default:
                    color = Color.Gray;
                    break;
            }

            return color;
        }
        public string RandomNumber(LifeGuy LifeG)
        {
            int value = 1;
            switch (GlobalVars.Instance.guycase)
            {
                case "default":
                    default:
                    value = GlobalVars.Instance.random.Next(1, 9);
                break;

                case "par":
                    value = GlobalVars.Instance.random.Next(1, 5) * 2;
                    break;

                case "impar":
                    value = GlobalVars.Instance.random.Next(1, 5) * 2 - 1;
                    break;
            }
            
            return value.ToString();
        }
        public void SetDirection(LifeGuy guy)
        {
            if(GlobalVars.Instance.grid)
            {
                do
                {
                    guy.dirX = GlobalVars.Instance.random.Next(-1, 2);
                    guy.dirY = GlobalVars.Instance.random.Next(-1, 2);
                } while (guy.dirX == 0 && guy.dirY == 0);
            }
            else
            {
                do
                {
                    guy.dirX = GlobalVars.Instance.random.Next(-1, 2);
                    guy.dirY = GlobalVars.Instance.random.Next(-1, 2);
                } while (guy.dirX == 0 || guy.dirY == 0);
            }
        }
        public double SizeRatio(mainForm form)
        {
            if (form == null || form.ClientSize == null)
            {
                return 1.0;
            }

            double baseSize = Math.Sqrt(form.ClientSize.Width * form.ClientSize.Height);
            double currentSize = Math.Sqrt(form.ClientSize.Width * form.ClientSize.Height);
            return currentSize / baseSize;
        }
        public void ModifyLife(mainForm mainform,LifeGuy guy = null, LifeGuy otherGuy = null,
            int number1 = 0, int number2 = 0)
        {
            int con = 0;
            int LocalSize = GlobalVars.Instance.lifeSize;
            LifeGuy buttonToRemove;

            if (GlobalVars.Instance.Life_List.Count < GlobalVars.Instance.maxLife)
            {
                switch (GlobalVars.Instance.guycase)
                {
                    case "default":
                        guy = new LifeGuy();
                        mainform.Controls.Add(guy);
                        guy.SetInitialLocation(mainform);

                        guy.Text = RandomNumber(guy);
                        AdjustTextSize(guy);

                        guy.Size = new Size(LocalSize, LocalSize);
                        guy.ForeColor = Color.White;
                        guy.BackColor = SetColor(Convert.ToInt32(guy.Text));

                        SetDirection(guy);

                        GlobalVars.Instance.Life_List.Add(guy);
                        timers.InitializeTimer(guy);
                        break;

                    case "par":
                        guy = new LifeGuy();
                        mainform.Controls.Add(guy);
                        guy.SetInitialLocation(mainform);

                        guy.Text = RandomNumber(guy);
                        AdjustTextSize(guy);

                        guy.Size = new Size(LocalSize, LocalSize);
                        guy.ForeColor = Color.White;
                        guy.BackColor = SetColor(Convert.ToInt32(guy.Text));

                        SetDirection(guy);

                        GlobalVars.Instance.Life_List.Add(guy);
                        timers.InitializeTimer(guy);
                        break;

                    case "impar":
                        guy = new LifeGuy();
                        mainform.Controls.Add(guy);
                        guy.SetInitialLocation(mainform);

                        guy.Text = RandomNumber(guy);
                        AdjustTextSize(guy);

                        guy.Size = new Size(LocalSize, LocalSize);
                        guy.ForeColor = Color.White;
                        guy.BackColor = SetColor(Convert.ToInt32(guy.Text));

                        SetDirection(guy);

                        GlobalVars.Instance.Life_List.Add(guy);
                        timers.InitializeTimer(guy);
                        break;

                    case "Exact":
                        guy = new LifeGuy();
                        mainform.Controls.Add(guy);
                        int x, y;
                        if(GlobalVars.Instance.grid)
                        {
                        x = RoundToNearestIncrement(number1, GlobalVars.Instance.lifeSize);
                        y = RoundToNearestIncrement(number2, GlobalVars.Instance.lifeSize);
                        }
                        else
                        {
                            x = number1;
                            y = number2;
                        }
                        guy.Location = new Point(x,y);

                        guy.Text = RandomNumber(guy);
                        AdjustTextSize(guy);

                        SetDirection(guy);

                        guy.Size = new Size(LocalSize, LocalSize);
                        guy.ForeColor = Color.White;
                        guy.BackColor = SetColor(Convert.ToInt32(guy.Text));


                        GlobalVars.Instance.Life_List.Add(guy);
                        timers.InitializeTimer(guy);
                        Console.WriteLine($"FormClick. 1:{GlobalVars.Instance.Life_List.Count}");
                        break;

                    case "imparpar":
                        con = 0;
                        number1 = Convert.ToInt32(guy.Text);
                        number2 = Convert.ToInt32(otherGuy.Text);
                        if (number1 < number2)
                        {
                            while (con < number1 && GlobalVars.Instance.Life_List.Count > 2)
                            {
                                buttonToRemove = GlobalVars.Instance.Life_List.Last();
                                mainform.Controls.Remove(buttonToRemove);
                                GlobalVars.Instance.Life_List.Remove(buttonToRemove);
                                con++;
                                Console.WriteLine($" IP1. {con}:{number2};{GlobalVars.Instance.Life_List.Count}");
                            }
                        }
                        else
                        {
                            while (con < number2 && GlobalVars.Instance.Life_List.Count > 2)
                            {
                                buttonToRemove = GlobalVars.Instance.Life_List.Last();
                                mainform.Controls.Remove(buttonToRemove);
                                GlobalVars.Instance.Life_List.Remove(buttonToRemove);
                                con++;
                                Console.WriteLine($"IP2. {con}:{number2};{GlobalVars.Instance.Life_List.Count}");
                            }
                        }
                        break;
                }
            }
        }
        public int RoundToNearestIncrement(int value, int increment)
        {
            int remainder = value % increment;
            if (remainder == 0)
            {
                return value;
            }
            else if (remainder < increment / 2)
            {
                return value - remainder;
            }
            else
            {
                return value + (increment - remainder);
            }
        }
        public void AdjustTextSize(LifeGuy guy = null, Button setting = null)
        {
            
            if (guy != null)
            {
                float maxSize = guy.Width;
                while (maxSize > 0)
                {
                    Font font = new Font("Consolas", maxSize);
                    SizeF textSize = TextRenderer.MeasureText(guy.Text, font);

                    if (textSize.Width <= guy.Width && textSize.Height <= guy.Height)
                    {
                        guy.Font = font;
                        guy.ImageAlign = ContentAlignment.TopCenter;
                        guy.TextAlign = ContentAlignment.MiddleCenter;
                        break;
                    }
                    else
                    {
                        maxSize -= 1;
                    }
                }
            }
            else
            {
                float maxSize = setting.Width;
                while (maxSize > 0)
                {
                    Font font = new Font("Consolas", maxSize);
                    SizeF textSize = TextRenderer.MeasureText(setting.Text, font);

                    if (textSize.Width <= setting.Width && textSize.Height <= setting.Height)
                    {
                        setting.Font = font;
                        setting.ImageAlign = ContentAlignment.TopCenter;
                        setting.TextAlign = ContentAlignment.MiddleCenter;
                        break;
                    }
                    else
                    {
                        maxSize -= 1;
                    }
                }
            }
        }
        public void PauseGame()
        {
            GlobalVars.Instance.pause = GlobalVars.Instance.pause ? false : true;
        }
        public void gridGame()
        {
            GlobalVars.Instance.grid = GlobalVars.Instance.grid ? false : true;
        }
        public void ChangeGridTxT(Control control)
        {
            control.Text = GlobalVars.Instance.grid ? "Desactivar" : "Activar";
        }
        public bool CheckSpaceAvailable(mainForm mainform)
        {
            int LifeWIdth = 0;
            int LifeHeight = 0;

            foreach (LifeGuy button in GlobalVars.Instance.Life_List)
            {
                LifeWIdth += button.Width;
                LifeHeight += button.Height;
            }

            int availableWidth = mainform.ClientSize.Width - LifeWIdth;
            int availableHeight = mainform.ClientSize.Height - LifeHeight;

            return availableWidth >= GlobalVars.Instance.lifeSize 
                && availableHeight >= GlobalVars.Instance.lifeSize;
        }
    }
}
