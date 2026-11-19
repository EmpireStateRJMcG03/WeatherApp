using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherStation2.Models;

public class Location
{
    public string Name { get; set; } = "";
    public string Country { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
