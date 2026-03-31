//20260329 mod start - supabase연동 검토 관련
using System.ComponentModel.DataAnnotations.Schema;
//20260329 mod end
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MediaDB.Models
{
    //20260329 mod start - supabase연동 검토 관련
    // [Table("users", Schema = "public")]
    // public class User
    // {
    //     [Column("id")]
    //     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //     public int Id { get; set; }

    //     [Column("username")]
    //     public string Username { get; set; }

    //     [Column("password")]
    //     public string Password { get; set; }
    // }
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    //20260329 mod end   
}
