using System.Data.SQLite;

namespace proyecto
{
    public static class BASE_DE_DATOS
    {
        private static string rutaBD = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dnd_totr.sqlite");
        private static string cadenaConexion = $"Data Source={rutaBD};Version=3;";


        // --------------------------------------------------------------------- ESTO ES SOLO PA CREAR TABLAS Y ESO SI NO EXISTE PERO LO PUSE PARA PROBAR
        static BASE_DE_DATOS()
        {
            try
            {
                if (!File.Exists(rutaBD))
                {
                    SQLiteConnection.CreateFile(rutaBD);
                }

                using var conn = new SQLiteConnection(cadenaConexion);
                conn.Open();

                string crearPersonajes = @"CREATE TABLE IF NOT EXISTS Personajes (ID INTEGER PRIMARY KEY AUTOINCREMENT, NOMBRE TEXT, RAZA TEXT, SUBRAZA TEXT, CLASE TEXT, TRASFONDO TEXT, ALINEAMIENTO TEXT, LVL INTEGER, STR INTEGER, DEX INTEGER, CON INTEGER, INTE INTEGER, WIS INTEGER, CHA INTEGER, HP INTEGER, CA INTEGER, VEL INTEGER, INI INTEGER);";
                string crearHabilidades = @"CREATE TABLE IF NOT EXISTS habilidades (ID INTEGER PRIMARY KEY AUTOINCREMENT, PersonajeID INTEGER, NOMBRE TEXT, STAT_ASOCIADO TEXT, MODIFICADOR_STAT INTEGER, BONIFICADOR_COMPETENCIA INTEGER, TOTAL INTEGER);";
                string crearArmas = @"CREATE TABLE IF NOT EXISTS armas (ID INTEGER PRIMARY KEY AUTOINCREMENT, PersonajeID INTEGER, NOMBRE TEXT);";
                string crearHechizos = @"CREATE TABLE IF NOT EXISTS hechizos (ID INTEGER PRIMARY KEY AUTOINCREMENT, PersonajeID INTEGER, NOMBRE TEXT);";

                new SQLiteCommand(crearPersonajes, conn).ExecuteNonQuery();
                new SQLiteCommand(crearHabilidades, conn).ExecuteNonQuery();
                new SQLiteCommand(crearArmas, conn).ExecuteNonQuery();
                new SQLiteCommand(crearHechizos, conn).ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error al inicializar la base de datos: " + ex.Message);
            }
        }


        // --------------------------------------------------------------------- GUARDAR PERSONAJE
        public static void GuardarPersonaje(Personaje p)
        {
            using var conn = new SQLiteConnection(cadenaConexion);
            conn.Open();

            string queryPersonaje = @"
            INSERT INTO Personajes 
            (NOMBRE, RAZA, SUBRAZA, CLASE, TRASFONDO, ALINEAMIENTO, LVL, STR, DEX, CON, INTE, WIS, CHA, HP, CA, VEL, INI)
            VALUES
            (@NOMBRE,@RAZA,@SUBRAZA,@CLASE,@TRASFONDO,@ALINEAMIENTO,@LVL,@STR,@DEX,@CON,@INTE,@WIS,@CHA,@HP,@CA,@VEL,@INI);";

            using (var cmd = new SQLiteCommand(queryPersonaje, conn))
            {
                cmd.Parameters.AddWithValue("@NOMBRE", p.NOMBRE);
                cmd.Parameters.AddWithValue("@RAZA", p.RAZA);
                cmd.Parameters.AddWithValue("@SUBRAZA", p.SUBRAZA);
                cmd.Parameters.AddWithValue("@CLASE", p.CLASE);
                cmd.Parameters.AddWithValue("@TRASFONDO", p.TRASFONDO);
                cmd.Parameters.AddWithValue("@ALINEAMIENTO", p.ALINEAMIENTO);
                cmd.Parameters.AddWithValue("@LVL", p.LVL);
                cmd.Parameters.AddWithValue("@STR", p.STR);
                cmd.Parameters.AddWithValue("@DEX", p.DEX);
                cmd.Parameters.AddWithValue("@CON", p.CON);
                cmd.Parameters.AddWithValue("@INTE", p.INT);
                cmd.Parameters.AddWithValue("@WIS", p.WIS);
                cmd.Parameters.AddWithValue("@CHA", p.CHA);
                cmd.Parameters.AddWithValue("@HP", p.HP);
                cmd.Parameters.AddWithValue("@CA", p.CA);
                cmd.Parameters.AddWithValue("@VEL", p.VEL);
                cmd.Parameters.AddWithValue("@INI", p.INI);
                cmd.ExecuteNonQuery();
            }

            long idPersonaje;
            using (var cmdId = new SQLiteCommand("SELECT last_insert_rowid();", conn))
            {
                idPersonaje = (long)cmdId.ExecuteScalar();
            }

            foreach (var h in p.HABILIDADES)
            {
                string queryH = @"INSERT INTO habilidades (PersonajeID, NOMBRE, STAT_ASOCIADO, MODIFICADOR_STAT, BONIFICADOR_COMPETENCIA, TOTAL)
                                  VALUES (@ID_PERSONAJE,@NOMBRE,@STAT,@MOD,@BON,@TOTAL);";
                using var cmdH = new SQLiteCommand(queryH, conn);
                cmdH.Parameters.AddWithValue("@ID_PERSONAJE", idPersonaje);
                cmdH.Parameters.AddWithValue("@NOMBRE", h.NOMBRE);
                cmdH.Parameters.AddWithValue("@STAT", h.STAT_ASOCIADO);
                cmdH.Parameters.AddWithValue("@MOD", h.MODIFICADOR_STAT);
                cmdH.Parameters.AddWithValue("@BON", h.BONIFICADOR_COMPETENCIA);
                cmdH.Parameters.AddWithValue("@TOTAL", h.TOTAL);
                cmdH.ExecuteNonQuery();
            }

            foreach (var a in p.ARMAS)
            {
                string queryA = @"INSERT INTO armas (PersonajeID, NOMBRE) VALUES (@ID_PERSONAJE,@NOMBRE);";
                using var cmdA = new SQLiteCommand(queryA, conn);
                cmdA.Parameters.AddWithValue("@ID_PERSONAJE", idPersonaje);
                cmdA.Parameters.AddWithValue("@NOMBRE", a);
                cmdA.ExecuteNonQuery();
            }

            foreach (var h in p.HECHIZOS)
            {
                string queryH = @"INSERT INTO hechizos (PersonajeID, NOMBRE) VALUES (@ID_PERSONAJE,@NOMBRE);";
                using var cmdH = new SQLiteCommand(queryH, conn);
                cmdH.Parameters.AddWithValue("@ID_PERSONAJE", idPersonaje);
                cmdH.Parameters.AddWithValue("@NOMBRE", h);
                cmdH.ExecuteNonQuery();
            }
        }

