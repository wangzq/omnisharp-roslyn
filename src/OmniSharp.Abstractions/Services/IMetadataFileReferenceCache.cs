using Microsoft.CodeAnalysis;

namespace OmniSharp.Services
{
    public interface IMetadataFileReferenceCache
    {
        MetadataReference GetMetadataReference(string path);
		string GetFilePath(MetadataReference metadata);
    }
}
