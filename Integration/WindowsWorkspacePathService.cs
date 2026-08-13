using OpenManage.Client.Integration.Models;
using System;
using System.Collections.Generic;

namespace OpenManage.Client.Integration
{
    public sealed class WindowsWorkspacePathService : IWorkspacePathService
    {
        public const string DefaultWorkspaceRoot = @"D:\Vault\";

        private readonly string _normalizedRoot;

        public WindowsWorkspacePathService(string workspaceRoot = DefaultWorkspaceRoot)
        {
            if (string.IsNullOrWhiteSpace(workspaceRoot))
                throw new ArgumentException("Workspace root cannot be empty.", nameof(workspaceRoot));

            _normalizedRoot = NormalizeAbsolutePath(workspaceRoot, true);
            WorkspaceRoot = _normalizedRoot + "\\";
        }

        public string WorkspaceRoot { get; }

        public WorkspaceDocumentInfo Resolve(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new WorkspacePathException("The active document must be saved before it can be added to OpenVault.");

            var normalizedPath = NormalizeAbsolutePath(fullPath, false);
            var prefix = _normalizedRoot + "\\";

            if (!normalizedPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                throw new WorkspacePathException(
                    "The document must be located inside the OpenVault workspace: " + WorkspaceRoot);

            var relativePath = normalizedPath.Substring(prefix.Length);
            var separatorIndex = relativePath.LastIndexOf('\\');
            var fileName = separatorIndex < 0
                ? relativePath
                : relativePath.Substring(separatorIndex + 1);

            if (string.IsNullOrWhiteSpace(fileName))
                throw new WorkspacePathException("The document path does not contain a file name.");

            return new WorkspaceDocumentInfo
            {
                FileName = fileName,
                RelativePath = relativePath
            };
        }

        private static string NormalizeAbsolutePath(string path, bool allowDirectory)
        {
            var value = path.Trim().Replace('/', '\\');

            if (value.Length < 3 || !char.IsLetter(value[0]) || value[1] != ':' || value[2] != '\\')
                throw new ArgumentException("An absolute Windows path is required.", nameof(path));

            var drive = char.ToUpperInvariant(value[0]) + ":";
            var segments = value.Substring(3).Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var normalizedSegments = new List<string>();

            foreach (var segment in segments)
            {
                if (segment == ".")
                    continue;

                if (segment == "..")
                {
                    if (normalizedSegments.Count == 0)
                        throw new ArgumentException("The path escapes the drive root.", nameof(path));

                    normalizedSegments.RemoveAt(normalizedSegments.Count - 1);
                    continue;
                }

                normalizedSegments.Add(segment);
            }

            if (!allowDirectory && normalizedSegments.Count == 0)
                throw new ArgumentException("The document path is invalid.", nameof(path));

            return normalizedSegments.Count == 0
                ? drive
                : drive + "\\" + string.Join("\\", normalizedSegments);
        }
    }
}
