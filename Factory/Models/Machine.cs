using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class Item
    {
        public Item()
        {
            this.JoinEntities = new HashSet<CategoryItem>();
        }

        public int ItemId { get; set; }
        public string Description { get; set; }
        public bool Complete { get; set; }
        public DateTime DueDate { get; set; }

        public virtual ICollection<CategoryItem> JoinEntities { get;}
    }
}