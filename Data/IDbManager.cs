using CADShark.Common.DBManager.Models;
using System.Collections.Generic;

namespace CADShark.Common.DBManager.Data
{
    public interface IDbManager
    {
        void Add(Items item);
        IEnumerable<Items> Items();
        void UpdateItem(Items item);
        void DeleteItem(int id);
        List<Items> SearchItems(string fileName = null, string docType = null);

        byte[] GetBlobById(int id);
    }
}
