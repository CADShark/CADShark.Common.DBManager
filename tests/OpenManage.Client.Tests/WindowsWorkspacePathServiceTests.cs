using OpenManage.Client.Integration;
using Xunit;

namespace OpenManage.Client.Tests
{
    public sealed class WindowsWorkspacePathServiceTests
    {
        [Fact]
        public void Resolve_ReturnsRelativePathAndFileName()
        {
            var service = new WindowsWorkspacePathService(@"D:\Vault\");

            var result = service.Resolve(@"D:\Vault\Project\ABCD\Part1.sldprt");

            Assert.Equal("Part1.sldprt", result.FileName);
            Assert.Equal(@"Project\ABCD\Part1.sldprt", result.RelativePath);
        }

        [Fact]
        public void Resolve_IsCaseInsensitive()
        {
            var service = new WindowsWorkspacePathService(@"D:\Vault\");

            var result = service.Resolve(@"d:\vault\Project\Part1.sldprt");

            Assert.Equal(@"Project\Part1.sldprt", result.RelativePath);
        }

        [Fact]
        public void Resolve_RejectsSiblingDirectoryWithCommonPrefix()
        {
            var service = new WindowsWorkspacePathService(@"D:\Vault\");

            Assert.Throws<WorkspacePathException>(
                () => service.Resolve(@"D:\VaultBackup\Part1.sldprt"));
        }

        [Fact]
        public void Resolve_NormalizesParentSegments()
        {
            var service = new WindowsWorkspacePathService(@"D:\Vault\");

            var result = service.Resolve(@"D:\Vault\Project\Temp\..\Part1.sldprt");

            Assert.Equal(@"Project\Part1.sldprt", result.RelativePath);
        }

        [Fact]
        public void Resolve_RejectsUnsavedDocument()
        {
            var service = new WindowsWorkspacePathService(@"D:\Vault\");

            Assert.Throws<WorkspacePathException>(() => service.Resolve(string.Empty));
        }
    }
}
