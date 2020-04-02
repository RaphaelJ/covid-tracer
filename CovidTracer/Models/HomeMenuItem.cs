using System;
using System.Collections.Generic;
using System.Text;

namespace CovidTracer.Models
{
    public enum MenuItemType
    {
        CovidTracer,
        About
    }

    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
