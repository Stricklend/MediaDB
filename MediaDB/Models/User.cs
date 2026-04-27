using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaDB.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("users")]
    public class User
    {
        [System.ComponentModel.DataAnnotations.Schema.Column("id")]
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("username")]
        public string UserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("password")]
        public string UserPassword { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("nickname")]
        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("email")]
        public string Email { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
