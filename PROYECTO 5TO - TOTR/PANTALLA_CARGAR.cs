using PROYECTO_5TO___TOTЯ;
using System.ComponentModel;
using System.Data.SQLite;

namespace proyecto
{
    public class PANTALLA_CARGAR : Form
    {
        private ListBox lstPersonajes;
        private Button btnCargar, btnCancelar;
        private Panel panelInfo;
        private Label lblInfo;
        private Image fondo;
        private Label lblTitulo = null!;



        public PANTALLA_CARGAR()
        {
            FUENTE.CargarFuente();
            InicializarFormulario();
            CrearControles();
            CargarPersonajesDesdeBD();
        }

        private void InicializarFormulario()
        {
            Text = "Cargar Partida";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;

            string rutaFondo = Path.Combine(Application.StartupPath, "Resources", "fondores.png");
            if (File.Exists(rutaFondo))
                fondo = Image.FromFile(rutaFondo);

            this.Paint += PANTALLA_CARGAR_Paint;
            this.Resize += (s, e) => this.Invalidate();
        }

        private void CrearControles()
        {
            // ---------------------------------------------------------------------------- TITULO 
            lblTitulo = new Label()
            {
                Text = "CARGAR PARTIDA",
                Font = FUENTE.ObtenerFont(40),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };
            Controls.Add(lblTitulo);

            int tituloWidth = 900;
            int tituloHeight = 60;
            lblTitulo.Size = new Size(tituloWidth, tituloHeight);

            lblTitulo.Left = (this.ClientSize.Width - lblTitulo.Width) / 2;
            lblTitulo.Top = 30;
            lblTitulo.BringToFront();

            this.Resize += (s, e) =>
            {
                lblTitulo.Left = (this.ClientSize.Width - lblTitulo.Width) / 2;
            };

            // ---------------------------------------------------------------------------- BOTON CERRAR
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
            btnCancelar.Click += (s, e) => { var menu = new MENU_PRINCIPAL(); menu.Show(); this.Close(); };
            Controls.Add(btnCancelar);
            btnCancelar.BringToFront();
            btnCancelar.Left = 10;
            btnCancelar.Top = 10;

            // ---------------------------------------------------------------------------- LAYOUT PRINCIPAL 
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(30)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));
            Controls.Add(mainLayout);

