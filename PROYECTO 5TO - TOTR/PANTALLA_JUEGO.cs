using proyecto;

namespace PROYECTO_5TO___TOTЯ
{
    public class PANTALLA_JUEGO : Form
    {
        private Personaje pj;
        private Panel panelMapa;
        private TableLayoutPanel bottomLayout;
        private PictureBox picPersonaje;
        private int[,] mapa;
        private Point posicionJugador;

        private Point camaraPos = new Point(0, 0);
        private int tamañoCelda = 40; //-------------------------------------------------------------- TAMAÑO CELDA
        private int filasVisibles, columnasVisibles;

        private BufferedGraphicsContext contextoBuffer;
        private BufferedGraphics buffer;

        private Bitmap mapaBuffer;

        private static Dictionary<string, Image> cacheImagenes = new Dictionary<string, Image>();
        private Dictionary<int, Image> spritesTerreno = new Dictionary<int, Image>();
        private Button btnCargar, btnCancelar;

        // ------------------------------------------------------------------------------------------ ENEMIGOS
        private List<ENEMIGOS> enemigos = new List<ENEMIGOS>();
        private System.Windows.Forms.Timer timerEnemigos = new System.Windows.Forms.Timer();

        public PANTALLA_JUEGO(Personaje personaje)
        {
            pj = personaje;
            InicializarFormulario();
            CrearInterfaz();
            panelMapa.DoubleBuffered(true);
            InicializarMapa();
            InicializarEnemigos();
            CargarSpritesTerreno();
            MostrarPersonaje();
            GenerarMapaBuffer();


        }

        private void InicializarFormulario()
        {

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;

            Text = "Pantalla de Juego";
            BackColor = Color.Black;

            contextoBuffer = BufferedGraphicsManager.Current;

            // ------------------------------------------------------------------------------------------ TIMER PARA MOVIMIENTO DEENEMIGOS
            timerEnemigos.Interval = 1000;
            timerEnemigos.Tick += (s, e) => MoverEnemigos();
            timerEnemigos.Start();
        }

        private void CrearInterfaz()
        {


            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            Controls.Add(mainLayout);

            // ------------------------------------------------------------------------------------------ PANEL MAPA
            panelMapa = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };
            panelMapa.Paint += DibujarMapa;
            panelMapa.Resize += (s, e) => CalcularCeldasVisibles();
            mainLayout.Controls.Add(panelMapa, 0, 0);

            // ------------------------------------------------------------------------------------------ MENU DE ABAJO
            bottomLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                BackColor = Color.FromArgb(30, 30, 30),
                Padding = new Padding(2)
            };
            bottomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            bottomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.Controls.Add(bottomLayout, 0, 1);

