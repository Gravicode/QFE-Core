using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QFE.WPF.Tools
{
    public class FileWriter
    {
        public static bool DeleteFile(String FileName)
        {
            try
            {
                FileInfo TheFile = new FileInfo(FileName);
                if (TheFile.Exists)
                {
                    System.IO.File.Delete(FileName);
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }

            catch (FileNotFoundException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static string ReadFile(String FileName)
        {

            FileInfo TheFile = new FileInfo(FileName);
            string str = "", strline;
            if (!TheFile.Exists)
            {
                return "";
            }
            StreamReader sRead = new StreamReader(FileName);
            do
            {
                strline = sRead.ReadLine();
                str += strline+System.Environment.NewLine;
            }
            while (strline != null);

            sRead.Close();
            return str;
        }
        public static void AppendToFile(String teks, String Path)
        {
            try
            {
                StreamWriter SW;
                SW = System.IO.File.AppendText(Path);
                SW.WriteLine(teks);
                SW.Close();
                //Console.WriteLine("Text Appended Successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }
        public static void WriteFile(String teks, String Path)
        {
            try
            {

                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter(Path);

                //Write a line of text
                sw.WriteLine(teks);

                //Close the file
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }
    }
}