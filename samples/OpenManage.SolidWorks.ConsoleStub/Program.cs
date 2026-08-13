using OpenManage.Client;
using OpenManage.Client.Integration;
using OpenManage.Client.Integration.Models;
using OpenManage.Client.Mapping;
using OpenManage.SolidWorks.Adapter;
using System;
using System.Text;
using System.Threading;

namespace OpenManage.SolidWorks.ConsoleStub
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (args.Length == 0 || !Uri.TryCreate(args[0], UriKind.Absolute, out var apiAddress))
            {
                Console.Error.WriteLine(
                    "Usage: OpenManage.SolidWorks.ConsoleStub.exe <OpenVaultApiUrl> [WorkspaceRoot]");
                return 4;
            }

            var workspaceRoot = args.Length > 1
                ? args[1]
                : WindowsWorkspacePathService.DefaultWorkspaceRoot;

            try
            {
                var document = new SolidWorksDocumentReader()
                    .GetActiveDocumentAsync(CancellationToken.None)
                    .GetAwaiter()
                    .GetResult();

                using (var client = new OpenManageClient(
                    new OpenManageClientOptions
                    {
                        BaseAddress = apiAddress
                    }))
                {
                    var service = new CreateOnlyDocumentService(client);
                    var result = service.CreateAsync(
                            new CreateOnlyDocumentRequest
                            {
                                Document = document,
                                WorkspaceRoot = workspaceRoot,
                                PropertyMappings = new[]
                                {
                                    new PropertyAttributeMapping
                                    {
                                        PropertyName = "Обозначение",
                                        AttributeId = 9
                                    },
                                    new PropertyAttributeMapping
                                    {
                                        PropertyName = "Наименование",
                                        AttributeId = 10
                                    }
                                }
                            },
                            CancellationToken.None)
                        .GetAwaiter()
                        .GetResult();

                    PrintResult(result);

                    return result.IsSuccess ? 0 : 5;
                }
            }
            catch (SolidWorksAdapterException exception)
            {
                Console.Error.WriteLine("SOLIDWORKS error: " + exception.Message);
                return 1;
            }
            catch (WorkspacePathException exception)
            {
                Console.Error.WriteLine("Workspace error: " + exception.Message);
                return 2;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("Unexpected error: " + exception);
                return 3;
            }
        }

        private static void PrintResult(CreateOnlyDocumentResult result)
        {
            Console.WriteLine("CreateOnly result: " + (result.IsSuccess ? "SUCCESS" : "FAILED"));
            Console.WriteLine("  ObjectId: " + Format(result.ObjectId));
            Console.WriteLine("  VersionId: " + Format(result.VersionId));
            Console.WriteLine("  Added attributes: " + result.AddedAttributeCount);
            Console.WriteLine("  FileId: " + Format(result.FileId));

            if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
            {
                Console.WriteLine();
                Console.WriteLine("Error: " + result.ErrorMessage);
                Console.WriteLine(
                    "Already-created server data was not deleted automatically.");
            }
        }

        private static string Format<T>(T? value)
            where T : struct
        {
            return value.HasValue ? value.Value.ToString() : "<not created>";
        }
    }
}
