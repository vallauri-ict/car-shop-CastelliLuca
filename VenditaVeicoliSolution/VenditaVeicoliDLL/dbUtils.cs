﻿using System;
using System.Data.OleDb;
using System.Data;
using System.IO;

namespace VenditaVeicoliDLLProject
{
    public class dbUtils
    {

        public static string path = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\Storage/";
        public static string connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + "CarShop.accdb";
        static Random rnd = new Random();
        public static SerializableBindingList<Veicolo> bindingListVeicoli = new SerializableBindingList<Veicolo>();

        public static void CreateTableCars()
        {
            if (connStr != null)
            {
                OleDbConnection con = new OleDbConnection(connStr);
                using (con)
                {
                    con.Open();

                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = con;

                    string[] datiProva = new string[20];
                    caricaDatiProva(datiProva);

                    try
                    {

                        cmd.CommandText = @"CREATE TABLE Veicoli(
                                        targa VARCHAR(255) UNIQUE NOT NULL PRIMARY KEY,
                                        marca VARCHAR(255) NOT NULL,
                                        modello VARCHAR(255) NOT NULL,
                                        colore VARCHAR(16) NOT NULL,
                                        cilindrata INT NOT NULL,
                                        potenzaKw INT NOT NULL,
                                        dataImmatricolazione DATETIME,
                                        isUsato BIT,
                                        isKm0 BIT,
                                        kmPercorsi INT NOT NULL,
                                        cmpSpec VARCHAR(16)
                                      )";
                        cmd.ExecuteNonQuery();
                    }
                    catch (OleDbException exc)
                    {
                        Console.WriteLine("\n " + exc.Message);
                        System.Threading.Thread.Sleep(2000);
                        return;
                    }

                    for (int i = 0; datiProva[i] != null; i++)
                    {
                        Add(datiProva[i]);
                    }

                    Console.WriteLine("\n Vehicles table created!!");
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        public static void caricaDatiProva(string[] datiProva)
        {
            StreamReader sr = new StreamReader(path + "datiProva.txt");
            string s = "";
            int len = -1;

            while (sr.Peek() != -1)
            {
                len++;
                s = sr.ReadLine();
                datiProva[len] = "" + s + "";
            }
            sr.Close();
        }

        public static void Add(string dati)
        {
            if (connStr != null)
            {
                OleDbConnection con = new OleDbConnection(connStr);
                using (con)
                {
                    con.Open();

                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = con;

                    try
                    {
                        string query = "INSERT INTO Veicoli(targa, marca, modello, colore, cilindrata, potenzaKw, dataImmatricolazione, isUsato, isKm0, kmPercorsi, cmpSpec) " +
                                       "VALUES(@targa, @marca, @modello, @colore, @cilindrata, @potenzaKw, @dataImmatricolazione, @isUsato, @isKm0, @kmPercorsi, @cmpSpec)";
                        cmd.CommandText = query;

                        string[] vet = dati.Split('|');
                        cmd.Parameters.Add("@targa", OleDbType.VarChar, 255).Value = vet[0].ToUpper();
                        cmd.Parameters.Add("@marca", OleDbType.VarChar, 255).Value = vet[1];
                        cmd.Parameters.Add("@modello", OleDbType.VarChar, 255).Value = vet[2];
                        cmd.Parameters.Add("@colore", OleDbType.VarChar, 16).Value = vet[3];
                        cmd.Parameters.Add("@cilindrata", OleDbType.Integer).Value = vet[4];
                        cmd.Parameters.Add("@potenzaKw", OleDbType.Integer).Value = vet[5];
                        cmd.Parameters.Add("@dataImmatricolazione", OleDbType.Date).Value = vet[6];
                        cmd.Parameters.Add("@isUsato", OleDbType.Boolean).Value = vet[7];
                        cmd.Parameters.Add("@isKm0", OleDbType.Boolean).Value = vet[8];
                        cmd.Parameters.Add("@kmPercorsi", OleDbType.Integer).Value = vet[9];
                        cmd.Parameters.Add("@cmpSpec", OleDbType.VarChar, 255).Value = vet[10];
                        cmd.Prepare();

                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("\n " + exc.Message);
                        System.Threading.Thread.Sleep(2000);
                        return;
                    }
                }
            }
        }
        public static void show()
        {
            Console.WriteLine("\n");
            for (int i = 0; i < bindingListVeicoli.Count; i++)
            {
                Console.WriteLine(" *" + bindingListVeicoli[i].Targa + " | " + bindingListVeicoli[i].Marca + " | " + bindingListVeicoli[i].Modello + " | " + bindingListVeicoli[i].Colore + "*");
            }
            Console.ReadKey();
        }

        public static void CreateList()
        {
            if (connStr != null)
            {
                OleDbConnection con = new OleDbConnection(connStr);
                using (con)
                {
                    con.Open();

                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = con;

                    try
                    {
                        OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM Veicoli", con);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        foreach (DataRow item in table.Rows)
                        {
                            if (int.TryParse(item.ItemArray[10].ToString(), out _))
                            {
                                Auto a = new Auto(item.ItemArray[0].ToString(), item.ItemArray[1].ToString(), item.ItemArray[2].ToString(), item.ItemArray[3].ToString(), Convert.ToInt32(item.ItemArray[4]), Convert.ToDouble(item.ItemArray[5]),
                                                 Convert.ToDateTime(item.ItemArray[6]), Convert.ToBoolean(item.ItemArray[7]), Convert.ToBoolean(item.ItemArray[8]), Convert.ToInt32(item.ItemArray[9]),
                                                 Convert.ToInt32(item.ItemArray[10]));
                                bindingListVeicoli.Add(a);
                            }
                            else
                            {
                                Moto m = new Moto(item.ItemArray[0].ToString(), item.ItemArray[1].ToString(), item.ItemArray[2].ToString(), item.ItemArray[3].ToString(), Convert.ToInt32(item.ItemArray[4]), Convert.ToDouble(item.ItemArray[5]),
                                                 Convert.ToDateTime(item.ItemArray[6]), Convert.ToBoolean(item.ItemArray[7]), Convert.ToBoolean(item.ItemArray[8]), Convert.ToInt32(item.ItemArray[9]),
                                                 item.ItemArray[10].ToString());
                                bindingListVeicoli.Add(m);
                            }
                        }
                    }
                    catch (OleDbException exc)
                    {
                        Console.WriteLine("\n " + exc.Message);
                        System.Threading.Thread.Sleep(4000);
                        return;
                    }
                }
            }
        }

        public static void delete(string targa)
        {
            if (connStr != null)
            {
                OleDbConnection con = new OleDbConnection(connStr);
                using (con)
                {
                    con.Open();

                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = con;

                    try
                    {
                        cmd.CommandText = "DELETE FROM Veicoli WHERE targa = '" + targa.ToUpper() + "'";

                        cmd.ExecuteNonQuery();
                    }
                    catch (OleDbException exc)
                    {
                        Console.WriteLine("\n " + exc.Message);
                        System.Threading.Thread.Sleep(5000);
                        return;
                    }

                    Console.WriteLine("\n Item deleted!!");
                    System.Threading.Thread.Sleep(1500);
                }
            }
        }
    }
}
