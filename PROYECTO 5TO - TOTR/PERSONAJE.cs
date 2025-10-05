using System.ComponentModel;

namespace proyecto
{
    public class Personaje
    {
        // -------------------------------------------------------------------- PROPIEDADES BÁSICAS 
        public int ID { get; set; }
        public string NOMBRE { get; set; } = "";
        public string RAZA { get; set; } = "";
        public string SUBRAZA { get; set; } = "";
        public string CLASE { get; set; } = "";
        public string TRASFONDO { get; set; } = "";
        public string ALINEAMIENTO { get; set; } = "";
        public int LVL { get; set; } = 1;

        // -------------------------------------------------------------------- ESTADÍSTICAS 
        public int STR { get; set; }
        public int DEX { get; set; }
        public int CON { get; set; }
        public int INT { get; set; }
        public int WIS { get; set; }
        public int CHA { get; set; }

        // -------------------------------------------------------------------- MODIFICADORES 
        public int MODSTR => (STR - 10) / 2;
        public int MODDEX => (DEX - 10) / 2;
        public int MODCON => (CON - 10) / 2;
        public int MODINT => (INT - 10) / 2;
        public int MODWIS => (WIS - 10) / 2;
        public int MODCHA => (CHA - 10) / 2;

        // -------------------------------------------------------------------- DERIVADOS 
        public int HP { get; set; }
        public int CA { get; set; }
        public int VEL { get; set; }
        public int INI { get; set; }

        // -------------------------------------------------------------------- INVENTARIO 
        public BindingList<string> ARMAS { get; set; } = new();
        public BindingList<string> HECHIZOS { get; set; } = new();

        // -------------------------------------------------------------------- HABILIDADES 
        public BindingList<Habilidad> HABILIDADES { get; set; } = new();

        // -------------------------------------------------------------------- MÉTODOS 
        public void RecalcularDerivados()
        {
            // HP SEGUN CLASE + MOD CON
            HP = CLASE.ToUpper() switch
            {
                "GUERRERO" => 10 + MODCON,
                "PICARO" => 8 + MODCON,
                "MAGO" => 6 + MODCON,
                "CLERIGO" => 8 + MODCON,
                _ => 5 + MODCON
            };

            // CA SEGUN CLASE
            CA = CLASE.ToUpper() switch
            {
                "GUERRERO" => 16,
                "PICARO" => 14,
                "MAGO" => 12,
                "CLERIGO" => 18,
                _ => 10
            };

            // VEL SEGUN RAZA 
            VEL = RAZA.ToUpper() switch
            {
                "HUMANO" => 30,
                "ENANO" => 25,
                "ELFO" => 30,
                "ORCO" => 30,
                _ => 30
            };
        }

        // CALCULA EL BONIFICADOR DE COMPETENCIA SEGUN EL LVL
        public int BonificadorCompetencia() => LVL switch
        {
            >= 17 => 6,
            >= 13 => 5,
            >= 9 => 4,
            >= 5 => 3,
            _ => 2
        };
    }

    public class Habilidad
    {
        public string NOMBRE { get; set; }
        public string STAT_ASOCIADO { get; set; }
        public int MODIFICADOR_STAT { get; set; }
        public int BONIFICADOR_COMPETENCIA { get; set; }
        public int TOTAL { get; set; }

    }
}
