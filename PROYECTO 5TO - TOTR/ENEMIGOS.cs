namespace proyecto
{
    public class ENEMIGOS
    {
        public Point Posicion { get; set; }
        public Point RangoOrigen { get; set; }
        public int RangoPatrulla { get; set; } = 10;
        public int Velocidad { get; set; } = 1;
        public Color Color { get; set; } = Color.Red;

        private Point objetivoActual;

        public ENEMIGOS(Point posicion)
        {
            Posicion = posicion;
            RangoOrigen = new Point(Math.Max(0, posicion.X - RangoPatrulla / 2), Math.Max(0, posicion.Y - RangoPatrulla / 2));
            objetivoActual = GetNuevoObjetivo();
        }

        private Point GetNuevoObjetivo()
        {
            Random rnd = new Random();
            int x = rnd.Next(RangoOrigen.X, RangoOrigen.X + RangoPatrulla);
            int y = rnd.Next(RangoOrigen.Y, RangoOrigen.Y + RangoPatrulla);
            return new Point(x, y);
        }

        public void Mover(int[,] mapa, Point jugadorPos)
        {
            int dx = jugadorPos.X - Posicion.X;
            int dy = jugadorPos.Y - Posicion.Y;
            int distanciaJugador = Math.Abs(dx) + Math.Abs(dy);

            if (distanciaJugador <= RangoPatrulla)
            {
                MoverHacia(jugadorPos, mapa);
            }
            else
            {
                if (Posicion == objetivoActual)
                    objetivoActual = GetNuevoObjetivo();
                MoverHacia(objetivoActual, mapa);
            }
        }

        private void MoverHacia(Point destino, int[,] mapa)
        {
            Point nuevaPos = Posicion;

            int dx = destino.X - Posicion.X;
            int dy = destino.Y - Posicion.Y;

            if (Math.Abs(dx) > Math.Abs(dy))
                nuevaPos.X += Math.Sign(dx) * Velocidad;
            else
                nuevaPos.Y += Math.Sign(dy) * Velocidad;

            if (nuevaPos.X >= 0 && nuevaPos.Y >= 0 &&
                nuevaPos.X < mapa.GetLength(1) && nuevaPos.Y < mapa.GetLength(0) &&
                mapa[nuevaPos.Y, nuevaPos.X] != 1)
            {
                Posicion = nuevaPos;
            }
        }

        public bool HaAtrapadoJugador(Point jugadorPos) => Posicion == jugadorPos;

        public void Dibujar(Graphics g, Point camaraPos, int tamañoCelda)
        {
            int ex = (Posicion.X - camaraPos.X) * tamañoCelda;
            int ey = (Posicion.Y - camaraPos.Y) * tamañoCelda;
            using (Brush b = new SolidBrush(Color))
                g.FillRectangle(b, ex, ey, tamañoCelda, tamañoCelda);
        }
    }

}