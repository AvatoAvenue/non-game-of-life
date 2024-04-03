using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static lifegame.scripts.Life;

namespace lifegame.scripts
{
    internal class Life
    {
        public class mainForm : Form
        {
            Processors processors = new Processors();
            public mainForm()
            {
                this.KeyPreview = true;
                this.DoubleBuffered = true;
                this.Resize += MainForm_SizeChanged;
                this.Click += Form_Click;
                this.KeyDown += MainForm_KeyDown;
            }
            private void MainForm_SizeChanged(object sender, EventArgs e)
            {
                
                for (int i = 0; i < GlobalVars.Instance.Life_List.Count; i++)
                {
                    GlobalVars.Instance.Life_List[i].AdjustPositionToStayVisible();
                }
            }
            //click de ventana
            private void Form_Click(object sender, EventArgs e)
            {
                Point mousePosition = this.PointToClient(Cursor.Position);

                bool buttonExistsAtMousePosition = false;
                foreach (var button in GlobalVars.Instance.Life_List)
                {
                    if (button.Bounds.Contains(mousePosition))
                    {
                        buttonExistsAtMousePosition = true;
                        break;
                    }
                }

                if (!buttonExistsAtMousePosition)
                {
                    GlobalVars.Instance.guycase = "Exact";
                    processors.ModifyLife(this, null, null, mousePosition.X, mousePosition.Y);
                    GlobalVars.Instance.guycase = "default";
                }

            }
            //teclas
            private void MainForm_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == GlobalVars.Instance.pauseKey)
                {
                    processors.PauseGame();
                }
            }
        }
        public partial class KeyDialog : Form
        {
            public Keys SelectedKey { get; private set; }

            public KeyDialog()
            {
                KeyDown += KeyDialog_KeyDown;
            }

            private void KeyDialog_KeyDown(object sender, KeyEventArgs e)
            {
                SelectedKey = e.KeyCode;
                DialogResult = DialogResult.OK;
            }

            private void KeyDialog_Load(object sender, EventArgs e)
            {
                KeyPreview = true;
            }
        }
        public class LifeGuy : Label
        {
            Processors processors = new Processors();
            private int con;
            public int dirX { get; set; }
            public int dirY { get; set; }

            public LifeGuy()
            {
                this.Click += LifeGuy_Click;
            }
            private void LifeGuy_Click(object sender, EventArgs e)
            {
                mainForm form = (mainForm)this.Parent;
                GlobalVars.Instance.Life_List.Remove(this);
                form.Controls.Remove(this);
                Console.WriteLine($"LifeClick. 1:{GlobalVars.Instance.Life_List.Count}");
            }
            public void Movement()
            {
                double sizeRatio = processors.SizeRatio((mainForm)this.Parent);
                int deltaX = (int)(this.dirX * GlobalVars.Instance.constVelocity * sizeRatio);
                int deltaY = (int)(this.dirY * GlobalVars.Instance.constVelocity * sizeRatio);

                this.Left += deltaX;
                this.Top += deltaY;
            }
            public void GridMovement()
            {
                int cellSize = GlobalVars.Instance.lifeSize;

                int deltaX = this.dirX * cellSize;
                int deltaY = this.dirY * cellSize;

                this.Left += deltaX;
                this.Top += deltaY;
            }
            public void BorderColliders()
            {
                if (this.Parent == null)
                {
                    return;
                }

                mainForm parentForm = (mainForm)this.Parent;

                int newLeft = this.Left;
                int newTop = this.Top;
                int newDirX = this.dirX;
                int newDirY = this.dirY;

                // Detectar colisión con bordes
                if (newLeft <= 0 || newLeft + this.Width >= parentForm.ClientSize.Width)
                {
                    newDirX *= -1;
                }
                if (newTop <= 0 || newTop + this.Height >= parentForm.ClientSize.Height)
                {
                    newDirY *= -1;
                }

                this.dirX = newDirX;
                this.dirY = newDirY;
            }
            public void GridBorderColliders()
            {
                if (this.Parent == null)
                {
                    return;
                }

                mainForm parentForm = (mainForm)this.Parent;
                int cellSize = GlobalVars.Instance.lifeSize;

                if (this.Left < 0 || this.Left + this.Width > parentForm.ClientSize.Width)
                {
                    this.dirX *= -1;
                    this.Left = Math.Max(0, Math.Min(this.Left, parentForm.ClientSize.Width - this.Width));
                    this.Left = processors.RoundToNearestIncrement(this.Left, cellSize);
                }
                if (this.Top < 0 || this.Top + this.Height > parentForm.ClientSize.Height)
                {
                    this.dirY *= -1;
                    this.Top = Math.Max(0, Math.Min(this.Top, parentForm.ClientSize.Height - this.Height));
                    this.Top = processors.RoundToNearestIncrement(this.Top, cellSize);
                }
            }
            public void GuyColliders()
            {
                if (this.Parent == null)
                {
                    return;
                }

                for (int i = 0; i < GlobalVars.Instance.Life_List.Count; i++)
                {
                    var otherGuy = GlobalVars.Instance.Life_List[i];
                    bool overlapping = this.Bounds.IntersectsWith(otherGuy.Bounds);

                    if (this != otherGuy && overlapping)
                    {
                        int temx = this.dirX;
                        int temy = this.dirY;

                        this.dirX = otherGuy.dirX;
                        this.dirY = otherGuy.dirY;
                        otherGuy.dirX = temx;
                        otherGuy.dirY = temy;

                        Overlap(otherGuy);
                        Mechannics(this, otherGuy);
                    }
                }
            }
            public void Overlap(LifeGuy otherGuy)
            {
                int deltaX = otherGuy.Left - this.Left;
                int deltaY = otherGuy.Top - this.Top;

                double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                if (distance == 0)
                    return;

                double overlapDistance = (this.Width + otherGuy.Width) / 2 - distance;

                if (overlapDistance > 0)
                {
                    double overlapRatio = overlapDistance / distance;
                    int separationX = (int)(deltaX * overlapRatio);
                    int separationY = (int)(deltaY * overlapRatio);

                    this.Left -= separationX;
                    this.Top -= separationY;
                    otherGuy.Left += separationX;
                    otherGuy.Top += separationY;
                }
            }
            public void Mechannics(LifeGuy guy, LifeGuy otherGuy)
            {
                con = 0;
                int number1 = Convert.ToInt32(guy.Text);
                int number2 = Convert.ToInt32(otherGuy.Text);

                // Ambos son pares
                if (number1 % 2 == 0 && number2 % 2 == 0)
                {
                    if (GlobalVars.Instance.Life_List.Count < GlobalVars.Instance.maxLife)
                    {
                        GlobalVars.Instance.guycase = "par";
                        while (con < (number1 + number2))
                        {
                            processors.ModifyLife((mainForm)this.Parent, this, otherGuy);
                            con++;
                            Console.WriteLine($"I. {con}:{number1 + number2};{GlobalVars.Instance.Life_List.Count}");
                        }
                    }
                    GlobalVars.Instance.clonated = true;
                }
                // Ambos son impares
                else if (number1 % 2 != 0 && number2 % 2 != 0)
                {
                    GlobalVars.Instance.guycase = "impar";
                    if (GlobalVars.Instance.Life_List.Count < GlobalVars.Instance.maxLife)
                    {
                        while (con < (number1 + number2))
                        {
                            processors.ModifyLife((mainForm)this.Parent, this, otherGuy);
                            con++;
                            Console.WriteLine($"P. {con}:{number1 + number2};{GlobalVars.Instance.Life_List.Count}");
                        }
                    }
                    GlobalVars.Instance.clonated = true;
                }
                // Uno es par y el otro impar
                else
                {
                    GlobalVars.Instance.guycase = "imparpar";
                    processors.ModifyLife((mainForm)this.Parent, this, otherGuy);
                    GlobalVars.Instance.clonated = true;

                }
                GlobalVars.Instance.clonated = false;
                GlobalVars.Instance.guycase = "default";
            }
            public void SetInitialLocation(mainForm mainForm)
            {
                Rectangle visibleArea = mainForm.ClientRectangle;

                bool collisionDetected;
                do
                {
                    int x = GlobalVars.Instance.random.Next(visibleArea.Width - this.Width);
                    int y = GlobalVars.Instance.random.Next(visibleArea.Height - this.Height);

                    x = processors.RoundToNearestIncrement(x, GlobalVars.Instance.lifeSize);
                    y = processors.RoundToNearestIncrement(y, GlobalVars.Instance.lifeSize);

                    x = Math.Max(0, Math.Min(x, visibleArea.Width - this.Width));
                    y = Math.Max(0, Math.Min(y, visibleArea.Height - this.Height));

                    this.Location = new Point(x, y);

                    collisionDetected = false;
                    foreach (var otherGuy in GlobalVars.Instance.Life_List)
                    {
                        if (otherGuy != this && this.Bounds.IntersectsWith(otherGuy.Bounds))
                        {
                            collisionDetected = true;
                            break;
                        }
                    }
                } while (collisionDetected);
            }
            public void AdjustPositionToStayVisible()
            {
                if (!this.Parent.ClientRectangle.Contains(this.Bounds))
                {
                    bool hasSpace = processors.CheckSpaceAvailable((mainForm)this.Parent);
                    mainForm main = (mainForm)this.Parent;
                    if (hasSpace)
                    {
                        Rectangle visibleArea = main.ClientRectangle;
                        int newX = Math.Max(0, Math.Min(this.Left, visibleArea.Width - this.Width));
                        int newY = Math.Max(0, Math.Min(this.Top, visibleArea.Height - this.Height));

                        bool overlapDetected = false;
                        foreach (var otherGuy in GlobalVars.Instance.Life_List)
                        {
                            if (otherGuy != this && new Rectangle(newX, newY,
                                this.Width, this.Height).IntersectsWith(otherGuy.Bounds))
                            {
                                overlapDetected = true;
                                break;
                            }
                        }
                        if (!overlapDetected)
                        {
                            this.Location = new Point(newX, newY);
                        }
                        else
                        {
                            this.Parent.Controls.Remove(this);
                            GlobalVars.Instance.Life_List.Remove(this);
                        }
                    }
                    else
                    {
                        this.Parent.Controls.Remove(this);
                        GlobalVars.Instance.Life_List.Remove(this);
                    }
                }
            }
        }
    }
}
