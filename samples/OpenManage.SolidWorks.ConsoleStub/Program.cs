using OpenManage.Client.Integration;
using OpenManage.Client.Mapping;
using OpenManage.SolidWorks.Adapter;
using System;
using System.Collections.Generic;
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

            try
            {
                var workspaceRoot = args.Length > 0
                    ? args[0]
                    : WindowsWorkspacePathService.DefaultWorkspaceRoot;

                var documentReader = new SolidWorksDocumentReader();
                var document = documentReader
                    .GetActiveDocumentAsync(CancellationToken.None)
                    .GetAwaiter()
                    .GetResult();

                var workspace = new WindowsWorkspacePathService(workspaceRoot);
                var workspaceDocument = workspace.Resolve(document.FullPath);

                var mapper = new EngineeringPropertyMapper();
                var mappings = new[]
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
                };

                var attributes = mapper.Map(
                    document.Properties,
                    mappings,
                    workspaceDocument.RelativePath);

                PrintDocument(document, workspace.WorkspaceRoot, workspaceDocument, attributes);
                return 0;
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

        private static void PrintDocument(
            OpenManage.Client.Integration.Models.EngineeringDocumentInfo document,
            string workspaceRoot,
            OpenManage.Client.Integration.Models.WorkspaceDocumentInfo workspaceDocument,
            IReadOnlyList<OpenVaultAttributeValue> attributes)
        {
            Console.WriteLine("Active SOLIDWORKS document");
            Console.WriteLine("  Full path:      " + document.FullPath);
            Console.WriteLine("  Workspace:      " + workspaceRoot);
            Console.WriteLine("  Relative path:  " + workspaceDocument.RelativePath);
            Console.WriteLine("  File name:      " + workspaceDocument.FileName);
            Console.WriteLine("  Document kind:  " + document.DocumentKind);
            Console.WriteLine("  Configuration:  " + (document.Configuration ?? "<not applicable>"));

            Console.WriteLine();
            Console.WriteLine("SOLIDWORKS properties:");
            foreach (var property in document.Properties)
                Console.WriteLine("  " + property.Key + " = " + property.Value);

            Console.WriteLine();
            Console.WriteLine("OpenVault attributes prepared for CreateOnly:");
            foreach (var attribute in attributes)
                Console.WriteLine("  AttributeId " + attribute.AttributeId + " = " + attribute.Value);

            Console.WriteLine();
            Console.WriteLine("No data was sent to OpenVault.");
        }
    }
}
