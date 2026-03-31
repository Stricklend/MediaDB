using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace MediaDB.Models
{
    //20260329 mod start - supabase연동 검토 관련
    // public class AlbumService : DbServiceBase
    // {

    //     public (bool Success, string Message, string ImagePath) AddAlbum(
    //     string artist, string albumTitle, string genre, string label, string releaseDate,
    //     string addedBy, string modifiedBy, HttpPostedFileBase albumImage, Func<string, string> mapPath)
    //     {
    //         if (string.IsNullOrWhiteSpace(artist))
    //             return (false, "아티스트를 입력해 주세요", null);
    //         if (string.IsNullOrWhiteSpace(albumTitle))
    //             return (false, "앨범명을 입력해 주세요", null);

    //         string imagePath = null;
    //         if (albumImage != null && albumImage.ContentLength > 0)
    //         {
    //             var fileName = Path.GetFileName(Guid.NewGuid() + "_" + albumImage.FileName);
    //             var serverPath = mapPath("~/Content");
    //             Directory.CreateDirectory(serverPath);
    //             var fullPath = Path.Combine(serverPath, fileName);
    //             albumImage.SaveAs(fullPath);
    //             imagePath = "/Content/" + fileName;
    //         }
      
    //         using (var conn = OpenConnection())
    //         using (var cmd = CreateCommand(conn, @"
    //         INSERT INTO album
    //         (artist, albumtitle, genre, label, releasedate, addedby, modifiedby, imagepath, createdat) 
    //         VALUES
    //         (@artist, @albumtitle, @genre, @label, @releasedate, @addedby, @modifiedby, @imagepath, CURRENT_TIMESTAMP)"))
    //         {
    //             AddParameter(cmd, "@artist", artist);
    //             AddParameter(cmd, "@albumtitle", albumTitle);
    //             AddParameter(cmd, "@genre", genre);
    //             AddParameter(cmd, "@label", label);
    //             AddParameter(cmd, "@releasedate", releaseDate);
    //             AddParameter(cmd, "@addedby", addedBy);
    //             AddParameter(cmd, "@modifiedby", modifiedBy);
    //             AddParameter(cmd, "@imagepath", imagePath);
    //             cmd.ExecuteNonQuery();
    //         }

    //         return (true, "등록이 완료되었습니다.", imagePath);
    //     }

    //     public List<AlbumInfo> GetAlbums()
    //     {
    //         var list = new List<AlbumInfo>();
    //         using (var conn = OpenConnection())
    //         using (var cmd = CreateCommand(conn, "SELECT id, artist, albumtitle, imagepath FROM album ORDER BY createdat DESC"))
    //         using (var reader = cmd.ExecuteReader())
    //         {
    //             while (reader.Read())
    //             {
    //                 list.Add(new AlbumInfo
    //                 {
    //                     Id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
    //                     Artist = reader["artist"] == DBNull.Value ? string.Empty : reader["artist"].ToString(),
    //                     AlbumTitle = reader["albumtitle"] == DBNull.Value ? string.Empty : reader["albumtitle"].ToString(),
    //                     ImagePath = reader["imagepath"] == DBNull.Value ? string.Empty : reader["imagepath"].ToString()
    //                 });
    //             }
    //         }
    //         return list;
    //     }

    //     public class AlbumInfo
    //     {
    //         //20260301 add start
    //         public int Id { get; set; }
    //         //20260301 add end
    //         public string Artist { get; set; }
    //         public string AlbumTitle { get; set; }
    //         public string ImagePath { get; set; }
    //     }

    //     //20260301 mod start - 앨범 상세  페이지 추가

    //     public AlbumDetailInfo GetAlbumDetail(int id)
    //     {
    //         using (var conn = OpenConnection())
    //         using (var cmd = CreateCommand(conn, @"
    //                  SELECT id, artist, albumtitle, genre, label, releasedate, addedby, modifiedby, imagepath, createdat
    //                  FROM album
    //                  WHERE id = @id"))
    //         {
    //             AddParameter(cmd, "@id", id);

    //             using (var reader = cmd.ExecuteReader())
    //             {
    //                 if (reader.Read())
    //                 {
    //                     return new AlbumDetailInfo
    //                     {
    //                         Id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
    //                         Artist = reader["artist"] == DBNull.Value ? string.Empty : reader["artist"].ToString(),
    //                         AlbumTitle = reader["albumtitle"] == DBNull.Value ? string.Empty : reader["albumtitle"].ToString(),
    //                         Genre = reader["genre"] == DBNull.Value ? string.Empty : reader["genre"].ToString(),
    //                         Label = reader["label"] == DBNull.Value ? string.Empty : reader["label"].ToString(),
    //                         ReleaseDate = reader["releasedate"] == DBNull.Value ? string.Empty : reader["releasedate"].ToString(),
    //                         AddedBy = reader["addedby"] == DBNull.Value ? string.Empty : reader["addedby"].ToString(),
    //                         ModifiedBy = reader["modifiedby"] == DBNull.Value ? string.Empty : reader["modifiedby"].ToString(),
    //                         ImagePath = reader["imagepath"] == DBNull.Value ? string.Empty : reader["imagepath"].ToString(),
    //                         CreatedAt = reader["createdat"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["createdat"])
    //                     };
    //                 }
    //             }
    //         }

    //         return null;
    //     }

    //     public class AlbumDetailInfo
    //     {
    //         public int Id { get; set; }
    //         public string Artist { get; set; }
    //         public string AlbumTitle { get; set; }
    //         public string Genre { get; set; }
    //         public string Label { get; set; }
    //         public string ReleaseDate { get; set; }
    //         public string AddedBy { get; set; }
    //         public string ModifiedBy { get; set; }
    //         public string ImagePath { get; set; }
    //         public DateTime? CreatedAt { get; set; }
    //     }
    //     //20260301 mod end
    // }    
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
    //20260329 mod end
}