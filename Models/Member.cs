using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DatabaseQuiz.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "身分證字號")]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "手機門號")]
        public string PhoneNumber { get; set; }

        [Display(Name = "是否為管理員")]
        public bool isAdmin { get; set; }
    }
}