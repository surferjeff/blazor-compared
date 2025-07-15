// Stuff needed to serve /ts from the Vite proxy.
//
// DotNet builds and serves typescript just fine, but it doesn't do hot
// reloading.  That makes editing JavaScript quite painful.
//
// This code runs the vite proxy for javascript files generated from our
// typescript so that hot reloading works.

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

/// <summary>
/// Doesn't serve /js/** from the file system because we want to proxy those
/// requests to vite.
/// </summary>
internal class KnockoutTs : IFileProvider
{
    IFileProvider inner;

    public KnockoutTs(IFileProvider inner)
    {
        this.inner = inner;
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        var contents = inner.GetDirectoryContents(subpath);
        if (!contents.Exists)
        {
            return contents;
        }

        // Filter out the excluded directory
        var filteredContents = contents.Where(f => !f.Name.ToLowerInvariant().StartsWith("/ts/"));
        return new KnockoutTsContents(filteredContents);
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        if (subpath.ToLowerInvariant().StartsWith("/ts/"))
        {
            return new NotFoundFileInfo(subpath);
        }
        return inner.GetFileInfo(subpath);
    }

    public IChangeToken Watch(string filter)
    {
        return inner.Watch(filter);
    }
}

// Helper class for filtered directory contents
public class KnockoutTsContents : IDirectoryContents
{
    private readonly IEnumerable<IFileInfo> infos;

    public KnockoutTsContents(IEnumerable<IFileInfo> fileInfos)
    {
        infos = fileInfos;
    }

    public bool Exists => infos.Any();

    public IEnumerator<IFileInfo> GetEnumerator() => infos.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return infos.GetEnumerator();
    }
}
