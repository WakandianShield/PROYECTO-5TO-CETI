using PROYECTO_5TO___TOTЯ;
using System.Drawing.Drawing2D;

namespace proyecto
{
    public class MENU_PRINCIPAL : Form
    {
        private Button btnNuevaPartida = null!;
        private Button btnContinuar = null!;
        private Button btnSalir = null!;
        private PictureBox logoPictureBox = null!;

        public MENU_PRINCIPAL()
        {
            FUENTE.CargarFuente();
            InicializarFormulario();
            CrearControles();
            this.Resize += (s, e) => ReposicionarControles();
        }

        private void InicializarFormulario()
        {
            Text = "Menu Principal (D&D)";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;

            string rutaFondo = Path.Combine(Application.StartupPath, "Resources", "fondoblur.png");
            if (File.Exists(rutaFondo))
            {
                BackgroundImage = Image.FromFile(rutaFondo);
                BackgroundImageLayout = ImageLayout.Stretch;
            }

            string rutaIcono = Path.Combine(Application.StartupPath, "Resources", "Icono.ico");
            if (File.Exists(rutaIcono))
            {
                Icon = new Icon(rutaIcono);
            }
        }

        private void CrearControles()
        {
            // -------------------------------------------------------------------- LOGO 
            logoPictureBox = new PictureBox()
            {
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            string rutaLogo = Path.Combine(Application.StartupPath, "Resources", "LOGO.png");
            if (File.Exists(rutaLogo))
            {
                logoPictureBox.Image = Image.FromFile(rutaLogo);
            }
            Controls.Add(logoPictureBox);

            // -------------------------------------------------------------------- BOTONES 
            btnNuevaPartida = CrearBoton("NUEVA PARTIDA");
            btnNuevaPartida.Click += (s, e) =>
            {
                try
                {
                    this.Hide(); // Oculta el menú principal

                    Personaje pj = null;
                    var cond = new CONDICIONALES_Y_CALCULOS();
                    string pasoActual = "RAZA";

                    while (true)
                    {
                        if (pasoActual == "RAZA")
                        {
                            using (var formRaza = new SELECCION_RAZA())
                            {
                                var res = formRaza.ShowDialog();
                                if (res == DialogResult.OK)
                                {
                                    pj = new Personaje
                                    {
                                        NOMBRE = "",
                                        RAZA = formRaza.RazaSeleccionada,
                                        SUBRAZA = "",
                                        CLASE = "",
                                        TRASFONDO = "",
                                        ALINEAMIENTO = "",
                                        LVL = 1
                                    };

                                    pasoActual = (pj.RAZA == "ELFO" || pj.RAZA == "ENANO") ? "SUBRAZA" : "CLASE";
                                }
                                else // Canceló en RAZA
                                {
                                    this.Show(); // vuelve al menú principal
                                    return;
                                }
                            }
                        }
                        else if (pasoActual == "SUBRAZA")
                        {
                            var formSubraza = new SELECCION_SUBRAZA(pj.RAZA);
                            var res = formSubraza.ShowDialog();

                            if (res == DialogResult.OK)
                            {
                                pj.SUBRAZA = formSubraza.SubrazaSeleccionada;
                                pasoActual = "CLASE";
                            }
                            else if (res == DialogResult.Cancel) // Volver
                            {
                                pasoActual = "RAZA";
                            }
                        }
                        else if (pasoActual == "CLASE")
                        {
                            var formClase = new SELECCION_CLASE(pj);
                            var res = formClase.ShowDialog();

                            if (res == DialogResult.OK)
                            {
                                pj.CLASE = formClase.ClaseSeleccionada;
                                pasoActual = "TRASFONDO";
                            }
                            else if (res == DialogResult.Cancel) // Volver
                            {
                                pasoActual = (pj.RAZA == "ELFO" || pj.RAZA == "ENANO") ? "SUBRAZA" : "RAZA";
                            }
                        }
                        else if (pasoActual == "TRASFONDO")
                        {
                            var formTrasfondo = new SELECCION_TRASFONDO();
                            var res = formTrasfondo.ShowDialog();

                            if (res == DialogResult.OK)
                            {
                                pj.TRASFONDO = formTrasfondo.TrasfondoSeleccionado;
                                pasoActual = "ALINEAMIENTO";
                            }
                            else if (res == DialogResult.Cancel) // Volver
                            {
                                pasoActual = "CLASE";
                            }
                        }
                        else if (pasoActual == "ALINEAMIENTO")
                        {
                            var formAlineamiento = new SELECCION_ALINEAMIENTO();
                            var res = formAlineamiento.ShowDialog();

                            if (res == DialogResult.OK)
                            {
                                pj.ALINEAMIENTO = formAlineamiento.AlineamientoSeleccionado;
                                pasoActual = "RESUMEN";
                            }
                            else if (res == DialogResult.Cancel) // Volver
                            {
                                pasoActual = "TRASFONDO";
                            }
                        }
                        else if (pasoActual == "RESUMEN")
                        {
                            cond.AsignarStatsIniciales(pj);
                            cond.AsignarHabilidades(pj);
                            cond.AsignarArmasYHechizos(pj);

                            var formResumen = new RESUMEN_PERSONAJE(pj);
                            var res = formResumen.ShowDialog();

                            if (res == DialogResult.OK)
                            {
                                // Abre la pantalla de juego y cierra todo lo anterior
                                var mapa = new PANTALLA_JUEGO(pj);
                                mapa.FormClosed += (s3, args3) => Application.Exit();
                                mapa.Show();
                                return;
                            }
                            else if (res == DialogResult.Cancel) // Volver
                            {
                                pasoActual = "ALINEAMIENTO";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al iniciar nueva partida: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Show(); // vuelve al menú principal en caso de error
                }
            };







            AplicarBordesRedondeados(btnNuevaPartida, 30, true, false);

            btnContinuar = CrearBoton("CONTINUAR");
            btnContinuar.Click += (s, e) =>
            {
                PANTALLA_CARGAR cargarForm = new PANTALLA_CARGAR();
                cargarForm.Show();
                this.Hide();
            };

            btnSalir = CrearBoton("SALIR");
            btnSalir.Click += (s, e) => Application.Exit();
            AplicarBordesRedondeados(btnSalir, 30, false, true);

            Controls.Add(btnNuevaPartida);
            Controls.Add(btnContinuar);
            Controls.Add(btnSalir);

            ReposicionarControles();
        }

        private Button CrearBoton(string texto)
        {
            Button btn = new Button()
            {
                Text = texto,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = FUENTE.ObtenerFont(20),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void ReposicionarControles()
        {
            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;

            // -------------------------------------------------------------------- LOGO 
            logoPictureBox.Width = (int)(w * 0.7);
            logoPictureBox.Height = (int)(h * 0.35);
            logoPictureBox.Left = (w - logoPictureBox.Width) / 2;
            logoPictureBox.Top = (h - logoPictureBox.Height) / 3;

            // -------------------------------------------------------------------- BOTONES 
            int botonWidth = (int)(w * 0.25);
            int botonHeight = (int)(h * 0.06);
            int centerX = (w - botonWidth) / 2;

            int startY = logoPictureBox.Bottom + (int)(h * 0.10);

            btnNuevaPartida.SetBounds(centerX, startY, botonWidth, botonHeight);
            btnContinuar.SetBounds(centerX, startY + botonHeight, botonWidth, botonHeight);
            btnSalir.SetBounds(centerX, startY + 2 * botonHeight, botonWidth, botonHeight);
        }

        private void AplicarBordesRedondeados(Button btn, int radio, bool redondearArriba, bool redondearAbajo)
        {
            btn.Paint += (s, e) =>
            {
                GraphicsPath path = new GraphicsPath();

                if (redondearArriba && redondearAbajo)
                {
                    path.AddArc(0, 0, radio, radio, 180, 90);
                    path.AddArc(btn.Width - radio, 0, radio, radio, 270, 90);
                    path.AddArc(btn.Width - radio, btn.Height - radio, radio, radio, 0, 90);
                    path.AddArc(0, btn.Height - radio, radio, radio, 90, 90);
                    path.CloseAllFigures();
                }
                else if (redondearArriba)
                {
                    path.AddArc(0, 0, radio, radio, 180, 90);
                    path.AddArc(btn.Width - radio, 0, radio, radio, 270, 90);
                    path.AddLine(btn.Width, btn.Height, 0, btn.Height);
                    path.CloseAllFigures();
                }
                else if (redondearAbajo)
                {
                    path.AddLine(0, 0, btn.Width, 0);
                    path.AddLine(btn.Width, 0, btn.Width, btn.Height - radio);
                    path.AddArc(btn.Width - radio, btn.Height - radio, radio, radio, 0, 90);
                    path.AddArc(0, btn.Height - radio, radio, radio, 90, 90);
                    path.CloseAllFigures();
                }

                btn.Region = new Region(path);
            };
        }
    }
}