        // --------------------------------------------------------------------- OBTENER PERSONAJES PARA LO DEL RESUMEN
        public static List<Personaje> ObtenerPersonajes()
        {
            var personajes = new List<Personaje>();
            using var conn = new SQLiteConnection(cadenaConexion);
            conn.Open();

            string query = "SELECT * FROM Personajes;";
            using var cmd = new SQLiteCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Personaje p = new Personaje
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
                personajes.Add(p);
            }

            return personajes;
        }



















        // Dentro de la clase BASE_DE_DATOS
        public static List<Habilidad> ObtenerHabilidades(int personajeID)
        {
            var lista = new List<Habilidad>();
            using var conn = new SQLiteConnection(cadenaConexion);
            conn.Open();

            string query = "SELECT * FROM habilidades WHERE PersonajeID = @ID;";
            using var cmd = new SQLiteCommand(query, conn);
            cmd.Parameters.AddWithValue("@ID", personajeID);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Habilidad
                {
                    NOMBRE = reader["NOMBRE"].ToString(),
                    STAT_ASOCIADO = reader["STAT_ASOCIADO"].ToString(),
                    MODIFICADOR_STAT = Convert.ToInt32(reader["MODIFICADOR_STAT"]),
                    BONIFICADOR_COMPETENCIA = Convert.ToInt32(reader["BONIFICADOR_COMPETENCIA"]),
                    TOTAL = Convert.ToInt32(reader["TOTAL"])
                });
            }

            return lista;
        }

        public static List<string> ObtenerArmas(int personajeID)
        {
            var lista = new List<string>();
            using var conn = new SQLiteConnection(cadenaConexion);
            conn.Open();

            string query = "SELECT NOMBRE FROM armas WHERE PersonajeID = @ID;";
            using var cmd = new SQLiteCommand(query, conn);
            cmd.Parameters.AddWithValue("@ID", personajeID);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(reader["NOMBRE"].ToString());
            }

            return lista;
        }

        public static List<string> ObtenerHechizos(int personajeID)
        {
            var lista = new List<string>();
            using var conn = new SQLiteConnection(cadenaConexion);
            conn.Open();

            string query = "SELECT NOMBRE FROM hechizos WHERE PersonajeID = @ID;";
            using var cmd = new SQLiteCommand(query, conn);
            cmd.Parameters.AddWithValue("@ID", personajeID);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(reader["NOMBRE"].ToString());
            }

            return lista;
        }





    }
}