            // ---------------------------------------------------------------------------- PANEL LISTA DE PARTIDAS GUARDADAS Y BOTON 
            TableLayoutPanel leftLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3
            };
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            mainLayout.Controls.Add(leftLayout, 0, 0);

            lstPersonajes = new ListBox
            {
                Font = FUENTE.ObtenerFont(25),
                Dock = DockStyle.Fill,
                IntegralHeight = false,
                Padding = new Padding(0, 20, 0, 0)
            };
            lstPersonajes.SelectedIndexChanged += LstPersonajes_SelectedIndexChanged;
            leftLayout.Controls.Add(lstPersonajes, 0, 1);

            btnCargar = new Button
            {
                Text = "CARGAR",
                Font = FUENTE.ObtenerFont(20),
                Dock = DockStyle.Fill
            };
            btnCargar.Click += BtnCargar_Click;
            Panel buttonPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 0) };
            buttonPanel.Controls.Add(btnCargar);
            leftLayout.Controls.Add(buttonPanel, 0, 2);

            // ---------------------------------------------------------------------------- PANEL CON STATS DE LA PARTIDA SELECCIONADA 
            Panel rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = true
            };
            mainLayout.Controls.Add(rightPanel, 1, 0);

            TableLayoutPanel rightLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3
            };
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            rightPanel.Controls.Add(rightLayout);

            lblInfo = new Label
            {
                Font = FUENTE.ObtenerFont(22),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(10),
                AutoSize = true
            };
            rightLayout.Controls.Add(lblInfo, 0, 1);
        }


        private void PANTALLA_CARGAR_Paint(object sender, PaintEventArgs e)
        {
            if (fondo != null)
                e.Graphics.DrawImage(fondo, new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));
        }

        private void CargarPersonajesDesdeBD()
        {
            lstPersonajes.Items.Clear();

            try
            {
                using (var conexion = new SQLiteConnection("Data Source=dnd_totr.sqlite;Version=3;"))
                {
                    conexion.Open();
                    string query = "SELECT NOMBRE FROM PERSONAJES";
                    using (var cmd = new SQLiteCommand(query, conexion))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nombre = reader["NOMBRE"].ToString()!;
                            lstPersonajes.Items.Add(nombre);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los personajes: " + ex.Message);
            }
        }

        private void LstPersonajes_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstPersonajes.SelectedIndex == -1) return;
            string nombre = lstPersonajes.SelectedItem.ToString()!;
            MostrarInfoPersonaje(nombre);
        }

        private void MostrarInfoPersonaje(string nombre)
        {
            try
            {
                using (var conexion = new SQLiteConnection("Data Source=dnd_totr.sqlite;Version=3;"))
                {
                    conexion.Open();
                    string query = "SELECT * FROM PERSONAJES WHERE NOMBRE=@nombre";
                    using (var cmd = new SQLiteCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Personaje pj = new Personaje()
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    NOMBRE = reader["NOMBRE"].ToString()!,
                                    RAZA = reader["RAZA"].ToString()!,
                                    SUBRAZA = reader["SUBRAZA"].ToString()!,
                                    CLASE = reader["CLASE"].ToString()!,
                                    TRASFONDO = reader["TRASFONDO"].ToString()!,
                                    ALINEAMIENTO = reader["ALINEAMIENTO"].ToString()!,
                                    LVL = Convert.ToInt32(reader["LVL"]),
                                    STR = Convert.ToInt32(reader["STR"]),
                                    DEX = Convert.ToInt32(reader["DEX"]),
                                    CON = Convert.ToInt32(reader["CON"]),
                                    INT = Convert.ToInt32(reader["INTE"]),
                                    WIS = Convert.ToInt32(reader["WIS"]),
                                    CHA = Convert.ToInt32(reader["CHA"]),
                                    HP = Convert.ToInt32(reader["HP"]),
                                    CA = Convert.ToInt32(reader["CA"]),
                                    VEL = Convert.ToInt32(reader["VEL"]),
                                    INI = Convert.ToInt32(reader["INI"])
                                };

                                pj.HABILIDADES = new BindingList<Habilidad>(BASE_DE_DATOS.ObtenerHabilidades(pj.ID));
                                pj.ARMAS = new BindingList<string>(BASE_DE_DATOS.ObtenerArmas(pj.ID));
                                pj.HECHIZOS = new BindingList<string>(BASE_DE_DATOS.ObtenerHechizos(pj.ID));


                                lblInfo.Text = $"NMB: {pj.NOMBRE}\n" +
                                               $"RZA: {pj.RAZA}\n" +
                                               $"SBZ: {pj.SUBRAZA}\n" +
                                               $"CLS: {pj.CLASE}\n" +
                                               $"TRF: {pj.TRASFONDO}\n" +
                                               $"ALN: {pj.ALINEAMIENTO}\n\n" +
                                               $"STR: {pj.STR}\n" +
                                               $"DEX: {pj.DEX}\n" +
                                               $"CON: {pj.CON}\n" +
                                               $"INT: {pj.INT}\n" +
                                               $"WIS: {pj.WIS}\n" +
                                               $"CHA: {pj.CHA}\n\n" +
                                               $"HP: {pj.HP}\n" +
                                               $"CA: {pj.CA}\n" +
                                               $"VEL: {pj.VEL}\n" +
                                               $"INI: {pj.INI}\n" +
                                               $"LVL: {pj.LVL}\n\n";

                                /*"HABILIDADES:\n" + string.Join("\n", pj.HABILIDADES.Select(h =>
                                    $"{h.NOMBRE} ({h.STAT_ASOCIADO}) - MOD:{h.MODIFICADOR_STAT}, COMP:{h.BONIFICADOR_COMPETENCIA}, TOTAL:{h.TOTAL}")) +
                                "\n\nARMAS:\n" + string.Join("\n", pj.ARMAS) +
                                "\n\nHECHIZOS:\n" + string.Join("\n", pj.HECHIZOS);*/
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar info del personaje: " + ex.Message);
            }
        }


        private void BtnCargar_Click(object? sender, EventArgs e)
        {
            if (lstPersonajes.SelectedIndex == -1)
            {
                MessageBox.Show("Selecciona un personaje para cargar");
                return;
            }

            string nombre = lstPersonajes.SelectedItem.ToString()!;

            try
            {
                using (var conexion = new SQLiteConnection("Data Source=dnd_totr.sqlite;Version=3;"))
                {
                    conexion.Open();
                    string query = "SELECT * FROM PERSONAJES WHERE NOMBRE=@nombre";
                    using (var cmd = new SQLiteCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Personaje pj = new Personaje()
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    NOMBRE = reader["NOMBRE"].ToString(),
                                    RAZA = reader["RAZA"].ToString(),
                                    SUBRAZA = reader["SUBRAZA"].ToString(),
                                    CLASE = reader["CLASE"].ToString(),
                                    TRASFONDO = reader["TRASFONDO"].ToString(),
                                    ALINEAMIENTO = reader["ALINEAMIENTO"].ToString(),
                                    LVL = Convert.ToInt32(reader["LVL"]),
                                    STR = Convert.ToInt32(reader["STR"]),
                                    DEX = Convert.ToInt32(reader["DEX"]),
                                    CON = Convert.ToInt32(reader["CON"]),
                                    INT = Convert.ToInt32(reader["INTE"]),
                                    WIS = Convert.ToInt32(reader["WIS"]),
                                    CHA = Convert.ToInt32(reader["CHA"]),
                                    HP = Convert.ToInt32(reader["HP"]),
                                    CA = Convert.ToInt32(reader["CA"]),
                                    VEL = Convert.ToInt32(reader["VEL"]),
                                    INI = Convert.ToInt32(reader["INI"])

                                };

                                var pantallaJuego = new PANTALLA_JUEGO(pj);
                                pantallaJuego.Show();
                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el personaje: " + ex.Message);
            }
        }
    }
}
