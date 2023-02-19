using PM2E10067.Modelo;
using SQLite;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PM2E10067.Controlador
{
    public class DataBase
    {
        readonly SQLiteAsyncConnection dbase;

        public DataBase(string dbpath)
        {
            dbase = new SQLiteAsyncConnection(dbpath);

            dbase.CreateTableAsync<Modelo.Sitios>();

        }

        public Task<int> SitioSave(Sitios sitio)
        {
            if (sitio.id != 0)
            {
                return dbase.UpdateAsync(sitio);
            }
            else
            {
                return dbase.InsertAsync(sitio);
            }

        }

        public Task<List<Sitios>> getListSitio()
        {
            return dbase.Table<Sitios>().ToListAsync();
        }

        public async Task<Sitios> getSitio(int pid)
        {
            return await dbase.Table<Sitios>()
                .Where(i => i.id == pid)
                .FirstOrDefaultAsync();
        }

        public async Task<int> DeleteSitio(Sitios sitio)
        {
            return await dbase.DeleteAsync(sitio);
        }
    }
}

