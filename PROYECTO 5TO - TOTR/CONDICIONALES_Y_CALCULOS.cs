namespace proyecto
{
    public class CONDICIONALES_Y_CALCULOS
    {
        // -------------------------------------------------------------------- SUBRAZAS Y CLASES 
        private readonly Dictionary<string, List<string>> SUBRAZAS_POR_RAZA = new()
        {
            { "HUMANO", new List<string>() },
            { "ORCO", new List<string>() },
            { "ELFO", new List<string>{ "ALTO ELFO", "ELFO SILVANO", "ELFO OSCURO" } },
            { "ENANO", new List<string>{ "ENANO DE LA MONTAÑA", "ENANO DE LAS COLINAS" } }
        };

        private readonly Dictionary<string, List<string>> CLASES_POR_RAZA = new()
        {
            { "HUMANO", new List<string>{ "GUERRERO", "MAGO", "CLERIGO", "PICARO" } },
            { "ORCO", new List<string>{ "GUERRERO", "MAGO", "CLERIGO", "PICARO" } },
            { "ELFO", new List<string>{ "GUERRERO", "MAGO", "CLERIGO", "PICARO" } },
            { "ENANO", new List<string>{ "GUERRERO", "MAGO", "CLERIGO", "PICARO" } }
        };

        public List<string> ObtenerTrasfondos()
        {
            return HABILIDADES_POR_TRASFONDO.Keys.ToList();
        }


        // -------------------------------------------------------------------- HABILIDADES 
        private readonly Dictionary<string, List<string>> HABILIDADES_POR_CLASE = new()
        {
            { "GUERRERO", new List<string>{ "ATLETISMO", "INTIMIDACION", "SUPERVIVENCIA" } },
            { "PICARO", new List<string>{ "ACROBACIAS", "SIGILO", "ENGANO", "PERSUASION", "PERCEPCION", "JUEGO DE MANOS" } },
            { "MAGO", new List<string>{ "ARCANO", "HISTORIA", "INVESTIGACION", "NATURALEZA" } },
            { "CLERIGO", new List<string>{ "MEDICINA", "RELIGION" } }
        };

        private readonly Dictionary<string, List<string>> HABILIDADES_POR_TRASFONDO = new()
        {
            { "SOLDADO", new List<string>{ "ATLETISMO", "INTIMIDACION" } },
            { "MERCADER", new List<string>{ "PERSUASION", "ENGANO" } },
            { "SABIO", new List<string>{ "HISTORIA", "ARCANO" } },
            { "EXPLORADOR", new List<string>{ "SUPERVIVENCIA", "PERCEPCION" } },
            { "CRIMINAL", new List<string>{ "SIGILO", "ENGANO" } },
            { "ARTESANO", new List<string>{ "HISTORIA", "PERSUASION" } },
            { "NOBLE", new List<string>{ "HISTORIA", "PERSUASION" } },
            { "FORASTERO", new List<string>{ "ATLETISMO", "SUPERVIVENCIA" } },
            { "HERMITANO", new List<string>{ "RELIGION", "MEDICINA" } },
            { "CHARLATAN", new List<string>{ "ENGANO", "JUEGO DE MANOS" } }
        };

        private readonly Dictionary<string, string> STAT_POR_HABILIDAD = new()
        {
            { "ACROBACIAS", "DEX" }, { "ATLETISMO", "STR" }, { "SIGILO", "DEX" }, { "ENGANO", "CHA" },
            { "INTIMIDACION", "CHA" }, { "PERSUASION", "CHA" }, { "SUPERVIVENCIA", "WIS" }, { "ARCANO", "INT" },
            { "HISTORIA", "INT" }, { "INVESTIGACION", "INT" }, { "NATURALEZA", "INT" }, { "RELIGION", "WIS" },
            { "PERCEPCION", "WIS" }, { "MEDICINA", "WIS" }, { "JUEGO DE MANOS", "DEX" }
        };

        // -------------------------------------------------------------------- ARMAS Y HECHIZOS POR CLASE 
        private readonly Dictionary<string, List<string>> ARMAS_POR_CLASE = new()
        {
            { "GUERRERO", new List<string>{ "ESPADA LARGA", "ESCUDO" } },
            { "MAGO", new List<string>{ "BASTON", "DAGA" } },
            { "CLERIGO", new List<string>{ "MARTILLO", "ESCUDO" } },
            { "PICARO", new List<string>{ "ESPADA CORTA", "DAGA" } },
        };

        private readonly Dictionary<string, List<string>> HECHIZOS_POR_CLASE = new()
        {
            { "MAGO", new List<string>{ "MISIL MAGICO", "PROTECCION" } },
            { "CLERIGO", new List<string>{ "CURAR HERIDAS", "BENDICION" } }
        };

        // -------------------------------------------------------------------- MÉTODOS 


        public List<string> ObtenerSubrazas(string raza) =>
            SUBRAZAS_POR_RAZA.ContainsKey(raza) ? SUBRAZAS_POR_RAZA[raza] : new List<string>();

        public List<string> ObtenerClases(string raza) =>
            CLASES_POR_RAZA.ContainsKey(raza) ? CLASES_POR_RAZA[raza] : new List<string>();


        // -------------------------------------------------------------------- ASIGNAR STATS INICIALES PERO ESTO SE TIENE QUE CAMBIAR PARA CUANDO PONGAMOS LOS DADOS
        public void AsignarStatsIniciales(Personaje p)
        {
            p.STR = 10; p.DEX = 10; p.CON = 10; p.INT = 10; p.WIS = 10; p.CHA = 10;

            // BONIFICADORES POR RAZA
            switch (p.RAZA)
            {
                case "ELFO": p.DEX += 2; break;
                case "ORCO": p.STR += 2; break;
                case "ENANO": p.CON += 2; break;
            }

            // BONIFICADORES POR CLASE
            switch (p.CLASE)
            {
                case "MAGO": p.INT += 2; break;
                case "CLERIGO": p.WIS += 2; break;
                case "GUERRERO": p.STR += 1; break;
            }

            CalcularDerivados(p);
        }

        // CALCULOS PARA HP, CA, VEL E INI
        public void CalcularDerivados(Personaje p)
        {
            // HP
            p.HP = p.CLASE switch
            {
                "GUERRERO" => 10 + p.MODCON,
                "PICARO" => 8 + p.MODCON,
                "MAGO" => 6 + p.MODCON,
                "CLERIGO" => 8 + p.MODCON,
                _ => 5 + p.MODCON
            };

            // BONIFICADOR POR SUBRAZA
            if (p.SUBRAZA == "ENANO DE COLINAS") p.HP += p.LVL;

            // CA
            p.CA = p.CLASE switch
            {
                "GUERRERO" => 16,
                "PICARO" => 14,
                "MAGO" => 12,
                "CLERIGO" => 18,
                _ => 10
            };

            // VEL
            p.VEL = p.RAZA switch
            {
                "HUMANO" => 30,
                "ORCO" => 30,
                "ELFO" => 30,
                "ENANO" => 25,
                _ => 30
            };

            if (p.SUBRAZA == "ELFO SILVANO") p.VEL += 5;

            // INI
            p.INI = p.MODDEX;
        }

        // PARA ASIGNAR LAS HABILIDADES Y EVITAR DUPLICADOS POR CLASE Y TRASFONDO
        public void AsignarHabilidades(Personaje p)
        {
            p.HABILIDADES.Clear();
            var habilidades = new HashSet<string>();

            // HABILIDADES POR CLASE
            if (HABILIDADES_POR_CLASE.ContainsKey(p.CLASE))
                habilidades.UnionWith(HABILIDADES_POR_CLASE[p.CLASE]);

            // HABILIDADES POR TRASFONDO
            if (HABILIDADES_POR_TRASFONDO.ContainsKey(p.TRASFONDO))
                habilidades.UnionWith(HABILIDADES_POR_TRASFONDO[p.TRASFONDO]);

            // CREAR HABILIDADES
            foreach (var h in habilidades)
            {
                int mod = h switch
                {
                    "ATLETISMO" => p.MODSTR,
                    "ACROBACIAS" => p.MODDEX,
                    "SIGILO" => p.MODDEX,
                    "ENGANO" => p.MODCHA,
                    "INTIMIDACION" => p.MODCHA,
                    "PERSUASION" => p.MODCHA,
                    "SUPERVIVENCIA" => p.MODWIS,
                    "ARCANO" => p.MODINT,
                    "HISTORIA" => p.MODINT,
                    "INVESTIGACION" => p.MODINT,
                    "NATURALEZA" => p.MODINT,
                    "RELIGION" => p.MODWIS,
                    "PERCEPCION" => p.MODWIS,
                    "MEDICINA" => p.MODWIS,
                    "JUEGO DE MANOS" => p.MODDEX,
                    _ => 0
                };

                // SOLKO ALGUNAS HABILDADES SE LES DA BONIFICADOR DE COMPETENCIA
                int competencia = new List<string> { "ATLETISMO", "ACROBACIAS", "SIGILO", "ENGANO", "INTIMIDACION", "PERSUASION" }.Contains(h)
                    ? p.BonificadorCompetencia()
                    : 0;

                p.HABILIDADES.Add(new Habilidad
                {
                    NOMBRE = h,
                    STAT_ASOCIADO = STAT_POR_HABILIDAD.ContainsKey(h) ? STAT_POR_HABILIDAD[h] : "",
                    MODIFICADOR_STAT = mod,
                    BONIFICADOR_COMPETENCIA = competencia,
                    TOTAL = mod + competencia
                });
            }
        }

        // ASIGNAR ARMAS Y HECHIZOS SEGUN LA CLASE
        public void AsignarArmasYHechizos(Personaje p)
        {
            p.ARMAS.Clear();
            p.HECHIZOS.Clear();

            if (ARMAS_POR_CLASE.ContainsKey(p.CLASE))
                foreach (var a in ARMAS_POR_CLASE[p.CLASE])
                    p.ARMAS.Add(a);

            if (HECHIZOS_POR_CLASE.ContainsKey(p.CLASE))
                foreach (var h in HECHIZOS_POR_CLASE[p.CLASE])
                    p.HECHIZOS.Add(h);
        }
    }
}
