﻿using System.ComponentModel.DataAnnotations;

namespace ProductLibPrototype.DataBase
{
    public class User
    {
        [Key]
        [MaxLength(32)]
        public string AccName { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(32)]
        public string Mail { get; set; }
        
    }
}