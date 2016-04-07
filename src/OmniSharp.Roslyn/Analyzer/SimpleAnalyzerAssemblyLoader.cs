using System;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace OmniSharp.Roslyn.Analyzer
{
    public class SimpleAnalyzerAssemblyLoader : IAnalyzerAssemblyLoader
    {
        public void AddDependencyLocation(string fullPath)
        {
            throw new NotImplementedException();
        }

        public Assembly LoadFromPath(string fullPath)
        {
#if NET451
            return Assembly.LoadFrom(fullPath);
#else
            throw new NotImplementedException();
#endif
        }
    }
}