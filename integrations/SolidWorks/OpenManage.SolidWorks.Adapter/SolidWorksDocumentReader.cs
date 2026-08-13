using OpenManage.Client.Integration.Models;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace OpenManage.SolidWorks.Adapter
{
    public sealed class SolidWorksDocumentReader : ISolidWorksDocumentReader
    {
        private const string SolidWorksProgId = "SldWorks.Application";
        public Task<EngineeringDocumentInfo> GetActiveDocumentAsync(
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                dynamic application = Marshal.GetActiveObject(SolidWorksProgId);
                if (application == null)
                    throw new SolidWorksAdapterException("SOLIDWORKS is not running.");

                dynamic document = application.ActiveDoc;
                if (document == null)
                    throw new SolidWorksAdapterException("SOLIDWORKS does not have an active document.");

                var fullPath = (string)document.GetPathName();
                if (string.IsNullOrWhiteSpace(fullPath))
                    throw new SolidWorksAdapterException(
                        "The active SOLIDWORKS document must be saved before it can be added to OpenVault.");

                var documentKind = MapDocumentKind(fullPath);
                var configurationName = GetActiveConfigurationName(document, documentKind);
                var result = new EngineeringDocumentInfo
                {
                    FullPath = fullPath,
                    Configuration = configurationName,
                    DocumentKind = documentKind
                };

                dynamic extension = document.Extension;
                ReadProperties(extension.CustomPropertyManager[string.Empty], result.Properties);

                if (!string.IsNullOrWhiteSpace(configurationName))
                    ReadProperties(
                        extension.CustomPropertyManager[configurationName],
                        result.Properties);

                return Task.FromResult(result);
            }
            catch (SolidWorksAdapterException)
            {
                throw;
            }
            catch (COMException exception)
            {
                throw new SolidWorksAdapterException(
                    "Cannot connect to the running SOLIDWORKS application.",
                    exception);
            }
            catch (Exception exception)
            {
                throw new SolidWorksAdapterException(
                    "Cannot read the active SOLIDWORKS document.",
                    exception);
            }
        }

        private static EngineeringDocumentKind MapDocumentKind(string fullPath)
        {
            switch (Path.GetExtension(fullPath).ToUpperInvariant())
            {
                case ".SLDPRT":
                    return EngineeringDocumentKind.Part;
                case ".SLDASM":
                    return EngineeringDocumentKind.Assembly;
                case ".SLDDRW":
                    return EngineeringDocumentKind.Drawing;
                default:
                    return EngineeringDocumentKind.Unknown;
            }
        }

        private static string GetActiveConfigurationName(
            dynamic document,
            EngineeringDocumentKind documentKind)
        {
            if (documentKind == EngineeringDocumentKind.Drawing)
                return null;

            dynamic configurationManager = document.ConfigurationManager;
            dynamic activeConfiguration = configurationManager == null
                ? null
                : configurationManager.ActiveConfiguration;

            return activeConfiguration == null
                ? null
                : (string)activeConfiguration.Name;
        }

        private static void ReadProperties(
            dynamic propertyManager,
            System.Collections.Generic.IDictionary<string, string> target)
        {
            if (propertyManager == null)
                return;

            object namesObject = propertyManager.GetNames();
            var names = namesObject as IEnumerable;
            if (names == null)
                return;

            foreach (var item in names)
            {
                var propertyName = item as string;
                if (string.IsNullOrWhiteSpace(propertyName))
                    continue;

                string rawValue;
                string resolvedValue;
                bool wasResolved;
                bool linkToProperty;

                propertyManager.Get6(
                    propertyName,
                    false,
                    out rawValue,
                    out resolvedValue,
                    out wasResolved,
                    out linkToProperty);

                target[propertyName] = string.IsNullOrEmpty(resolvedValue)
                    ? rawValue ?? string.Empty
                    : resolvedValue;
            }
        }
    }
}
