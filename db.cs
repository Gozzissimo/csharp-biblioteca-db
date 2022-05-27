using System.Data.SqlClient;
namespace csharp_biblioteca_db
{
    internal class db
    {
        
        private static string stringaDiConnessione = "Data Source=localhost;Initial Catalog=Biblioteca;Integrated Security=True;Pooling=False";
        
        //funzione per connettere il db
        private static SqlConnection Connect()
        {
            SqlConnection conn = new SqlConnection(stringaDiConnessione);

            try
            {
                conn.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return conn;

        }
        internal static long GetUniqueId()
        {
            var conn = Connect();
            if (conn == null)
                throw new Exception("Unable to connect to the dabatase");

            string cmd = "UPDATE codiceunico SET codice = codice + 1 OUTPUT INSERTED.codice";
            long id;
            using (SqlCommand select = new SqlCommand(cmd, conn))
            {
                using (SqlDataReader reader = select.ExecuteReader())
                {
                    reader.Read();
                    id = reader.GetInt64(0);
                }
            }
            conn.Close();
            return id;
        }
        internal static bool DoSql(SqlConnection conn, string sql)
        {

            using (SqlCommand sqlCmd = new SqlCommand(sql, conn))
            {
                try
                {
                    sqlCmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
        internal static int DvdAdd(DVD dvd, List<Autore> lAutori)
        {
            //devo collegarmi e inviare un comando di insert del nuovo scaffale
            var conn = Connect();
            if (conn == null)
            {
                throw new System.Exception("Unable to connect to database");
            }

            //----------------

            //Verifico se gli autori esistono
            string cmdAutori;
            foreach (Autore autore in lAutori)
            {
                long lCodiceAutore = 0;
                int iInserFlag = 0;
                cmdAutori = string.Format("select codice from Autori where Nome='{0}' and Cognome='{1}' and mail='{2}'", autore.Nome, autore.Cognome, autore.sMail);
                using (SqlCommand select = new SqlCommand(cmdAutori, conn))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        Console.WriteLine(reader.FieldCount);
                        if (reader.Read())
                        {
                            lCodiceAutore = reader.GetInt64(0);
                        }
                        else
                        {
                            lCodiceAutore = autore.iCodiceAutore;
                            iInserFlag = 1;
                        }
                        reader.Close();

                    }

                }
                if (iInserFlag == 1)
                {
                    string cmd5 = string.Format(@"INSERT INTO Autori(codice, Nome, Cognome, Mail) values({0},'{1}','{2}','{3}')", lCodiceAutore, autore.Nome, autore.Cognome, autore.sMail);
                    using (SqlCommand insert = new SqlCommand(cmd5, conn))
                    {
                        try
                        {
                            var numrows = insert.ExecuteNonQuery();
                            if (numrows != 1)
                            {
                                DoSql(conn, "rollback transaction");
                                conn.Close();
                                throw new System.Exception("Valore di ritorno errato seconda query");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            DoSql(conn, "rollback transaction");
                            conn.Close();
                            return 0;
                        }
                    }
                }
            }

            //--------------

            var ok = DoSql(conn, "begin transaction");

            if (!ok)
                throw new System.Exception("Errore in begin transaction");


            var cmd = string.Format(@"insert into dbo.DOCUMENTI(Codice, Titolo, Settore, Stato, Tipo, Scaffale)
            VALUES({0}, '{1}', '{2}', '{3}', 'DVD', '{4}')", dvd.Codice, dvd.Titolo, dvd.Settore, dvd.Stato.ToString(), dvd.Scaffale.Numero);

            using (SqlCommand insert = new SqlCommand(cmd, conn))
            {
                try
                {
                    var numrows = insert.ExecuteNonQuery();
                    if (numrows != 1)
                    {
                        DoSql(conn, "rollback transaction");
                        conn.Close();
                        throw new System.Exception("Valore di ritorno errato prima query");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    DoSql(conn, "rollback transaction");
                    conn.Close();
                    return 0;
                }
            }
            var cmd1 = string.Format(@"insert into dbo.DVD(Codice, Durata) VALUES({0},{1})", dvd.Codice, dvd.Durata);
            using (SqlCommand insert = new SqlCommand(cmd1, conn))
            {
                try
                {
                    var numrows = insert.ExecuteNonQuery();
                    if (numrows != 1)
                    {
                        DoSql(conn, "rollback transaction");
                        conn.Close();
                        throw new System.Exception("Valore di ritorno errato seconda query");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    DoSql(conn, "rollback transaction");
                    conn.Close();
                    return 0;
                }
            }

            string cmd2;
            foreach (Autore autore in lAutori)
            {
                cmd2 = string.Format(@"INSERT INTO AUTORI(Codice, Nome, Cognome, mail) values('{0}','{1}','{2}','{3}') ", autore.iCodiceAutore, autore.Nome, autore.Cognome, autore.sMail);
                using (SqlCommand insert = new SqlCommand(cmd2, conn))
                {
                    try
                    {
                        var numrows = insert.ExecuteNonQuery();
                        if (numrows != 1)
                        {
                            DoSql(conn, "rollback transaction");
                            conn.Close();
                            throw new System.Exception("Valore di ritorno errato terza query");
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        DoSql(conn, "rollback transaction");
                        conn.Close();
                        return 0;
                    }
                }
            }

            string cmd4;
            foreach (Autore autore in lAutori)
            {
                cmd4 = string.Format(@"INSERT INTO AUTORI_DOCUMENTI(codice_autore, codice_documento) values({0},{1})", autore.iCodiceAutore, dvd.Codice);
                using (SqlCommand insert = new SqlCommand(cmd4, conn))
                {
                    try
                    {
                        var numrows = insert.ExecuteNonQuery();
                        if (numrows != 1)
                        {
                            DoSql(conn, "rollback transaction");
                            conn.Close();
                            throw new System.Exception("Valore di ritorno errato seconda query");
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        DoSql(conn, "rollback transaction");
                        conn.Close();
                        return 0;
                    }
                }
            }
            DoSql(conn, "commit transaction");
            conn.Close();
            return 0;
        }
        internal static int libroAdd(Libro libro, List<Autore> lAutori)
        {
            //devo collegarmi e inviare un comando di insert del nuovo scaffale
            var conn = Connect();
            if (conn == null)
            {
                throw new System.Exception("Unable to connect to database");
            }

            //----------------

            //Verifico se gli autori esistono
            string cmdAutori;
            foreach (Autore autore in lAutori)
            {
                long lCodiceAutore = 0;
                int iInserFlag = 0;
                cmdAutori = string.Format("select codice from Autori where Nome='{0}' and Cognome='{1}' and mail='{2}'", autore.Nome, autore.Cognome, autore.sMail);
                using (SqlCommand select = new SqlCommand(cmdAutori, conn))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        Console.WriteLine(reader.FieldCount);
                        if (reader.Read())
                        {
                            lCodiceAutore = reader.GetInt64(0);
                        }
                        else
                        {
                            lCodiceAutore = autore.iCodiceAutore;
                            iInserFlag = 1;
                        }
                        reader.Close();

                    }

                }
                if (iInserFlag == 1)
                {
                    string cmd5 = string.Format(@"INSERT INTO Autori(codice, Nome, Cognome, Mail) values({0},'{1}','{2}','{3}')", lCodiceAutore, autore.Nome, autore.Cognome, autore.sMail);
                    using (SqlCommand insert = new SqlCommand(cmd5, conn))
                    {
                        try
                        {
                            var numrows = insert.ExecuteNonQuery();
                            if (numrows != 1)
                            {
                                DoSql(conn, "rollback transaction");
                                conn.Close();
                                throw new System.Exception("Valore di ritorno errato seconda query");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            DoSql(conn, "rollback transaction");
                            conn.Close();
                            return 0;
                        }
                    }
                }
            }

            //--------------

            var ok = DoSql(conn, "begin transaction");

            if (!ok)
                throw new System.Exception("Errore in begin transaction");


            var cmd = string.Format(@"insert into dbo.DOCUMENTI(Codice, Titolo, Settore, Stato, Tipo, Scaffale)
            VALUES({0}, '{1}', '{2}', '{3}', 'LIBRO', '{4}')", libro.Codice,libro.Titolo,libro.Settore,libro.Stato.ToString(),libro.Scaffale.Numero);

            using (SqlCommand insert = new SqlCommand(cmd, conn))
            {
                try
                {
                    var numrows = insert.ExecuteNonQuery();
                    if (numrows != 1)
                    {
                        DoSql(conn, "rollback transaction");
                        conn.Close();
                        throw new System.Exception("Valore di ritorno errato prima query");
                    }
                        
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    DoSql(conn, "rollback transaction");
                    conn.Close();
                    return 0;
                }
            }
            var cmd1 = string.Format(@"insert into dbo.LIBRI(Codice, NumPagine) VALUES({0},{1})",libro.Codice,libro.NumeroPagine);
            using (SqlCommand insert = new SqlCommand(cmd1, conn))
            {
                try
                {
                    var numrows = insert.ExecuteNonQuery();
                    if (numrows != 1)
                    {
                        DoSql(conn, "rollback transaction");
                        conn.Close();
                        throw new System.Exception("Valore di ritorno errato seconda query");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    DoSql(conn, "rollback transaction");
                    conn.Close();
                    return 0;
                }
            }
            string cmd2;
            foreach (Autore autore in lAutori)
            {
                cmd2 = string.Format(@"INSERT INTO AUTORI(Codice, Nome, Cognome, mail) values('{0}','{1}','{2}','{3}') ",autore.iCodiceAutore, autore.Nome, autore.Cognome, autore.sMail);
                using (SqlCommand insert = new SqlCommand(cmd2, conn))
                {
                    try
                    {
                        var numrows = insert.ExecuteNonQuery();
                        if (numrows != 1)
                        {
                            DoSql(conn, "rollback transaction");
                            conn.Close ();
                            throw new System.Exception("Valore di ritorno errato terza query");
                        }
                            
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        DoSql(conn, "rollback transaction");
                        conn.Close();
                        return 0;
                    }
                }
            }

            string cmd4;
            foreach (Autore autore in lAutori)
            {
                cmd4 = string.Format(@"INSERT INTO AUTORI_DOCUMENTI(codice_autore, codice_documento) values({0},{1})", autore.iCodiceAutore, libro.Codice);
                using (SqlCommand insert = new SqlCommand(cmd4, conn))
                {
                    try
                    {
                        var numrows = insert.ExecuteNonQuery();
                        if (numrows != 1)
                        {
                            DoSql(conn, "rollback transaction");
                            conn.Close();
                            throw new System.Exception("Valore di ritorno errato seconda query");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        DoSql(conn, "rollback transaction");
                        conn.Close();
                        return 0;
                    }
                }
            }
            DoSql(conn, "commit transaction");
            conn.Close();
            return 0;
        }
        internal static int scaffaleAdd(string nuovo)
        {
            //devo collegarmi e inviare un comando di insert del nuovo scaffale
            var conn = Connect();
            if (conn == null)
            {
                throw new Exception("Unable to connect to database");
            }
            var cmd = string.Format("insert into Scaffale (Scaffale) values ('{0}')", nuovo);
            using (SqlCommand insert = new SqlCommand(cmd, conn))
            {
                try
                {
                    var numrows = insert.ExecuteNonQuery();
                    return numrows;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return 0;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        internal static List<string> scaffaliGet()
        {
            List<string> ls = new List<string>();
            var conn = Connect();
            if (conn == null)
            {
                throw new Exception("Unable to connect to database");
            }
            //query per prendere tutto lo scaffale dal db
            var cmd = string.Format("select Scaffale from Scaffale");
            using (SqlCommand query = new SqlCommand(cmd, conn))
            {
                using (SqlDataReader reader = query.ExecuteReader())
                {
                    Console.WriteLine(reader.FieldCount);
                    while (reader.Read())
                    {
                        ls.Add(reader.GetString(0));
                    }
                }
            }
            conn.Close();
            return ls;
        }
        //nel caso ci siano più attributi, allora potete utilizzare le tuple
        internal static List<Tuple<long, string, string, string, string, string>> documentiGet()
        {
            var ld = new List<Tuple<long, string, string, string, string, string>>();
            var conn = Connect();
            if (conn == null)
                throw new Exception("Unable to connect to the dabatase");
            var cmd = String.Format("select codice, Titolo, Settore, Stato, Tipo, Scaffale from Documenti");  //Li prendo tutti
            using (SqlCommand select = new SqlCommand(cmd, conn))
            {
                using (SqlDataReader reader = select.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var data = new Tuple<Int64, string, string, string, string, string>(
                            reader.GetInt64(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetString(4),
                            reader.GetString(5));
                        ld.Add(data);
                    }
                }
            }
            conn.Close();
            foreach (var utente in ld)
            {
                Console.WriteLine("Documento:\nCodice: {0}\nTitolo: {1}\nSettore: {2}\nStato: {3}\nTipo: {4}\nScaffale: {5}\n",
                    utente.Item1,
                    utente.Item2,
                    utente.Item3,
                    utente.Item4,
                    utente.Item5,
                    utente.Item6);
            }
            return ld;
        }

        //string cmdLibroAutori = string.Format("select codice from Autori where Nome='{0}' and Cognome='{1}' and mail='{2}'", autore.Nome, autore.Cognome, autore.sMail);
    }
}

