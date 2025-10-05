using PROYECTO_5TO___TOTЯ;

namespace proyecto
{
    public class SELECCION_TRASFONDO : Form
    {
        private List<string> trasfondos = new();
        private int indiceActual = 0;

        private PictureBox pbTrasfondo = null!;
        private Label lblNombreTrasfondo = null!;
        private Button btnIzquierda = null!;
        private Button btnDerecha = null!;
        private Button btnConfirmar = null!;
        private Button btnVolver = null!;
        private Label lblTitulo = null!;

        public string TrasfondoSeleccionado { get; private set; } = "";

        public SELECCION_TRASFONDO()
        {
            FUENTE.CargarFuente();
            InicializarFormulario();

            var cond = new CONDICIONALES_Y_CALCULOS();
            trasfondos = cond.ObtenerTrasfondos();


            if (trasfondos.Count == 0)
            {
                MessageBox.Show("No hay trasfondos disponibles.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            CrearControles();
            MostrarTrasfondoActual();
        }

        private void InicializarFormulario()
        {
            Text = "Seleccionar Trasfondo";
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
            pbTrasfondo = new PictureBox() { SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Transparent };
            Controls.Add(pbTrasfondo);

            lblTitulo = new Label()
            {
                Text = "SELECCION DE TRASFONDO",
                Font = FUENTE.ObtenerFont(40),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.None
            };
            Controls.Add(lblTitulo);

            lblNombreTrasfondo = new Label()
            {
                Font = FUENTE.ObtenerFont(30),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblNombreTrasfondo);

            btnIzquierda = CrearBotonFlecha("<", (s, e) =>
            {
                indiceActual = (indiceActual - 1 + trasfondos.Count) % trasfondos.Count;
                MostrarTrasfondoActual();
            });
            Controls.Add(btnIzquierda);

            btnDerecha = CrearBotonFlecha(">", (s, e) =>
            {
                indiceActual = (indiceActual + 1) % trasfondos.Count;
                MostrarTrasfondoActual();
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
            pbTrasfondo.SetBounds(imgX, imgY, imgWidth, imgHeight);

            int nombreY = pbTrasfondo.Bottom + 20;
            lblNombreTrasfondo.SetBounds((w - pbTrasfondo.Width) / 2, nombreY, pbTrasfondo.Width, 60);
            lblNombreTrasfondo.TextAlign = ContentAlignment.MiddleCenter;

            btnConfirmar.Left = (w - btnConfirmar.Width) / 2;
            btnConfirmar.Top = lblNombreTrasfondo.Bottom + 30;

            int btnMargin = 20;
            int centerY = h / 2 - btnIzquierda.Height / 2;

            btnIzquierda.Left = pbTrasfondo.Left - btnIzquierda.Width - btnMargin;
            btnIzquierda.Top = centerY;

            btnDerecha.Left = pbTrasfondo.Right + btnMargin;
            btnDerecha.Top = centerY;

            btnVolver.Left = 10;
            btnVolver.Top = 10;
        }





        private void MostrarTrasfondoActual()
        {
            TrasfondoSeleccionado = trasfondos[indiceActual];
            lblNombreTrasfondo.Text = TrasfondoSeleccionado;

            string rutaImagen = Path.Combine(Application.StartupPath, "Resources", $"{TrasfondoSeleccionado}.png");
            if (File.Exists(rutaImagen))
                pbTrasfondo.Image = Image.FromFile(rutaImagen);
            else
                pbTrasfondo.Image = null;
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
