//Copyright (c) 2009 Adiel
using System;
using System.Data;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
/// <summary>
/// Summary description for FungsiDB
/// </summary>
/// 
namespace QFE.DAL
{
    public class FungsiDB
    {
      
        public static string KoneksiStr = "";
        public FungsiDB()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static DataTable RetrieveData(String SQLSyntax)
        {
            SQLiteConnection MyConnection = null;
            SQLiteCommand MyCommand = null;
            SQLiteDataAdapter MyAdapter = null;
            DataTable MyTable = new DataTable();
            MyConnection = new SQLiteConnection();
            MyConnection.ConnectionString = KoneksiStr;
            MyCommand = new SQLiteCommand();
            MyCommand.CommandText = SQLSyntax;
            MyCommand.CommandType = CommandType.Text;
            MyCommand.Connection = MyConnection;
            MyAdapter = new SQLiteDataAdapter();
            MyAdapter.SelectCommand = MyCommand;
            MyAdapter.Fill(MyTable);
            if (MyConnection.State == ConnectionState.Open)
                MyConnection.Close();
            return MyTable;
        }

        public static object RetrieveOneData(String SQLSyntax)
        {
            object value = null;
            SQLiteConnection MyConnection = null;
            SQLiteCommand MyCommand = null;
            MyConnection = new SQLiteConnection();
            MyConnection.ConnectionString = KoneksiStr;
            MyCommand = new SQLiteCommand();
            MyCommand.CommandText = SQLSyntax;
            MyCommand.CommandType = CommandType.Text;
            MyCommand.Connection = MyConnection;
            MyConnection.Open();
            value = MyCommand.ExecuteScalar() != null ? MyCommand.ExecuteScalar() : null;
            if (MyConnection.State == ConnectionState.Open)
                MyConnection.Close();
            return value;
        }

        public static int CountData(String SQLSyntax)
        {
            SQLiteConnection MyConnection = null;
            SQLiteCommand MyCommand = null;
            SQLiteDataAdapter MyAdapter = null;
            DataTable MyTable = new DataTable();
            MyConnection = new SQLiteConnection();
            MyConnection.ConnectionString = KoneksiStr;
            MyCommand = new SQLiteCommand();
            MyCommand.CommandText = SQLSyntax;
            MyCommand.CommandType = CommandType.Text;
            MyCommand.Connection = MyConnection;
            MyAdapter = new SQLiteDataAdapter();
            MyAdapter.SelectCommand = MyCommand;
            MyAdapter.Fill(MyTable);
            return MyTable.Rows.Count;
        }

        public static Boolean MasukanData(String[] KolomStr, String Tabel, String[] IsiStr)
        {
            SQLiteConnection MyConnection = null;
            SQLiteCommand MyCommand = null;
            MyConnection = new SQLiteConnection();
            string StrQuery = "";
            StrQuery = "Insert into " + Tabel + " (";
            for (int i = 0; i <= KolomStr.GetUpperBound(0); i++)
                if (i < KolomStr.GetUpperBound(0))
                    StrQuery = StrQuery + KolomStr[i] + ",";
                else
                    StrQuery = StrQuery + KolomStr[i] + ") ";
            StrQuery = StrQuery + "values (";
            for (int i = 0; i <= IsiStr.GetUpperBound(0); i++)
                if (i < IsiStr.GetUpperBound(0))
                    StrQuery = StrQuery + IsiStr[i] + ",";
                else
                    StrQuery = StrQuery + IsiStr[i] + ") ";
            try
            {
                MyConnection.ConnectionString = KoneksiStr;
                MyCommand = new SQLiteCommand();
                MyCommand.CommandText = StrQuery;
                MyCommand.CommandType = CommandType.Text;
                MyCommand.Connection = MyConnection;
                MyConnection.Open();
                if (MyCommand.ExecuteNonQuery() > 0)
                {
                    MyCommand.Dispose();
                    MyConnection.Dispose();
                    return true;
                }
                else return false;
            }
            catch (Exception) // catches without assigning to a variable
            {
                return false;
            }
        }



