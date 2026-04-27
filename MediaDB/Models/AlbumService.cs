using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using Npgsql;
using NpgsqlTypes;

namespace MediaDB.Models
{
    public class AlbumService
    {
        private const int ArtistMaxLength = 100;
        private const int AlbumTitleMaxLength = 100;
        private const int GenreMaxLength = 100;
        private const int LabelMaxLength = 100;
        private const int ReleaseDateMaxLength = 10;
        private const int AddedByMaxLength = 100;
        private const int ModifiedByMaxLength = 100;
        private const int ImagePathMaxLength = 100;

        private readonly string connectionString;

        public AlbumService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public (bool Success, string Message, string ImagePath) AddAlbum(
            string artist, string albumTitle, string genre, string label, string releaseDate,
            string addedBy, string modifiedBy, HttpPostedFileBase albumImage, Func<string, string> mapPath)
        {
            var normalizedArtist = NormalizeText(artist);
            var normalizedAlbumTitle = NormalizeText(albumTitle);
            var normalizedGenre = NormalizeText(genre);
            var normalizedLabel = NormalizeText(label);
            var normalizedReleaseDate = NormalizeText(releaseDate);
            var normalizedAddedBy = NormalizeText(addedBy);
            var normalizedModifiedBy = NormalizeText(modifiedBy);

            if (string.IsNullOrWhiteSpace(normalizedArtist))
                return (false, "Artist is required.", null);
            if (string.IsNullOrWhiteSpace(normalizedAlbumTitle))
                return (false, "Album title is required.", null);
            if (normalizedArtist.Length > ArtistMaxLength)
                return (false, "Artist must be 100 characters or less.", null);
            if (normalizedAlbumTitle.Length > AlbumTitleMaxLength)
                return (false, "Album title must be 100 characters or less.", null);
            if (normalizedGenre != null && normalizedGenre.Length > GenreMaxLength)
                return (false, "Genre must be 100 characters or less.", null);
            if (normalizedLabel != null && normalizedLabel.Length > LabelMaxLength)
                return (false, "Label must be 100 characters or less.", null);
            if (normalizedReleaseDate != null && normalizedReleaseDate.Length > ReleaseDateMaxLength)
                return (false, "Release date must use the YYYY-MM-DD format.", null);
            if (normalizedAddedBy != null && normalizedAddedBy.Length > AddedByMaxLength)
                return (false, "Added by must be 100 characters or less.", null);
            if (normalizedModifiedBy != null && normalizedModifiedBy.Length > ModifiedByMaxLength)
                return (false, "Modified by must be 100 characters or less.", null);

            string imagePath = null;
            if (albumImage != null && albumImage.ContentLength > 0)
            {
                var extension = Path.GetExtension(albumImage.FileName) ?? string.Empty;
                var fileName = string.Concat(Guid.NewGuid().ToString("N"), extension);
                imagePath = "/Content/" + fileName;

                if (imagePath.Length > ImagePathMaxLength)
                    return (false, "Image path is too long.", null);

                var serverPath = mapPath("~/Content");
                Directory.CreateDirectory(serverPath);
                var fullPath = Path.Combine(serverPath, fileName);
                albumImage.SaveAs(fullPath);
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                const string query = @"
INSERT INTO album
(artist, albumtitle, genre, label, releasedate, addedby, modifiedby, imagepath)
VALUES
(@artist, @albumtitle, @genre, @label, @releasedate, @addedby, @modifiedby, @imagepath)";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    AddVarCharParameter(cmd, "@artist", ArtistMaxLength, normalizedArtist);
                    AddVarCharParameter(cmd, "@albumtitle", AlbumTitleMaxLength, normalizedAlbumTitle);
                    AddVarCharParameter(cmd, "@genre", GenreMaxLength, normalizedGenre);
                    AddVarCharParameter(cmd, "@label", LabelMaxLength, normalizedLabel);
                    AddVarCharParameter(cmd, "@releasedate", ReleaseDateMaxLength, normalizedReleaseDate);
                    AddVarCharParameter(cmd, "@addedby", AddedByMaxLength, normalizedAddedBy);
                    AddVarCharParameter(cmd, "@modifiedby", ModifiedByMaxLength, normalizedModifiedBy);
                    AddVarCharParameter(cmd, "@imagepath", ImagePathMaxLength, imagePath);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return (true, "Album saved successfully.", imagePath);
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
            using (var conn = new NpgsqlConnection(connectionString))
            {
                const string query = "SELECT id, artist, albumtitle, imagepath FROM album ORDER BY createdat DESC";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new AlbumInfo
                            {
                                Id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
                                Artist = reader["artist"] == DBNull.Value ? string.Empty : reader["artist"].ToString(),
                                AlbumTitle = reader["albumtitle"] == DBNull.Value ? string.Empty : reader["albumtitle"].ToString(),
                                ImagePath = reader["imagepath"] == DBNull.Value ? string.Empty : reader["imagepath"].ToString()
                            });
                        }
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
            using (var conn = new NpgsqlConnection(connectionString))
            {
                const string query = @"
SELECT id, artist, albumtitle, genre, label, releasedate, addedby, modifiedby, imagepath, createdat
FROM album
WHERE id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new AlbumDetailInfo
                            {
                                Id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
                                Artist = reader["artist"] == DBNull.Value ? string.Empty : reader["artist"].ToString(),
                                AlbumTitle = reader["albumtitle"] == DBNull.Value ? string.Empty : reader["albumtitle"].ToString(),
                                Genre = reader["genre"] == DBNull.Value ? string.Empty : reader["genre"].ToString(),
                                Label = reader["label"] == DBNull.Value ? string.Empty : reader["label"].ToString(),
                                ReleaseDate = reader["releasedate"] == DBNull.Value ? string.Empty : reader["releasedate"].ToString(),
                                AddedBy = reader["addedby"] == DBNull.Value ? string.Empty : reader["addedby"].ToString(),
                                ModifiedBy = reader["modifiedby"] == DBNull.Value ? string.Empty : reader["modifiedby"].ToString(),
                                ImagePath = reader["imagepath"] == DBNull.Value ? string.Empty : reader["imagepath"].ToString(),
                                CreatedAt = reader["createdat"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["createdat"])
                            };
                        }
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

        private static void AddVarCharParameter(NpgsqlCommand command, string parameterName, int length, string value)
        {
            var parameter = command.Parameters.Add(parameterName, NpgsqlDbType.Varchar, length);
            parameter.Value = (object)value ?? DBNull.Value;
        }

        private static string NormalizeText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
