using System.Collections.Generic;

namespace ItsMyDoliprane.Generator
{
    public class FileMedicament
    {
        public string Nom { get; set; }
        public List<FilePosologie> Posologies { get; } = new List<FilePosologie>();
    }

    public class FilePosologie
    {
        public string Categorie { get; set; }
        public int DureeHeures { get; set; }
        public List<FileRegle> Regles { get; } = new List<FileRegle>();
    }

    public class FileRegle
    {
        public string Type { get; set; }
        public string Medicament { get; set; }
        public List<FilePlage> Plages { get; } = new List<FilePlage>();
    }

    public class FilePlage
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public string Avis { get; set; }
    }
}
