using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustDrive.Models
{
    public class Car
    {
       
        public int CarId { get; set; }
        [Require]
        public string Name { get; set; }
       
        public string SerialNumber { get; set; }

        public decimal Price { get; set; }


        //relation
        public List<Image> Image { get; set; }
        

        public Category Category { get; set; }
        public int CategoryId { get; set; }

    }
}
