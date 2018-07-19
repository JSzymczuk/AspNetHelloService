using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Engine.Models
{
    public class Entry
    {
        [Key]
        public long Id { get; set; }

        public int Value { get; set; }

        public string Name { get; set; }

        public DateTime? DateCreated { get; set; }

        public byte[] FileContent { get; set; }
    }
}