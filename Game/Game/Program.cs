using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            // new GameManager().Run();
            Database.Driver();
        }
    }

    class GameManager
    {
        ArrayList Players = new ArrayList();

        public void CreatePlayers()
        {
            for (int i = 0; i < 4; i++)
            {
                Players.Add(new Player(10000));
            }
        }
        public void Run()
        {
            GameBoard.SetupMyGameBoard();
            this.CreatePlayers();
            int n = -1;
            while (true)
            {
                n++;
                Console.WriteLine("Player {0}'s Turn", n);
                doStuff((Player)Players[n]);
                if (n > Players.Count - 2) { n = -1; }
            }
        }
        public void doStuff(Player aPlayer)
        {
            int numberOfSpacesToMove = 0;
            Console.WriteLine("Move how many spaces?");
            try { numberOfSpacesToMove = Convert.ToInt16(Console.ReadLine()); }
            catch (Exception e) { Console.WriteLine(e.ToString()); }

            this.MovePlayer(numberOfSpacesToMove, aPlayer);
            aPlayer.DisplayStatus();

            if (aPlayer.CurrentAddress.PropertyOwner == null)
            {
                // present user with option to purchase
                // if they try to purchase and do not have sufficient funds, back out of the transaction
                aPlayer.BuyAProperty(aPlayer.CurrentAddress);
            }
            else
            {
                // else charge then rent
                // 
                // if The System tries to charge them RENT, and they have an INSUFFICIENT BALANCE, 
                //  A. Transfer all their money to the property's owner
                //  B. Inform them they are BANKRUPT.
                //  C. Remove that Player Object from the Array List of Players
            }

        }

        public void MovePlayer(int HowManySpaces, Player aPlayer)
        {
            // get a handle on the ARRAY INDEX NUMBER of the CURRENT ADDRESS for the player
            int ARRAY_INDEX_CURRENT_ADDRESS = GameBoard.FindLocationOfProperty(aPlayer.CurrentAddress);
            // ADD to that the number of spaces they want to move
            int NewPlayerAddress_IndexLocation = ARRAY_INDEX_CURRENT_ADDRESS + HowManySpaces;
            // figure out what PROPERTY corrsponds to that new index
            aPlayer.CurrentAddress = (Property)GameBoard.MyGameBoard[NewPlayerAddress_IndexLocation];
            // ASSIGN the player's current address to that new property object reference

        }
    }

    class Player
    {
        public double CurrentBalance;
        public Property CurrentAddress;
        public ArrayList PropertiesOwned;

        public Player(double InitialBalance)
        {
            this.CurrentBalance = InitialBalance;
            this.CurrentAddress = null;
        }

        public void BuyAProperty(Property aProperty)
        {
            Console.WriteLine("Would you like to buy this property? The cost is {0}", aProperty.PropertyValue);
            string Buy = Console.ReadLine();
            if (Buy.Equals("y")) { this.PerformBuyTransaction(); }
        }

        public void PerformBuyTransaction() { }

        public void PayRent(Player PropertyOwner)
        {

        }

        public void DeclareBankruptcy(Player PlayerToPayTo)
        {

        }

        public void DisplayStatus()
        {
            Console.WriteLine("{0}", this.CurrentBalance);
        }
    }

    class Property
    {
        public Player PropertyOwner = null;
        Random r1 = new Random();
        public double PropertyValue;
        public Property()
        {
            this.PropertyValue = 100 + 700 * r1.NextDouble();
        }
    }

    class GameBoard
    {
        public static ArrayList MyGameBoard = new ArrayList();

        public static void SetupMyGameBoard()
        {
            // create 100 Properties on The Board
            for (int i = 0; i < 100; i++)
            {
                MyGameBoard.Add(new Property());
            }
        }

        public static int FindLocationOfProperty(Property PropertyToLookFor)
        {
            int IndexInArray = 0;
            foreach (Property aProperty in MyGameBoard)
            {
                if (PropertyToLookFor == aProperty)
                {
                    return IndexInArray;
                }
            }
            return -1;

        }
    }

    class Database
    {
        public static void Driver()
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();
            // CreateTable(sqlite_conn);
            InsertData(sqlite_conn);
            ReadData(sqlite_conn);
        }

        static SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source= database.db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            { Console.WriteLine(ex.ToString()); }
            return sqlite_conn;
        }

        static void CreateTable(SQLiteConnection conn)
        {

            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE SampleTable (Col1 VARCHAR(20), Col2 INT)";
            string Createsql1 = "CREATE TABLE SampleTable1 (Col1 VARCHAR(20), Col2 INT)";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = Createsql1;
            sqlite_cmd.ExecuteNonQuery();

        }

        static void InsertData(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO SampleTable (Col1, Col2) VALUES('Test Text ', 1); ";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "INSERT INTO SampleTable  (Col1, Col2) VALUES('Test1 Text1 ', 2); ";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "INSERT INTO SampleTable (Col1, Col2) VALUES('Test2 Text2 ', 3); ";
            sqlite_cmd.ExecuteNonQuery();

            sqlite_cmd.CommandText = "INSERT INTO SampleTable1 (Col1, Col2) VALUES('Test3 Text3 ', 3); ";
            sqlite_cmd.ExecuteNonQuery();

        }

        static void ReadData(SQLiteConnection conn)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM SampleTable";

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                string myreader = sqlite_datareader.GetString(0);
                Console.WriteLine(myreader);
            }
            conn.Close();
        }
    }
}
