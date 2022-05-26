using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_biblioteca_db
{
    public class Autore : Persona
    {
        public long iCodiceAutore;
        public string sMail;
        public Autore(string Nome, string Cognome, string sPosta) : base(Nome, Cognome)
        {
            sMail = sPosta;
            iCodiceAutore = GeneraCodiceAutore();
        }
        public long GeneraCodiceAutore()
        {
            return db.GetUniqueId();
        }
    }

}
