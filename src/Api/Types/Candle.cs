﻿
namespace Boukenken.Gdax
{
    public class Candle
    {
        public double time { get; set; }
        public decimal low { get; set; }
        public decimal high { get; set; }
        public decimal open { get; set; }
        public decimal close { get; set; }
        public decimal volume{ get; set; }
    }
}