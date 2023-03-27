using System;
using System.Collections.Generic;
using System.Text;
using QFE.DAL;
using Microsoft.EntityFrameworkCore;

namespace QFE.BLL {
    public class DataContext : DbContext {
        public DbSet<ayah> ayahs { get; set; }
        public DbSet<bookmark> bookmarks { get; set; }
        public DbSet<hizb> hizbs { get; set; }
        public DbSet<juz> juzs { get; set; }
        public DbSet<language> languages { get; set; }
        public DbSet<manzil> manzils { get; set; }
        public DbSet<quran> qurans { get; set; }
        public DbSet<quran_text> quran_texts { get; set; }
        public DbSet<reciter> reciters { get; set; }
        public DbSet<surah> surahs { get; set; }
        public DbSet<transliteration> transliterations { get; set; }

        public string DbPath { get; }

        public DataContext() {

            //var folder = Environment.SpecialFolder.LocalApplicationData;
            //DbPath = quran_data.Conn;
            //string.IsNullOrEmpty(quran_data.Conn) ? Environment.GetFolderPath(folder) : 
            //DbPath = System.IO.Path.Join(path, "/model-builder");
            //if (!Directory.Exists(DbPath))
            //    Directory.CreateDirectory(DbPath);
            //DbPath = System.IO.Path.Join(DbPath, "/ml.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(quran_data.Conn);
    }
   
}