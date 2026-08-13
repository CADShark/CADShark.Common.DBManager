using OpenManage.Client;
using OpenManage.Client.Objects.Models;
using System;
using System.Threading.Tasks;

namespace Compatibility.Net48
{
    public static class CompatibilitySmoke
    {
        public static async Task<long> CreateAsync(Uri baseAddress)
        {
            using (var client = new OpenManageClient(
                new OpenManageClientOptions { BaseAddress = baseAddress }))
            {
                ObjectResponse created = await client.Objects.CreateAsync(1);
                return created.ObjectId;
            }
        }
    }
}