        public static bool UpdateRecord(String TableName, String[] ColumnID, String[] IDValues, String[] Column, String[] ColumnValues)
        {
            SQLiteDataAdapter da = null;
            da = new SQLiteDataAdapter();
            SQLiteCommand cmd = null;
            SQLiteConnection conn = null;
            conn = new SQLiteConnection();

            string QueryStr = "";
            string ValueStr = "";
            conn.ConnectionString = KoneksiStr;

            if (ColumnID == null || IDValues == null || Column == null || ColumnValues == null)
                return false;
            else
            {
                QueryStr = "";
                ValueStr = "";
                for (int i = 0; i <= ColumnID.GetUpperBound(0); i++)
                {

                    QueryStr = QueryStr + ColumnID[i] + " = " + IDValues[i];
                    if (i < ColumnID.GetUpperBound(0))
                    {
                        QueryStr = QueryStr + " and ";
                    }

                }
                for (int i = 0; i <= Column.GetUpperBound(0); i++)
                {

                    ValueStr = ValueStr + Column[i] + " = " + ColumnValues[i];
                    if (i < Column.GetUpperBound(0))
                    {
                        ValueStr = ValueStr + ",";
                    }

                }
                cmd = new SQLiteCommand("UPDATE " + TableName + " SET " + ValueStr + " " +
                                 "WHERE " + QueryStr, conn);

            }
            conn.Open();
            try
            {
                if (cmd.ExecuteNonQuery() > 0)
                {
                    conn.Close();
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }

        public static bool HapusData(String TableName, String[] ColumnID, String[] IDValues)
        {

            SQLiteDataAdapter da = null;
            da = new SQLiteDataAdapter();
            SQLiteCommand cmd = null;
            SQLiteConnection conn = null;
            conn = new SQLiteConnection();

            string QueryStr = "";
            conn.ConnectionString = KoneksiStr;

            if (ColumnID == null || IDValues == null)
                return false;
            else
            {
                QueryStr = "";
                for (int i = 0; i <= ColumnID.GetUpperBound(0); i++)
                {

                    QueryStr = QueryStr + ColumnID[i] + " = " + IDValues[i];
                    if (i < ColumnID.GetUpperBound(0))
                    {
                        QueryStr = QueryStr + " and ";
                    }

                }

                cmd = new SQLiteCommand("delete from " + TableName + " WHERE " + QueryStr, conn);

            }
            conn.Open();
            try
            {
                if (cmd.ExecuteNonQuery() > 0)
                {
                    conn.Close();
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }

        public static String getUniqueNumber(String AwalStr, String Tabel, String Kolom)
        {
            int MaxNo = 0;
            int Pjg = 0;
            String RsltStr;
            SQLiteConnection MyConnection = null;
            SQLiteCommand MyCommand = null;
            SQLiteDataAdapter MyAdapter = null;
            DataTable MyTable = null;
            MyTable = new DataTable();
            MyConnection = new SQLiteConnection();
            MyConnection.ConnectionString = KoneksiStr;
            MyCommand = new SQLiteCommand();
            MyCommand.CommandText = "SELECT Count (" + Kolom + ") from dbo." + Tabel;
            MyCommand.CommandType = CommandType.Text;
            MyCommand.Connection = MyConnection;
            MyAdapter = new SQLiteDataAdapter();
            MyAdapter.SelectCommand = MyCommand;
            MyAdapter.Fill(MyTable);
            if (MyTable.Rows[0][0].ToString() == "0" || MyTable.Rows.Count <= 0) MaxNo = 0;
            else
                MaxNo = Convert.ToInt32(MyTable.Rows[0][0].ToString());
            MaxNo = MaxNo + 1;
            Pjg = MaxNo.ToString().Trim().Length;
            RsltStr = AwalStr;
            if (Pjg < 3)
            {
                for (int i = 1; i < 4 - Pjg; i++) RsltStr = RsltStr + "0";
            }
            RsltStr = RsltStr + MaxNo.ToString().Trim();
            return RsltStr;
        }
        public static Boolean HapusData(String Tabel, String KolPk, String Pk)
        {
            SQLiteConnection MyConnection = null;
            SQLiteCommand MyCommand = null;
            MyConnection = new SQLiteConnection();
            string StrQuery = "";
            StrQuery = "Delete from " + Tabel + " where " + KolPk.Trim() + " ='" + Pk.Trim() + "'";
            try
            {
                MyConnection.ConnectionString = KoneksiStr;
                MyCommand = new SQLiteCommand();
                MyCommand.CommandText = StrQuery;
                MyCommand.CommandType = CommandType.Text;
                MyCommand.Connection = MyConnection;
                MyConnection.Open();
                if (MyCommand.ExecuteNonQuery() > 0)
                {
                    MyCommand.Dispose();
                    MyConnection.Dispose();
                    return true;
                }
                else return false;
            }
            catch (Exception) // catches without assigning to a variable
            {
                return false;
            }
        }

        public static Boolean ExecuteNonQuery(String StrQuery)
        {
            SQLiteConnection MyConnection = null;
            SQLiteCommand MyCommand = null;
            MyConnection = new SQLiteConnection();
            try
            {
                MyConnection.ConnectionString = KoneksiStr;
                MyCommand = new SQLiteCommand();
                MyCommand.CommandText = StrQuery;
                MyCommand.CommandType = CommandType.Text;
                MyCommand.Connection = MyConnection;
                MyConnection.Open();
                if (MyCommand.ExecuteNonQuery() > 0)
                {
                    MyCommand.Dispose();
                    MyConnection.Dispose();
                    return true;
                }
                else return false;
            }
            catch (Exception) // catches without assigning to a variable
            {
                return false;
            }
        }

        public static Boolean isNumber(String N)
        {
            int hasNumber = 0;
            for (int i = 0; i < N.Length; i++)
            {
                if (N[i] == '.') continue;
                if (N[i] < '0' || N[i] > '9') return false;
                else
                    hasNumber++;
            }
            return (hasNumber > 0 ? true : false);
        }
        public static Boolean isDecimal(String N)
        {
            for (int i = 0; i < N.Length; i++)
            {
                if (N[i] != '.')
                    if (N[i] < '0' || N[i] > '9') return false;
            }
            return true;
        }
        public static String getMonth(int CountMonth)
        {
            switch (CountMonth)
            {
                case 1: return "January";
                case 2: return "February";
                case 3: return "March";
                case 4: return "April";
                case 5: return "May";
                case 6: return "June";
                case 7: return "July";
                case 8: return "August";
                case 9: return "September";
                case 10: return "October";
                case 11: return "November";
                case 12: return "December";
            }
            return "";
        }

        public static String GenerateNewKey(String TableName, String FieldName, String Prefix, int KeyLength)
        {
            try
            {
                String SQLSyntax = "Select Max(Substring(" + FieldName + "," + Convert.ToString(Prefix.Length + 1) + "," + Convert.ToString(KeyLength - Prefix.Length) + ")) From " + TableName;
                SQLiteConnection MyConnection = null;
                SQLiteCommand MyCommand = null;
                SQLiteDataAdapter MyAdapter = null;
                DataTable MyTable = new DataTable();
                MyConnection = new SQLiteConnection();
                MyConnection.ConnectionString = KoneksiStr;
                MyCommand = new SQLiteCommand();
                MyCommand.CommandText = SQLSyntax;
                MyCommand.CommandType = CommandType.Text;
                MyCommand.Connection = MyConnection;
                MyAdapter = new SQLiteDataAdapter();
                MyAdapter.SelectCommand = MyCommand;
                MyAdapter.Fill(MyTable);
                if (MyConnection.State == ConnectionState.Open)
                    MyConnection.Close();
                if (MyTable.Rows.Count > 0)
                {
                    if (MyTable.Rows[0][0] == DBNull.Value)
                        return Prefix + FillWithZero(KeyLength - Prefix.Length, "0", 1);
                    int Cnt = Convert.ToInt32(MyTable.Rows[0][0]) + 1;
                    return Prefix + FillWithZero(KeyLength - Prefix.Length, Cnt.ToString(), 1);
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static String FillWithZero(int NumberLength, String Number, int InsertType)
        {
            String Tmp = "";
            for (int i = 0; i < NumberLength - Number.Trim().Length; i++)
            {
                Tmp += "0";
            }
            if (InsertType == 1)
            {
                return Tmp + Number.Trim();
            }
            else
            {
                return Number.Trim() + Tmp;
            }
        }

        public static String QueryGenerator(String[] FieldName, int[] TypeFilter, String[] ValKey)
        {
            String SQLStr = " Where ";
            if (FieldName.GetUpperBound(0) != TypeFilter.GetUpperBound(0) || TypeFilter.GetUpperBound(0) != ValKey.GetUpperBound(0)) return "";
            //1 = equal
            //2 = not equal
            //3 = like
            String[] Operators = { " = ", " != ", " LIKE " };
            int Counter = 0;
            for (int i = 0; i <= FieldName.GetUpperBound(0); i++)
            {
                if (string.IsNullOrEmpty(ValKey[i].Trim())) continue;
                string SQLStr2 = "[" + FieldName[i] + "]" + Operators[TypeFilter[i] - 1];
                if (TypeFilter[i] == 3)
                    SQLStr2 += "'%" + ValKey[i] + "%'";
                else
                    SQLStr2 += "'" + ValKey[i] + "'";
                if (Counter > 0) SQLStr2 = " AND " + SQLStr2;
                SQLStr += SQLStr2;
                Counter++;
            }
            if (SQLStr.Trim().ToLower() == "where") return string.Empty;
            return SQLStr;
        }
        public static DataTable ExecuteSP(string SPName, string[] ParamName, object[] ParamValue)
        {
            SQLiteConnection MyConnection = null;
            SQLiteCommand MyCommand = null;
            SQLiteDataAdapter MyAdapter = null;
            DataTable MyTable = new DataTable();
            MyConnection = new SQLiteConnection();
            MyConnection.ConnectionString = KoneksiStr;
            MyCommand.CommandType = CommandType.StoredProcedure;
            MyCommand.Connection = MyConnection;
            MyAdapter = new SQLiteDataAdapter();
            MyCommand.CommandText = SPName;
            MyCommand.CommandType = CommandType.StoredProcedure;
            for (int i = 0; i <= ParamName.GetUpperBound(0); i++)
            {
                DbParameter param1 = MyCommand.CreateParameter();
                param1.ParameterName= ParamName[i];
                param1.Value = ParamValue[i];
                MyCommand.Parameters.Add(param1);
            }
            MyAdapter.SelectCommand = MyCommand;
            MyAdapter.Fill(MyTable);
            return MyTable;
        }
    }
}