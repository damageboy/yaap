using System;
using System.Linq;
using System.Threading;
using Yaap;

namespace Demo
{
    static class Program
    {
        static void Main(string[] args)
        {           
            foreach (var i in Enumerable.Range(0, 10000).Yaap()) {
                Thread.Sleep(100);                
            }            
        }
    }
}