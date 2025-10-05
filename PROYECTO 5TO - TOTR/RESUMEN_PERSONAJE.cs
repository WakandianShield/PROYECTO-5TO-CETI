using PROYECTO_5TO___TOTЯ;

namespace proyecto
{
    public class RESUMEN_PERSONAJE : Form
    {
        private Personaje pj;
        private TextBox txtNombrePartida;
        private Button btnGuardar, btnCancelar;
        private Image fondo;

        public RESUMEN_PERSONAJE(Personaje personaje)
        {
            pj = personaje;
            FUENTE.CargarFuente();
            InicializarFormulario();
            CrearControles();
        }

        private void InicializarFormulario()
        {
            Text = "Resumen del Personaje";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;

            string rutaFondo = Path.Combine(Application.StartupPath, "Resources", "fondores.png");
            if (File.Exists(rutaFondo))
                fondo = Image.FromFile(rutaFondo);

            this.Paint += RESUMEN_PERSONAJE_Paint;
            this.Resize += (s, e) => this.Invalidate();
        }

        private void CrearControles()
        {
            int w = this.ClientSize.Width;

            btnCancelar = new Button()
            {
                Text = "X",
                Font = FUENTE.ObtenerFont(18),
                Size = new Size(45, 40),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnCancelar.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnCancelar.MouseEnter += (s, e) => { btnCancelar.ForeColor = Color.Red; };
            btnCancelar.MouseLeave += (s, e) => { btnCancelar.ForeColor = Color.White; };

            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnCancelar);
            btnCancelar.BringToFront();
            btnCancelar.Left = 10;
            btnCancelar.Top = 10;

            Label lblTitulo = new Label()
            {
                Text = "RESUMEN DEL PERSONAJE",
                Font = FUENTE.ObtenerFont(40),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 80
            };
            Controls.Add(lblTitulo);

            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent,
                Padding = new Padding(30, 30 + lblTitulo.Height + 30, 30, 30)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 75F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Controls.Add(mainLayout);

            TableLayoutPanel leftLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 12,
                ColumnCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 15, 0)
            };
            for (int i = 0; i < 12; i++)
                leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 12F));

            leftLayout.Controls.Add(CrearLabelMargen($"RAZA: {pj.RAZA}", 30, 10), 0, 0);
            leftLayout.Controls.Add(CrearLabelMargen($"SUBRAZA: {(string.IsNullOrEmpty(pj.SUBRAZA) ? "" : pj.SUBRAZA)}", 30, 10), 0, 1);
            leftLayout.Controls.Add(CrearLabelMargen($"CLASE: {pj.CLASE}", 30, 10), 0, 2);
            leftLayout.Controls.Add(CrearLabelMargen($"TRASFONDO: {pj.TRASFONDO}", 30, 10), 0, 3);
            leftLayout.Controls.Add(CrearLabelMargen($"ALINEAMIENTO: {pj.ALINEAMIENTO}", 30, 10), 0, 4);
            leftLayout.Controls.Add(CrearLabelMargen($"STR: {pj.STR}", 30, 10), 0, 5);
            leftLayout.Controls.Add(CrearLabelMargen($"DEX: {pj.DEX}", 30, 10), 0, 6);
            leftLayout.Controls.Add(CrearLabelMargen($"CON: {pj.CON}", 30, 10), 0, 7);
            leftLayout.Controls.Add(CrearLabelMargen($"INT: {pj.INT}", 30, 10), 0, 8);
            leftLayout.Controls.Add(CrearLabelMargen($"WIS: {pj.WIS}", 30, 10), 0, 9);
            leftLayout.Controls.Add(CrearLabelMargen($"CHA: {pj.CHA}", 30, 10), 0, 10);
            leftLayout.Controls.Add(CrearLabelMargen($"HP: {pj.HP}  CA: {pj.CA}  VEL: {pj.VEL}  INI: {pj.INI}", 30, 10), 0, 11);

            mainLayout.Controls.Add(leftLayout, 0, 0);

            TableLayoutPanel rightLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 15, 0),
                AutoSize = true
            };

            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));

            rightLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            Label lblHabilidades = new Label
            {
                Text = "HABILIDADES:\n\n" + string.Join("\n", pj.HABILIDADES.Select(h =>
                        $"{h.NOMBRE} ({h.STAT_ASOCIADO}) - MOD:{h.MODIFICADOR_STAT}, COMP:{h.BONIFICADOR_COMPETENCIA}, TOTAL:{h.TOTAL}")),
                Font = FUENTE.ObtenerFont(25),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 10)
            };
            rightLayout.Controls.Add(lblHabilidades, 0, 0);

            Label lblArmasHechizos = new Label
            {
                Text = $"ARMAS:\n{string.Join("\n", pj.ARMAS)}\n\nHECHIZOS:\n{string.Join("\n", pj.HECHIZOS)}",
                Font = FUENTE.ObtenerFont(25),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 10)
            };
            rightLayout.Controls.Add(lblArmasHechizos, 0, 1);

            mainLayout.Controls.Add(rightLayout, 1, 0);



            FlowLayoutPanel bottomLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            bottomLayout.Padding = new Padding(0, 0, 0, 20);

            Label lblNombre = new Label()
            {
                Text = "NOMBRE DE LA PARTIDA:",
                Font = FUENTE.ObtenerFont(30),
                ForeColor = Color.White,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 5)
            };
            bottomLayout.Controls.Add(lblNombre);

            txtNombrePartida = new TextBox()
            {
                Font = FUENTE.ObtenerFont(16),
                Size = new Size(250, 30),
                Margin = new Padding(0, 0, 0, 10)
            };
            bottomLayout.Controls.Add(txtNombrePartida);

            btnGuardar = new Button()
            {
                Text = "GUARDAR",
                Font = FUENTE.ObtenerFont(18),
                Size = new Size(250, 50),
                Margin = new Padding(0, 0, 0, 0)
            };
            btnGuardar.Click += BtnGuardar_Click;
            bottomLayout.Controls.Add(btnGuardar);

            mainLayout.Controls.Add(bottomLayout, 0, 1);
            mainLayout.SetColumnSpan(bottomLayout, 2);
        }

        private Label CrearLabelMargen(string texto, int fontSize, int margenInferior)
        {
            return new Label()
            {
                Text = texto,
                Font = FUENTE.ObtenerFont(fontSize),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                AutoSize = false,
                Margin = new Padding(0, 0, 0, margenInferior)
            };
        }

        private void RESUMEN_PERSONAJE_Paint(object sender, PaintEventArgs e)
        {
            if (fondo != null)
                e.Graphics.DrawImage(fondo, new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombrePartida.Text))
            {
                MessageBox.Show("Debes escribir un nombre de partida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            pj.NOMBRE = txtNombrePartida.Text.Trim();

            try
            {
                BASE_DE_DATOS.GuardarPersonaje(pj);
                MessageBox.Show("¡Personaje guardado correctamente!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
