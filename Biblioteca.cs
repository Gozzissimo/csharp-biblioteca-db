using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_biblioteca_db
{
    public class Biblioteca
    {

        public string Nome { get; set; }
        public List<Scaffale> ScaffaliBiblioteca { get; set; }


        public Biblioteca(string Nome)
        {
            this.Nome = Nome;
            ScaffaliBiblioteca = new List<Scaffale>();

            //Recuperare l'elenco degli scaffali
            List<string> elencoScaffali = db.scaffaliGet();
            elencoScaffali.ForEach(item =>
            {
                Scaffale nuovo = new Scaffale(item);
                ScaffaliBiblioteca.Add(nuovo);
            });

            //Recuperare tutti i dati permanenti
        }

        public void AggiungiScaffale(string nomescaffale, bool addToDb=false)

        {
            Scaffale nuovo = new Scaffale(nomescaffale);

            ScaffaliBiblioteca.Add(nuovo);
            if (addToDb)
            {
                db.scaffaleAdd(nuovo.Numero);
            }
        }

        public void AggiungiLibro(long lCodice, string sTitolo, int iAnno, string sSettore, int iNumPagine, string sScaffale, List<Autore> lListaAutori)
        {
            Libro MioLibro = new Libro(lCodice, sTitolo, iAnno, sSettore, iNumPagine, sScaffale);
            MioLibro.Stato = Stato.Disponibile;
            db.libroAdd(MioLibro, lListaAutori);
        }
        
        public void AggiungiDvd(long lCodice, string sTitolo, int iAnno, string sSettore, int iDurata, string sScaffale, List<Autore> lListaAutori)
        {
            DVD MioDvd = new DVD(lCodice, sTitolo, iAnno, sSettore, iDurata, sScaffale);
            MioDvd.Stato = Stato.Disponibile;
            db.DvdAdd(MioDvd, lListaAutori);
        }

        public int GestisciOperazioneBiblioteca(int iCodiceOperazione)
        {
            List<Documento> lResult;
            string sAppo;
            switch (iCodiceOperazione)
            {
                case 1:
                    Console.WriteLine("inserisci autore");
                    sAppo = Console.ReadLine();
                    lResult = SearchByAutore(sAppo);
                    if (lResult == null)
                        return 1;   //da implementare uscita da inserimento autore
                    else
                        StampaListaDocumenti(lResult);
                    break;

            }
            return 0;
        }
        public void StampaListaDocumenti(List<Documento> lListDoc)
        {
             //da implementare  
        }

        public List<Documento> SearchByCodice(string Codice)
        {
            Console.WriteLine("Metodo da implementare");
            return null;
        }

        public List<Documento> SearchByTitolo(string Titolo)
        {
            Console.WriteLine("Metodo da implementare");
            return null;

        }

        public List<Documento> SearchByAutore(string Titolo)
        {
            Console.WriteLine("Metodo da implementare");

             //connetti al db
             //fare una query, quindi select titolo.scaffale, stato , tipo  from 
             //documenti, autori_documenti,  inner join
             //stampa 

            return null;

        }

        public List<Prestito> SearchPrestiti(string Numero)
        {
            Console.WriteLine("Metodo da implementare");
            return null;
        }

        public List<Prestito> SearchPrestiti(string Nome, string Cognome)
        {
            Console.WriteLine("Metodo da implementare");
            return null; ;
        }
    }
}
