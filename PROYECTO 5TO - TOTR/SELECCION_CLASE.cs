using PROYECTO_5TO___TOTЯ;

namespace proyecto
{
    public class SELECCION_CLASE : Form
    {
        private List<string> clases = new();
        private int indiceActual = 0;

        private PictureBox pbClase = null!;
        private Label lblNombreClase = null!;
        private Button btnIzquierda = null!;
        private Button btnDerecha = null!;
        private Button btnConfirmar = null!;
        private Button btnVolver = null!;
        private Label lblTitulo = null!;

        public string ClaseSeleccionada { get; private set; } = "";

        public string RazaActual { get; set; } = "";
        public string SubrazaActual { get; set; } = "";

        private Personaje pj = null!;

        public SELECCION_CLASE(Personaje personaje)
        {
            FUENTE.CargarFuente();
            InicializarFormulario();

            pj = personaje ?? throw new ArgumentNullException(nameof(personaje));
            RazaActual = pj.RAZA;
            SubrazaActual = pj.SUBRAZA;

            var cond = new CONDICIONALES_Y_CALCULOS();
            clases = cond.ObtenerClases(RazaActual);

            if (clases.Count == 0)
            {
                MessageBox.Show("No hay clases disponibles para esta raza.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            CrearControles();
            MostrarClaseActual();
        }

        private void InicializarFormulario()
        {
            Text = "Seleccionar Clase";
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
            pbClase = new PictureBox() { SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Transparent };
            Controls.Add(pbClase);

            lblTitulo = new Label()
            {
                Text = "SELECCION DE CLASE",
                Font = FUENTE.ObtenerFont(40),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.None
            };
            Controls.Add(lblTitulo);

            lblNombreClase = new Label()
            {
                Font = FUENTE.ObtenerFont(30),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblNombreClase);

            btnIzquierda = CrearBotonFlecha("<", (s, e) =>
            {
                indiceActual = (indiceActual - 1 + clases.Count) % clases.Count;
                MostrarClaseActual();
            });
            Controls.Add(btnIzquierda);

            btnDerecha = CrearBotonFlecha(">", (s, e) =>
            {
                indiceActual = (indiceActual + 1) % clases.Count;
                MostrarClaseActual();
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
            Controls.Add(btnConfirmar);
            btnConfirmar.Click += BtnConfirmar_Click;

            btnVolver = new Button()
            {
                Text = " X",
                Font = FUENTE.ObtenerFont(18),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 45,
                Height = 40
            };
            btnVolver.FlatAppearance.BorderSize = 0;
            btnVolver.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnVolver.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnVolver.MouseEnter += (s, e) => { btnVolver.ForeColor = Color.Red; };
            btnVolver.MouseLeave += (s, e) => { btnVolver.ForeColor = Color.White; };
            btnVolver.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnVolver);

            int radio = 30;
            RedondearBoton(btnConfirmar, radio);
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
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btn.MouseEnter += (s, e) => { btn.ForeColor = Color.DarkGray; };
            btn.MouseLeave += (s, e) => { btn.ForeColor = Color.White; };
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
            pbClase.SetBounds(imgX, imgY, imgWidth, imgHeight);

            int nombreY = pbClase.Bottom + 20;
            lblNombreClase.SetBounds((w - pbClase.Width) / 2, nombreY, pbClase.Width, 60);
            lblNombreClase.TextAlign = ContentAlignment.MiddleCenter;

            btnConfirmar.Left = (w - btnConfirmar.Width) / 2;
            btnConfirmar.Top = lblNombreClase.Bottom + 30;

            int btnMargin = 20;
            int centerY = h / 2 - btnIzquierda.Height / 2;

            btnIzquierda.Left = pbClase.Left - btnIzquierda.Width - btnMargin;
            btnIzquierda.Top = centerY;

            btnDerecha.Left = pbClase.Right + btnMargin;
            btnDerecha.Top = centerY;

            btnVolver.Left = 10;
            btnVolver.Top = 10;
        }

        public void MostrarClaseActual()
        {
            ClaseSeleccionada = clases[indiceActual];
            lblNombreClase.Text = ClaseSeleccionada;

            string rutaBase = Path.Combine(Application.StartupPath, "Resources");
            string nombreArchivo;

            if (pj.RAZA.ToUpper() == "HUMANO" || pj.RAZA.ToUpper() == "ORCO")
            {
                nombreArchivo = $"{pj.RAZA} {ClaseSeleccionada}.png";
            }
            else if (pj.RAZA.ToUpper() == "ELFO" || pj.RAZA.ToUpper() == "ENANO")
            {
                nombreArchivo = $"{pj.SUBRAZA} {ClaseSeleccionada}.png";
            }
            else
            {
                nombreArchivo = $"{ClaseSeleccionada}.png";
            }

            string rutaImagen = Path.Combine(rutaBase, nombreArchivo);

            if (File.Exists(rutaImagen))
                pbClase.Image = Image.FromFile(rutaImagen);
            else
                pbClase.Image = null;
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            pj.CLASE = ClaseSeleccionada;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
