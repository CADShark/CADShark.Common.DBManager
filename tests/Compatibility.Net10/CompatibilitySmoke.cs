using OpenManage.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Compatibility.Net10;

public static class CompatibilitySmoke
{
    public static async Task<long> CreateAsync(
        Uri baseAddress,
        CancellationToken cancellationToken = default)
    {
        using var client = new OpenManageClient(
            new OpenManageClientOptions { BaseAddress = baseAddress });

        var created = await client.Objects.CreateAsync(
            objectType: 1,
            cancellationToken);

        return created.ObjectId;
    }
}
