﻿namespace MyLeasing.Web.Data.Entities
{
    using System.Collections.Generic;

    public class Lessee
    {
        public int Id { get; set; }

        public User User { get; set; }

        public ICollection<Contract> Contracts { get; set; }

    }
}
