using System.Drawing.Text;

namespace PROYECTO_5TO___TOTЯ
{
    public static class FUENTE
    {
        private static PrivateFontCollection pfc = new PrivateFontCollection();

        public static void CargarFuente()
        {
            if (pfc.Families.Length > 0) return;

            string rutaFuente = Path.Combine(Application.StartupPath, "Resources", "Minecraft.ttf");
            if (File.Exists(rutaFuente))
                pfc.AddFontFile(rutaFuente);
        }

        public static Font ObtenerFont(float tamaño)
        {
            if (pfc.Families.Length > 0)
                return new Font(pfc.Families[0], tamaño);
            else
                return new Font("Segoe UI", tamaño);
        }
    }
}
