using proyecto;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

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

        // Cámara
        private Point camaraPos = new Point(0, 0);
        private int tamañoCelda = 32; // Cada celda mide 32x32 px
        private int filasVisibles, columnasVisibles;

        // BufferedGraphics para evitar parpadeo
        private BufferedGraphicsContext contextoBuffer;
        private BufferedGraphics buffer;

        // Mapa en memoria
        private Bitmap mapaBuffer;

        // Sprites (comentados hasta que los tengas)
        // private Dictionary<int, Image> sprites = new Dictionary<int, Image>();

        private static Dictionary<string, Image> cacheImagenes = new Dictionary<string, Image>();
        private Dictionary<int, Image> spritesTerreno = new Dictionary<int, Image>();


        // ENEMIGOS
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
            Text = "Pantalla de Juego";
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;
            BackColor = Color.Black;
            KeyPreview = true;

            contextoBuffer = BufferedGraphicsManager.Current;

            // Timer para movimiento de enemigos
            timerEnemigos.Interval = 500; // cada 0.5 segundos
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

            // Panel mapa
            panelMapa = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };
            panelMapa.Paint += DibujarMapa;
            panelMapa.Resize += (s, e) => CalcularCeldasVisibles();
            mainLayout.Controls.Add(panelMapa, 0, 0);

            // Panel inferior
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

            // Imagen personaje
            picPersonaje = new PictureBox
            {
                Size = new Size(100, 180),
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = new Padding(2),
                Anchor = AnchorStyles.None
            };
            bottomLayout.Controls.Add(picPersonaje, 0, 0);
            CargarImagenPersonajeSegura();

            // Panel stats + botones
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

            // Stats en 3 columnas
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

            // Info básica
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

            // Atributos
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

            // Combate
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

            // Botones
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
            layoutBotones.Controls.Add(CrearBotonSeguro(Path.Combine(rutaBase, "MENU.png"), 64, 64, (s, e) => MessageBox.Show("Menú")), 0, 0);
            layoutBotones.Controls.Add(CrearBotonSeguro(Path.Combine(rutaBase, "MAPA.png"), 64, 64, (s, e) => MessageBox.Show("Mapa")), 1, 0);
            layoutBotones.Controls.Add(CrearBotonSeguro(Path.Combine(rutaBase, "MISIONES.png"), 64, 64, (s, e) => MessageBox.Show("Misiones")), 0, 1);
            layoutBotones.Controls.Add(CrearBotonSeguro(Path.Combine(rutaBase, "INVENTARIO.png"), 64, 64, (s, e) => MessageBox.Show("Inventario")), 1, 1);

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
            Button btn = new Button { Text = "", Dock = DockStyle.Fill, FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, Margin = new Padding(1) };
            btn.FlatAppearance.BorderSize = 0;
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
            int filas = 50;
            int columnas = 80;
            mapa = new int[filas, columnas];

            // Todo grass como base
            for (int i = 0; i < filas; i++)
                for (int j = 0; j < columnas; j++)
                    mapa[i, j] = 0;

            // Bordes con rocas
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

            // Bosques
            for (int i = 5; i < 20; i++)
                for (int j = 5; j < 15; j++)
                    mapa[i, j] = 5;

            // Desiertos
            for (int i = 30; i < 40; i++)
                for (int j = 60; j < 75; j++)
                    mapa[i, j] = 2;

            // Lago
            for (int i = 10; i < 15; i++)
                for (int j = 40; j < 50; j++)
                    mapa[i, j] = 3;

            // Camino principal
            for (int j = 1; j < columnas - 1; j++)
                mapa[25, j] = 4;

            // Cofres, NPCs, tiendas y puerta
            mapa[24, 10] = 6; // cofre
            mapa[24, 20] = 6;
            mapa[24, 30] = 6;
            mapa[25, 50] = 7; // NPC
            mapa[25, 55] = 8; // tienda
            mapa[25, 75] = 9; // puerta

            // Posición inicial jugador
            posicionJugador = new Point(2, 2);

            CalcularCeldasVisibles();
        }

        private void CargarSpritesTerreno()
        {
            string rutaBase = Path.Combine(Application.StartupPath, "Resources");

            // Cargamos los sprites
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
            foreach (var e in enemigos)
            {
                e.Mover(mapa, posicionJugador);

                if (e.HaAtrapadoJugador(posicionJugador))
                {
                    MessageBox.Show("¡Has sido atacado por un enemigo!");
                    // Aplicar daño, reiniciar posición, etc.
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

                        // Opcional: bordes de celda
                         //g.DrawRectangle(Pens.Black, j * tamañoCelda, i * tamañoCelda, tamañoCelda, tamañoCelda);
                    }
                }
            }
        }


        private void DibujarMapa(object sender, PaintEventArgs e)
        {
            if (mapaBuffer == null) return;

            // Solo dibuja la parte visible del mapa (no recrear mapaBuffer)
            int anchoVista = panelMapa.Width;
            int altoVista = panelMapa.Height;

            Rectangle origen = new Rectangle(
                camaraPos.X * tamañoCelda,
                camaraPos.Y * tamañoCelda,
                anchoVista,
                altoVista
            );

            // Dibujar mapa estático
            // Dibujar mapa estático usando Rectangle
            e.Graphics.DrawImage(mapaBuffer, new Rectangle(0, 0, anchoVista, altoVista), origen, GraphicsUnit.Pixel);

            // Dibujar jugador encima
            int jugadorX = (posicionJugador.X - camaraPos.X) * tamañoCelda;
            int jugadorY = (posicionJugador.Y - camaraPos.Y) * tamañoCelda;
            e.Graphics.FillEllipse(Brushes.Blue, jugadorX, jugadorY, tamañoCelda, tamañoCelda);

            // Dibujar enemigos encima
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
