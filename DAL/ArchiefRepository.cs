using BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ArchiefRepository
    {
        private readonly TraktContext ctx = new TraktContext();

        public List<Archief> ReadArchieven()
        {
            return ctx.Archieven.ToList();
        }
    }
}
