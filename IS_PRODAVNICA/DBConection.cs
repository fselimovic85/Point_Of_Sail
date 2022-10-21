using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_PRODAVNICA
{
    class DBConection
    {
        public string MyConection()
        {
            string con = @"Data Source=ADMINPC; Initial Catalog=IS_PRODAVNICE; Integrated Security=True";
            return con;
        }   
    }
}