            CrearPanelInferior();
        }

        private void CalcularCeldasVisibles()
        {
            columnasVisibles = (int)Math.Ceiling((double)panelMapa.Width / tamañoCelda);
            filasVisibles = (int)Math.Ceiling((double)panelMapa.Height / tamañoCelda);

            if (panelMapa.Width > 0 && panelMapa.Height > 0)
            {
                buffer?.Dispose();
                buffer = contextoBuffer.Allocate(panelMapa.CreateGraphics(), panelMapa.DisplayRectangle);
            }

            panelMapa.Invalidate();
        }


        private void CrearPanelInferior()
        {
            bottomLayout.Controls.Clear();

            // ------------------------------------------------------------------------------------------ IMAGEN DE PERSONAJE
            picPersonaje = new PictureBox
            {
                Size = new Size(100, 180),
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = new Padding(2),
                Anchor = AnchorStyles.None
            };
            bottomLayout.Controls.Add(picPersonaje, 0, 0);
            CargarImagenPersonajeSegura();

            // ------------------------------------------------------------------------------------------ STATS Y BOTONES
            TableLayoutPanel statsAndButtons = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(2),
                Padding = new Padding(0)
            };
            statsAndButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            statsAndButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            bottomLayout.Controls.Add(statsAndButtons, 1, 0);

            TableLayoutPanel statsColumns = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                Margin = new Padding(2),
                Padding = new Padding(0)
            };
            statsColumns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            statsColumns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            statsColumns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            statsAndButtons.Controls.Add(statsColumns, 0, 0);

            // ------------------------------------------------------------------------------------------ INFO BASICAS
            FlowLayoutPanel infoBasica = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10),
                Margin = new Padding(0),
                AutoSize = true
            };
            infoBasica.Controls.Add(CrearLabel($"NMB: {pj.NOMBRE}"));
            infoBasica.Controls.Add(CrearLabel($"RZA: {pj.RAZA}"));
            infoBasica.Controls.Add(CrearLabel($"SBZ: {pj.SUBRAZA}"));
            infoBasica.Controls.Add(CrearLabel($"CLS: {pj.CLASE}"));
            infoBasica.Controls.Add(CrearLabel($"TRF: {pj.TRASFONDO}"));
            infoBasica.Controls.Add(CrearLabel($"ALN: {pj.ALINEAMIENTO}"));
            statsColumns.Controls.Add(infoBasica, 0, 0);

            // ------------------------------------------------------------------------------------------ STATS
            FlowLayoutPanel atributos = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10),
                Margin = new Padding(0),
                AutoSize = true
            };
            atributos.Controls.Add(CrearLabel($"STR: {pj.STR}"));
            atributos.Controls.Add(CrearLabel($"DEX: {pj.DEX}"));
            atributos.Controls.Add(CrearLabel($"CON: {pj.CON}"));
            atributos.Controls.Add(CrearLabel($"INT: {pj.INT}"));
            atributos.Controls.Add(CrearLabel($"WIS: {pj.WIS}"));
            atributos.Controls.Add(CrearLabel($"CHA: {pj.CHA}"));
            statsColumns.Controls.Add(atributos, 1, 0);

            // ------------------------------------------------------------------------------------------ STATS DE COMBATE
            FlowLayoutPanel combate = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10),
                Margin = new Padding(0),
                AutoSize = true
            };
            combate.Controls.Add(CrearLabel($"HP: {pj.HP}"));
            combate.Controls.Add(CrearLabel($"CA: {pj.CA}"));
            combate.Controls.Add(CrearLabel($"VEL: {pj.VEL}"));
            combate.Controls.Add(CrearLabel($"INI: {pj.INI}"));
            combate.Controls.Add(CrearLabel($"LVL: {pj.LVL}"));
            statsColumns.Controls.Add(combate, 2, 0);

            // ------------------------------------------------------------------------------------------ BOTONES
            Panel panelBotones = new Panel { BackColor = Color.Transparent, Margin = new Padding(2), Padding = new Padding(0) };
            statsAndButtons.Layout += (s, e) =>
            {
                int alto = statsAndButtons.GetRowHeights()[0];
                panelBotones.Size = new Size(alto, alto);
                panelBotones.Dock = DockStyle.Right;
            };

            TableLayoutPanel layoutBotones = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 2 };
            layoutBotones.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            layoutBotones.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            layoutBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layoutBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panelBotones.Controls.Add(layoutBotones);

            string rutaBase = Path.Combine(Application.StartupPath, "Resources");
            layoutBotones.Controls.Add(CrearBotonSeguro(Path.Combine(rutaBase, "MENU.png"), 64, 64, (s, e) => MostrarMenuPausa()), 0, 0);
            layoutBotones.Controls.Add(CrearBotonSeguro(Path.Combine(rutaBase, "MAPA.png"), 64, 64, (s, e) => MostrarMenuMapa()), 0, 0);
            layoutBotones.Controls.Add(CrearBotonSeguro(Path.Combine(rutaBase, "MISIONES.png"), 64, 64, (s, e) => MostrarMenuPausa()), 0, 0);
            layoutBotones.Controls.Add(CrearBotonSeguro(Path.Combine(rutaBase, "INVENTARIO.png"), 64, 64, (s, e) => MostrarMenuPausa()), 0, 0);


            statsAndButtons.Controls.Add(panelBotones, 1, 0);
        }

        private Label CrearLabel(string texto) => new Label
        {
            Text = texto,
            ForeColor = Color.White,
            Font = FUENTE.ObtenerFont(18),
            AutoSize = true,
            Margin = new Padding(2)
        };

        private Button CrearBotonSeguro(string ruta, int ancho, int alto, EventHandler evento)
        {
            Button btn = new Button
            {
                Cursor = Cursors.Hand,
                Text = "",
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Margin = new Padding(1)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn.Click += evento;

            if (File.Exists(ruta))
            {
                if (!cacheImagenes.ContainsKey(ruta))
                    cacheImagenes[ruta] = CargarImagenOpt(ruta, ancho, alto);

                btn.Image = cacheImagenes[ruta];
                btn.ImageAlign = ContentAlignment.MiddleCenter;
            }
            return btn;
        }

        private void CargarImagenPersonajeSegura()
        {
            string rutaBase = Path.Combine(Application.StartupPath, "Resources");
            string ruta = Path.Combine(rutaBase, "DEFAULT.PNG");

            string raza = pj.RAZA?.ToUpper() ?? "";
            string subraza = pj.SUBRAZA?.ToUpper() ?? "";
            string clase = pj.CLASE?.ToUpper() ?? "";

            if (raza == "HUMANO") ruta = Path.Combine(rutaBase, $"HUMANO {clase}.PNG");
            else if (raza == "ORCO") ruta = Path.Combine(rutaBase, $"ORCO {clase}.PNG");
            else if (raza == "ELFO") ruta = Path.Combine(rutaBase, $"{subraza} {clase}.PNG");
            else if (raza == "ENANO") ruta = Path.Combine(rutaBase, $"{subraza} {clase}.PNG");

            if (!File.Exists(ruta)) ruta = Path.Combine(rutaBase, "DEFAULT.PNG");
            if (!File.Exists(ruta)) return;

            int ancho = picPersonaje.Width > 0 ? picPersonaje.Width : 120;
            int alto = picPersonaje.Height > 0 ? picPersonaje.Height : 180;

            if (picPersonaje.Image != null)
            {
                picPersonaje.Image.Dispose();
                picPersonaje.Image = null;
            }

            picPersonaje.Image = CargarImagenOpt(ruta, ancho, alto);
        }

        private Image CargarImagenOpt(string ruta, int ancho, int alto)
        {
            try
            {
                using (var fs = new FileStream(ruta, FileMode.Open, FileAccess.Read))
                using (var imgTemp = Image.FromStream(fs))
                {
                    Bitmap bmp = new Bitmap(ancho, alto);
                    using (Graphics g = Graphics.FromImage(bmp))
                        g.DrawImage(imgTemp, 0, 0, ancho, alto);
                    return bmp;
                }
            }
            catch { return new Bitmap(ancho, alto); }
        }

        private void InicializarMapa()
        {
            int filas = 100;
            int columnas = 100;
            mapa = new int[filas, columnas];

            // TODO CON GRASS COMO BASE POR AHORA
            for (int i = 0; i < filas; i++)
                for (int j = 0; j < columnas; j++)
                    mapa[i, j] = 0;

            // PIEDRA
            for (int i = 0; i < filas; i++)
            {
                mapa[i, 0] = 1;
                mapa[i, columnas - 1] = 1;
            }
            for (int j = 0; j < columnas; j++)
            {
                mapa[0, j] = 1;
                mapa[filas - 1, j] = 1;
            }

            // HOJAS
            mapa[4, 6] = 5;

            // ARENA
            mapa[3, 5] = 2;

            // AGUA
            for (int i = 10; i < 15; i++)
                for (int j = 40; j < 50; j++)
                    mapa[i, j] = 3;

            // CAMINOS
            for (int j = 1; j < columnas - 1; j++)
                mapa[25, j] = 4;

            // ESTRATEGICOS
            mapa[24, 10] = 6; // COFRE
            mapa[25, 50] = 7; // NPC
            mapa[25, 55] = 8; // SHOP
            mapa[25, 75] = 9; // DOOR

            // POSICION DEL JUGADOR INICIAL
            posicionJugador = new Point(2, 2);

            CalcularCeldasVisibles();
        }

        private void CargarSpritesTerreno()
        {
            string rutaBase = Path.Combine(Application.StartupPath, "Resources");

            // PARA PONER TODOS LOS SPRITES DE TERRENO
            spritesTerreno[0] = CargarImagenOpt(Path.Combine(rutaBase, "grass.png"), tamañoCelda, tamañoCelda);
            spritesTerreno[1] = CargarImagenOpt(Path.Combine(rutaBase, "rock.png"), tamañoCelda, tamañoCelda);
            spritesTerreno[2] = CargarImagenOpt(Path.Combine(rutaBase, "sand.png"), tamañoCelda, tamañoCelda);
            spritesTerreno[3] = CargarImagenOpt(Path.Combine(rutaBase, "water.png"), tamañoCelda, tamañoCelda);
            spritesTerreno[4] = CargarImagenOpt(Path.Combine(rutaBase, "path.png"), tamañoCelda, tamañoCelda);
            spritesTerreno[5] = CargarImagenOpt(Path.Combine(rutaBase, "tree.png"), tamañoCelda, tamañoCelda);
            spritesTerreno[6] = CargarImagenOpt(Path.Combine(rutaBase, "chest.png"), tamañoCelda, tamañoCelda);
            spritesTerreno[7] = CargarImagenOpt(Path.Combine(rutaBase, "npc.png"), tamañoCelda, tamañoCelda);
            spritesTerreno[8] = CargarImagenOpt(Path.Combine(rutaBase, "shop.png"), tamañoCelda, tamañoCelda);
            spritesTerreno[9] = CargarImagenOpt(Path.Combine(rutaBase, "door.png"), tamañoCelda, tamañoCelda);

        }



        private void InicializarEnemigos()
        {
            enemigos.Clear();
            enemigos.Add(new ENEMIGOS(new Point(10, 10)));
            enemigos.Add(new ENEMIGOS(new Point(15, 20)));
            enemigos.Add(new ENEMIGOS(new Point(30, 35)));
            enemigos.Add(new ENEMIGOS(new Point(20, 12)));
            enemigos.Add(new ENEMIGOS(new Point(45, 30)));

        }


        private void MoverEnemigos()
        {
            if (juegoPausado) return;

            foreach (var e in enemigos)
            {
                e.Mover(mapa, posicionJugador);

                if (e.HaAtrapadoJugador(posicionJugador))
                {
                    MessageBox.Show("¡Has sido atacado por un enemigo!");
                    // -------------------------------------------------------------------------------------- AQUI INICIARIA EL COMBATE
                }
            }

            panelMapa.Invalidate();
        }


        private void GenerarMapaBuffer()
        {
            int filas = mapa.GetLength(0);
            int columnas = mapa.GetLength(1);
            mapaBuffer = new Bitmap(columnas * tamañoCelda, filas * tamañoCelda);

            using (Graphics g = Graphics.FromImage(mapaBuffer))
            {
                for (int i = 0; i < filas; i++)
                {
                    for (int j = 0; j < columnas; j++)
                    {
                        int tipo = mapa[i, j];
                        if (!spritesTerreno.ContainsKey(tipo)) tipo = 0;

                        g.DrawImage(spritesTerreno[tipo], j * tamañoCelda, i * tamañoCelda, tamañoCelda, tamañoCelda);

                    }
                }
            }
        }


        private void DibujarMapa(object sender, PaintEventArgs e)
        {
            if (mapaBuffer == null) return;

            int anchoVista = panelMapa.Width;
            int altoVista = panelMapa.Height;

            Rectangle origen = new Rectangle(
                camaraPos.X * tamañoCelda,
                camaraPos.Y * tamañoCelda,
                anchoVista,
                altoVista
            );

            e.Graphics.DrawImage(mapaBuffer, new Rectangle(0, 0, anchoVista, altoVista), origen, GraphicsUnit.Pixel);

            // --------------------------------------------------------------------------------------------------------------- DIBUJA AL JUGADOR POR ENCIMA
            int jugadorX = (posicionJugador.X - camaraPos.X) * tamañoCelda;
            int jugadorY = (posicionJugador.Y - camaraPos.Y) * tamañoCelda;
            e.Graphics.FillEllipse(Brushes.Blue, jugadorX, jugadorY, tamañoCelda, tamañoCelda);

            // --------------------------------------------------------------------------------------------------------------- DIBUJA ENEMIGOS POR ENCIMA
            foreach (var enemigo in enemigos)
                enemigo.Dibujar(e.Graphics, camaraPos, tamañoCelda);
        }




        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Point nuevaPos = posicionJugador;
            if (keyData == Keys.W) nuevaPos.Y--;
            else if (keyData == Keys.S) nuevaPos.Y++;
            else if (keyData == Keys.A) nuevaPos.X--;
            else if (keyData == Keys.D) nuevaPos.X++;

            if (nuevaPos.X >= 0 && nuevaPos.Y >= 0 &&
                nuevaPos.X < mapa.GetLength(1) && nuevaPos.Y < mapa.GetLength(0) &&
                mapa[nuevaPos.Y, nuevaPos.X] != 1)
            {
                posicionJugador = nuevaPos;

                camaraPos.X = Math.Max(0, Math.Min(posicionJugador.X - columnasVisibles / 2, mapa.GetLength(1) - columnasVisibles));
                camaraPos.Y = Math.Max(0, Math.Min(posicionJugador.Y - filasVisibles / 2, mapa.GetLength(0) - filasVisibles));

                panelMapa.Invalidate();

                if (mapa[nuevaPos.Y, nuevaPos.X] == 2)
                {
                    MessageBox.Show($"Has interactuado con un punto de interés en X={nuevaPos.X}, Y={nuevaPos.Y}");
                    mapa[nuevaPos.Y, nuevaPos.X] = 0;
                }
            }

            if (mapa[nuevaPos.Y, nuevaPos.X] == 6)
                MessageBox.Show("Has encontrado un cofre con oro!");
            else if (mapa[nuevaPos.Y, nuevaPos.X] == 7)
                MessageBox.Show("NPC: ¡Hola aventurero! Tengo una misión para ti.");
            else if (mapa[nuevaPos.Y, nuevaPos.X] == 8)
                MessageBox.Show("Bienvenido a la tienda. (Aquí podrías abrir un formulario más adelante)");
            else if (mapa[nuevaPos.Y, nuevaPos.X] == 9)
                MessageBox.Show("Entraste a la mazmorra principal.");




            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MostrarPersonaje()
        {
            Label lblNombre = new Label
            {
                Text = pj.NOMBRE,
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = Color.White,
                Font = FUENTE.ObtenerFont(20)
            };
            Controls.Add(lblNombre);
        }




















        // ---------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------ MENU DE PAUSA
        // ---------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------------------------------------------------------------



        private bool juegoPausado = false;
        private Panel panelMenuPausa;
        private int indiceSubMenu = 0;
        private List<string> subMenus = new List<string> { "MENÚ", "INVENTARIO", "MISIONES", "MAPA" };
        private PictureBox picPersonajeMenu;
        private Label lblStatsMenu;

        private Bitmap CapturarMapa()
        {
            Bitmap bmp = new Bitmap(panelMapa.Width, panelMapa.Height);
            panelMapa.DrawToBitmap(bmp, new Rectangle(0, 0, panelMapa.Width, panelMapa.Height));
            return bmp;

        }

        private void MostrarMenuPausa()
        {
            if (panelMenuPausa != null && panelMenuPausa.Visible) return;

            juegoPausado = true;

            // ------------------------------------------------------------------------------------------ FONDO CONGELADO DEL MAPA
            panelMenuPausa = new Panel
            {
                Dock = DockStyle.Fill,
                BackgroundImage = CapturarMapa(),
                BackgroundImageLayout = ImageLayout.Stretch,
            };

            // ------------------------------------------------------------------------------------------ OVERLAY SEMITRANSPARENTE PARA VER MAPA
            Panel overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(120, 0, 0, 0)
            };
            panelMenuPausa.Controls.Add(overlay);

            // ------------------------------------------------------------------------------------------ VENTANA DE MENU  
            Panel ventanaMenu = new Panel
            {
                Size = new Size(500, 500),
                BackColor = Color.FromArgb(220, 40, 40, 40),
                Location = new Point((overlay.Width - 500) / 2, (overlay.Height - 500) / 2)
            };
            overlay.Controls.Add(ventanaMenu);
            ventanaMenu.Anchor = AnchorStyles.None;
            RedondearMenu(ventanaMenu, 35);

            // ------------------------------------------------------------------------------------------ BOTÓN CERRAR 
            Button btnCerrar = new Button
            {
                Text = "X",
                Font = FUENTE.ObtenerFont(18),
                Size = new Size(45, 45),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Location = new Point(5, 5)
            };

            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnCerrar.FlatAppearance.MouseDownBackColor = Color.Transparent;

            btnCerrar.MouseEnter += (s, e) => btnCerrar.ForeColor = Color.Red;
            btnCerrar.MouseLeave += (s, e) => btnCerrar.ForeColor = Color.White;

            btnCerrar.Click += (s, e) => CerrarMenuPausa();
            ventanaMenu.Controls.Add(btnCerrar);

            // ------------------------------------------------------------------------------------------ IMAGEN DEL PERSONAJE
            int anchoNuevo = 135;
            int altoNuevo = 245;

            Bitmap bmpPersonajeEscalado = new Bitmap(anchoNuevo, altoNuevo);
            using (Graphics g = Graphics.FromImage(bmpPersonajeEscalado))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                g.DrawImage(picPersonaje.Image, 0, 0, anchoNuevo, altoNuevo);
            }

            picPersonajeMenu = new PictureBox
            {
                Image = bmpPersonajeEscalado,
                SizeMode = PictureBoxSizeMode.Normal,
                Size = new Size(anchoNuevo, altoNuevo),
                Location = new Point(50, 40)
            };
            ventanaMenu.Controls.Add(picPersonajeMenu);


            // ------------------------------------------------------------------------------------------ PANEL DE ESTADÍSTICAS 
            lblStatsMenu = new Label
            {
                Text = $"NMB: {pj.NOMBRE}\nDEX: {pj.DEX}\nCON: {pj.CON}\nINT: {pj.INT}\nWIS: {pj.WIS}\nCHA: {pj.CHA}\nHP: {pj.HP}\nCA: {pj.CA}\nVEL: {pj.VEL}\nINI: {pj.INI}",
                ForeColor = Color.White,
                Font = FUENTE.ObtenerFont(18),
                Size = new Size(250, 250),
                Location = new Point(ventanaMenu.Width - 270, 50)
            };
            ventanaMenu.Controls.Add(lblStatsMenu);

            // ------------------------------------------------------------------------------------------ PANEL DE BOTONES
            FlowLayoutPanel panelBotones = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                Size = new Size(490, 400),
                Location = new Point(ventanaMenu.Width - 490, 300),
                AutoScroll = true,
                WrapContents = false
            };
            ventanaMenu.Controls.Add(panelBotones);

            panelBotones.Controls.Add(CrearBotonMenu("Guardar", Color.DodgerBlue, (s, e) =>
            {
                GuardarPartida();
            }));

            panelBotones.Controls.Add(CrearBotonMenu("Guardar y Salir", Color.MediumPurple, (s, e) =>
            {
                GuardarPartida();
                var mainForm = new MENU_PRINCIPAL();
                mainForm.Show();
                this.Hide();
            }));

            panelBotones.Controls.Add(CrearBotonMenu("Salir del Juego", Color.Firebrick, (s, e) =>
            {
                Application.Exit();
            }));

            // ------------------------------------------------------------------------------------------ FLECHAS DEL CARRUSEL
            Button flechaIzq = new Button
            {
                Text = "<",
                Width = 50,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                Font = FUENTE.ObtenerFont(30),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Location = new Point(10, (this.ClientSize.Height - 50) / 2)
            };
            flechaIzq.FlatAppearance.BorderSize = 0;
            flechaIzq.FlatAppearance.MouseOverBackColor = Color.Transparent;
            flechaIzq.FlatAppearance.MouseDownBackColor = Color.Transparent;
            flechaIzq.MouseEnter += (s, e) => flechaIzq.ForeColor = Color.DarkGray;
            flechaIzq.MouseLeave += (s, e) => flechaIzq.ForeColor = Color.White;
            flechaIzq.Click += (s, e) => MessageBox.Show("Funcionalidad de flecha izquierda no implementada aún.");
            overlay.Controls.Add(flechaIzq);

            Button flechaDer = new Button
            {
                Text = ">",
                Width = 50,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                Font = FUENTE.ObtenerFont(30),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Location = new Point(this.ClientSize.Width - 50 - 10, (this.ClientSize.Height - 50) / 2)
            };
            flechaDer.FlatAppearance.BorderSize = 0;
            flechaDer.FlatAppearance.MouseOverBackColor = Color.Transparent;
            flechaDer.FlatAppearance.MouseDownBackColor = Color.Transparent;
            flechaDer.MouseEnter += (s, e) => flechaDer.ForeColor = Color.DarkGray;
            flechaDer.MouseLeave += (s, e) => flechaDer.ForeColor = Color.White;

            flechaDer.Click += (s, e) => MessageBox.Show("Funcionalidad de flecha derecha no implementada aún.");
            overlay.Controls.Add(flechaDer);

            // ------------------------------------------------------------------------------------------ AÑADIR PANEL DE MENÚ A FORM 
            this.Controls.Add(panelMenuPausa);
            panelMenuPausa.BringToFront();
        }

        private Button CrearBotonMenu(string texto, Color color, EventHandler click)
        {
            Button btn = new Button
            {
                Text = texto,
                Width = 480,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = FUENTE.ObtenerFont(18),
                Margin = new Padding(0, 10, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            RedondearBoton(btn, 25);
            btn.Click += click;
            return btn;
        }

        private void CerrarMenuPausa()
        {
            if (panelMenuPausa != null)
            {
                this.Controls.Remove(panelMenuPausa);
                panelMenuPausa.Dispose();
                panelMenuPausa = null;
            }
            juegoPausado = false;
        }

        private void GuardarPartida()
        {
            MessageBox.Show("¡Partida guardada correctamente!");
            // ----------------------------------------------------------------------------- AQUI PARA ACTUALIZAR BASE DE DATOS EN GUARDADO
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

        private void RedondearMenu(Panel panel, int radio)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.StartFigure();
            path.AddArc(0, 0, radio, radio, 180, 90);
            path.AddArc(panel.Width - radio, 0, radio, radio, 270, 90);
            path.AddArc(panel.Width - radio, panel.Height - radio, radio, radio, 0, 90);
            path.AddArc(0, panel.Height - radio, radio, radio, 90, 90);
            path.CloseFigure();
            panel.Region = new Region(path);
        }



































        
        private Panel panelMenuMapa;          
        private Panel picContainerMenu;  
        private PictureBox picMapaMenu; 
        private List<Point> marcasMapa = new List<Point>(); 
        private Bitmap mapaBaseBitmapMenu; 
        private float mapaZoom = 1.0f; 
        private float mapaFitZoom = 1.0f; 
        private float mapaZoomMin = 0.2f;
        private float mapaZoomMax = 5.0f;
        private bool mapaDragging = false;
        private Point mapaDragStartPoint;
        private Point mapaDragStartScroll;
        private int mapaCellBaseSize = 20;

        private void MostrarMenuMapa()
        {
            if (panelMenuMapa != null && panelMenuMapa.Visible) return;
            juegoPausado = true;

            panelMenuMapa = new Panel
            {
                Dock = DockStyle.Fill,
                BackgroundImage = CapturarMapa(),
                BackgroundImageLayout = ImageLayout.Stretch
            };
            this.Controls.Add(panelMenuMapa);
            panelMenuMapa.BringToFront();

            Panel overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(160, 0, 0, 0)
            };
            panelMenuMapa.Controls.Add(overlay);

            // ------------------------------------------------------------------------------------------ TAMAÑO DE LA VENTANA
            int tamañoVentana = Math.Min(this.ClientSize.Width-20, this.ClientSize.Height) - 80;

            Panel ventanaMapa = new Panel
            {
                Size = new Size(tamañoVentana, tamañoVentana),
                BackColor = Color.FromArgb(230, 20, 20, 20),
                Location = new Point((overlay.Width - tamañoVentana) / 2, (overlay.Height - tamañoVentana) / 2)
            };
            overlay.Controls.Add(ventanaMapa);
            RedondearMenu(ventanaMapa, 26);

            // ------------------------------------------------------------------------------------------ BOTON CERRAR
            Button btnCerrar = new Button
            {
                Text = "X",
                Font = FUENTE.ObtenerFont(18),
                Size = new Size(30, 30),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Location = new Point(5, 5)
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnCerrar.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnCerrar.MouseEnter += (s, e) => btnCerrar.ForeColor = Color.Red;
            btnCerrar.MouseLeave += (s, e) => btnCerrar.ForeColor = Color.White;
            RedondearBoton(btnCerrar, 26);
            btnCerrar.Click += (s, e) => CerrarMenuMapa();
            ventanaMapa.Controls.Add(btnCerrar);

            // ------------------------------------------------------------------------------------------ FLECHAS DEL CARRUSEL
            Button flechaIzq = new Button
            {
                Text = "<",
                Width = 50,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                Font = FUENTE.ObtenerFont(30),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Location = new Point(10, (this.ClientSize.Height - 50) / 2)
            };
            flechaIzq.FlatAppearance.BorderSize = 0;
            flechaIzq.FlatAppearance.MouseOverBackColor = Color.Transparent;
            flechaIzq.FlatAppearance.MouseDownBackColor = Color.Transparent;
            flechaIzq.MouseEnter += (s, e) => flechaIzq.ForeColor = Color.DarkGray;
            flechaIzq.MouseLeave += (s, e) => flechaIzq.ForeColor = Color.White;
            flechaIzq.Click += (s, e) => MessageBox.Show("Funcionalidad de flecha izquierda no implementada aún.");
            overlay.Controls.Add(flechaIzq);

            Button flechaDer = new Button
            {
                Text = ">",
                Width = 50,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                Font = FUENTE.ObtenerFont(30),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Location = new Point(this.ClientSize.Width - 50 - 10, (this.ClientSize.Height - 50) / 2)
            };
            flechaDer.FlatAppearance.BorderSize = 0;
            flechaDer.FlatAppearance.MouseOverBackColor = Color.Transparent;
            flechaDer.FlatAppearance.MouseDownBackColor = Color.Transparent;
            flechaDer.MouseEnter += (s, e) => flechaDer.ForeColor = Color.DarkGray;
            flechaDer.MouseLeave += (s, e) => flechaDer.ForeColor = Color.White;

            flechaDer.Click += (s, e) => MessageBox.Show("Funcionalidad de flecha derecha no implementada aún.");
            overlay.Controls.Add(flechaDer);

            // ------------------------------------------------------------------------------------------ PANEL CONTENEDOR DEL MAPA
            picContainerMenu = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(ventanaMapa.Width - 20, ventanaMapa.Height - 20),
                AutoScroll = false,
                BackColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle
            };
            ventanaMapa.Controls.Add(picContainerMenu);

            // ------------------------------------------------------------------------------------------ PICTUREBOX DEL MAPA
            picMapaMenu = new PictureBox
            {
                Location = new Point(0, 0),
                BackColor = Color.Black
            };
            picContainerMenu.Controls.Add(picMapaMenu);
            btnCerrar.BringToFront();

            GenerarBaseBitmap();

            float ratioX = (float)picContainerMenu.ClientSize.Width / mapaBaseBitmapMenu.Width;
            float ratioY = (float)picContainerMenu.ClientSize.Height / mapaBaseBitmapMenu.Height;
            mapaZoom = mapaFitZoom = Math.Min(ratioX, ratioY); 
            mapaZoomMin = mapaFitZoom;
            mapaZoomMax = mapaFitZoom * 20f;

            UpdateScaledImage(null);

            picMapaMenu.MouseDown += PicMapaMenu_MouseDown;
            picMapaMenu.MouseMove += PicMapaMenu_MouseMove;
            picMapaMenu.MouseUp += PicMapaMenu_MouseUp;
            picMapaMenu.MouseClick += PicMapaMenu_MouseClick;
        }


        // --------------------------------------------------------------------------------------------- GENERAR TERRENO MARCAS JUGADOR
        private void GenerarBaseBitmap()
        {
            try { mapaBaseBitmapMenu?.Dispose(); } catch { }
            int filas = (mapa != null) ? mapa.GetLength(0) : 1;
            int columnas = (mapa != null) ? mapa.GetLength(1) : 1;

            int ancho = columnas * mapaCellBaseSize;
            int alto = filas * mapaCellBaseSize;
            if (ancho <= 0) ancho = 1;
            if (alto <= 0) alto = 1;

            mapaBaseBitmapMenu = new Bitmap(ancho, alto);
            using (Graphics g = Graphics.FromImage(mapaBaseBitmapMenu))
            {
                g.Clear(Color.Black);

                // ------------------------------------------------------------------------------------------ TERRENO CON CUADRÍCULA DE COLORES
                for (int y = 0; y < filas; y++)
                {
                    for (int x = 0; x < columnas; x++)
                    {
                        int tipo = mapa[y, x];
                        Color c = tipo switch
                        {
                            0 => Color.FromArgb(24, 140, 24),   // PASTO
                            1 => Color.FromArgb(120, 120, 120), // PIEDRA
                            2 => Color.FromArgb(220, 200, 150), // ARENA
                            3 => Color.FromArgb(36, 130, 200),  // AGUA
                            4 => Color.FromArgb(140, 110, 60),  // CAMINO
                            5 => Color.FromArgb(18, 90, 18),    // ARBOL
                            6 => Color.Gold,                    // COFRE
                            7 => Color.Orange,                  // NPC
                            8 => Color.Violet,                  // TIENDA
                            9 => Color.Brown,                   // PUERTA
                            _ => Color.DarkGreen
                        };
                        using (Brush b = new SolidBrush(c))
                        {
                            g.FillRectangle(b, x * mapaCellBaseSize, y * mapaCellBaseSize, mapaCellBaseSize, mapaCellBaseSize);
                        }
                    }
                }

                // ------------------------------------------------------------------------------------------ MARCAS CON ETIQUETAS
                using (Brush bm = new SolidBrush(Color.Black))
                using (Pen pm = new Pen(Color.Black, 1))
                using (Font f = FUENTE.ObtenerFont(15))
                {
                    foreach (var m in marcasMapa)
                    {
                        int cx = m.X * mapaCellBaseSize;
                        int cy = m.Y * mapaCellBaseSize;
                        int size = Math.Max(6, mapaCellBaseSize - 2);
                        g.FillEllipse(bm, cx + (mapaCellBaseSize - size) / 2, cy + (mapaCellBaseSize - size) / 2, size, size);
                        g.DrawEllipse(pm, cx + (mapaCellBaseSize - size) / 2, cy + (mapaCellBaseSize - size) / 2, size, size);

                        
                        string txt = $"{m}";
                        SizeF ts = g.MeasureString(txt, f);
                        int tx = cx + mapaCellBaseSize;
                        int ty = cy;
                        
                        if (tx + ts.Width > mapaBaseBitmapMenu.Width) tx = cx - (int)ts.Width - 2;
                        if (ty + ts.Height > mapaBaseBitmapMenu.Height) ty = cy - (int)ts.Height - 2;
                        g.FillRectangle(Brushes.Black, tx - 2, ty - 1, (int)ts.Width + 4, (int)ts.Height + 2);
                        g.DrawString(txt, f, Brushes.White, tx, ty);
                    }
                }

                // ------------------------------------------------------------------------------------------ JUGADOR
                if (posicionJugador != Point.Empty)
                {
                    using (Brush bp = new SolidBrush(Color.Salmon))
                    {
                        int jx = posicionJugador.X * mapaCellBaseSize;
                        int jy = posicionJugador.Y * mapaCellBaseSize;
                        int psize = Math.Max(mapaCellBaseSize, mapaCellBaseSize + 4);
                        g.FillEllipse(bp, jx + (mapaCellBaseSize - psize) / 2, jy + (mapaCellBaseSize - psize) / 2, psize, psize);
                        g.DrawEllipse(Pens.Black, jx + (mapaCellBaseSize - psize) / 2, jy + (mapaCellBaseSize - psize) / 2, psize, psize);
                    }
                }
            }
        }

        private void UpdateScaledImage(Point? mouseInContainer)
        {
            if (mapaBaseBitmapMenu == null || picMapaMenu == null || picContainerMenu == null) return;

            int newW = (int)(mapaBaseBitmapMenu.Width * mapaZoom);
            int newH = (int)(mapaBaseBitmapMenu.Height * mapaZoom);

            Bitmap scaled = new Bitmap(newW, newH);
            using (Graphics g = Graphics.FromImage(scaled))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(mapaBaseBitmapMenu, new Rectangle(0, 0, newW, newH));
            }

            var prev = picMapaMenu.Image;
            picMapaMenu.Image = scaled;
            picMapaMenu.Size = scaled.Size;
            try { prev?.Dispose(); } catch { }

            if (Math.Abs(mapaZoom - mapaFitZoom) < 0.0001f)
            {
                picMapaMenu.Location = new Point(0, 0);
            }
            else
            {
                
                if (mouseInContainer.HasValue)
                {
                    Point mouse = mouseInContainer.Value;
                    float relX = (mouse.X - picMapaMenu.Left) / (float)picMapaMenu.Width;
                    float relY = (mouse.Y - picMapaMenu.Top) / (float)picMapaMenu.Height;

                    int newLeft = mouse.X - (int)(relX * newW);
                    int newTop = mouse.Y - (int)(relY * newH);

                    if (picMapaMenu.Width > picContainerMenu.ClientSize.Width)
                    {
                        if (newLeft > 0) newLeft = 0;
                        if (newLeft + newW < picContainerMenu.ClientSize.Width) newLeft = picContainerMenu.ClientSize.Width - newW;
                    }
                    else
                    {
                        newLeft = (picContainerMenu.ClientSize.Width - newW) / 2;
                    }

                    if (picMapaMenu.Height > picContainerMenu.ClientSize.Height)
                    {
                        if (newTop > 0) newTop = 0;
                        if (newTop + newH < picContainerMenu.ClientSize.Height) newTop = picContainerMenu.ClientSize.Height - newH;
                    }
                    else
                    {
                        newTop = (picContainerMenu.ClientSize.Height - newH) / 2;
                    }

                    picMapaMenu.Location = new Point(newLeft, newTop);
                }
                else
                {
                    int cx = (int)(posicionJugador.X * mapaCellBaseSize * mapaZoom - picContainerMenu.ClientSize.Width / 2);
                    int cy = (int)(posicionJugador.Y * mapaCellBaseSize * mapaZoom - picContainerMenu.ClientSize.Height / 2);
                    picMapaMenu.Location = new Point(-cx, -cy);
                }
            }
        }

        private void PicMapaMenu_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle)
            {
                mapaDragging = true;
                mapaDragStartPoint = Cursor.Position;
                mapaDragStartScroll = picMapaMenu.Location;
                picMapaMenu.Cursor = Cursors.Hand;
            }
        }

        private void PicMapaMenu_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mapaDragging) return;

            Point cur = Cursor.Position;
            int dx = cur.X - mapaDragStartPoint.X;
            int dy = cur.Y - mapaDragStartPoint.Y;

            int newX = mapaDragStartScroll.X + dx;
            int newY = mapaDragStartScroll.Y + dy;

            if (picMapaMenu.Width > picContainerMenu.ClientSize.Width)
            {
                if (newX > 0) newX = 0;
                if (newX + picMapaMenu.Width < picContainerMenu.ClientSize.Width)
                    newX = picContainerMenu.ClientSize.Width - picMapaMenu.Width;
            }
            else
            {
                newX = (picContainerMenu.ClientSize.Width - picMapaMenu.Width) / 2;
            }

            if (picMapaMenu.Height > picContainerMenu.ClientSize.Height)
            {
                if (newY > 0) newY = 0;
                if (newY + picMapaMenu.Height < picContainerMenu.ClientSize.Height)
                    newY = picContainerMenu.ClientSize.Height - picMapaMenu.Height;
            }
            else
            {
                newY = (picContainerMenu.ClientSize.Height - picMapaMenu.Height) / 2;
            }

            picMapaMenu.Location = new Point(newX, newY);
        }

        private void PicMapaMenu_MouseUp(object sender, MouseEventArgs e)
        {
            mapaDragging = false;
            picMapaMenu.Cursor = Cursors.Default;
        }

        private void PicMapaMenu_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            Point imgPoint = picMapaMenu.PointToClient(Cursor.Position);
            int imgX = (int)((imgPoint.X - picMapaMenu.Left) / mapaZoom);
            int imgY = (int)((imgPoint.Y - picMapaMenu.Top) / mapaZoom);

            int cellX = imgX / mapaCellBaseSize;
            int cellY = imgY / mapaCellBaseSize;

            if (cellX < 0 || cellY < 0 || cellX >= mapa.GetLength(1) || cellY >= mapa.GetLength(0)) return;

            Point nueva = new Point(cellX, cellY);
            if (marcasMapa.Contains(nueva)) marcasMapa.Remove(nueva);
            else marcasMapa.Add(nueva);

            GenerarBaseBitmap();
            UpdateScaledImage(picContainerMenu.PointToClient(Cursor.Position));
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (panelMenuMapa == null || picContainerMenu == null)
            {
                base.OnMouseWheel(e);
                return;
            }

            Point mouseInContainer = picContainerMenu.PointToClient(Cursor.Position);
            if (picContainerMenu.ClientRectangle.Contains(mouseInContainer))
            {
                float factor = e.Delta > 0 ? 1.5f : 0.7f; // ------------------------------------------ ZOOM IN / OUT
                mapaZoom *= factor;
                if (mapaZoom < mapaZoomMin) mapaZoom = mapaZoomMin;
                if (mapaZoom > mapaZoomMax) mapaZoom = mapaZoomMax;

                UpdateScaledImage(mouseInContainer);
                return;
            }

            base.OnMouseWheel(e);
        }

        private void CerrarMenuMapa()
        {
            try
            {
                if (picMapaMenu != null)
                {
                    picMapaMenu.MouseDown -= PicMapaMenu_MouseDown;
                    picMapaMenu.MouseMove -= PicMapaMenu_MouseMove;
                    picMapaMenu.MouseUp -= PicMapaMenu_MouseUp;
                    picMapaMenu.MouseClick -= PicMapaMenu_MouseClick;
                }
                try { picContainerMenu.MouseEnter -= (s, e) => picContainerMenu.Focus(); } catch { }

                if (picMapaMenu != null)
                {
                    try { picMapaMenu.Image?.Dispose(); } catch { }
                    picMapaMenu.Image = null;
                }

                try { mapaBaseBitmapMenu?.Dispose(); } catch { mapaBaseBitmapMenu = null; }

                if (panelMenuMapa != null)
                {
                    this.Controls.Remove(panelMenuMapa);
                    try { panelMenuMapa.Dispose(); } catch { }
                    panelMenuMapa = null;
                }
            }
            finally
            {
                juegoPausado = false;
            }
        }
    }


    public static class Extensiones
    {
        public static void DoubleBuffered(this Control c, bool valor)
        {
            System.Reflection.PropertyInfo aProp =
                  typeof(Control).GetProperty("DoubleBuffered",
                  System.Reflection.BindingFlags.NonPublic |
                  System.Reflection.BindingFlags.Instance);
            aProp.SetValue(c, valor, null);
        }
    }

}
