using PROYECTO_5TO___TOTЯ;

namespace proyecto
{
    public class SELECCION_RAZA : Form
    {
        private List<string> razas = new() { "HUMANO", "ELFO", "ENANO", "ORCO" };
        private int indiceActual = 0;

        private PictureBox pbRaza = null!;
        private Label lblNombreRaza = null!;
        private Button btnIzquierda = null!;
        private Button btnDerecha = null!;
        private Button btnConfirmar = null!;
        private Button btnVolver = null!;
        private Label lblTitulo = null!;

        public string RazaSeleccionada { get; private set; } = "";

        public SELECCION_RAZA()
        {
            FUENTE.CargarFuente();
            InicializarFormulario();
            CrearControles();
            MostrarRazaActual();
        }

        private void InicializarFormulario()
        {
            Text = "Seleccionar Raza";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;

            string rutaFondo = Path.Combine(Application.StartupPath, "Resources", "fondoblur.png");
            if (File.Exists(rutaFondo))
            {
                BackgroundImage = Image.FromFile(rutaFondo);
                BackgroundImageLayout = ImageLayout.Stretch;
            }
        }

        private void CrearControles()
        {
            pbRaza = new PictureBox() { SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Transparent };
            Controls.Add(pbRaza);

            lblTitulo = new Label()
            {
                Text = "SELECCION DE RAZA",
                Font = FUENTE.ObtenerFont(40),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.None
            };
            Controls.Add(lblTitulo);

            lblNombreRaza = new Label()
            {
                Font = FUENTE.ObtenerFont(30),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblNombreRaza);

            btnIzquierda = CrearBotonFlecha("<", (s, e) =>
            {
                indiceActual = (indiceActual - 1 + razas.Count) % razas.Count;
                MostrarRazaActual();
            });
            Controls.Add(btnIzquierda);

            btnDerecha = CrearBotonFlecha(">", (s, e) =>
            {
                indiceActual = (indiceActual + 1) % razas.Count;
                MostrarRazaActual();
            });
            Controls.Add(btnDerecha);

            btnConfirmar = new Button()
            {
                Text = "CONFIRMAR",
                Font = FUENTE.ObtenerFont(20),
                BackColor = Color.White,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Width = 250,
                Height = 60
            };
            btnConfirmar.Click += BtnConfirmar_Click;
            Controls.Add(btnConfirmar);

            btnVolver = new Button()
            {
                Text = "X",
                Font = FUENTE.ObtenerFont(18),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 50,
                Height = 50
            };
            btnVolver.FlatAppearance.BorderSize = 0;
            btnVolver.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnVolver);

            RedondearBoton(btnConfirmar, 30);
            RedondearBoton(btnVolver, 10);

            this.Resize += (s, e) => ReposicionarControles();
            ReposicionarControles();
        }


        private void RedondearBoton(Button boton, int radio)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.StartFigure();
            path.AddArc(0, 0, radio, radio, 180, 90);
            path.AddArc(boton.Width - radio, 0, radio, radio, 270, 90);
            path.AddArc(boton.Width - radio, boton.Height - radio, radio, radio, 0, 90);
            path.AddArc(0, boton.Height - radio, radio, radio, 90, 90);
            path.CloseFigure();
            boton.Region = new Region(path);
        }

        private Button CrearBotonFlecha(string texto, EventHandler click)
        {
            Button btn = new Button()
            {
                Text = texto,
                Font = FUENTE.ObtenerFont(30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Width = 80,
                Height = 80
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += click;
            return btn;
        }

        private void ReposicionarControles()
        {
            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;

            if (lblTitulo != null)
            {
                int tituloWidth = 900;
                int tituloHeight = 60;
                lblTitulo.SetBounds((w - tituloWidth) / 2, 30, tituloWidth, tituloHeight);
                lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            }

            int imgWidth = (int)(w * 0.5);
            int imgHeight = (int)(h * 0.5);
            int imgX = (w - imgWidth) / 2;
            int imgY = (h - imgHeight) / 2 - 30;
            pbRaza.SetBounds(imgX, imgY, imgWidth, imgHeight);

            int nombreY = pbRaza.Bottom + 20;
            lblNombreRaza.SetBounds((w - pbRaza.Width) / 2, nombreY, pbRaza.Width, 60);
            lblNombreRaza.TextAlign = ContentAlignment.MiddleCenter;

            btnConfirmar.Left = (w - btnConfirmar.Width) / 2;
            btnConfirmar.Top = lblNombreRaza.Bottom + 30;

            int btnMargin = 20;
            int centerY = h / 2 - btnIzquierda.Height / 2;

            btnIzquierda.Left = pbRaza.Left - btnIzquierda.Width - btnMargin;
            btnIzquierda.Top = centerY;

            btnDerecha.Left = pbRaza.Right + btnMargin;
            btnDerecha.Top = centerY;

            btnVolver.Left = 10;
            btnVolver.Top = 10;
        }






        private void MostrarRazaActual()
        {
            RazaSeleccionada = razas[indiceActual];
            lblNombreRaza.Text = RazaSeleccionada;

            string rutaImagen = Path.Combine(Application.StartupPath, "Resources", $"{RazaSeleccionada}.png");
            pbRaza.Image = File.Exists(rutaImagen) ? Image.FromFile(rutaImagen) : null;
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(RazaSeleccionada))
            {
                MessageBox.Show("Debes seleccionar una raza.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Solo asigna la raza y cierra el form con OK
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}
