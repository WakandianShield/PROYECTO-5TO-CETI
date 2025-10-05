using PROYECTO_5TO___TOTЯ;

namespace proyecto
{
    public class SELECCION_ALINEAMIENTO : Form
    {
        private List<string> alineamientos = new()
        {
            "LEGAL BUENO",
            "NEUTRAL BUENO",
            "CAOTICO BUENO",
            "LEGAL NEUTRAL",
            "NEUTRAL VERDADERO",
            "CAOTICO NEUTRAL",
            "LEGAL MALVADO",
            "NEUTRAL MALVADO",
            "CAOTICO MALVADO"
        };

        private int indiceActual = 0;

        private Label lblNombreAlineamiento = null!;
        private Button btnIzquierda = null!;
        private Button btnDerecha = null!;
        private Button btnConfirmar = null!;
        private Button btnVolver = null!;
        private Label lblTitulo = null!;

        public string AlineamientoSeleccionado { get; private set; } = "";

        public SELECCION_ALINEAMIENTO()
        {
            FUENTE.CargarFuente();
            InicializarFormulario();
            CrearControles();
            MostrarAlineamientoActual();
        }

        private void InicializarFormulario()
        {
            Text = "Seleccionar Alineamiento";
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
            lblTitulo = new Label()
            {
                Text = "SELECCION DE ALINEAMIENTO",
                Font = FUENTE.ObtenerFont(40),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.None
            };
            Controls.Add(lblTitulo);

            lblNombreAlineamiento = new Label()
            {
                Font = FUENTE.ObtenerFont(48),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblNombreAlineamiento);

            btnIzquierda = new Button()
            {
                Text = "<",
                Font = FUENTE.ObtenerFont(30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Width = 80,
                Height = 80
            };
            btnIzquierda.FlatAppearance.BorderSize = 0;
            btnIzquierda.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnIzquierda.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnIzquierda.MouseEnter += (s, e) => { btnIzquierda.ForeColor = Color.LightGray; };
            btnIzquierda.MouseLeave += (s, e) => { btnIzquierda.ForeColor = Color.White; };
            btnIzquierda.Click += (s, e) =>
            {
                indiceActual = (indiceActual - 1 + alineamientos.Count) % alineamientos.Count;
                MostrarAlineamientoActual();
            };
            Controls.Add(btnIzquierda);

            btnDerecha = new Button()
            {
                Text = ">",
                Font = FUENTE.ObtenerFont(30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Width = 80,
                Height = 80
            };
            btnDerecha.FlatAppearance.BorderSize = 0;
            btnDerecha.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnDerecha.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnDerecha.MouseEnter += (s, e) => { btnDerecha.ForeColor = Color.LightGray; };
            btnDerecha.MouseLeave += (s, e) => { btnDerecha.ForeColor = Color.White; };
            btnDerecha.Click += (s, e) =>
            {
                indiceActual = (indiceActual + 1) % alineamientos.Count;
                MostrarAlineamientoActual();
            };
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
            RedondearBoton(btnVolver, radio);

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

            if (lblNombreAlineamiento != null)
            {
                int lblHeight = 100;
                int lblWidth = 800;
                lblNombreAlineamiento.SetBounds((w - lblWidth) / 2, (h - lblHeight) / 2, lblWidth, lblHeight);
                lblNombreAlineamiento.TextAlign = ContentAlignment.MiddleCenter;
            }

            int btnMargin = 100;
            int centerY = h / 2 - btnIzquierda.Height / 2;

            btnIzquierda.Left = lblNombreAlineamiento.Left - btnIzquierda.Width - btnMargin;
            btnIzquierda.Top = centerY;

            btnDerecha.Left = lblNombreAlineamiento.Right + btnMargin;
            btnDerecha.Top = centerY;

            btnConfirmar.Left = (w - btnConfirmar.Width) / 2;
            btnConfirmar.Top = h - 180;

            btnVolver.Left = 10;
            btnVolver.Top = 10;


        }

        private void MostrarAlineamientoActual()
        {
            AlineamientoSeleccionado = alineamientos[indiceActual];
            lblNombreAlineamiento.Text = AlineamientoSeleccionado;
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
