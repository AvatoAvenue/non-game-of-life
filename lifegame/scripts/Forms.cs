using System;
using System.Drawing;
using System.Windows.Forms;
using static lifegame.scripts.Life;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace lifegame.scripts
{
    internal class Forms
    {
        Processors processors = new Processors();
        private mainForm main;
        private bool started = false;
        private ConfigSettings currentSettings = new ConfigSettings();
        private Panel menuPanel;
        private TableLayoutPanel tableLayout;
        private class ConfigSettings
        {
            public bool Grid { get; set; }
            public Keys PauseKey { get; set; }
            public int InitialLife { get; set; }
            public int MaxLife { get; set; }
            public int MinLife { get; set; }
            public float ConstVelocity { get; set; }
            public int LifeSize { get; set; }
        }
        //initial app
        public void StartApp()
        {
            main = new mainForm();
            main.WindowState = FormWindowState.Maximized;
            main.BackColor = Color.Black;

            menuPanel = new Panel();
            menuPanel.Dock = DockStyle.Fill;
            main.Controls.Add(menuPanel);

            ShowMenu();

            Application.Run(main);
        }
        //Start a new game
        private void StartGame()
        {
            menuPanel.Visible = false;

            startGame start = new startGame();
            start.ClearGameElements(main);
            RestoreSavedSettings();
            BackButton();
            start.Start(main);
        }
        //menu
        private void ShowMenu()
        {
            menuPanel.Controls.Clear();

            Button startButton = new Button();
            startButton.Text = "Start Game";
            startButton.ForeColor = Color.White;
            processors.AdjustTextSize(null,startButton);
            startButton.Click += StartButton_Click;

            Button settingsButton = new Button();
            settingsButton.Text = "Settings";
            settingsButton.ForeColor = Color.White;
            processors.AdjustTextSize(null, settingsButton);
            settingsButton.Click += SettingsButton_Click;

            Button exitButton = new Button();
            exitButton.Text = "Exit";
            exitButton.ForeColor = Color.White;
            processors.AdjustTextSize(null, exitButton);
            exitButton.Click += ExitButton_Click;

            main.Resize += (sender, e) => AdjustButtons(main, startButton, settingsButton, exitButton);

            AdjustButtons(main, startButton, settingsButton, exitButton);

            menuPanel.Controls.Add(startButton);
            menuPanel.Controls.Add(settingsButton);
            menuPanel.Controls.Add(exitButton);
        }
        //configuration
        private void ShowConfig()
        {
            menuPanel.Controls.Clear();

            tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.ColumnCount = 2;
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            menuPanel.Controls.Add(tableLayout);

            Button gridButton = CreateButton("Activar", GridButton_Click);
            Button pauseButton = CreateButton(GlobalVars.Instance.pauseKey.ToString(), PauseButton_Click);
            TrackBar initLifeTrackBar = CreateTrackBar(GlobalVars.Instance.initquantity, 0, GlobalVars.Instance.maxLife, (sender, e)
                => { GlobalVars.Instance.initquantity = (sender as TrackBar).Value; });
            TrackBar maxLifeTrackBar = CreateTrackBar(GlobalVars.Instance.maxLife, 1, 500, (sender, e)
                => { GlobalVars.Instance.maxLife = (sender as TrackBar).Value; });
            TrackBar constVelocityTrackBar = CreateTrackBar((int)(GlobalVars.Instance.constVelocity * 10), 1, 100, (sender, e)
                => { GlobalVars.Instance.constVelocity = (sender as TrackBar).Value / 10f; });
            TrackBar lifeSizeTrackBar = CreateTrackBar(GlobalVars.Instance.lifeSize, 1, 100, (sender, e)
                    => { GlobalVars.Instance.lifeSize = (sender as TrackBar).Value; });

            AddConfigControl(tableLayout, "Grid", "cuadrícula", gridButton);
            AddConfigControl(tableLayout, "Pause", "Tecla de pausa", pauseButton);
            AddConfigControl(tableLayout, "InitialLife", "Vida inicial", initLifeTrackBar);
            AddConfigControl(tableLayout, "MaxLife", "Vida máxima", maxLifeTrackBar);
            AddConfigControl(tableLayout, "ConstVelocity", "Velocidad", constVelocityTrackBar);
            AddConfigControl(tableLayout, "LifeSize", "Tamaño", lifeSizeTrackBar);

            AdjustControls(main, gridButton, pauseButton, initLifeTrackBar,
                maxLifeTrackBar, constVelocityTrackBar, lifeSizeTrackBar);

            BackButton(tableLayout);
        }
        private void AddConfigControl(TableLayoutPanel tableLayout, string label,
            string labelText, Control control)
        {
            Label titleLabel = new Label();
            titleLabel.Text = labelText;
            titleLabel.ForeColor = Color.White;
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;

            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.Controls.Add(titleLabel, 0, tableLayout.RowCount - 1);
            tableLayout.Controls.Add(control, 1, tableLayout.RowCount - 1);

            if (control is TrackBar)
            {
                TrackBar trackBar = (TrackBar)control;
                Label valueLabel = new Label();
                valueLabel.ForeColor = Color.White;
                valueLabel.Text = trackBar.Value.ToString();
                valueLabel.TextAlign = ContentAlignment.MiddleLeft;

                trackBar.Scroll += (sender, e) =>
                {
                    valueLabel.Text = trackBar.Value.ToString();
                };

                tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tableLayout.Controls.Add(valueLabel, 2, tableLayout.RowCount - 1);
                tableLayout.SetColumnSpan(valueLabel, 2);
            }
        }
        private Button CreateButton(string text, EventHandler onClick)
        {
            Button button = new Button();
            button.Text = text;
            button.ForeColor = Color.White;
            button.Click += onClick;
            processors.AdjustTextSize(null, button);
            return button;
        }
        private TrackBar CreateTrackBar(int value, int minimum, int maximum, EventHandler valueChanged)
        {
            TrackBar trackBar = new TrackBar();
            trackBar.Minimum = minimum;
            trackBar.Maximum = maximum;
            trackBar.Value = value;
            trackBar.TickStyle = TickStyle.None;
            trackBar.ValueChanged += valueChanged;
            return trackBar;
        }
        //adjustable settings
        private void AdjustControls(mainForm main, Button GridButton, Button pauseButton,
            TrackBar initLifeTrackBar, TrackBar maxLifeTrackBar,
            TrackBar constVelocityTrackBar, TrackBar lifeSizeTrackBar)
        {
            int controlWidth = main.Width / 2;
            int controlHeight = main.Height / 20;
            int controlSpacing = main.Height / 30;
            int totalControlHeight = (controlHeight + controlSpacing) * 6; // Seis controles en total
            int startY = (main.Height - totalControlHeight) / 2;

            GridButton.Size = new Size(controlWidth, controlHeight);
            GridButton.Location = new Point((main.Width - controlWidth) / 2, startY);

            startY += controlHeight + controlSpacing;

            pauseButton.Size = new Size(controlWidth, controlHeight);
            pauseButton.Location = new Point((main.Width - controlWidth) / 2, startY);

            startY += controlHeight + controlSpacing;

            initLifeTrackBar.Size = new Size(controlWidth, controlHeight);
            initLifeTrackBar.Location = new Point((main.Width - controlWidth) / 2, startY);

            Label initLifeLabel = new Label();
            initLifeLabel.Text = "Vida inicial";
            initLifeLabel.ForeColor = Color.White;
            initLifeLabel.Size = new Size(controlWidth, controlHeight);
            initLifeLabel.TextAlign = ContentAlignment.MiddleCenter;
            initLifeLabel.Location = new Point((main.Width - controlWidth) / 2, startY - controlHeight / 2);
            menuPanel.Controls.Add(initLifeLabel);

            startY += controlHeight + controlSpacing;

            maxLifeTrackBar.Size = new Size(controlWidth, controlHeight);
            maxLifeTrackBar.Location = new Point((main.Width - controlWidth) / 2, startY);

            Label maxLifeLabel = new Label();
            maxLifeLabel.Text = "Vida máxima";
            maxLifeLabel.ForeColor = Color.White;
            maxLifeLabel.Size = new Size(controlWidth, controlHeight);
            maxLifeLabel.TextAlign = ContentAlignment.MiddleCenter;
            maxLifeLabel.Location = new Point((main.Width - controlWidth) / 2, startY - controlHeight / 2);
            menuPanel.Controls.Add(maxLifeLabel);

            startY += controlHeight + controlSpacing;

            //Ajustar posición y añadir deslizador de velocidad
            constVelocityTrackBar.Size = new Size(controlWidth, controlHeight);
            constVelocityTrackBar.Location = new Point((main.Width - controlWidth) / 2, startY);

            Label constVelocityLabel = new Label();
            constVelocityLabel.Text = "Velocidad";
            constVelocityLabel.ForeColor = Color.White;
            constVelocityLabel.Size = new Size(controlWidth, controlHeight);
            constVelocityLabel.TextAlign = ContentAlignment.MiddleCenter;
            constVelocityLabel.Location = new Point((main.Width -
                controlWidth) / 2, startY - controlHeight / 2);
            menuPanel.Controls.Add(constVelocityLabel);

            startY += controlHeight + controlSpacing;

            //Ajustar posición y añadir deslizador de tamaño de vida
            lifeSizeTrackBar.Size = new Size(controlWidth, controlHeight);
            lifeSizeTrackBar.Location = new Point((main.Width - controlWidth) / 2, startY);

            Label lifeSizeLabel = new Label();
            lifeSizeLabel.Text = "Tamaño";
            lifeSizeLabel.ForeColor = Color.White;
            lifeSizeLabel.Size = new Size(controlWidth, controlHeight);
            lifeSizeLabel.TextAlign = ContentAlignment.MiddleCenter;
            lifeSizeLabel.Location = new Point((main.Width -
                controlWidth) / 2, startY - controlHeight / 2);
            menuPanel.Controls.Add(lifeSizeLabel);
        }

        //adjustable buttons
        private void AdjustButtons(mainForm main, Button startButton,
            Button settingsButton, Button exitButton)
        {
            int controlWidth = main.Width / 4;
            int controlHeight = main.Height / 10;
            int controlSpacing = main.Height / 20;
            int startY = (main.Height - (controlHeight * 3 + controlSpacing * 2)) / 2;

            startButton.Size = new Size(controlWidth, controlHeight);
            startButton.Location = new Point((main.Width - controlWidth) / 2, startY);

            settingsButton.Size = new Size(controlWidth, controlHeight);
            settingsButton.Location = new Point((main.Width - controlWidth) / 2,
                startY + controlHeight + controlSpacing);

            exitButton.Size = new Size(controlWidth, controlHeight);
            exitButton.Location = new Point((main.Width - controlWidth) / 2,
                startY + 2 * (controlHeight + controlSpacing));
        }
        //back to menu button
        private void BackButton(TableLayoutPanel tab = null)
        {
            Button backButton = new Button();
            backButton.Text = "Menú";
            backButton.ForeColor = Color.White;
            processors.AdjustTextSize(null, backButton);
            backButton.Click += BackButton_Click;
            backButton.Size = new Size(120, 40);
            backButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            main.Controls.Add(backButton);
            if (tab != null ) { tab.Controls.Add(backButton ); }
            main.Resize += (sender, e) =>
            {
                backButton.Location = new Point(main.Width - backButton.Width - 20, 20);
            };
        }
        //iniciar juego
        private void StartButton_Click(object sender, EventArgs e)
        {
            if (!started)
            {
                SaveCurrentSettings();
                started = true;
                StartGame();
            }
        }
        //volver al menú
        private void BackButton_Click(Object sender, EventArgs e)
        {
            started = false;
            menuPanel.Controls.Clear();
            menuPanel.Visible = true;
            SaveCurrentSettings();
            ShowMenu();
        }
        //cambio tecla
        private void PauseButton_Click(object sender, EventArgs e)
        {
            Keys key = GlobalVars.Instance.pauseKey;
            using (var keyDialog = new KeyDialog())
            {
                if (keyDialog.ShowDialog() == DialogResult.OK)
                {
                    GlobalVars.Instance.pauseKey = keyDialog.SelectedKey;
                }
            }
            UpdatePauseButton(key);
        }
        private void UpdatePauseButton(Keys key)
        {
            foreach (Control control in tableLayout.Controls)
            {
                if (control is Button && control.Text == key.ToString())
                {
                    control.Text = GlobalVars.Instance.pauseKey.ToString();
                    break;
                }
            }
        }
        //configuración
        private void SettingsButton_Click(object sender, EventArgs e)
        {
            SaveCurrentSettings();
            ShowConfig();
        }
        //salir
        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void GridButton_Click(object sender, EventArgs e)
        {
            foreach (Control control in tableLayout.Controls)
            {
                if (control is Button &&
                    (control.Text == "Desactivar" || control.Text == "Activar"))
                {
                    processors.gridGame();
                    processors.ChangeGridTxT(control);
                    break;
                }
            }
        }
        private void SaveCurrentSettings()
        {
            currentSettings.Grid = GlobalVars.Instance.grid;
            currentSettings.PauseKey = GlobalVars.Instance.pauseKey;
            currentSettings.InitialLife = GlobalVars.Instance.initquantity;
            currentSettings.MaxLife = GlobalVars.Instance.maxLife;
            currentSettings.MinLife = GlobalVars.Instance.minLife;
            currentSettings.ConstVelocity = GlobalVars.Instance.constVelocity;
            currentSettings.LifeSize = GlobalVars.Instance.lifeSize;

            RestoreSavedSettings();
        }
        private void RestoreSavedSettings()
        {
            GlobalVars.Instance.grid = currentSettings.Grid;
            GlobalVars.Instance.initquantity = currentSettings.InitialLife;
            GlobalVars.Instance.maxLife = currentSettings.MaxLife;
            GlobalVars.Instance.constVelocity = currentSettings.ConstVelocity;
            GlobalVars.Instance.lifeSize = currentSettings.LifeSize;
            GlobalVars.Instance.pauseKey = currentSettings.PauseKey;
        }
    }
}
