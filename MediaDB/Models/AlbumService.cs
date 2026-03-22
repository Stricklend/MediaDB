using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace MediaDB.Models
{
    public class AlbumService
    {
        private readonly string connectionString;

        public AlbumService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public (bool Success, string Message, string ImagePath) AddAlbum(
        string artist, string albumTitle, string genre, string label, string releaseDate,
        string addedBy, string modifiedBy, HttpPostedFileBase albumImage, Func<string, string> mapPath)
        {
            if (string.IsNullOrWhiteSpace(artist))
                return (false, "아티스트를 입력해 주세요", null);
            if (string.IsNullOrWhiteSpace(albumTitle))
                return (false, "앨범명을 입력해 주세요", null);

            string imagePath = null;
            if (albumImage != null && albumImage.ContentLength > 0)
            {
                var fileName = Path.GetFileName(Guid.NewGuid() + "_" + albumImage.FileName);
                var serverPath = mapPath("~/Content");
                Directory.CreateDirectory(serverPath);
                var fullPath = Path.Combine(serverPath, fileName);
                albumImage.SaveAs(fullPath);
                imagePath = "/Content/" + fileName;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            INSERT INTO Album
            (Artist, AlbumTitle, Genre, Label, ReleaseDate, AddedBy, ModifiedBy, ImagePath, CreatedAt)
            VALUES
            (@Artist, @AlbumTitle, @Genre, @Label, @ReleaseDate, @AddedBy, @ModifiedBy, @ImagePath, GETDATE())";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Artist", artist);
                cmd.Parameters.AddWithValue("@AlbumTitle", albumTitle);
                cmd.Parameters.AddWithValue("@Genre", (object)genre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Label", (object)label ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ReleaseDate", (object)releaseDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AddedBy", (object)addedBy ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ModifiedBy", (object)modifiedBy ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ImagePath", (object)imagePath ?? DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return (true, "등록이 완료되었습니다.", imagePath);
        }

        //20260301 mod start
        //public List<AlbumInfo> GetAlbums()
        // {
        //     var list = new List<AlbumInfo>();
        //     using (SqlConnection conn = new SqlConnection(connectionString))
        //     {
        //         string query = "SELECT Artist, AlbumTitle, ImagePath FROM Album ORDER BY CreatedAt DESC";
        //         SqlCommand cmd = new SqlCommand(query, conn);
        //         conn.Open();
        //         using (var reader = cmd.ExecuteReader())
        //         {
        //             while (reader.Read())
        //             {
        //                 list.Add(new AlbumInfo
        //                 {
        //                     Artist = reader["Artist"].ToString(),
        //                     AlbumTitle = reader["AlbumTitle"].ToString(),
        //                     ImagePath = reader["ImagePath"].ToString()
        //                 });
        //             }
        //         }
        //     }
        //     return list;
        // }

        public List<AlbumInfo> GetAlbums()
        {
            var list = new List<AlbumInfo>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Artist, AlbumTitle, ImagePath FROM Album ORDER BY CreatedAt DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AlbumInfo
                        {
                            Id = reader["Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Id"]),
                            Artist = reader["Artist"] == DBNull.Value ? string.Empty : reader["Artist"].ToString(),
                            AlbumTitle = reader["AlbumTitle"] == DBNull.Value ? string.Empty : reader["AlbumTitle"].ToString(),
                            ImagePath = reader["ImagePath"] == DBNull.Value ? string.Empty : reader["ImagePath"].ToString()
                        });
                    }
                }
            }
            return list;
        }
        //20260301 mod end

        public class AlbumInfo
        {
            //20260301 add start
            public int Id { get; set; }
            //20260301 add end
            public string Artist { get; set; }
            public string AlbumTitle { get; set; }
            public string ImagePath { get; set; }
        }

        //20260301 mod start

        public AlbumDetailInfo GetAlbumDetail(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                     SELECT Id, Artist, AlbumTitle, Genre, Label, ReleaseDate, AddedBy, ModifiedBy, ImagePath, CreatedAt
                     FROM Album
                     WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new AlbumDetailInfo
                        {
                            Id = reader["Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Id"]),
                            Artist = reader["Artist"] == DBNull.Value ? string.Empty : reader["Artist"].ToString(),
                            AlbumTitle = reader["AlbumTitle"] == DBNull.Value ? string.Empty : reader["AlbumTitle"].ToString(),
                            Genre = reader["Genre"] == DBNull.Value ? string.Empty : reader["Genre"].ToString(),
                            Label = reader["Label"] == DBNull.Value ? string.Empty : reader["Label"].ToString(),
                            ReleaseDate = reader["ReleaseDate"] == DBNull.Value ? string.Empty : reader["ReleaseDate"].ToString(),
                            AddedBy = reader["AddedBy"] == DBNull.Value ? string.Empty : reader["AddedBy"].ToString(),
                            ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? string.Empty : reader["ModifiedBy"].ToString(),
                            ImagePath = reader["ImagePath"] == DBNull.Value ? string.Empty : reader["ImagePath"].ToString(),
                            CreatedAt = reader["CreatedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CreatedAt"])
                        };
                    }
                }
            }

            return null;
        }

        public class AlbumDetailInfo
        {
            public int Id { get; set; }
            public string Artist { get; set; }
            public string AlbumTitle { get; set; }
            public string Genre { get; set; }
            public string Label { get; set; }
            public string ReleaseDate { get; set; }
            public string AddedBy { get; set; }
            public string ModifiedBy { get; set; }
            public string ImagePath { get; set; }
            public DateTime? CreatedAt { get; set; }
        }
        //20260301 mod end





    }
}

//CREATE TABLE Album(
//    Id INT IDENTITY(1, 1) PRIMARY KEY,
//    Artist NVARCHAR(100) NOT NULL,
//    AlbumTitle NVARCHAR(100) NOT NULL,
//    Genre NVARCHAR(100),
//    Label NVARCHAR(100),
//    ReleaseDate NVARCHAR(10),
//    AddedBy NVARCHAR(100),
//    ModifiedBy NVARCHAR(100),
//    ImagePath NVARCHAR(255),
//    CreatedAt DATETIME NOT NULL
//)

