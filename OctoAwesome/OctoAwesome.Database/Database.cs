using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace OctoAwesome.Database
{
    public class Database
    {
        private readonly KeyStore keyStore;
        private readonly ValueStore valueStore;

        public Database()
        {
            keyStore = new KeyStore();
            valueStore = new ValueStore();

        }      
    }
}
